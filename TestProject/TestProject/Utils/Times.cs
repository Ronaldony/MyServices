using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Utils
{
    public static class Time
    {
        private static DateTime _startTime = DateTime.UtcNow;
        public static float time => (float)(DateTime.UtcNow - _startTime).TotalSeconds;
    }
}
