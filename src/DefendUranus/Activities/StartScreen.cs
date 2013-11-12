#region Using Statements
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core;
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
        private GameInput _gameInput;
        private Texture2D _title;
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
            _gameInput = new GameInput();
            _title = Content.Load<Texture2D>("Images/Title");

            SoundManager.PlayBGM("Pamgaea");
        }

        protected override void Deactivating()
        {
            base.Deactivating();
        }

        protected override Task IntroductionAnimation()
        {
            return TaskEx.WhenAll(
                FloatAnimation(100, 2, 1, v => _scale = new Vector2(v)),
                base.IntroductionAnimation());
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _gameInput.Update();

            if (_gameInput.Cancel)
                Exit(Options.Exit);
            else if (_gameInput.Help)
                Exit(Options.HowToPlay);
            else if (_gameInput.Confirm)
                Exit(Options.Play);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            SpriteBatch.Draw(_title, Vector2.Zero, scale: _scale);
            SpriteBatch.End();
        }
        #endregion
    }
}
