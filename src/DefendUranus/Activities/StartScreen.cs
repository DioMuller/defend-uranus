using Microsoft.Xna.Framework.Input;

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

        #region Constructors
        public StartScreen(MainGame game)
            : base(game)
        {

        }
        #endregion

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(Options.Exit);
        }
    }
}
