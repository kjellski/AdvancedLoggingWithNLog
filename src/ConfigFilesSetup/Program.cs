using System;
using NLog;

namespace ConfigFilesSetup
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Log.Trace("Sample trace message");
            Log.Debug("Sample debug message");
            Console.ReadLine();
            Log.Info("Sample informational message");
            Log.Warn("Sample warning message");
            Console.ReadLine();
            Log.Error("Sample error message");
            Log.Fatal("Sample fatal error message");

            // alternatively you can call the Log() method and pass log level as the parameter.
            Log.Log(LogLevel.Info, "Sample informational message");

            Console.WriteLine("Press <Enter> to exit...");
            Console.ReadLine();
        }
    }
}