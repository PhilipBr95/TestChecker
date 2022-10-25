using NetSystem = System;
using System.Collections.Generic;
using TestChecker.Core.Enums;
using System;
using System.Linq;

namespace TestChecker.Core
{
    public interface IListOrObject<T>
    {
        void Add(T obj);
        void Add(IEnumerable<T> list);
    }

    public class VersionInfoSummary : IListOrObject<VersionInfoSummary>
    {
        public List<VersionInfo> VersionInfos { get; set; } = new List<VersionInfo>();

        public void Add(VersionInfoSummary obj) => VersionInfos.AddRange(obj.VersionInfos);

        public void Add(IEnumerable<VersionInfoSummary> objs) => VersionInfos.AddRange(objs.SelectMany(s => s.VersionInfos));

        public void AddVersionInfo(VersionInfo versionInfo) => VersionInfos.Add(versionInfo);
        public void AddVersionInfo(VersionInfoSummary dependencyVersions) => VersionInfos.AddRange(dependencyVersions.VersionInfos);
    }

    public class VersionInfo
    {
        public string System { get; set; } = NetSystem.Reflection.Assembly.GetEntryAssembly().FullName;
        public string Version { get; set; } = typeof(VersionInfo).Assembly.GetName().Version.ToString();
        public IEnumerable<string> Features { get; set; } = NetSystem.Enum.GetNames(typeof(Actions));
        public string Environment { get; set; } = NetSystem.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";     //todo - bad
        public List<VersionInfo> ChildVersions { get; set; } = new List<VersionInfo>();

        public void AddChildVersions(List<VersionInfoSummary> dependencyVersions)
        {
            //This needs fixing!!!
            //Get rid of VersionInfoSummary
            ChildVersions.AddRange(dependencyVersions.SelectMany(s => s.VersionInfos));
        }
    }
}