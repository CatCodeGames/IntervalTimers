using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DateTimeProcessor : TimerProcessor
    {
        public Func<DateTime> GetTime;
        public DateTimeTimerData Data;

        private void Init(DateTimeTimerData data, TimerTickData tickData, Func<DateTime> getTime, Action onElapsed, Action onFinished)
        {
            Data = data;
            TickData = tickData;
            GetTime = getTime;
            OnElapsed = onElapsed;
            OnFinished = onFinished;
        }

        private void Reset()
        {
            Data = null;
            TickData = null;
            GetTime = null;
            OnElapsed = null;
            OnFinished = null;
        }

        private static readonly ObjectPool<DateTimeProcessor> _pool = new(() => new());

        public static DateTimeProcessor Create(DateTimeTimerData data, TimerTickData tickData, Func<DateTime> getTime, Action onElapsed, Action onFinished)
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

        private bool InfinityMultiInvoke(DateTime time)
        {
            var elapsedTime = time - Data.LastTime;
            var ticks = (int)Math.Floor(elapsedTime.TotalMilliseconds / Data.Interval.TotalMilliseconds);
            return ProcessTicks(ticks);
        }

        private bool LimitedMultiInvoke(DateTime time)
        {
            var elapsedTime = time - Data.LastTime;
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var ticks = (int)(elapsedTime / Data.Interval);
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
            var lastTime = Data.LastTime;
            var result = true;

            TickData.TicksPerFrame = ticks;
            for (int i = 0; i < ticks; i++)
            {
                TickData.TickNumber = i + 1;
                Data.LastTime = lastTime + (i + 1) * Data.Interval;
                Data.CompletedTicks++;

                OnElapsed?.Invoke();

                if (!IsActive)
                {
                    result = false;
                    break;
                }
            }
            TickData.Reset();
            return result;
        }


        private bool InfinitySingleInvoke(DateTime time)
        {
            var elapsedTime = time - Data.LastTime;
            var ticks = (int)Math.Floor(elapsedTime / Data.Interval);

            TickData.TicksPerFrame = ticks;
            TickData.TickNumber = 0;
            Data.CompletedTicks++;
            Data.LastTime += ticks * Data.Interval;

            OnElapsed?.Invoke();

            TickData.Reset();
            return true;
        }

        private bool LimitedSingleInvoke(DateTime time)
        {
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var elapsedTime = time - Data.LastTime;
            var ticks = (int)Math.Floor(elapsedTime / Data.Interval);

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