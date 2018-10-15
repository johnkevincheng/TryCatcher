using System;
using RockFluid;

namespace TryCatcherTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var staticFunctionTest = TryCatcher.RunFunction(() => StaticFunction("Value from a function call."));
            if (staticFunctionTest.RunSuccessful)
                Console.WriteLine(staticFunctionTest.Value);


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
                var err = 0;
                var xx = 2 / err;
            };

            TryCatcher.RunMethod(errorFunction);
            TryCatcher.Logger = new TestLogger();
            TryCatcher.RunMethod(errorFunction);


            Console.Write("\nEnd tests. Press any key to exit...");
            Console.ReadKey();
        }

        public static String StaticFunction(String param)
        {
            return param;
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
    }
}
