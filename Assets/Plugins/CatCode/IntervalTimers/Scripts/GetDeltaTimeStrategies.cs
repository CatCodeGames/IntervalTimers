using System;
using UnityEngine;

namespace CatCode.Timers
{
    public static class GetDeltaTimeStrategies
    {
        public static Func<float> GetDeltaTimeStrategy(bool unscaled)
            => unscaled ? GetUnscaledDeltaTime : GetScaledDeltaTime;

        public static float GetUnscaledDeltaTime() => Time.unscaledDeltaTime;
        public static float GetScaledDeltaTime() => Time.deltaTime;

        public static float GetScaledFixedDeltaTime() => Time.fixedDeltaTime;
        public static float GetUnscaledFixedDeltaTime() => Time.fixedUnscaledDeltaTime;
    }
}