using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace RealWorldExample
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            // Step 1. Create configuration object
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            // Step 3. Set target properties
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;

            Log.Trace("Sample trace message");
            Log.Debug("Sample debug message");
            Log.Info("Sample informational message");
            Log.Warn("Sample warning message");
            Log.Error("Sample error message");
            Log.Fatal("Sample fatal error message");

            // alternatively you can call the Log() method and pass log level as the parameter.
            Log.Log(LogLevel.Info, "Sample informational message");

            Console.WriteLine("Press <Enter> to exit...");
            Console.ReadLine();
        }
    }
}