using System;

namespace TestChecker.Core.Enums
{
    [Flags]
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
    }
}
