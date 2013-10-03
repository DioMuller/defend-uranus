#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
#endregion

namespace DefendUranus.Activities
{
    class ShowResults : GameActivity<bool>
    {
        #region Attributes
        KeyboardWatcher _keyboard;
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
            _keyboard = new KeyboardWatcher();
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            if (_keyboard.IsPressed(Keys.Escape) || _keyboard.IsPressed(Keys.Enter))
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
