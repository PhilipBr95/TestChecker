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

        /// <summary>
        /// Determines whether the specified test method should be executed based on the current test settings
        /// </summary>
        /// <remarks>If test settings are not configured, all test methods are considered eligible for
        /// execution. When 'GetNamesOnly' is enabled, the method records the test method name and returns false,
        /// indicating the test should not be run. If specific test methods are defined in the current settings, only
        /// those matching the provided method name are eligible for execution.</remarks>
        /// <param name="method">The name of the test method to evaluate for execution. Cannot be null or empty.</param>
        /// <returns>true if the test method should be executed; otherwise, false.</returns>
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
                return CurrentTestSettings.TestMethods == null ||
                        CurrentTestSettings.TestMethods.Contains($"{ObjectName}.{method}") || 
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