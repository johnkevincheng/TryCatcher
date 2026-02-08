using Microsoft.Extensions.Logging; // Requires Microsoft.Extensions.Logging.Abstractions NuGet
using System;
using System.Collections.Generic;
using System.Threading;

namespace RockFluid
{
    public class RetryResult
    {
        public bool IsSuccess { get; set; }
        public int Tries { get; set; }
        public Exception Error { get; set; }
    }

    public class RetryResult<T> : RetryResult
    {
        public T Value { get; set; }
    }

    public class ReCatcher
    {
        private int _retryCount = 1;
        private int _delaySeconds = 0;
        private readonly List<ILogger> _loggers = new List<ILogger>();

        private ReCatcher() { }

        public static ReCatcher With() => new ReCatcher();

        public ReCatcher RetryCount(int count)
        {
            _retryCount = count;
            return this;
        }

        public ReCatcher IntervalDelay(int seconds)
        {
            _delaySeconds = seconds;
            return this;
        }

        /// <summary>
        /// Adds a logger to the resilience chain. Can be called multiple times to add multiple loggers.
        /// </summary>
        public ReCatcher WithLogger(ILogger logger)
        {
            if (logger != null) _loggers.Add(logger);
            return this;
        }

        public RetryResult Try(Action action)
        {
            var result = new RetryResult();
            for (int i = 1; i <= _retryCount; i++)
            {
                result.Tries = i;
                try
                {
                    action();
                    result.IsSuccess = true;
                    result.Error = null;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                    LogToAll(i, ex);
                    if (i < _retryCount && _delaySeconds > 0)
                        Thread.Sleep(_delaySeconds * 1000);
                }
            }
            result.IsSuccess = false;
            return result;
        }

        public RetryResult<T> Try<T>(Func<T> function)
        {
            var result = new RetryResult<T>();
            for (int i = 1; i <= _retryCount; i++)
            {
                result.Tries = i;
                try
                {
                    result.Value = function();
                    result.IsSuccess = true;
                    result.Error = null;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Error = ex;
                    LogToAll(i, ex);
                    if (i < _retryCount && _delaySeconds > 0)
                        Thread.Sleep(_delaySeconds * 1000);
                }
            }
            result.IsSuccess = false;
            result.Value = default(T);
            return result;
        }

        private void LogToAll(int attempt, Exception ex)
        {
            if (_loggers.Count == 0) return;

            foreach (var logger in _loggers)
            {
                logger.LogError(ex, "ReCatcher detected failure on attempt {Attempt}. Retrying...", attempt);
            }
        }
    }
}
