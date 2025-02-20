using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DateTimeProcessor : TimerProcessor
    {
        private static readonly ObjectPool<DateTimeProcessor> _pool = new(() => new());

        public static DateTimeProcessor Create(DateTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<DateTime> getTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getTime, onFinished);
            return result;
        }

        public DateTimeTimerData TimerData;
        public Func<DateTime> GetTime;

        private void Init(DateTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<DateTime> getTime, Action onFinished)
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

        private bool InfinityMultiInvoke(DateTime time)
        {
            var elapsedTime = time - TimerData.LastTime;
            var ticks = (int)Math.Floor(elapsedTime.TotalMilliseconds / TimerData.Interval.TotalMilliseconds);
            return ProcessTicks(ticks);
        }

        private bool LimitedMultiInvoke(DateTime time)
        {
            var elapsedTime = time - TimerData.LastTime;
            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var ticks = (int)(elapsedTime / TimerData.Interval);
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
            var result = true;

            TickInfo.TicksPerFrame = ticks;
            for (int i = 0; i < ticks; i++)
            {
                TickInfo.TickIndex = i;
                TimerData.LastTime = lastTime + (i + 1) * TimerData.Interval;
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


        private bool InfinitySingleInvoke(DateTime time)
        {
            var elapsedTime = time - TimerData.LastTime;
            var ticks = (int)Math.Floor(elapsedTime / TimerData.Interval);

            TickInfo.TicksPerFrame = ticks;
            TickInfo.TickIndex = -1;
            TickData.CompletedTicks++;
            TimerData.LastTime += ticks * TimerData.Interval;

            TickData.OnTick?.Invoke();

            TickInfo.Reset();
            return true;
        }

        private bool LimitedSingleInvoke(DateTime time)
        {
            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var elapsedTime = time - TimerData.LastTime;
            var ticks = (int)Math.Floor(elapsedTime / TimerData.Interval);

            var newCompletedTicks = completedTicks + ticks;
            if (newCompletedTicks >= totalTicks)
            {
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                time = TimerData.LastTime + ticks * TimerData.Interval;
            }

            TickInfo.TicksPerFrame = ticks;
            TickInfo.TickIndex = -1;
            TickData.CompletedTicks += ticks;
            TimerData.LastTime = time;

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