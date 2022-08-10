using Microsoft.Extensions.Logging;
using System;

namespace TestChecker.Core
{
    internal static class LoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }
}