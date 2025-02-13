using System;


namespace CatCode.Timers
{
    public abstract class IntervalTimer
    {
        private TimerState _state;
        protected Action _elapsed;
        protected InvokeMode _invokeMode;

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

        public InvokeMode InvokeMode => _invokeMode;

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
                    OnStop();
                    State = TimerState.Paused;
                    break;
            }
        }

        public void Reset()
        {          
            OnReset();
        }

        protected abstract void OnFirstStart();
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void OnReset();
    }
}