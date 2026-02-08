using System;
using Microsoft.Extensions.Logging;
using RockFluid;

namespace TryCatcherTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var staticFunctionTest = ReCatcher.With().Try<string>(() => StaticFunction("Value from a function call."));
            if (staticFunctionTest.IsSuccess)
                Console.WriteLine(staticFunctionTest.Value);


            ReCatcher.With().Try(() =>
                StaticMethod("Value from a method call.")
            );


            var o = new MyTestClass();
            o.RunNonStatic();

            var objectMemberTest = ReCatcher.With().Try<string>(() =>
                o.NonStaticFunction("Value from instantiated class' function.")
            );
            Console.WriteLine(objectMemberTest.Value);


            var anonymousFunctionTest = ReCatcher.With().Try<string>(() =>
            {
                var msg = "Value from anonymous function delegate.";
                return msg;
            });
            if (anonymousFunctionTest.IsSuccess)
                Console.WriteLine(anonymousFunctionTest.Value);


            var anonymousMethodTest = ReCatcher.With().Try(() =>
            {
                var msg = "Value from an anonymous method delegate.";
                Console.WriteLine(msg);
            });


            Action actionDelegate = () =>
            {
                var msg = "Value from an Action delegate.";
                Console.WriteLine(msg);
            };
            var actionVarTest = ReCatcher.With().Try(actionDelegate);


            var testValue = "Value from outer scope.";
            var externalScopeTest = ReCatcher.With().Try(() =>
            {
                Console.WriteLine(testValue);
            });



            Action errorFunction = () =>
            {
                Console.WriteLine("Attempting to cause exception.");
                throw new Exception("I've caused an exception!");
            };

            ReCatcher.With().Try(errorFunction);
            ReCatcher.With().WithLogger(new TestLogger2()).Try(errorFunction);
            ReCatcher.With().WithLogger(new TestLogger1()).WithLogger(new TestLogger2()).Try(errorFunction);


            Console.Write("\nEnd tests. Press any key to exit...");
            Console.ReadKey();
        }

        private static void OldVersionTests()
        {
            var staticFunctionTest = TryCatcher.RunFunction(() => StaticFunction("Value from a function call."));
            if (staticFunctionTest.RunSuccessful)
                Console.WriteLine(staticFunctionTest.Value);


            TryCatcher.RunMethod(() =>
                StaticMethod("Value from a method call.")
            );


            var o = new MyTestClass();
            o.RunNonStatic();

            var objectMemberTest = TryCatcher.RunFunction(() =>
                o.NonStaticFunction("Value from instantiated class' function.")
            );
            Console.WriteLine(objectMemberTest.Value);


            var anonymousFunctionTest = TryCatcher.RunFunction(() =>
            {
                var msg = "Value from anonymous function delegate.";
                return msg;
            });
            if (anonymousFunctionTest.RunSuccessful)
                Console.WriteLine(anonymousFunctionTest.Value);


            var anonymousMethodTest = TryCatcher.RunMethod(() =>
            {
                var msg = "Value from an anonymous method delegate.";
                Console.WriteLine(msg);
            });


            Action actionDelegate = () =>
            {
                var msg = "Value from an Action delegate.";
                Console.WriteLine(msg);
            };
            var actionVarTest = TryCatcher.RunMethod(actionDelegate);


            var testValue = "Value from outer scope.";
            var externalScopeTest = TryCatcher.RunMethod(() =>
            {
                Console.WriteLine(testValue);
            });



            Action errorFunction = () =>
            {
                Console.WriteLine("Attempting to cause exception.");
                throw new Exception("I've caused an exception!");
            };

            TryCatcher.RunMethod(errorFunction);
            TryCatcher.RegisterLogger(new TestLogger());
            TryCatcher.RunMethod(errorFunction);
        }

        public static String StaticFunction(String param)
        {
            return param;
        }

        public static void StaticMethod(String msg)
        {
            Console.WriteLine(msg);
        }

        public class MyTestClass
        {
            public void RunNonStatic()
            {

                var result = TryCatcher.RunFunction(() => NonStaticFunction("Value from non-static function."));
                Console.WriteLine(result.Value);
            }

            public String NonStaticFunction(String param)
            {
                return param;
            }
        }

        private class TestLogger : TryCatcher.ICatchLogger
        {
            public void LogError(Exception ex)
            {
                Console.WriteLine($"Logged an error: {ex.Message}");
            }
        }

        private class TestLogger1 : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine($"Logged an error [A]: {exception.Message}");
            }
        }

        private class TestLogger2 : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine($"Logged an error [B]: {exception.Message}");
            }
        }
    }
}