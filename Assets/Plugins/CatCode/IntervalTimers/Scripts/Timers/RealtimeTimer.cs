using CatCode.Timers;
using System;

namespace CatCode.Timers
{
    public sealed class RealtimeTimer : IntervalTimer
    {
        private readonly RealtimeTimerData _timerData = new();
        private Func<float> _getTime;

        public float LastTime
        {
            get => _timerData.LastTime;
            set
            {
                _timerData.LastTime = value;
                SetInitialized();
            }
        }

        public float Interval
        {
            get => _timerData.Interval;
            set => _timerData.Interval = value;
        }


        public RealtimeTimer()
        {
            _getTime = GetRealtimeStrategies.GetTime(RealtimeMode.Scaled);
        }

        public void SetTimeFunction(RealtimeMode mode)
            => _getTime = GetRealtimeStrategies.GetTime(mode);

        public void SetTimeFunction(Func<float> getTime)
            => _getTime = getTime;


        protected override void OnFirstStart()
        {
            _timerData.LastTime = _getTime();
            _tickData.CompletedTicks = 0;
        }

        protected override void OnReset()
        {
            _timerData.LastTime = 0;
            _tickData.CompletedTicks = 0;
        }

        protected override TimerProcessor GetProcessor(Action onFinished)
        {
            var processor = CreateProcessor(_mode);
            processor.Init(_timerData, _tickData, _tickInfo, ref _getTime, onFinished);
            return processor;
        }

        private RealtimeProcessor CreateProcessor(TimerMode mode)
            => mode switch
            {
                TimerMode.Dynamic => DynamicRealtimeProcessor.Create(),
                TimerMode.Multi => MultiInvokeRealtimeProcessor.Create(),
                TimerMode.Single => SingleInvokeRealtimeProcessor.Create(),
                _ => DynamicRealtimeProcessor.Create()
            };

    }
}