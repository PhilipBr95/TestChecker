using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TestChecker.Core.Serialisation.Converters;

namespace TestChecker.Core
{
    [JsonConverter(typeof(SystemInfoConverter))]
    [DebuggerDisplay("{Name}")]
    public class SystemInfo
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string Url { get; set; }

        internal static SystemInfo GenerateFrom(string systemInfoString)
        {
            return new SystemInfo { Name = systemInfoString };
        }
    }
}
