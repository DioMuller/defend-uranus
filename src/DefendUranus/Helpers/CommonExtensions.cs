using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Helpers
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Mod operation that will only return positive values.
        /// </summary>
        /// <param name="value">Initial value.</param>
        /// <param name="max">Maximum result value.</param>
        /// <returns>Value that cycles through the possible options.</returns>
        public static int Mod(this int value, int max)
        {
            if (value < 0)
                return value % max + max;
            return value % max;
        }
    }
}
