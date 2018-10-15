using System;

namespace RockFluid
{
    /// <summary>
    /// Execute delegates in the context of a reusable Try-Catch block.
    /// </summary>
    public static class TryCatcher
    {
        /// <summary>
        /// Run the provided function or delegate in the context of a Try-Catch block, and log and exceptions thrown if a Logger is configured.
        /// </summary>
        /// <typeparam name="TResultType">The type of the function result if successfully run.</typeparam>
        /// <param name="function">The function or delegate to run.</param>
        /// <returns></returns>
        public static (Boolean RunSuccessful, TResultType Value, Exception Exception) RunFunction<TResultType>(Func<TResultType> function)
        {
            TResultType returnValue;

            try
            {
                returnValue = function.Invoke();
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError(ex);

                return (false, default(TResultType), ex);
            }

            return (true, returnValue, null);
        }

        /// <summary>
        /// Run the provided method or delegate in the context of a Try-Catch block, and log and exceptions thrown if a Logger is configured.
        /// </summary>
        /// <param name="method">The method or delegate to run.</param>
        /// <returns></returns>
        public static (Boolean RunSuccessful, Exception Exception) RunMethod(Action method)
        {
            try
            {
                method.Invoke();
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError(ex);

                return (false, ex);
            }

            return (true, null);
        }

        /// <summary>
        /// Gets or sets the ICatchLogger to use when an exception is encountered via the TryCatcher wrappers.
        /// </summary>
        public static ICatchLogger Logger { get; set; }

        public interface ICatchLogger
        {
            void LogError(Exception ex);
        }

        /// <summary>
        /// Set the ICatchLogger to use when an exception is encountered.
        /// </summary>
        /// <param name="logger"></param>
        public static void RegisterLogger(ICatchLogger logger)
        {
            Logger = logger;
        }
    }
}
