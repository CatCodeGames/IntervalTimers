using Cysharp.Threading.Tasks;
using System;

namespace CatCode.Timers
{
    public sealed class DateTimeTimer : IntervalTimer
    {
        private readonly DateTimeTimerData _data = new();
        private Func<DateTime> _getTime;

        public DateTimeTimerData Data => _data;

        private static DateTime GetDateTime() => DateTime.Now;

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

        private DateTimeTimer() { }

        public void Init(TimeSpan interval, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
        {
            Init(interval, GetDateTime, loopsCount, invokeMode, playerLoopTiming);
        }

        public void Init(TimeSpan interval, Func<DateTime> getTime, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
        {
            Stop();

            _data.LastTime = DateTime.MinValue;
            _data.CompletedTicks = 0;
            _data.Interval = interval;
            _data.TotalTicks = loopsCount;

            _tickData.Reset();

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
            _data.LastTime = DateTime.MinValue;
            _data.CompletedTicks = 0;
        }

        protected override TimerProcessor GetProcessor(Action onFinished)
            => DateTimeProcessor.Create(_data, _tickData, _getTime, _elapsed, onFinished);
    }
}