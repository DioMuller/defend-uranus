#region Using Statements
using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using MonoGameLib.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoGameLib.Core.Input;
#endregion

namespace DefendUranus.Activities
{
    class GamePlay : GameActivity<GamePlay.Result>
    {
        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }
            public int Winner { get; set; }
            public TimeSpan Duration { get; set; }
        }
        #endregion

        #region Attributes
        private KeyboardWatcher _keyboard;
        #endregion

        #region Constructors
        public GamePlay(MainGame game, GamePlaySetup.Result setup)
            : base(game)
        {
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _keyboard = new KeyboardWatcher();
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            if (_keyboard.IsPressed(Keys.Escape))
                Exit(new Result { Aborted = true });
            else if (_keyboard.IsPressed(Keys.Enter))
                Exit(new Result { Aborted = false });
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //SpriteBatch.Begin();
            //SpriteBatch.End();
        }
        #endregion
    }

}
