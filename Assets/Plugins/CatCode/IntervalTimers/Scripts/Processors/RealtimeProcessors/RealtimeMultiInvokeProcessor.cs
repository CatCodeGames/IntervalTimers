using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class RealtimeMultiInvokeProcessor : RealtimeProcessor
    {
        private static readonly ObjectPool<RealtimeMultiInvokeProcessor> _pool = new(() => new());


        public static RealtimeMultiInvokeProcessor Create(RealtimeTimerData data, Func<float> getTime, Action onElapsed, Action onFinished)
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

        private bool InfinityMultiInvoke(float time)
        {
            var elapsedTime = time - Data.LastTime;
            var ticks = Mathf.FloorToInt(elapsedTime / Data.Interval);
            var lastTime = Data.LastTime;

            for (int i = 0; i < ticks; i++)
            {
                Data.LastTime = lastTime + (i + 1) * Data.Interval;
                Data.CompletedTicks++;
                OnElapsed?.Invoke();
                if (!IsActive)
                    return false;
            }
            return true;
        }

        private bool LimitedMultiInvoke(float time)
        {
            var elapsedTime = time - Data.LastTime;
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / Data.Interval);
            var newCompletedTicks = completedTicks + ticks;

            Data.LastTime += ticks * Data.Interval;
            if (newCompletedTicks >= totalTicks)
            {
                var targetLoops = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                ProcessLoops(targetLoops);
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
            var lastTime = Data.LastTime;
            for (int i = 0; i < ticks; i++)
            {
                Data.LastTime = lastTime + (i + 1) * Data.Interval;
                Data.CompletedTicks++;
                OnElapsed?.Invoke();
                if (!IsActive)
                    break;
            }
        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }
}