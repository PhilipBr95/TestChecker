using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
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

        internal static SystemInfo Create(string url = null)
        {
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName();
            return new SystemInfo { Name = assemblyName.Name, Version = assemblyName.Version, Url = url };
        }

        internal static SystemInfo CreateFrom(string systemInfoString)
        {
            var splits = systemInfoString.Split(new string[] { ", " }, StringSplitOptions.None);

            if(splits.Any())
                return new SystemInfo { Name = splits[0] };

            return new SystemInfo { Name = systemInfoString };
        }
    }
}
