using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;


namespace CatCode.Timers
{
    public abstract class IntervalTimer
    {
        private TimerState _state;
        protected readonly TimerTickData _tickData = new();

        protected InvokeMode _invokeMode;
        protected PlayerLoopTiming _playerLoopTiming;
        protected TimerProcessor _processor;
        protected Action _elapsed;

        public TimerState State
        {
            get => _state; set
            {
                if (_state == value) return;
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

        public TimerTickData TickData => _tickData;

        public InvokeMode InvokeMode
        {
            get => _invokeMode;
            set
            {
                _invokeMode = value;
                if (_processor != null)
                    _processor.InvokeMode = _invokeMode;
            }
        }

        public event Action Started;
        public event Action Stopped;
        public event Action Completed;
        public event Action Elapsed
        {
            add => _elapsed += value;
            remove => _elapsed -= value;
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

        private void OnStart()
        {
            _processor = GetProcessor(OnFinished);
            _processor.InvokeMode = _invokeMode;
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