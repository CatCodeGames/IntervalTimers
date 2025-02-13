using Cysharp.Threading.Tasks;
using System;

namespace CatCode.Timers
{
    public sealed class DeltaTimeTimer : IntervalTimer
    {
        private readonly DeltaTimeTimerData _data = new();
        private DeltaTimeProcessor _processor;

        private Func<float> _getDeltaTime;
        private PlayerLoopTiming _playerLoopTiming;

        public DeltaTimeTimerData Data => _data;

        private DeltaTimeTimer()
        { }

        public static DeltaTimeTimer Create(float interval, int loopCount = 1, InvokeMode invokeMode = InvokeMode.Single, bool unscaledTime = false, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
        {
            var timer = new DeltaTimeTimer();
            timer.Init(interval, loopCount, invokeMode, unscaledTime, playerLoopTiming);
            return timer;
        }

        public static DeltaTimeTimer Create(float interval, Func<float> getDeltaTime, int loopCount = 1, InvokeMode invokeMode = InvokeMode.Single, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
        {
            var timer = new DeltaTimeTimer();
            timer.Init(interval, getDeltaTime, loopCount, invokeMode, playerLoopTiming);
            return timer;
        }

        public void Init(float interval, int loopCount, InvokeMode invokeMode, bool unscaledTime, PlayerLoopTiming playerLoopTiming)
        {
            _getDeltaTime = unscaledTime
                ? GetDeltaTimeStrategies.GetUnscaledDeltaTime
                : GetDeltaTimeStrategies.GetScaledDeltaTime;
            Init(interval, _getDeltaTime, loopCount, invokeMode, playerLoopTiming);
        }

        public void Init(float interval, Func<float> getDeltaTime, int loopCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
        {
            Stop();

            _data.ElapsedTime = 0f;
            _data.CompletedTicks = 0;
            _data.Interval = interval;
            _data.TotalTicks = loopCount;

            _invokeMode = invokeMode;
            _getDeltaTime = getDeltaTime;
            _playerLoopTiming = playerLoopTiming;
        }


        protected override void OnFirstStart()
        {
            _data.ElapsedTime = 0;
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
            _data.ElapsedTime = 0;
            _data.CompletedTicks = 0;
        }

        private void OnFinished()
        {
            _processor = null;
            State = TimerState.Completed;
        }

        private DeltaTimeProcessor GetProcessor()
            => _invokeMode switch
            {
                InvokeMode.Single => DeltaTimeSingleInvokeProcessor.Create(_data, _getDeltaTime, _elapsed, OnFinished),
                InvokeMode.Multi => DeltaTimeMultiInvokeProcessor.Create(_data, _getDeltaTime, _elapsed, OnFinished),
                _ => DeltaTimeSingleInvokeProcessor.Create(_data, _getDeltaTime, _elapsed, OnFinished),
            };
    }
}