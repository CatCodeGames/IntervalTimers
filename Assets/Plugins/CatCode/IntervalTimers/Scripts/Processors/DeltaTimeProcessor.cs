﻿using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DeltaTimeProcessor : TimerProcessor
    {
        public Func<float> GetDeltaTime;
        public DeltaTimeTimerData Data;


        private void Init(DeltaTimeTimerData data, TimerTickData tickData, Func<float> getDeltaTime, Action onElapsed, Action onFinished)
        {
            Data = data;
            TickData = tickData;
            GetDeltaTime = getDeltaTime;
            OnElapsed = onElapsed;
            OnFinished = onFinished;
        }

        private void Reset()
        {
            Data = null;
            TickData = null;
            GetDeltaTime = null;
            OnElapsed = null;
            OnFinished = null;
        }


        private static readonly ObjectPool<DeltaTimeProcessor> _pool = new(() => new());

        private DeltaTimeProcessor() { }

        public static DeltaTimeProcessor Create(DeltaTimeTimerData data, TimerTickData tickData, Func<float> getDeltaTime, Action onElapsed, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(data, tickData, getDeltaTime, onElapsed, onFinished);
            return result;
        }

        protected override bool MoveNextCore()
        {
            var deltaTime = GetDeltaTime();
            Data.ElapsedTime += deltaTime;
            if (Data.ElapsedTime < Data.Interval)
                return true;

            if (Data.TotalTicks < 0)
            {
                return InvokeMode == InvokeMode.Multi
                    ? InfinityMultiInvoke()
                    : InfinitySingleInvoke();
            }
            else
            {
                return InvokeMode == InvokeMode.Multi
                    ? LimitedMultiInvoke()
                    : LimitedSingleInvoke();
            }
        }



        private bool InfinityMultiInvoke()
        {
            int ticks = Mathf.FloorToInt(Data.ElapsedTime / Data.Interval);
            TickData.TicksPerFrame = ticks;
            return ProcessTicks(ticks);
        }

        private bool LimitedMultiInvoke()
        {
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            int ticks = Mathf.FloorToInt(Data.ElapsedTime / Data.Interval);

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
            TickData.TicksPerFrame = ticks;
            var elapsedTime = Data.ElapsedTime;
            var result = true;
            for (int i = 0; i < ticks; i++)
            {
                TickData.TickNumber = i + 1;
                Data.ElapsedTime = elapsedTime - Data.Interval * (i + 1);
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


        private bool InfinitySingleInvoke()
        {
            var elapsedTime = Data.ElapsedTime;
            var interval = Data.Interval;

            int ticks = Mathf.FloorToInt(elapsedTime / interval);
            var remainingTime = elapsedTime % interval;

            TickData.TicksPerFrame = ticks;
            TickData.TickNumber = 0;
            Data.CompletedTicks += ticks;
            Data.ElapsedTime = remainingTime;

            OnElapsed?.Invoke();

            TickData.Reset();
            return true;
        }

        private bool LimitedSingleInvoke()
        {
            var elapsedTime = Data.ElapsedTime;
            var interval = Data.Interval;
            var completedTicks = Data.CompletedTicks;
            var totalTicks = Data.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);
            var remainingTime = elapsedTime % interval;

            var newCompletedTicks = completedTicks + ticks;
            if (newCompletedTicks >= totalTicks)
            {
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                remainingTime = ticks * Data.Interval;
            }

            TickData.TicksPerFrame = ticks;
            TickData.TickNumber = 0;
            Data.CompletedTicks += ticks;
            Data.ElapsedTime = remainingTime;

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