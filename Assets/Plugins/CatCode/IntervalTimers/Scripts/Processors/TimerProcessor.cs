using Cysharp.Threading.Tasks;
using System;

namespace CatCode.Timers
{
    public abstract class TimerProcessor : IPlayerLoopItem
    {
        public bool IsActive;
        public Action OnFinished;
        public Action OnElapsed;

        public bool MoveNext()
        {
            if (!IsActive)
            {
                OnStopped();
                return false;
            }

            if (MoveNextCore())
                return true;
            else
            {
                IsActive = false;
                OnFinished?.Invoke();
                OnStopped();
                return false;
            }
        }

        protected abstract bool MoveNextCore();
        protected abstract void OnStopped();
    }

}