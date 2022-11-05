using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using TestChecker.Core.Serialisation;

namespace TestChecker.Core.Enums
{
    [Flags]
    [JsonConverter(typeof(FlagConverter<Actions>))]
    public enum Actions
    {
        //DON'T change these due to backwards compat!!!
        GetTestData = 1,
        RunReadTests = 2,
        RunWriteTests = 4,

        WithDetail = 8,
        GetNames = 16,
        GetVersion = 32,

        RunTests = RunReadTests | RunWriteTests,
        RunTestsWithDetail = RunTests | WithDetail,
        RunReadWithDetail = RunReadTests | WithDetail,
        RunWriteWithDetail = RunWriteTests | WithDetail,
    }

    public static class ActionsExtensions
    {
        public static bool HasRunReadTests(this Actions actions) => actions.HasFlag(Actions.RunReadTests);
        public static bool HasGetNames(this Actions actions) => actions.HasFlag(Actions.GetNames);
        public static bool IsValid(this Actions actions) => (int)actions < 64;
    }
}
