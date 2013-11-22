#region Using Statements
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        Texture2D _background;
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

            _background = Content.Load<Texture2D>("Images/Controls");
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
            var screen = GraphicsDevice.Viewport;

            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();
            SpriteBatch.Draw(_background, screen.Bounds, Color.White);
            SpriteBatch.End();
        }
        #endregion
    }
}
