using System;

namespace RockFluid
{
    public static class TryCatcher
    {
        public static (Boolean RunSuccessful, T Value, Exception Exception) RunFunction<T>(Func<T> function)
        {
            T returnValue;

            try
            {
                returnValue = function.Invoke();
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError(ex);

                return (false, default(T), ex);
            }

            return (true, returnValue, null);
        }

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

        public static ICatchLogger Logger { get; set; }

        public interface ICatchLogger
        {
            void LogError(Exception ex);
        }
    }
}
