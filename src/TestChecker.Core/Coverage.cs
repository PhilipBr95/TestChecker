using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    [DebuggerDisplay("Coverage = {Percentage}%")]
    public class Coverage
    {
        public string Object { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CoverageMethod? CoverageMethod { get; private set; }

        public decimal Percentage { get; set; }

        public string Detail { get; set; }

        [JsonIgnore]
        public IList<string> Hits { get; private set; } = new List<string>();
        [JsonIgnore]
        public IList<string> Total { get; private set; } = new List<string>();

        public Coverage()
        {

        }

        public Coverage(decimal percentage, string detail)
        {
            Percentage = percentage;
            Detail = detail;
        }

        private void UpdateStats()
        {
            Percentage = Total.Count == 0 ? 0 : Math.Round((1.0M * Hits.Count() / Total.Count) * 100, 2);
            Detail = $"[{Hits.Count()} / {Total.Count()}]";
        }

        public Coverage(string objectName, CoverageMethod? coverageMethod, IList<string> hits, IList<string> total)
        {
            Object = objectName;
            CoverageMethod = coverageMethod;            
            Total = total;

            if(hits != null)
                Hits = hits;

            UpdateStats();
        }

        public Coverage(List<Coverage> list)
        {
            Hits = list.Where(w => w?.Hits != null).SelectMany(s => s.Hits).Distinct().ToList();
            Total = list.Where(w => w?.Total != null).SelectMany(s => s.Total).Distinct().ToList();

            UpdateStats();
        }
    }
}