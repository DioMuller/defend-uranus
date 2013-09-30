using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using System;

namespace DefendUranus.Activities
{
    class StartScreen : GameActivity<StartScreen.Options>
    {
        #region Nested
        public enum Options
        {
            Play,
            HowToPlay,
            Exit
        }
        #endregion

        #region Attributes
        KeyboardWatcher _keyboard;
        #endregion

        #region Constructors
        public StartScreen(MainGame game)
            : base(game)
        {
        }
        #endregion

        protected override void Activating()
        {
            _keyboard = new KeyboardWatcher();
            base.Activating();
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _keyboard.Update();

            if (_keyboard.IsPressed(Keys.Escape))
                Exit(Options.Exit);

            else if (_keyboard.IsPressed(Keys.F1))
                Exit(Options.HowToPlay);
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            //SpriteBatch.Begin();
            //SpriteBatch.End();
        }
    }
}
