using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DateTimeMultiInvokeProcessor : DateTimeProcessor
    {
        private static readonly ObjectPool<DateTimeMultiInvokeProcessor> _pool = new(() => new());

        public static DateTimeMultiInvokeProcessor Create(DateTimeTimerData data, Func<DateTime> getTime, Action onElapsed, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(data, getTime, onElapsed, onFinished);
            return result;
        }

        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var elapsedTime = time - Data.LastTime;
            if (elapsedTime < Data.Interval)
                return true;

            return Data.TotalTicks < 0
                ? InfinityMultiInvoke(time)
                : LimitedMultiInvoke(time);
        }

        private bool InfinityMultiInvoke(DateTime time)
        {
            var elapsedTime = time - Data.LastTime;
            var ticks = (int)Math.Floor(elapsedTime.TotalMilliseconds / Data.Interval.TotalMilliseconds);

            Data.LastTime += ticks * Data.Interval;
            for (int i = 0; i < ticks; i++)
            {
                Data.CompletedTicks++;
                OnElapsed?.Invoke();
            }
            return true;
        }

        private bool LimitedMultiInvoke(DateTime time)
        {
            var elapsedTime = time - Data.LastTime;
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var ticks = (int)(elapsedTime / Data.Interval);
            var newCompletedTicks = completedTicks + ticks;

            Data.LastTime += ticks * Data.Interval;
            if (newCompletedTicks >= totalTicks)
            {
                var targetTicks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                ProcessLoops(targetTicks);
                return false;
            }
            else
            {
                ProcessLoops(ticks);
                return true;
            }
        }

        private void ProcessLoops(int ticks)
        {
            for (int i = 0; i < ticks; i++)
            {
                Data.CompletedTicks++;
                OnElapsed?.Invoke();
            }
        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }
}