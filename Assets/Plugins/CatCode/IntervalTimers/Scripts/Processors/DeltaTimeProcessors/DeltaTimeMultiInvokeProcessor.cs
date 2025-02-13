using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DeltaTimeMultiInvokeProcessor : DeltaTimeProcessor
    {
        private static readonly ObjectPool<DeltaTimeMultiInvokeProcessor> _pool = new(() => new());

        public static DeltaTimeMultiInvokeProcessor Create(DeltaTimeTimerData data, Func<float> getDeltaTime, Action onElapsed, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(data, getDeltaTime, onElapsed, onFinished);
            return result;
        }

        protected override bool MoveNextCore()
        {
            var deltaTime = GetDeltaTime();
            Data.ElapsedTime += deltaTime;
            if (Data.ElapsedTime < Data.Interval)
                return true;

            return Data.TotalTicks < 0
                ? InfinityMultiInvokeStrategy()
                : LimitedMultiInvokeStrategy();
        }

        private bool InfinityMultiInvokeStrategy()
        {
            var elapsedTime = Data.ElapsedTime;
            var interval = Data.Interval;

            int ticks = Mathf.FloorToInt(elapsedTime / interval);
            var remainingTime = elapsedTime % interval;

            for (int i = 0; i < ticks; i++)
            {
                Data.ElapsedTime = Data.Interval * (i + 1);
                Data.CompletedTicks++;
                OnElapsed?.Invoke();
                if (!IsActive)
                    return false;
            }
            Data.ElapsedTime = remainingTime;

            return true;
        }

        private bool LimitedMultiInvokeStrategy()
        {
            var elapsedTime = Data.ElapsedTime;
            var interval = Data.Interval;
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);

            var newCompletedTicks = completedTicks + ticks;
            if (newCompletedTicks >= totalTicks)
            {
                var targetLoops = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                ProcessLoops(targetLoops);
                return false;
            }
            else
            {
                ProcessLoops(ticks);
                Data.ElapsedTime = elapsedTime % interval;
                return true;
            }
        }

        private void ProcessLoops(int ticks)
        {
            for (int i = 0; i < ticks; i++)
            {
                Data.ElapsedTime = Data.Interval * (i + 1);
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