using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities
{
    class Laser : GamePlayEntity
    {
        #region Attributes
        bool _collidedWithOwner;
        GamePlayEntity _owner = null;
        #endregion

        public Laser(GamePlayEntity owner, Vector2 momentum, Vector2 position, Vector2 direction)
            : base(owner.Level, "Sprites/Laser")
        {
            _owner = owner;
            Momentum = momentum;
            Position = position + direction * _owner.Size / 2;

            Mass = 0.1f;
            MaxSpeed = 50;
            RotateToMomentum = true;

            Collided += Laser_Collided;
        }

        void Laser_Collided(object sender, Events.EntityCollisionEventArgs e)
        {
            _collidedWithOwner = e.CollidedWith == _owner;
            if (_collidedWithOwner)
                return;

            var health = e.CollidedWith.Health;
            if (health != null)
                health.Quantity--;

            e.Level.RemoveEntity(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_collidedWithOwner)
                _owner = null;

            base.Update(gameTime);
        }
    }
}
