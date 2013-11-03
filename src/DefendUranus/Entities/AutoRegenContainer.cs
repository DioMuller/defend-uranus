using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities
{
    /// <summary>
    /// A container that can regenerate itself.
    /// </summary>
    class AutoRegenContainer : Container
    {
        #region Attributes
        TimeSpan _currentRegenTime;
        bool _ignoreValueChange;
        #endregion

        #region Properties
        /// <summary>
        /// Maximum time that it takes for the container to fill.
        /// </summary>
        public TimeSpan RegenTime { get; set; }

        /// <summary>
        /// True if this container does auto regenerate.
        /// </summary>
        public bool Regenerate { get; set; }
        #endregion

        #region Constructors
        public AutoRegenContainer(int maximum, TimeSpan regenTime)
        {
            ValueChanged += AutoRegenContainer_ValueChanged;
            RegenTime = regenTime;
            Maximum = maximum;
            Quantity = maximum;
            Regenerate = true;
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (Regenerate && !IsFull)
            {
                _currentRegenTime += gameTime.ElapsedGameTime;
                _ignoreValueChange = true;
                Quantity = (int)Easing.Quadratic.In(_currentRegenTime, 0, Maximum.Value, RegenTime);
                _ignoreValueChange = false;
            }
        }

        void AutoRegenContainer_ValueChanged(object sender, EventArgs e)
        {
            if (!_ignoreValueChange)
                _currentRegenTime = Easing.Quadratic.ReverseIn(Quantity, 0, Maximum.Value, RegenTime);
        }
    }
}
