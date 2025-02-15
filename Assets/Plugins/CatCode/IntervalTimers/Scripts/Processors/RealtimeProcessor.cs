using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public class RealtimeProcessor : TimerProcessor
    {
        public Func<float> GetTime;
        public RealtimeTimerData Data;

        protected void Init(RealtimeTimerData data, TimerTickData tickData, Func<float> getTime, Action onElapsed, Action onFinished)
        {
            Data = data;
            TickData = tickData;
            GetTime = getTime;
            OnElapsed = onElapsed;
            OnFinished = onFinished;
        }

        protected void Reset()
        {
            Data = null;
            TickData = null;
            GetTime = null;
            OnElapsed = null;
            OnFinished = null;
        }


        private static readonly ObjectPool<RealtimeProcessor> _pool = new(() => new());

        public static RealtimeProcessor Create(RealtimeTimerData data, TimerTickData tickData, Func<float> getTime, Action onElapsed, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(data, tickData, getTime, onElapsed, onFinished);
            return result;
        }

        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var elapsedTime = time - Data.LastTime;
            if (elapsedTime < Data.Interval)
                return true;


            if (Data.TotalTicks < 0)
            {
                return InvokeMode == InvokeMode.Multi
                    ? InfinityMultiInvoke(time)
                    : InfinitySingleInvoke(time);
            }
            else
            {
                return InvokeMode == InvokeMode.Multi
                    ? LimitedMultiInvoke(time)
                    : LimitedSingleInvoke(time);
            }
        }

        private bool InfinityMultiInvoke(float time)
        {
            var elapsedTime = time - Data.LastTime;
            var ticks = Mathf.FloorToInt(elapsedTime / Data.Interval);
            var lastTime = Data.LastTime;

            TickData.TicksPerFrame = ticks;
            for (int i = 0; i < ticks; i++)
            {
                TickData.TickNumber = i + 1;
                Data.LastTime = lastTime + (i + 1) * Data.Interval;
                Data.CompletedTicks++;

                OnElapsed?.Invoke();

                if (!IsActive)
                    return false;
            }
            TickData.Reset();
            return true;
        }

        private bool LimitedMultiInvoke(float time)
        {
            var elapsedTime = time - Data.LastTime;
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / Data.Interval);
            var newCompletedTicks = completedTicks + ticks;

            if (newCompletedTicks >= totalTicks)
            {
                var targetTicks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                TickData.TicksPerFrame = targetTicks;
                ProcessTicks(targetTicks);
                TickData.Reset();
                return false;
            }
            else
            {
                TickData.TicksPerFrame = ticks;
                ProcessTicks(ticks);
                TickData.Reset();
                return true;
            }
        }

        private void ProcessTicks(int ticks)
        {
            var lastTime = Data.LastTime;

            for (int i = 0; i < ticks; i++)
            {
                TickData.TickNumber = i + 1;
                Data.LastTime = lastTime + (i + 1) * Data.Interval;
                Data.CompletedTicks++;

                OnElapsed?.Invoke();

                if (!IsActive)
                    return;
            }
        }

        private bool InfinitySingleInvoke(float time)
        {
            var elapsedTime = time - Data.LastTime;
            var ticks = Mathf.FloorToInt(elapsedTime / Data.Interval);

            TickData.TicksPerFrame = ticks;
            TickData.TickNumber = 0;
            Data.CompletedTicks++;
            Data.LastTime += ticks * Data.Interval;

            OnElapsed?.Invoke();

            TickData.Reset();
            return true;
        }

        private bool LimitedSingleInvoke(float time)
        {
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var elapsedTime = time - Data.LastTime;
            var ticks = Mathf.FloorToInt(elapsedTime / Data.Interval);

            var newCompletedTicks = completedTicks + ticks;
            if (newCompletedTicks >= totalTicks)
            {
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                time = Data.LastTime + ticks * Data.Interval;
            }

            TickData.TicksPerFrame = ticks;
            TickData.TickNumber = 0;
            Data.CompletedTicks += ticks;
            Data.LastTime = time;

            OnElapsed?.Invoke();

            TickData.Reset();
            return Data.CompletedTicks < Data.TotalTicks;

        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }
}