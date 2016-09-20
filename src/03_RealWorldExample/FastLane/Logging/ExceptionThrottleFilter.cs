using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Filters;

namespace Shared.Util.Logging
{
    [Filter("throttle")]
    public class ExceptionThrottleFilter : Filter
    {
        private readonly Dictionary<string, ThrottledLogEntry> _thorttledEntries = new Dictionary<string, ThrottledLogEntry>();

        [RequiredParameter]
        public uint ThrottleSeconds { get; set; }

        protected override FilterResult Check(LogEventInfo logEvent)
        {
            CleanupOldEntries();

            var token = logEvent.Message;

            lock (_thorttledEntries)
            {
                ThrottledLogEntry entry;
                if (!_thorttledEntries.TryGetValue(token, out entry))
                {
                    entry = new ThrottledLogEntry(token, DateTime.UtcNow, DateTime.UtcNow, logEvent);
                    _thorttledEntries.Add(token, entry);
                }

                entry.Increment();

                if (entry.Count == 1)
                {
                    return FilterResult.Log;
                }

                return FilterResult.Ignore;
            }
        }

        private void CleanupOldEntries()
        {
            lock (_thorttledEntries)
            {
                var now = DateTime.UtcNow;
                var throttledLogEntries = _thorttledEntries.Values.ToList();
                var throttleTimeSpan = TimeSpan.FromSeconds(ThrottleSeconds);

                foreach (var throttledEntry in throttledLogEntries)
                {
                    if (throttledEntry.LastCaptured + throttleTimeSpan < now)
                    {
                        _thorttledEntries.Remove(throttledEntry.Md5Sum);

                        if (throttledEntry.Count > 1)
                        {
                            var logEventInfo = throttledEntry.LogEventInfo;
                            logEventInfo.Message = $"\nSeen {throttledEntry.Count} times since {throttledEntry.FirstCaptured}, last seen {throttledEntry.LastCaptured}, unthrotteling:\n{logEventInfo.Message}";
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