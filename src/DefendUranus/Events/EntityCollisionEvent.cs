using DefendUranus.Activities;
using DefendUranus.Entities;
using Microsoft.Xna.Framework;

namespace DefendUranus.Events
{
    class EntityCollisionEventArgs : EntityEventArgs
    {
        public EntityCollisionEventArgs(GamePlayEntity entity, GamePlayEntity collidedWith, GameTime gameTime, GamePlay level)
            : base(entity, gameTime, level)
        {
            CollidedWith = collidedWith;
        }

        public GamePlayEntity CollidedWith { get; private set; }
        public bool IgnoreCollision { get; set; }
    }

    delegate void EntityCollisionEventHandler(object sender, EntityCollisionEventArgs e);
}
