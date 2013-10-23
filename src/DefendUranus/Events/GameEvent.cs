using Microsoft.Xna.Framework;
using System;

namespace DefendUranus.Events
{
    public class GameEventArgs : EventArgs
    {
        public GameEventArgs(GameTime gameTime)
        {
            GameTime = gameTime;
        }

        public GameTime GameTime { get; private set; }
    }

    public delegate void GameEventHandler(object sender, GameEventArgs e);
}
