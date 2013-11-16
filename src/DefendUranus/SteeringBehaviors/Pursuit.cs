using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Extensions;
using DefendUranus.Entities;

namespace DefendUranus.SteeringBehaviors
{
    /// <summary>
    /// Behavior: Target pursuit, with movement preview.
    /// </summary>
    class Pursuit : SteeringBehavior
    {
        public Pursuit(SteeringEntity parent) : base(parent) { }

        /// <summary>
        /// Calculates movement vector for this behavior.
        /// </summary>
        /// <returns>Movement vector</returns>
        public override Vector2 Calculate(GameTime gameTime)
        {
            if( Target != null )
            {
                Vector2 futurePosition;
                Vector2 toEvader = Target.Position - Parent.Position;
                float relativeDir = Vector2.Dot(Parent.Direction, Target.Direction);

                if( Vector2.Dot(toEvader, Target.Direction) > 0 && (relativeDir < -0.95 ) ) //acos(0.95) = 18 degs
                {
                    futurePosition = Target.Position;
                }
                else
                {
                    float lookAheadTime = toEvader.Length() / (Parent.MaxSpeed + Target.Speed);
                    futurePosition = Target.Position + (Target.Momentum * lookAheadTime);
                }
            
             

                #region Seek
                Vector2 desiredVelocity = futurePosition - Parent.Position;
                desiredVelocity.Normalize();
                desiredVelocity *= Parent.MaxSpeed;
                #endregion Seek

                return desiredVelocity;
            }

            return Vector2.Zero;
        }
    }
}
