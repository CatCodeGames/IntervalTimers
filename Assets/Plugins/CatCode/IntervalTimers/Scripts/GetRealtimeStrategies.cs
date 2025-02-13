using System;
using UnityEngine;

namespace CatCode.Timers
{
    public static class GetRealtimeStrategies
    {
        public static Func<float> GetTime(RealtimeMode mode)
            => mode switch
            {
                RealtimeMode.Scaled => GetScaledTime,
                RealtimeMode.Unscaled => GetUnscaledTime,
                RealtimeMode.SinceStartup => GetSinceStartup,
                _ => GetScaledTime,
            };


        public static float GetScaledTime()
            => Time.time;

        public static float GetUnscaledTime()
            => Time.unscaledTime;

        public static float GetSinceStartup()
            => Time.realtimeSinceStartup;
    }
}