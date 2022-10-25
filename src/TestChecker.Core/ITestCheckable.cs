﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TestChecker.Core.Enums;

namespace TestChecker.Core
{
    public interface ITestCheckable
    {
        string BaseUrl { get; }
        Task<T> RunTestAsync<T>(TestSettings testSettings);
        Task<List<NamedTestData>> GetTestDataAsync();
    }
}
