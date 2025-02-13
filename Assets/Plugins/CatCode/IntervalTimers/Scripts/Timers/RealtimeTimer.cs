using Cysharp.Threading.Tasks;
using System;

namespace CatCode.Timers
{
    public sealed class RealtimeTimer : IntervalTimer
    {
        private readonly RealtimeTimerData _data = new();
        private RealtimeProcessor _processor;

        private Func<float> _getTime;
        private PlayerLoopTiming _playerLoopTiming;

        public RealtimeTimerData Data => _data;

        private RealtimeTimer() { }

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

        protected override void OnStart()
        {
            _processor = GetProcessor();
            _processor.IsActive = true;
            PlayerLoopHelper.AddAction(_playerLoopTiming, _processor);
        }

        protected override void OnStop()
        {
            _processor.IsActive = false;
            _processor = null;
        }

        protected override void OnReset()
        {
            _data.LastTime = 0f;
            _data.CompletedTicks = 0;
        }

        private void OnFinished()
        {
            _processor = null;
            State = TimerState.Completed;
        }

        private RealtimeProcessor GetProcessor()
            => _invokeMode switch
            {
                InvokeMode.Single => RealtimeSingleInvokeProcessor.Create(_data, _getTime, _elapsed, OnFinished),
                InvokeMode.Multi => RealtimeMultiInvokeProcessor.Create(_data, _getTime, _elapsed, OnFinished),
                _ => RealtimeSingleInvokeProcessor.Create(_data, _getTime, _elapsed, OnFinished),
            };
    }
}