using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DefendUranus.Helpers
{
    public class WorldHelper
    {
        #region Constants
        /// <summary>
        /// Pixels per Meter.
        /// </summary>
        private const float PPM = 8f;

        /// <summary>
        /// Meters per Pixel.
        /// </summary>
        private const float MPP = 1f / PPM;
        #endregion Constants

        #region Methods
        public static float MetersToPixels(float value)
        {
            return value / MPP;
        }

        public static float PixelsToMeters(float value)
        {
            return value * PPM;
        }

        public static Vector2 MetersToPixels(Vector2 vector)
        {
            return vector / MPP;
        }
        #endregion Methods
    }
}
