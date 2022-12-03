using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TestChecker.Core.Enums;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core
{
    public partial class TestCheck
    {
        public static string AssemblyName;
        public static TestSettings CurrentTestSettings = null;

        public static bool GetNamesOnly => CurrentTestSettings?.Action.HasFlag(Actions.GetNames) == true;

        public bool RunTest(string method)
        {
            if (CurrentTestSettings == null) 
                return true;

            if (GetNamesOnly)
            {
                Add(new TestCheck() { Method = method }, true);
                return false;
            }

            if (CurrentTestSettings.HasTestMethods(AssemblyName))
            {
                return CurrentTestSettings.TestMethods.Contains(method) || 
                        CurrentTestSettings.TestMethods.Contains($"{AssemblyName}.{method}");
            }

            return true;
        }

        /// <summary>
        /// Reset the current settings config
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="settings"></param>
        public static void SetTestSetting(Assembly assembly, TestSettings settings)
        {
            AssemblyName = assembly.GetName().Name;
            CurrentTestSettings = settings;
        }
    }
}