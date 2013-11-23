using DefendUranus.SteeringBehaviors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameLib.Core.Particles;

namespace DefendUranus.Entities.SpecialAttacks
{
    class WandererProbe : SpecialAttack
    {
        #region Attributes
        private Wander _wander;
        private Seek _seek;
        #endregion

        #region Constructors
        public WandererProbe(Ship owner, float lifetime)
            : base(owner.Level, "Sprites/Explorer-WandererProbe.png", Color.CornflowerBlue, owner, lifetime)
        {
            _owner = owner;
            RotateToMomentum = true;
            Momentum = Vector2.One;
            MaxSpeed = 12f;

            _wander = new Wander(this)
            {
                Jitter = 1.25f,
                WanderDistance = 1f,
                WanderRadius = 100f,
            };

            _seek = new Seek(this) { Target =  null };
            
            SteeringBehaviors.Add(_seek);
            SteeringBehaviors.Add(_wander);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (_seek.Target == null || !_seek.Target.Visible )
            {
                var target = _owner.Level.Entities
                    .Where(s => s != _owner && s != this && s.InteractWithEntities)
                    .OrderBy(s => (s.Position - Position).LengthSquared())
                    .FirstOrDefault();

                if ( target != null && (target.Position - Position).LengthSquared() < 32000)
                {
                    _seek.Target = target;   
                }
            }

            base.Update(gameTime);
        }
        #endregion
    }
}
