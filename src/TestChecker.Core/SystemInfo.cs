using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TestChecker.Core
{
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
