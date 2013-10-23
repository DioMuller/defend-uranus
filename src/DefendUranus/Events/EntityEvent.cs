using DefendUranus.Activities;
using DefendUranus.Entities;
using Microsoft.Xna.Framework;

namespace DefendUranus.Events
{
    class EntityEventArgs : GamePlayEventArgs
    {
        public EntityEventArgs(GamePlayEntity entity, GameTime gameTime, GamePlay level)
            : base(gameTime, level)
        {
            Entity = entity;
        }

        public GamePlayEntity Entity { get; private set; }
    }

    delegate void EntityEventHandler(object sender, EntityEventArgs e);
}
