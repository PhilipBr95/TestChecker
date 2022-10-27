using NetSystem = System;
using System.Collections.Generic;
using TestChecker.Core.Enums;
using System;
using System.Linq;

namespace TestChecker.Core
{    
    public class VersionInfo
    {
        public string System { get; set; } = NetSystem.Reflection.Assembly.GetEntryAssembly().FullName;
        public string Version { get; set; } = typeof(VersionInfo).Assembly.GetName().Version.ToString();
        public IEnumerable<string> Features { get; set; } = NetSystem.Enum.GetNames(typeof(Actions));
        public string Environment { get; set; } = NetSystem.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";     //todo - bad
        public List<VersionInfo> Dependencies { get; set; } = new List<VersionInfo>();

        public void AddDependencies(List<VersionInfo> dependencyVersions)
        {
            Dependencies.AddRange(dependencyVersions);
        }
    }
}