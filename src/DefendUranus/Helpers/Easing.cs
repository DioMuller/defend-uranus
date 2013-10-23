using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Helpers
{
    public static class Easing
    {
        public static class Quadratic
        {
            public static float In(float t, float b, float c, float d)
            {
                return c * (t /= d) * t + b;
            }

            public static float ReverseIn(float v, float b, float c, float d)
            {
                return (float)Math.Sqrt((v - b) / c) * d;
            }

            public static float In(TimeSpan elapsed, float b, float c, TimeSpan duration)
            {
                return In((float)elapsed.TotalMilliseconds, b, c, (float)duration.TotalMilliseconds);
            }

            public static TimeSpan ReverseIn(float value, float b, float c, TimeSpan duration)
            {
                var msec = ReverseIn(value, b, c, (float)duration.TotalMilliseconds);
                return TimeSpan.FromMilliseconds(msec);
            }
        }
    }
}
