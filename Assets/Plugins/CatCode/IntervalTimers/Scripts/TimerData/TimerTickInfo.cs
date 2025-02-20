namespace CatCode.Timers
{
    public sealed class TimerTickInfo
    {
        public int TicksPerFrame;
        public int TickIndex;

        public void Reset()
        {
            TicksPerFrame = 0;
            TickIndex = -1;
        }
    }
}