#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using DefendUranus.Helpers;
using MonoGameLib.Core;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.GUI.Components;
using MonoGameLib.GUI.Base;
#endregion

namespace DefendUranus.Activities
{
    class ShowResults : GameActivity<bool>
    {
        #region Attributes
        Label _lblWinner;
        GameInput _gameInput;
        Texture2D _background;
        #endregion

        #region Constructors
        public ShowResults(MainGame game, GamePlay.Result results)
            : base(game)
        {
            string winnerMessage;
            if (results.Winner < 0)
                winnerMessage = "Everyone loses at war.";
            else
                winnerMessage = "Player " + ( results.Winner + 1 ) + " Wins!";

            _lblWinner = new Label(winnerMessage, "Fonts/BigFont")
            {
                HorizontalOrigin = HorizontalAlign.Center,
                VerticalOrigin = VerticalAlign.Middle,
                Color = Color.White
            };
        }
        #endregion
        
        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _gameInput = new GameInput();

            _background = Content.Load<Texture2D>("Backgrounds/Background");

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
            var screen = GraphicsDevice.Viewport;
            
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();
            SpriteBatch.Draw(_background, screen.Bounds, Color.White);
            _lblWinner.Position = screen.CenterPoint();
            _lblWinner.Draw(gameTime, SpriteBatch);
            SpriteBatch.End();
        }
        #endregion
    }
}
