using DefendUranus.SteeringBehaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities.SpecialAttacks
{
    class PursuiterMissile : SpecialAttack
    {
        #region Attributes
        Pursuit _pursuit;
        private Wander _wander;
        #endregion

        #region Constructors
        public PursuiterMissile(Ship owner, float lifetime)
            : base(owner, "Sprites/Avenger-PursuiterMissile.png", Color.Red, lifetime)
        {
            RotateToMomentum = true;
            Momentum = Vector2.One;
            MaxSpeed = 10f;

            _pursuit = new Pursuit(this);
            SteeringBehaviors.Add(_pursuit);

            // Wander, for when the target is not visible.
            _wander = new Wander(this)
            {
                Jitter = 1.25f,
                WanderDistance = 1f,
                WanderRadius = 100f,
            };

            SteeringBehaviors.Add(_wander);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (_pursuit.Target == null || !_pursuit.Target.Visible)
            {
                var target = Level.Entities.OfType<Ship>()
                    .Where(s => s != Owner && s.Visible)
                    .OrderBy(s => (s.Position - Position).LengthSquared())
                    .FirstOrDefault();

                _pursuit.Target = target;
            }

            base.Update(gameTime);
        }
        #endregion
    }
}
