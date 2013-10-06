using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.SteeringBehaviors;
using Microsoft.Xna.Framework;

namespace DefendUranus.Entities
{
    public class SteeringEntity : PhysicsEntity
    {
        #region Properties
        /// <summary>
        /// Entity maximum velocity.
        /// </summary>
        public float MaximumVelocity { get; set; }

        /// <summary>
        /// Entity steering behaviors.
        /// </summary>
        public List<SteeringBehavior> SteeringBehaviors { get; private set; }
        #endregion Properties

        #region Constructor
        public SteeringEntity() : base()
        {
            SteeringBehaviors = new List<SteeringBehavior>();
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates entity considering the steering behaviors.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public override void Update(GameTime gameTime)
        {
            Vector2 steering = Vector2.Zero;

            foreach( SteeringBehavior sb in SteeringBehaviors )
            {
                steering += sb.Calculate(gameTime);
            }

            ConstantForces["Steering"] = steering;
            base.Update(gameTime);
        }
        #endregion Methods
    }
}
