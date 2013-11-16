using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Entities;
using DefendUranus.Entities;

namespace DefendUranus.SteeringBehaviors
{
    /// <summary>
    /// Behavior: Seeks a target.
    /// </summary>
    class Seek : SteeringBehavior
    {
        public Seek(SteeringEntity parent) : base(parent) { }

        /// <summary>
        /// Calculates movement vector for this behavior.
        /// </summary>
        /// <returns>Movement vector</returns>
        public override Vector2 Calculate(GameTime gameTime)
        {
            if( Target != null )
            {
                Vector2 desiredVelocity = Target.Position - Parent.Position;
                desiredVelocity.Normalize();
                desiredVelocity *= Parent.MaxSpeed;

                return desiredVelocity;
            }

            return Vector2.Zero;
        }
    }
}
