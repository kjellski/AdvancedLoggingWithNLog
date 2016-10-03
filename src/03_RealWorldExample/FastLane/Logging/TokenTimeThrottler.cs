using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Common;
using NLog.Config;

namespace RealWorldExample.FastLane.Logging
{
    public interface ITokenTimeThrottler
    {
        bool CheckAllow(AsyncLogEventInfo token);
    }

    public class TokenTimeThrottler : ITokenTimeThrottler
    {
        private readonly Dictionary<string, ThrottledLogEntry> _throttledEntries = new Dictionary<string, ThrottledLogEntry>();

        public TokenTimeThrottler(uint throttleSeconds)
        {
            ThrottleSeconds = throttleSeconds;
        }

        [RequiredParameter]
        public uint ThrottleSeconds { get; }

        public bool CheckAllow(AsyncLogEventInfo asynclogEvent)
        {
            CleanupOldEntries();

            var logEvent = asynclogEvent.LogEvent;
            var token = logEvent.Message;

            lock (_throttledEntries)
            {
                ThrottledLogEntry entry;
                if (!_throttledEntries.TryGetValue(token, out entry))
                {
                    entry = new ThrottledLogEntry(token, DateTime.UtcNow, DateTime.UtcNow, logEvent);
                    _throttledEntries.Add(token, entry);
                }

                entry.Increment();

                if (entry.Count == 1)
                {
                    return true;
                }

                return false;
            }
        }

        private void CleanupOldEntries()
        {
            lock (_throttledEntries)
            {
                var now = DateTime.UtcNow;
                var throttledLogEntries = _throttledEntries.Values.ToList();
                var throttleTimeSpan = TimeSpan.FromSeconds(ThrottleSeconds);

                foreach (var throttledEntry in throttledLogEntries)
                {
                    if (throttledEntry.LastCaptured + throttleTimeSpan < now)
                    {
                        _throttledEntries.Remove(throttledEntry.Token);

                        if (throttledEntry.Count > 1)
                        {
                            var logEventInfo = throttledEntry.LogEventInfo;
                            logEventInfo.Message = $"Seen {throttledEntry.Count} times since {throttledEntry.FirstCaptured}, last seen {throttledEntry.LastCaptured}, unthrotteling: {logEventInfo.Message}";
                            var logger = LogManager.GetCurrentClassLogger();
                            if (logEventInfo.Exception != null)
                            {
                                logger.Log(logEventInfo.Level, logEventInfo.Exception, logEventInfo.Message);
                            }
                            else
                            {
                                logger.Log(logEventInfo.Level, logEventInfo.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}