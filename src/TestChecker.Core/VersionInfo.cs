using NetSystem = System;
using System.Collections.Generic;
using TestChecker.Core.Enums;
using System;

namespace TestChecker.Core
{
    public class VersionInfo
    {
        public SystemInfo System { get; set; } = SystemInfo.Create();
        public Actions AvailableActions { get; set; } = Actions.GetTestData | Actions.RunReadTests | Actions.RunWriteTests;   //Compat, default values
        public string Environment { get; set; } = NetSystem.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";     //todo - bad
        public Version TestCheckerVersion { get; set; } = null;
        public List<VersionInfo> Dependencies { get; set; } = new List<VersionInfo>();
        
        public VersionInfo()
        {
        }

        public VersionInfo(bool getActions)
        {
            TestCheckerVersion = typeof(VersionInfo).Assembly.GetName().Version;

            if (getActions)
            {
                Actions actions = 0;
                foreach(Actions actionValue in Enum.GetValues(typeof(Actions)))
                    actions |= actionValue;

                AvailableActions = actions;
            }
        }

        public bool HasVersion() => TestCheckerVersion != null;

        public void AddDependencies(List<VersionInfo> dependencyVersions) => Dependencies.AddRange(dependencyVersions);

        public bool HasAvailableAction(Actions actions) => AvailableActions.HasFlag(actions);
    }
}