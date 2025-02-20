using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace CatCode.Timers
{
    public sealed class CountdownTimer
    {
        //public float Interval;
        //public float TotalTime;
        //public float TimeRemaining;

        //public event Action Tick;
        //public event Action Completed;

        //public void Start()
        //{
        //    var ticks = Mathf.FloorToInt(TotalTime / Interval);
        //    var t = TotalTime % Interval;

        //    var timer = DeltaTimeTimer.Create(Interval);
        //    timer.Data.TotalTicks = ticks;
        //    timer.Elapsed += OnTimerElapsed;
        //    timer.Start();
        //    timer.Data.ElapsedTime = Interval - t;
        //}

        //private void OnTimerElapsed()
        //{
        //    Tick?.Invoke();
        //    Completed?.Invoke();
        //}
    }

    public sealed class TempProcessor : IPlayerLoopItem
    {
        private float _interval;
        private float _totalTime;

        public bool MoveNext()
        {
            var time = Time.time;
            return true;
        }
    }
}