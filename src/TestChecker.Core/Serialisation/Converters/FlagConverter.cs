using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestChecker.Core.Enums;

namespace TestChecker.Core.Serialisation.Converters
{
    public class FlagConverter<T> : JsonConverter where T : Enum
    {
        public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            UInt64 result = 0;

            var values = token.ToObject<string[]>();

            foreach (T f in Enum.GetValues(typeof(T)))
            {
                if (values.Contains(f.ToString()))
                {
                    var enumValue = Convert.ToUInt64(f);
                    result |= enumValue;
                }
            }

            return (T)Enum.ToObject(typeof(T), result);
        }

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            var flags = new List<string>();

            T f = (T)value;
            foreach (T ff in Enum.GetValues(typeof(T)))
            {
                if(f.HasFlag(ff))
                    flags.Add(ff.ToString());
            }

            writer.WriteRawValue(JsonConvert.SerializeObject(flags.OrderBy(o => o)));
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
