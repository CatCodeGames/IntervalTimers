using System;

namespace CatCode.Timers
{
    public sealed class TimerTickData
    {
        public int CompletedTicks;
        public int TotalTicks;

        public InvokeMode InvokeMode;
        public Action OnTick;

        public void Reset() 
        { 
            CompletedTicks = 0;
        }
    }
}