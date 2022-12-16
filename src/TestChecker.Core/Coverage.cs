using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    [DebuggerDisplay("{Detail} @ {Percentage}%")]
    public class Coverage
    {
        public string Object { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CoverageMethod? CoverageMethod { get; private set; }

        public decimal Percentage { get; set; }

        public string Detail { get; set; }

        [JsonIgnore]
        public List<string> Hits { get; private set; } = new List<string>();
        [JsonIgnore]
        public List<string> Total { get; private set; } = new List<string>();

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

        public Coverage(string objectName, CoverageMethod? coverageMethod, List<string> hits, List<string> total)
        {
            Object = objectName;
            CoverageMethod = coverageMethod;            
            Total = total;

            if(hits != null)
                Hits = hits;

            UpdateStats();
        }

        public Coverage(List<Coverage> coverages)
        {            
            foreach(var coverage in coverages)
            {
                if (coverage == null)
                    continue;

                if(coverage.Total.Count > 0)
                {
                    Hits.AddRange(coverage.Hits.Distinct().ToList());
                    Total.AddRange(coverage.Total.Distinct().ToList());
                }
                else
                {
                    if (coverage.Detail.Length > 4)
                    {
                        //Bit of a hack - todo
                        var sections = coverage.Detail.Substring(1, coverage.Detail.Length-2).Split('/');
                        if (sections.Length == 2)
                        {
                            Hits.AddRange(Enumerable.Range(0, int.Parse(sections[0].Trim())).Select(s => $"Faked {s}"));
                            Total.AddRange(Enumerable.Range(0, int.Parse(sections[1].Trim())).Select(s => $"Faked {s}"));
                        }
                    }
                }
            }

            UpdateStats();
        }
    }
}