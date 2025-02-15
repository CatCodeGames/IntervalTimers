namespace CatCode.Timers
{
    public sealed class TimerTickData
    {
        public int TicksPerFrame;
        public int TickNumber;

        public void Reset()
        {
            TicksPerFrame = 0;
            TickNumber = 0;
        }
    }
}