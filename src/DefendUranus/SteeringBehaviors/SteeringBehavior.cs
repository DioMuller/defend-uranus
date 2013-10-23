using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.Entities;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Entities;

namespace DefendUranus.SteeringBehaviors
{
    /// <summary>
    /// Steering behavior base class.
    /// </summary>
    abstract class SteeringBehavior
    {
        #region Properties
        /// <summary>
        /// Entity where the behavior is applied
        /// </summary>
        public SteeringEntity Parent { get; private set; }

        /// <summary>
        /// Desided target.
        /// </summary>
        public PhysicsEntity Target { get; set; }
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Steering Behavior constructor
        /// </summary>
        /// <param name="parent">Entity where the behavior is applied</param>
        public SteeringBehavior(SteeringEntity parent)
        {
            Parent = parent;
        }
        #endregion Constructor

        #region Virtual Methods
        /// <summary>
        /// Calculates movement vector for this behavior.
        /// </summary>
        /// <returns>Movement vector</returns>
        public abstract Vector2 Calculate(GameTime gameTime);
        #endregion Virtual Methods
    }
}
