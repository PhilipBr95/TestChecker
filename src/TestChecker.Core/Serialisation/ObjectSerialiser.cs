using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using TestChecker.Core.ContractResolver;
using TestChecker.Core.Extensions;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core.Serialisation
{
    public class ObjectSerialiser : IObjectSerialiser
    {
        private object _object;
        private bool? _isObject;
        private int _minDepth;

        private ILogger _logger;
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            MaxDepth = 1,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new SimpleContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };
        
        public Dictionary<string, ValueRetriever> Mappings { get; private set; } = new Dictionary<string, ValueRetriever>();
        
        public ObjectSerialiser(ILogger logger = null)
        {
            _logger = logger;

            //Default mappings   
            Mappings.Add("System.Web.Http.Results.InternalServerErrorResult", new ValueRetriever(obj => false, obj => new { StatusCode = 500 }, 2));         
            Mappings.Add("Microsoft.AspNetCore.Mvc.ObjectResult", new ValueRetriever(obj => obj?.StatusCode?.ToString()[0] == '2' ? true : false, obj => new { StatusCode = GetStatusCodeName(obj.StatusCode), obj?.Value }, 2));
            Mappings.Add("Microsoft.AspNetCore.Mvc.StatusCodeResult", new ValueRetriever(obj => false, obj => new { StatusCode = GetStatusCodeName(obj.StatusCode) }, 2));
            Mappings.Add("System.Web.Http.Results.OkNegotiatedContentResult`1", new ValueRetriever(obj => true, obj => new { StatusCode = 200, obj?.Content }, 2));
            Mappings.Add("System.Web.Http.Results.JsonResult`1", new ValueRetriever(obj => obj?.Content));
            Mappings.Add("Microsoft.AspNetCore.Mvc.ActionResult`1", new ValueRetriever(obj => obj?.Result?.Value ?? obj?.Value));            
            Mappings.Add("Microsoft.AspNetCore.Mvc.IConvertToActionResult", new ValueRetriever(obj => obj?.Result?.Value ?? obj?.Value));
            Mappings.Add("Microsoft.AspNetCore.Mvc.IActionResult", new ValueRetriever(obj => obj?.Value));
            Mappings.Add("Microsoft.AspNetCore.Mvc.IStatusCodeActionResult", new ValueRetriever(obj => obj?.Value));    
            Mappings.Add("System.Web.Http.Results.ResponseMessageResult", new ValueRetriever(obj => obj?.Response?.Content));        
        }

        private string GetStatusCodeName(int statusCode)
        {
            return $"{((HttpStatusCode)statusCode)}({statusCode})";
        }

        public ObjectSerialiser(Dictionary<string, ValueRetriever> mappings, ILogger logger = null) : this(logger)
        {
            Mappings = mappings;
        }

        private void Reset()
        {
            _object = null;
            _isObject = null;
            _minDepth = 1;
        }

        public void SetObject(dynamic obj)
        {
            Reset();

            if (obj == null)
            {
                _isObject = false;
                return;
            }

            Type type = obj.GetType();

            if (TryGetValueRetriever(type, out ValueRetriever valueRetriever))
            {
                _object = valueRetriever.GetValue(obj);
                _isObject = valueRetriever.IsObject(obj);
                _minDepth = valueRetriever.MinDepth;
            }
            else
            {
                _object = obj;
            }

            if (_isObject == null)
                _isObject = IsValidObject();
        }

        private bool TryGetValueRetriever(Type type, out ValueRetriever valueRetriever)
        {
            var baseFullname = type.BaseType.GetFullName();

            //Check the BaseClasses
            if (Mappings.ContainsKey(baseFullname))
            {
                valueRetriever = Mappings[baseFullname];

                _logger?.LogInformation($"Class mapping found for {type.GetFullName()} to {baseFullname}");
                return true;
            }            

            var fullname = type.GetFullName();

            //Check the ClassNames
            if (Mappings.ContainsKey(fullname))
            {                
                valueRetriever = Mappings[fullname];

                _logger?.LogInformation($"Class mapping found for {type.GetFullName()} to {fullname}");
                return true;
            }

            //Check interfaces
            foreach(var i in type.GetInterfaces())
            {
                var interfaceFullname = i.GetFullName();
                if (Mappings.ContainsKey(interfaceFullname))
                {
                    valueRetriever = Mappings[interfaceFullname];

                    _logger?.LogInformation($"Interface mapping found for {type.GetFullName()} to {fullname} => {interfaceFullname}");
                    return true;
                }
            }

            valueRetriever = null;

            _logger?.LogInformation($"No Mapping found for {fullname}");
            return false;
        }

        public bool IsObject()
        {
            return _isObject.GetValueOrDefault();
        }

        private bool IsValidObject()
        {
            if (_object is Exception)
                return false;

            if (_object is IQueryable)
            {
                //Todo - there must be a better way
                var json = SerialiseObject(1);
                return json != "[]";
            }

            return _object != null;
        }

        public string SerialiseObject(int maxDepth)
        {
            if (_minDepth > maxDepth) maxDepth = _minDepth;
            if (_object == null) return null;
            if (IsStream(_object.GetType())) return JsonConvert.SerializeObject(ReadStream(_object as Stream), _jsonSerializerSettings);

            Func<object, JsonSerializerSettings, string> serialiser = JsonConvert.SerializeObject;
            if (maxDepth > 1) serialiser = new DepthSerialiser(maxDepth).SerializeObject;

            string json;

            if (IsComplex(_object.GetType()))
            {
                json = serialiser(_object, _jsonSerializerSettings);
            }
            else
            {
                json = _object.ToString();
            }

            if (maxDepth == 1 && json == "{}")
                _logger?.LogWarning($"Try setting the {nameof(maxDepth)} for the Test to 2 for the output {_object.GetType().FullName}");

            return json;
        }

        private static bool IsStream(Type typeIn)
        {
            return (typeIn.IsSubclassOf(typeof(Stream)));
        }

        private static bool IsComplex(Type typeIn)
        {
            if (typeIn.IsSubclassOf(typeof(System.ValueType)) || typeIn.Equals(typeof(string))) //|| typeIn.IsPrimitive
                return false;
            else
                return true;
        }

        private static PartialStream ReadStream(Stream obj)
        {
            return new PartialStream(obj);
        }
    }
}