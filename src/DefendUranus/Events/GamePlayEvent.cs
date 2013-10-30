using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Events;

namespace DefendUranus.Events
{
    class GamePlayEventArgs : GameLoopEventArgs
    {
        public GamePlayEventArgs(GameTime gameTime, GamePlay level)
            : base(gameTime)
        {
            Level = level;
        }

        public GamePlay Level { get; private set; }
    }

    delegate void GamePlayEventHandler(object sender, GamePlayEventArgs e);
}
