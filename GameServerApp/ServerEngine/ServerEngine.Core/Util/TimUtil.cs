namespace ServerEngine.Core.Util
{
    public static class TimeUtil
    {
        /// <summary>
        /// Check dst is past src.
        /// </summary>
        /// <param name="src">Source time</param>
        /// <param name="dst">Destination time</param>
        /// <returns>
        /// true: dst is past src. 
        /// false: otherwise.
        /// </returns>
        public static bool IsPastTime(DateTime src, DateTime dst)
        {
            // Year.
            if (dst.Year > src.Year)
            {
                return true;
            }

            // Total day.
            if (dst.DayOfYear > src.DayOfYear)
            {
                return true;
            }

            if (dst.Ticks > src.Ticks)
            {
                return true;
            }

            return false;
        }
    }
}
