using System;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class RealtimeProcessor : TimerProcessor
    {
        private static readonly ObjectPool<RealtimeProcessor> _pool = new(() => new());

        public static RealtimeProcessor Create(RealtimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getTime, onFinished);
            return result;
        }


        public RealtimeTimerData TimerData;
        public Func<float> GetTime;

        private RealtimeProcessor() { }


        private void Init(RealtimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getTime, Action onFinished)
        {
            TickData = tickData;
            TickInfo = tickInfo;
            TimerData = timerData;
            GetTime = getTime;
            OnFinished = onFinished;
        }

        private void Reset()
        {
            TickData = null;
            TickInfo = null;
            TimerData = null;
            GetTime = null;
            OnFinished = null;
        }



        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var elapsedTime = time - TimerData.LastTime;
            if (elapsedTime < TimerData.Interval)
                return true;

            if (TickData.TotalTicks < 0)
            {
                return TickData.InvokeMode == InvokeMode.Multi
                    ? InfinityMultiInvoke(time)
                    : InfinitySingleInvoke(time);
            }
            else
            {
                return TickData.InvokeMode == InvokeMode.Multi
                    ? LimitedMultiInvoke(time)
                    : LimitedSingleInvoke(time);
            }
        }


        private bool InfinityMultiInvoke(float time)
        {
            var elapsedTime = time - TimerData.LastTime;
            var interval = TimerData.Interval;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);

            return ProcessTicks(ticks);
        }

        private bool LimitedMultiInvoke(float time)
        {
            var elapsedTime = time - TimerData.LastTime;
            var interval = TimerData.Interval;
            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);
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
            var lastTime = TimerData.LastTime;
            var interval = TimerData.Interval;

            var result = true;

            TickInfo.TicksPerFrame = ticks;
            for (int i = 0; i < ticks; i++)
            {
                TimerData.LastTime = lastTime + (i + 1) * interval;
                TickInfo.TickIndex = i;
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


        private bool InfinitySingleInvoke(float time)
        {
            var elapsedTime = time - TimerData.LastTime;
            var interval = TimerData.Interval;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);

            TimerData.LastTime += ticks * interval;
            TickInfo.TicksPerFrame = ticks;
            TickInfo.TickIndex = -1;
            TickData.CompletedTicks++;

            TickData.OnTick?.Invoke();

            TickInfo.Reset();

            return true;
        }

        private bool LimitedSingleInvoke(float time)
        {
            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var elapsedTime = time - TimerData.LastTime;
            var interval = TimerData.Interval;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);

            var newCompletedTicks = completedTicks + ticks;
            if (newCompletedTicks >= totalTicks)
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
            time = TimerData.LastTime + ticks * interval;

            TimerData.LastTime = time;
            TickInfo.TicksPerFrame = ticks;
            TickInfo.TickIndex = -1;
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