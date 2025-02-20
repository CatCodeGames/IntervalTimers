using Cysharp.Threading.Tasks;
using System;

namespace CatCode.Timers
{
    public abstract class IntervalTimer
    {
        private TimerState _state = TimerState.Idle;
        protected PlayerLoopTiming _playerLoopTiming = PlayerLoopTiming.Update;

        protected TimerProcessor _processor;

        protected readonly TimerTickData _tickData = new();
        protected readonly TimerTickInfo _tickInfo = new();

        public TimerState State
        {
            get => _state;
            private set
            {
                if (_state == value)
                    return;
                _state = value;
                switch (_state)
                {
                    case TimerState.Active:
                        Started?.Invoke();
                        break;
                    case TimerState.Paused:
                        Stopped?.Invoke();
                        break;
                    case TimerState.Completed:
                        Completed?.Invoke();
                        break;
                }
            }
        }

        public int TotalTicks
        {
            get => _tickData.TotalTicks;
            set => _tickData.TotalTicks = value;
        }

        public InvokeMode InvokeMode
        {
            get => _tickData.InvokeMode;
            set => _tickData.InvokeMode = value;
        }

        public PlayerLoopTiming PlayerLoopTimiming
        {
            get => _playerLoopTiming;
            set => _playerLoopTiming = value;
        }

        public int CompletedTicks
        {
            get => _tickData.CompletedTicks;
            set
            {
                _tickData.CompletedTicks = value;
                SetInitialized();
            }
        }

        public int TickIndex => _tickInfo.TickIndex;
        public int TicksPerFrame => _tickInfo.TicksPerFrame;

        public event Action Started;
        public event Action Stopped;
        public event Action Completed;
        public event Action Tick
        {
            add => _tickData.OnTick += value;
            remove => _tickData.OnTick -= value;
        }


        public void Start()
        {
            switch (_state)
            {
                case TimerState.Idle:
                    OnFirstStart();
                    State = TimerState.Active;
                    OnStart();
                    break;
                case TimerState.Initialized:
                case TimerState.Paused:
                    State = TimerState.Active;
                    OnStart();
                    break;
            }
        }

        public void Stop()
        {
            switch (_state)
            {
                case TimerState.Active:
                    _processor.IsActive = false;
                    _processor = null;
                    State = TimerState.Paused;
                    break;
            }
        }

        public void Reset()
        {
            if (_processor != null)
            {
                _processor.IsActive = false;
                _processor = null;
            }
            _tickData.Reset();
            OnReset();
            State = TimerState.Idle;
        }

        protected void SetInitialized()
        {
            if (_state == TimerState.Idle)
                _state = TimerState.Initialized;
        }

        private void OnStart()
        {
            _processor = GetProcessor(OnFinished);
            _processor.IsActive = true;
            PlayerLoopHelper.AddAction(_playerLoopTiming, _processor);
        }

        private void OnFinished()
        {
            _processor = null;
            State = TimerState.Completed;
        }

        protected abstract void OnFirstStart();
        protected abstract void OnReset();
        protected abstract TimerProcessor GetProcessor(Action onFinished);
    }
}