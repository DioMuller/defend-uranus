using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.SteeringBehaviors;
using Microsoft.Xna.Framework;
using DefendUranus.Activities;

namespace DefendUranus.Entities
{
    class SteeringEntity : GamePlayEntity
    {
        #region Properties
        /// <summary>
        /// Entity steering behaviors.
        /// </summary>
        public List<SteeringBehavior> SteeringBehaviors { get; private set; }
        #endregion Properties

        #region Constructor
        public SteeringEntity(GamePlay level)
            : base(level)
        {
            SteeringBehaviors = new List<SteeringBehavior>();
            Health = new Container(1);
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Updates entity considering the steering behaviors.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public override void Update(GameTime gameTime)
        {
            foreach( SteeringBehavior sb in SteeringBehaviors )
                ApplyForce(sb.Calculate(gameTime));

            base.Update(gameTime);
        }
        #endregion Methods
    }
}
