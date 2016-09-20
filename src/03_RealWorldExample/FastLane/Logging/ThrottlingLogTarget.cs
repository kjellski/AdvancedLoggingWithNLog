using NLog.Common;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Shared.Util.Logging
{
    [Target("TokenTimeThrottler", IsCompound = true)]
    public class ThrottlingLogTarget : CompoundTargetBase
    {
        private ITokenTimeThrottler _tokenTimeThrottler;

        public ThrottlingLogTarget()
            : this(new Target[0])
        {
        }

        public ThrottlingLogTarget(params Target[] targets)
            : base(targets)
        {
        }

        [RequiredParameter]
        public uint ThrottleSeconds { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            _tokenTimeThrottler = new TokenTimeThrottler(ThrottleSeconds);
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            if (Targets.Count == 0)
            {
                logEvent.Continuation(null);
                return;
            }

            if (_tokenTimeThrottler.CheckAllow(logEvent))
            {
                foreach (var target in Targets)
                {
                    target.WriteAsyncLogEvent(logEvent);
                }
            }
        }
    }
}