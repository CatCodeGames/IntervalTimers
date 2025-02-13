using Cysharp.Threading.Tasks;
using System;

namespace CatCode.Timers
{
    public sealed class DateTimeTimer : IntervalTimer
    {
        private readonly DateTimeTimerData _data = new();
        private DateTimeProcessor _processor;

        private Func<DateTime> _getTime;
        private PlayerLoopTiming _playerLoopTiming;

        public DateTimeTimerData Data => _data;

        private DateTimeTimer() { }

        public static DateTimeTimer Create(TimeSpan interval, int loopsCount = 1, InvokeMode invokeMode = InvokeMode.Single, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
        {
            var timer = new DateTimeTimer();
            timer.Init(interval, loopsCount, invokeMode, playerLoopTiming);
            return timer;
        }

        public static DateTimeTimer Create(TimeSpan interval, Func<DateTime> getTime, int loopsCount = 1, InvokeMode invokeMode = InvokeMode.Single, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update)
        {
            var timer = new DateTimeTimer();
            timer.Init(interval, getTime, loopsCount, invokeMode, playerLoopTiming);
            return timer;
        }

        public void Init(TimeSpan interval, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
        {
            Func<DateTime> getTime = () => DateTime.Now;
            Init(interval, getTime, loopsCount, invokeMode, playerLoopTiming);
        }

        public void Init(TimeSpan interval, Func<DateTime> getTime, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
        {
            Stop();

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
            _data.LastTime = DateTime.MinValue;
            _data.CompletedTicks = 0;
        }

        private void OnFinished()
        {
            _processor = null;
            State = TimerState.Completed;
        }

        private DateTimeProcessor GetProcessor()
            => _invokeMode switch
            {
                InvokeMode.Single => DateTimeSingleInvokeProcessor.Create(_data, _getTime, _elapsed, OnFinished),
                InvokeMode.Multi => DateTimeMultiInvokeProcessor.Create(_data, _getTime, _elapsed, OnFinished),
                _ => DateTimeSingleInvokeProcessor.Create(_data, _getTime, _elapsed, OnFinished),
            };
    }
}