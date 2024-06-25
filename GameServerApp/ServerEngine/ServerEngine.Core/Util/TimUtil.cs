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
            if (DateTime.MinValue == src)
            {
                return false;
            }

            if (dst.CompareTo(src) > 0 )
            {
                return false;
            }

            return true;
        }
    }
}
