#region Using Statements
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using System;
#endregion

namespace DefendUranus.Activities
{
    class HowToPlay : GameActivity<bool>
    {
        #region Attributes
        GameInput _gameInput;
        #endregion

        #region Constructors
        public HowToPlay(MainGame game)
            : base(game)
        {
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _gameInput = new GameInput();
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _gameInput.Update();
            if (_gameInput.Cancel)
                Exit(false);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);

            //SpriteBatch.Begin();
            //SpriteBatch.End();
        }
        #endregion
    }
}
