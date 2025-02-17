namespace ServerEngine.Core.Util
{
    public static class TimeUtil
    {
        /// <summary>
        /// Now time.
        /// </summary>
        public static DateTime Now => _now;
        private static DateTime _now
        {
            get
            {
                if (0 != _addMinute)
                {
                    return DateTime.Now.AddMinutes(_addMinute);
                }

                return DateTime.Now;
            }
        }

        /// <summary>
        /// Add time from minute.
        /// </summary>
        public static void AddNowTime(int minute) => _addMinute = minute;
        private static int _addMinute;

        /// <summary>
        /// Default time.
        /// </summary>
        private static DateTime _defaultTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// Check dst is past src.
        /// </summary>
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

        /// <summary>
        /// Compare both time is same.
        /// </summary>
        public static bool IsSameDay(DateTime src, DateTime dst)
        {
            if ((IsNullOrDefault(dst) == true) || (IsNullOrDefault(src) == true))
            {
                return false;
            }

            // Year.
            if (false == src.Year.Equals(dst.Year))
            {
                return false;
            }

            // DayOfYear.
            if (false == src.DayOfYear.Equals(dst.DayOfYear))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check the time is null or MinValue.
        /// </summary>
        public static bool IsNullOrDefault(DateTime src)
        {
            if (true == src.Equals(null))
            {
                return true;
            }

            // Less than default time.
            if (src.CompareTo(_defaultTime) < 0)
            {
                return true;
            }

            return false;
        }
    }
}
