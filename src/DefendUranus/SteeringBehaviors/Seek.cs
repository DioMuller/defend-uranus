using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Entities;

namespace DefendUranus.SteeringBehaviors
{
    /// <summary>
    /// Behavior: Seeks a target.
    /// </summary>
    public class Seek : SteeringBehavior
    {      
        /// <summary>
        /// Calculates movement vector for this behavior.
        /// </summary>
        /// <returns>Movement vector</returns>
        public override Vector2 Calculate()
        {
            Vector2 desiredVelocity = Target.Position - Parent.Position;
            desiredVelocity.Normalize();
            desiredVelocity *= Parent.MaximumVelocity;

            return desiredVelocity;
        }
    }
}
