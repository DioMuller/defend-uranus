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
        public Asteroid(GamePlay level) : base(level, "Sprites/Asteroid")
        {
            Collided += OnCollided;
        }

        private void OnCollided(object sender, EntityCollisionEventArgs entityCollisionEventArgs)
        {
            Destroy();
        }
    }
}
