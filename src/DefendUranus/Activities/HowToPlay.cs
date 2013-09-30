using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using System;

namespace DefendUranus.Activities
{
    class HowToPlay : GameActivity<bool>
    {
        #region Attributes
        KeyboardWatcher _keyboard;
        #endregion

        public HowToPlay(MainGame game)
            : base(game)
        {
        }

        protected override void Activating()
        {
            _keyboard = new KeyboardWatcher();
            base.Activating();
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            if (_keyboard.IsPressed(Keys.Escape))
                Exit(false);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //SpriteBatch.Begin();
            //SpriteBatch.End();
        }
    }
}
