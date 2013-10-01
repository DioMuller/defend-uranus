using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Texture2D _title;
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
            _title = Content.Load<Texture2D>("Images/Title.png");
            base.Activating();
        }

        protected override void Deactivating()
        {
            
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

            SpriteBatch.Begin();
            SpriteBatch.Draw(_title, Vector2.Zero, Color.White);
            SpriteBatch.End();
        }
    }
}
