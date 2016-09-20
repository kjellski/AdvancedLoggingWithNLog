using System;
using NLog;

namespace Shared.Util.Logging
{
    public class ThrottledLogEntry
    {
        public ThrottledLogEntry(string md5Sum, DateTime firstCaptured, DateTime lastCaptured, LogEventInfo logEventInfo, int count = 0)
        {
            Md5Sum = md5Sum;
            FirstCaptured = firstCaptured;
            LastCaptured = lastCaptured;
            LogEventInfo = logEventInfo;
            Count = count;
        }

        public int Count { get; private set; }

        public DateTime FirstCaptured { get; }

        public DateTime LastCaptured { get; private set; }

        public LogEventInfo LogEventInfo { get; }

        public string Md5Sum { get; }

        public void Increment()
        {
            LastCaptured = DateTime.UtcNow;
            Count++;
        }
    }
}