using DefendUranus.SteeringBehaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities.SpecialAttacks
{
    class FleeingFake : SpecialAttack
    {
        #region Attributes
        Flee _flee;
        Ship _owner;
        #endregion

        #region Constructors
        public FleeingFake(Ship owner)
            : base(owner.Level, "Sprites/Fatboy-FleeingFake.png")
        {
            _owner = owner;
            RotateToMomentum = true;
            Momentum = Vector2.One;
            MaxSpeed = 10f;

            _flee = new Flee(this)
            {
                PanicDistance = 500f,
            };
            SteeringBehaviors.Add(_flee);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (_flee.Target == null)
            {
                var target = _owner.Level.Entities.OfType<Ship>()
                    .Where(s => s != _owner)
                    .OrderBy(s => (s.Position - Position).LengthSquared())
                    .FirstOrDefault();

                _flee.Target = target;
            }

            base.Update(gameTime);
        }
        #endregion
    }
}
