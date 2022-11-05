using NetSystem = System;
using System.Collections.Generic;
using TestChecker.Core.Enums;
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TestChecker.Core.Extensions;

namespace TestChecker.Core
{    
    public class VersionInfo
    {
        private const string NOT_AVAILABLE = "N/A";
        public string TestVersion { get; set; } = NOT_AVAILABLE;
        public string System { get; set; } = NetSystem.Reflection.Assembly.GetEntryAssembly().FullName;
        public Actions AvailableActions { get; set; } = Actions.GetTestData | Actions.RunReadTests | Actions.RunWriteTests;   //Compat, default values
        public string Environment { get; set; } = NetSystem.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";     //todo - bad
        public List<VersionInfo> Dependencies { get; set; } = new List<VersionInfo>();

        public VersionInfo()
        {
        }

        public VersionInfo(bool getActions)
        {
            TestVersion = typeof(VersionInfo).Assembly.GetName().Version.ToString();

            if (getActions)
            {
                Actions actions = 0;
                foreach(Actions actionValue in Enum.GetValues(typeof(Actions)))
                    actions |= actionValue;

                AvailableActions = actions;
            }
        }

        public bool HasVersion() => !TestVersion.Equals(NOT_AVAILABLE);

        public void AddDependencies(List<VersionInfo> dependencyVersions)
        {
            Dependencies.AddRange(dependencyVersions);
        }

        public bool HasAvailableAction(Actions actions)
        {
            return AvailableActions.HasFlag(actions);
        }
    }
}