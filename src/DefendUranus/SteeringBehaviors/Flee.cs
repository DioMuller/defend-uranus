using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DefendUranus.Entities;

namespace DefendUranus.SteeringBehaviors
{
    /// <summary>
    /// Behavior: Flees a target if in panic radius.
    /// </summary>
    public class Flee : SteeringBehavior
    {
        /// <summary>
        /// Panic Distance.
        /// </summary>
        public float PanicDistance { get; set; }

        public Flee(SteeringEntity parent) : base(parent) { }

        /// <summary>
        /// Calculates movement vector for this behavior.
        /// </summary>
        /// <returns>Movement vector</returns>
        public override Vector2 Calculate(GameTime gameTime)
        {
            Vector2 desiredVelocity = Parent.Position - Target.Position;
            float distance = desiredVelocity.Length();

            if( distance == 0.0f || distance < PanicDistance )
            {
                desiredVelocity.Normalize();
                desiredVelocity *= Parent.MaximumVelocity;

                return desiredVelocity - Parent.Momentum;
            }

            return Vector2.Zero;
        }
    }
}
