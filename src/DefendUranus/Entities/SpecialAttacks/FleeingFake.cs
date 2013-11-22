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
        #endregion

        #region Constructors
        public FleeingFake(Ship owner, float lifetime)
            : base(owner.Level, "Sprites/Fatboy-FleeingFake.png", Color.Yellow, owner, lifetime)
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
            #region Selects target
            var target = _owner.Level.Entities.OfType<SpecialAttack>()
               .Where(s => s != this && s.Visible)
               .OrderBy(s => (s.Position - Position).LengthSquared())
               .FirstOrDefault();

            // If you got a new target...
            if (target != null && _flee.Target != target)
            {
                //target.SteeringBehaviors.Clear();
                target.SteeringBehaviors.Insert(0, new Pursuit(target) { Target = this });
                _flee.Target = target;
            }

            #endregion Selects target

            base.Update(gameTime);
        }
        #endregion
    }
}
