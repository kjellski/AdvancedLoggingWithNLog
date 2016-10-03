using System;
using NLog;

namespace RealWorldExample.FastLane.Logging
{
    public class ThrottledLogEntry
    {
        public ThrottledLogEntry(string token, DateTime firstCaptured, DateTime lastCaptured, LogEventInfo logEventInfo, int count = 0)
        {
            Token = token;
            FirstCaptured = firstCaptured;
            LastCaptured = lastCaptured;
            LogEventInfo = logEventInfo;
            Count = count;
        }

        public int Count { get; private set; }

        public DateTime FirstCaptured { get; }

        public DateTime LastCaptured { get; private set; }

        public LogEventInfo LogEventInfo { get; }

        public string Token { get; }

        public void Increment()
        {
            LastCaptured = DateTime.UtcNow;
            Count++;
        }
    }
}