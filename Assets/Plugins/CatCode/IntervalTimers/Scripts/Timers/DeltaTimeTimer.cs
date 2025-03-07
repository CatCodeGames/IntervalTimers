using CatCode.Timers;
using System;

namespace CatCode.Timers
{
    public sealed class DeltaTimeTimer : IntervalTimer
    {
        private readonly DeltaTimeTimerData _data = new();
        private Func<float> _getDeltaTime;

        public float Interval
        {
            get => _data.Interval;
            set => _data.Interval = value;
        }

        public float ElapsedTime
        {
            get => _data.ElapsedTime;
            set
            {
                _data.ElapsedTime = value;
                SetInitialized();
            }
        }

        public DeltaTimeTimer()
        {
            _getDeltaTime = GetDeltaTimeStrategies.GetDeltaTimeStrategy(false);
        }


        public void SetDeltaTimeFunction(bool unscaled)
        {
            _getDeltaTime = GetDeltaTimeStrategies.GetDeltaTimeStrategy(unscaled);
        }

        public void SetDeltaTimeFunction(Func<float> getDeltaTime)
        {
            _getDeltaTime = getDeltaTime;
        }


        protected override void OnFirstStart()
        {
            _data.ElapsedTime = 0;
        }

        protected override void OnReset()
        {
            _data.ElapsedTime = 0;
        }

        protected override TimerProcessor GetProcessor(Action onFinished)
        {
            var processor = CreateProcessor(_mode);
            processor.Init(_data, _tickData, _tickInfo, ref _getDeltaTime, onFinished);
            return processor;
        }


        private DeltaTimeProcessor CreateProcessor(TimerMode mode)
            => mode switch
            {
                TimerMode.Dynamic => DynamicDeltaTimeProcessor.Create(),
                TimerMode.Multi => MultiInvokeDeltaTimeProcessor.Create(),
                TimerMode.Single => SingleInvokeDeltaTimeProcessor.Create(),
                _ => DynamicDeltaTimeProcessor.Create(),
            };
    }
}