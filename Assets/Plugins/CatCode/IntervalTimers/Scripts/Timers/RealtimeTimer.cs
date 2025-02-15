using Cysharp.Threading.Tasks;
using System;

namespace CatCode.Timers
{
    public sealed class RealtimeTimer : IntervalTimer
    {
        private readonly RealtimeTimerData _data = new();
        private Func<float> _getTime;

        public RealtimeTimerData Data => _data;

        public static RealtimeTimer Create(float interval, int loopsCount = 1, InvokeMode invokeMode = InvokeMode.Single, RealtimeMode timeMode = RealtimeMode.Scaled, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
        {
            var timer = new RealtimeTimer();
            timer.Init(interval, loopsCount, invokeMode, timeMode, playerLoopTiming);
            return timer;
        }

        public static RealtimeTimer Create(float interval, Func<float> getTime, int loopsCount = 1, InvokeMode invokeMode = InvokeMode.Single, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
        {
            var timer = new RealtimeTimer();
            timer.Init(interval, getTime, loopsCount, invokeMode, playerLoopTiming);
            return timer;
        }

        private RealtimeTimer() { }

        public void Init(float interval, int loopsCount, InvokeMode invokeMode, RealtimeMode timeMode, PlayerLoopTiming playerLoopTiming)
        {
            var getTime = GetRealtimeStrategies.GetTime(timeMode);
            Init(interval, getTime, loopsCount, invokeMode, playerLoopTiming);
        }

        public void Init(float interval, Func<float> getTime, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
        {
            Stop();

            _data.LastTime = 0f;
            _data.CompletedTicks = 0;

            _data.Interval = interval;
            _data.TotalTicks = loopsCount;

            _invokeMode = invokeMode;
            _getTime = getTime;
            _playerLoopTiming = playerLoopTiming;
        }

        protected override void OnFirstStart()
        {
            _data.LastTime = _getTime();
            _data.CompletedTicks = 0;
        }

        protected override void OnReset()
        {
            _data.LastTime = 0f;
            _data.CompletedTicks = 0;
        }

        protected override TimerProcessor GetProcessor(Action onFinished)
            => RealtimeProcessor.Create(_data, _tickData, _getTime, _elapsed, onFinished);
    }
}