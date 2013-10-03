#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using System.Threading.Tasks;
#endregion

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
        private KeyboardWatcher _keyboard;
        private Texture2D _title;
        private Color _drawColor;
        private Vector2 _scale;
        #endregion

        #region Constructors
        public StartScreen(MainGame game)
            : base(game)
        {
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _keyboard = new KeyboardWatcher();
            _title = Content.Load<Texture2D>("Images/Title");
        }

        protected override void Deactivating()
        {
            base.Deactivating();
        }

        protected async override Task<StartScreen.Options> RunActivity()
        {
            await TaskEx.WhenAll(
                FadeIn(100, c => _drawColor = c),
                FloatAnimation(100, 2, 1, v => _scale = new Vector2(v)));

            var result = await base.RunActivity();

            await FadeOut(100, c => _drawColor = c);
            return result;
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _keyboard.Update();

            if (_keyboard.IsPressed(Keys.Escape))
                Exit(Options.Exit);
            else if (_keyboard.IsPressed(Keys.F1))
                Exit(Options.HowToPlay);
            else if (_keyboard.IsPressed(Keys.Enter))
                Exit(Options.Play);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            SpriteBatch.Draw(_title, Vector2.Zero, color: _drawColor, scale: _scale);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
