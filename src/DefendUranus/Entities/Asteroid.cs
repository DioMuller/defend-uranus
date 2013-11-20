using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.Activities;
using DefendUranus.Events;

namespace DefendUranus.Entities
{
    class Asteroid : GamePlayEntity
    {
        #region Contants
        new public const int Mass = 5;
        new public const int MaxSpeed = 10;
        new public const int MaxRotationSpeed = 2;
        #endregion

        public Asteroid(GamePlay level)
            : base(level, "Sprites/Asteroid")
        {
            base.Mass = Asteroid.Mass;
            base.MaxSpeed = Asteroid.MaxSpeed;
            base.MaxRotationSpeed = Asteroid.MaxRotationSpeed;

            Collided += OnCollided;
        }

        private void OnCollided(object sender, EntityCollisionEventArgs entityCollisionEventArgs)
        {
            Destroy();
        }
    }
}
