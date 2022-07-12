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

        RunTests = RunReadTests | RunWriteTests,
        RunTestsWithDetail = RunTests | WithDetail,
        RunReadWithDetail = RunReadTests | WithDetail,
        RunWriteWithDetail = RunWriteTests | WithDetail,
    }
}
