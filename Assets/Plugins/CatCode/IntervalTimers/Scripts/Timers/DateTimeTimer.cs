using System;

namespace CatCode.Timers
{
    public sealed class DateTimeTimer : IntervalTimer
    {
        public static DateTime GetDateTime() => DateTime.Now;

        private readonly DateTimeTimerData _data = new();
        private Func<DateTime> _getTime;

        public DateTime LastTime
        {
            get => _data.LastTime;
            set
            {
                _data.LastTime = value;
                SetInitialized();
            }
        }

        public TimeSpan Interval
        {
            get => _data.Interval;
            set => _data.Interval = value;
        }

        public void SetTimeFunction(Func<DateTime> getTime)
        {
            _getTime = getTime;
        }

        public DateTimeTimer()
        {
            _getTime = GetDateTime;
        }

        protected override void OnFirstStart()
        {
            _data.LastTime = _data.GetTime();
            _tickData.CompletedTicks = 0;
        }

        protected override void OnReset()
        {
            _data.LastTime = DateTime.MinValue;
            _tickData.CompletedTicks = 0;
        }

        protected override TimerProcessor GetProcessor(Action onFinished)
            => DateTimeProcessor.Create(_data, _tickData, _tickInfo, ref _getTime, onFinished);
    }
}