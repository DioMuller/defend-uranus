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
        private Wander _wander;
        private Seek _seek;
        private Ship _owner;
        #endregion

        #region Constructors
        public WandererProbe(Ship owner)
            : base(owner.Level, "Sprites/Explorer-WandererProbe.png", Color.CornflowerBlue)
        {
            _owner = owner;
            RotateToMomentum = true;
            Momentum = Vector2.One;
            MaxSpeed = 12f;

            _wander = new Wander(this)
            {
                Jitter = 1.25f,
                WanderDistance = 50f,
                WanderRadius = 90f,
            };

            _seek = new Seek(this) { Target =  null };
            
            SteeringBehaviors.Add(_seek);
            SteeringBehaviors.Add(_wander);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (_seek.Target == null)
            {
                var target = _owner.Level.Entities.OfType<PhysicsEntity>()
                    .Where(s => s != _owner && s != this)
                    .OrderBy(s => (s.Position - Position).LengthSquared())
                    .FirstOrDefault();

                if ( target != null && (target.Position - Position).LengthSquared() < 5000)
                {
                    _seek.Target = target;   
                }
            }

            base.Update(gameTime);
        }
        #endregion
    }
}
