using DefendUranus.SteeringBehaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities.SpecialAttacks
{
    class WandererProbe : SpecialAttack
    {
        #region Attributes
        Wander _wander;
        Ship _owner;
        #endregion

        #region Constructors
        public WandererProbe(Ship owner)
            : base(owner.Level, "Sprites/Explorer-WandererProbe.png", Color.CornflowerBlue)
        {
            _owner = owner;
            RotateToMomentum = true;
            Momentum = Vector2.One;
            MaxSpeed = 10f;

            _wander = new Wander(this)
            {
                Jitter = 1.25f,
                WanderDistance = 50f,
                WanderRadius = 90f,
            };
            SteeringBehaviors.Add(_wander);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (_wander.Target == null)
            {
                var target = _owner.Level.Entities.OfType<Ship>()
                    .Where(s => s != _owner)
                    .OrderBy(s => (s.Position - Position).LengthSquared())
                    .FirstOrDefault();

                _wander.Target = target;
            }

            base.Update(gameTime);
        }
        #endregion
    }
}
