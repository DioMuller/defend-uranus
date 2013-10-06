#region Using Statements
using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using MonoGameLib.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoGameLib.Core.Input;
using DefendUranus.Entities;
#endregion

namespace DefendUranus.Activities
{
    class GamePlay : GameActivity<GamePlay.Result>
    {
        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }
            public int Winner { get; set; }
            public TimeSpan Duration { get; set; }
        }
        #endregion

        #region Attributes
        private KeyboardWatcher _keyboard;
        private TimeSpan _duration;
        private List<Entities.PhysicsEntity> _entities;
        private List<Ship> _ships;
        #endregion

        #region Constructors
        public GamePlay(MainGame game, GamePlaySetup.Result setup)
            : base(game)
        {
            // TODO: Set initial position based on ship size
            setup.Player1Ship.Position = new Vector2(-10, 0);
            setup.Player2Ship.Position = new Vector2(10, 0);

            _ships = new List<Ship>
            {
                setup.Player1Ship,
                setup.Player2Ship
            };
            _entities = new List<PhysicsEntity>(_ships);
            _duration = TimeSpan.Zero;
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
        Vector2 _debugDirection = new Vector2(-1, 0);
        protected override void Update(GameTime gameTime)
        {
            _duration += gameTime.ElapsedGameTime;
            _keyboard.Update();

            _ships.First().Position += _debugDirection;

            if (_keyboard.IsPressed(Keys.Space))
                _debugDirection *= -1;

            if (IsGameEnded())
                return;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        GetCameraTransformation(Game.GraphicsDevice));
            foreach (var ent in _entities.ToList())
                ent.Draw(gameTime, SpriteBatch);
            SpriteBatch.End();
        }

        bool IsGameEnded()
        {
            // Check game abort
            if (_keyboard.IsPressed(Keys.Escape))
            {
                Exit(new Result { Aborted = true });
                return true;
            }

            // Check player death
            if (_entities.OfType<Ship>().Count() <= 1)
            {
                Exit(new Result
                {
                    Aborted = false,
                    Duration = _duration,
                    Winner = _ships.IndexOf(_entities.OfType<Ship>().FirstOrDefault())
                });
                return true;
            }

            return false;
        }

        Matrix GetCameraTransformation(GraphicsDevice graphicsDevice)
        {
            const float maxZoomFactor = 2;
            const float minZoomFactor = 0.5f;

            const float minZoomDistance = 200 / maxZoomFactor;
            const float maxZoomDistance = 800 / minZoomFactor;

            var dist = _ships.Last().Position - _ships.First().Position;
            var distLength = dist.Length();

            Vector2 pos = (_ships.Last().Position + _ships.First().Position) / 2;

            float zoom;
            if (distLength < minZoomDistance)
                zoom = maxZoomFactor;
            else if (distLength > maxZoomDistance)
                zoom = minZoomFactor;
            else
                zoom = XNATweener.Cubic.EaseOut(distLength - minZoomDistance, maxZoomFactor, minZoomFactor - maxZoomFactor, maxZoomDistance - minZoomDistance);

            return Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                //Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
        }
        #endregion
    }

}
