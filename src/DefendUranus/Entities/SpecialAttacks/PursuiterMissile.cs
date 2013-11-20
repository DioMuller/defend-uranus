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
        #endregion

        #region Constructors
        public PursuiterMissile(Ship owner, float lifetime)
            : base(owner.Level, "Sprites/Avenger-PursuiterMissile.png", Color.Red, owner, lifetime)
        {
            _owner = owner;
            RotateToMomentum = true;
            Momentum = Vector2.One;
            MaxSpeed = 10f;

            _pursuit = new Pursuit(this);
            SteeringBehaviors.Add(_pursuit);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (_pursuit.Target == null || !_pursuit.Target.Visible)
            {
                var target = _owner.Level.Entities.OfType<Ship>()
                    .Where(s => s != _owner && s.Visible)
                    .OrderBy(s => (s.Position - Position).LengthSquared())
                    .FirstOrDefault();

                _pursuit.Target = target;
            }

            base.Update(gameTime);
        }
        #endregion
    }
}
