using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.Entities;
using Microsoft.Xna.Framework;
using MonoGameLib.Core;

namespace DefendUranus.SteeringBehaviors
{
    /// <summary>
    /// Behavior: Wander.
    /// </summary>
    class Wander : SteeringBehavior
    {
        #region Attributes
        private Vector2 _target;
        #endregion Attributes

        #region Properties
        /// <summary>
        /// Wander behavior jitter value.
        /// </summary>
        float Jitter { get; set; }

        /// <summary>
        /// Wander target distance.
        /// </summary>
        float WanderDistance { get; set; }

        /// <summary>
        /// Wander circle radius.
        /// </summary>
        float WanderRadius { get; set; }
        #endregion Properties
        
        #region Constructor
        public Wander(SteeringEntity parent) : base(parent) 
        {
            _target = (Parent.Direction * WanderDistance); 
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Calculates movement vector for this behavior.
        /// </summary>
        /// <returns>Movement vector</returns>
        public override Vector2 Calculate(GameTime gameTime)
        {
            float jitterThisTimeSlice = Jitter * gameTime.ElapsedGameTime.Milliseconds * 9;
            Vector2 temp = new Vector2( RandomNumberGenerator.Next(-1f, 1f) * jitterThisTimeSlice,
                                        RandomNumberGenerator.Next(-1f, 1f) * jitterThisTimeSlice);

            //Add a small random vector to the position
            _target += temp;
            //Reprojects the new vector in a unit circle.
            _target.Normalize();
            //Increases target to the same radius of the circle.
            _target *= WanderRadius;

            Vector2 position = Parent.Position;
            position += (Parent.Direction * WanderDistance);
            position += _target;

            return position - Parent.Position;
        }
        #endregion Methods
    }
}
