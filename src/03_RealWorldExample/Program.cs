using System;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace RealWorldExample
{
    public class Program
    {
        private static readonly int _sleepInMs = 0 * 1000;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Log.Info("Sample informational message");
            Log.Info("Sample informational message");
            Log.Info("Sample informational message");
            Log.Info("Sample informational message");

            Thread.Sleep(_sleepInMs);
            Log.Info("Sample informational message");

            // alternatively you can call the Log() method and pass log level as the parameter.
            Log.Log(LogLevel.Info, "Sample informational message");

            Console.WriteLine("Press <Enter> to exit...");
            Console.ReadLine();
        }
    }
}