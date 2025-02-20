using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DeltaTimeProcessor : TimerProcessor
    {

        private static readonly ObjectPool<DeltaTimeProcessor> _pool = new(() => new());

        public static DeltaTimeProcessor Create(DeltaTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getDeltaTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getDeltaTime, onFinished);
            return result;
        }


        public DeltaTimeTimerData TimerData;
        public Func<float> GetDeltaTime;

        private DeltaTimeProcessor() { }

        private void Init(DeltaTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getDeltaTime, Action onFinished)
        {
            TickData = tickData;
            TickInfo = tickInfo;
            TimerData = timerData;
            GetDeltaTime = getDeltaTime;
            OnFinished = onFinished;
        }

        private void Reset()
        {
            TickData = null;
            TickInfo = null;
            TimerData = null;
            GetDeltaTime = null;
            OnFinished = null;
        }


        protected override bool MoveNextCore()
        {
            var deltaTime = GetDeltaTime();
            TimerData.ElapsedTime += deltaTime;
            if (TimerData.ElapsedTime < TimerData.Interval)
                return true;

            if (TickData.TotalTicks < 0)
            {
                return TickData.InvokeMode == InvokeMode.Multi
                    ? InfinityMultiInvoke()
                    : InfinitySingleInvoke();
            }
            else
            {
                return TickData.InvokeMode == InvokeMode.Multi
                    ? LimitedMultiInvoke()
                    : LimitedSingleInvoke();
            }
        }


        private bool InfinityMultiInvoke()
        {
            int ticks = Mathf.FloorToInt(TimerData.ElapsedTime / TimerData.Interval);
            TickInfo.TicksPerFrame = ticks;
            return ProcessTicks(ticks);
        }

        private bool LimitedMultiInvoke()
        {
            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            int ticks = Mathf.FloorToInt(TimerData.ElapsedTime / TimerData.Interval);

            var newCompletedTicks = completedTicks + ticks;
            if (newCompletedTicks >= totalTicks)
            {
                var targetTicks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                ProcessTicks(targetTicks);
                return false;
            }
            else
            {
                return ProcessTicks(ticks);
            }
        }

        private bool ProcessTicks(int ticks)
        {
            var elapsedTime = TimerData.ElapsedTime;
            var result = true;
            TickInfo.TicksPerFrame = ticks;
            for (int i = 0; i < ticks; i++)
            {
                TickInfo.TickIndex = i;
                TimerData.ElapsedTime = elapsedTime - TimerData.Interval * (i + 1);
                TickData.CompletedTicks++;

                TickData.OnTick?.Invoke();

                if (!IsActive)
                {
                    result = false;
                    break;
                }
            }
            TickInfo.Reset();
            return result;
        }


        private bool InfinitySingleInvoke()
        {
            var elapsedTime = TimerData.ElapsedTime;
            var interval = TimerData.Interval;

            int ticks = Mathf.FloorToInt(elapsedTime / interval);
            var remainingTime = elapsedTime % interval;

            TimerData.ElapsedTime = remainingTime;

            TickInfo.TicksPerFrame = ticks;
            TickInfo.TickIndex = -1;
            TickData.CompletedTicks += ticks;

            TickData.OnTick?.Invoke();

            TickInfo.Reset();

            return true;
        }

        private bool LimitedSingleInvoke()
        {
            var elapsedTime = TimerData.ElapsedTime;
            var interval = TimerData.Interval;
            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);
            var remainingTime = elapsedTime % interval;

            var newCompletedTicks = completedTicks + ticks;
            if (newCompletedTicks >= totalTicks)
            {
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                remainingTime = ticks * interval;
            }

            TimerData.ElapsedTime = remainingTime;

            TickInfo.TickIndex = -1;
            TickInfo.TicksPerFrame = ticks;
            TickData.CompletedTicks += ticks;

            TickData.OnTick?.Invoke();

            TickInfo.Reset();

            return TickData.CompletedTicks < TickData.TotalTicks;
        }


        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }
}