#region Using Statements
using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using MonoGameLib.Core.Input;
#endregion

namespace DefendUranus.Activities
{
    class GamePlaySetup : GameActivity<GamePlaySetup.Result>
    {
        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }

            public string Player1Ship { get; set; }
            public string Player2Ship { get; set; }
        }
        #endregion

        #region Attributes
        KeyboardWatcher _keyboard;
        Result _result;
        #endregion

        #region Constructors
        public GamePlaySetup(MainGame game)
            : base(game)
        {
            _result = new Result();
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Starting()
        {
            base.Starting();
            _result.Aborted = false;
        }

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
            {
                _result.Aborted = true;
                Exit(_result);
            }
            else if (_keyboard.IsPressed(Keys.Enter))
            {
                Exit(_result);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            //SpriteBatch.Begin();
            //SpriteBatch.End();
        }
        #endregion
    }
}

