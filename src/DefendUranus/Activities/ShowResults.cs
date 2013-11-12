#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using DefendUranus.Helpers;
using MonoGameLib.Core;
#endregion

namespace DefendUranus.Activities
{
    class ShowResults : GameActivity<bool>
    {
        #region Attributes
        GameInput _gameInput;
        #endregion

        #region Constructors
        public ShowResults(MainGame game, GamePlay.Result results)
            : base(game)
        {
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _gameInput = new GameInput();

            SoundManager.PlayBGM("Clean Soul");
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _gameInput.Update();
            if (_gameInput.Cancel)
                Exit(false);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);

            //SpriteBatch.Begin();
            //SpriteBatch.End();
        }
        #endregion
    }
}
