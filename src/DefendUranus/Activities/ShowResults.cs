#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using DefendUranus.Helpers;
using MonoGameLib.Core;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DefendUranus.Activities
{
    class ShowResults : GameActivity<bool>
    {
        #region Attributes
        GameInput _gameInput;
        Texture2D _background;
        SpriteFont _font;

        string _winnerMessage;
        Vector2 _textSize;
        #endregion

        #region Constructors
        public ShowResults(MainGame game, GamePlay.Result results)
            : base(game)
        {
            _winnerMessage = "Player " + ( results.Winner + 1 ) + " Wins!";
            
        }
        #endregion
        
        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _gameInput = new GameInput();

            _background = Content.Load<Texture2D>("Backgrounds/Background");
            _font = Content.Load<SpriteFont>("Fonts/BigFont");
            _textSize = _font.MeasureString(_winnerMessage);

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
            Rectangle screen = GraphicsDevice.PresentationParameters.Bounds;
            Vector2 textPosition = new Vector2((screen.Width - screen.X)/2 - (_textSize.X / 2),
                                                (screen.Height - screen.Y)/2 - (_textSize.Y / 2));
            
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();
            SpriteBatch.Draw(_background, screen, Color.White);
            SpriteBatch.DrawString(_font, _winnerMessage, textPosition, Color.White);
            SpriteBatch.End();
        }
        #endregion
    }
}
