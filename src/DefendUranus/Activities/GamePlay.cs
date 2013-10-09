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
using DefendUranus.Helpers;
#endregion

namespace DefendUranus.Activities
{
    class GamePlay : GameActivity<GamePlay.Result>
    {
        #region Constants
        const float MaxZoomFactor = 2f;
        const float MinZoomFactor = 0.5f;

        const float MinZoomDistance = 150;
        const float MaxZoomDistance = 650;
        #endregion

        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }
            public int Winner { get; set; }
            public TimeSpan Duration { get; set; }
        }
        #endregion

        #region Attributes
        private GameInput _gameInput;
        private TimeSpan _duration;
        private List<Entities.PhysicsEntity> _entities;
        private List<Ship> _ships;
        #endregion

        #region Constructors
        public GamePlay(MainGame game, GamePlaySetup.Result setup)
            : base(game)
        {
            var p1Ship = setup.Player1Ship.BuildShip();
            var p2Ship = setup.Player2Ship.BuildShip();

            // TODO: Set initial position based on ship size
            p1Ship.Position = new Vector2(-10, 0);
            p2Ship.Position = new Vector2(10, 0);

            p1Ship.Behaviors.Add(new ShipInputBehavior(PlayerIndex.One, p1Ship));
            p2Ship.Behaviors.Add(new ShipInputBehavior(PlayerIndex.Two, p2Ship));

            _ships = new List<Ship> { p1Ship, p2Ship };
            _entities = new List<PhysicsEntity>(_ships);
            _duration = TimeSpan.Zero;
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _gameInput = new GameInput();
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _duration += gameTime.ElapsedGameTime;
            _gameInput.Update();

            if (IsGameEnded())
                return;

            foreach (var ent in _entities.ToList())
                ent.Update(gameTime);

            UpdateCyclicSpace();
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
            if (_gameInput.Cancel)
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
            var dist = _ships.Last().Position - _ships.First().Position;
            var distLength = dist.Length();

            Vector2 pos = (_ships.Last().Position + _ships.First().Position) / 2;

            float zoom;
            if (distLength < MinZoomDistance)
                zoom = MaxZoomFactor;
            else if (distLength > MaxZoomDistance)
                zoom = MinZoomFactor;
            else
                zoom = XNATweener.Cubic.EaseOut(distLength - MinZoomDistance, MaxZoomFactor, MinZoomFactor - MaxZoomFactor, MaxZoomDistance - MinZoomDistance);

            return Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                //Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
        }

        void UpdateCyclicSpace()
        {
            var p1 = _ships[0];
            var p2 = _ships[1];
            var dist = p2.Position - p1.Position;
            if (Math.Abs(dist.X) > Game.Window.ClientBounds.Width / MinZoomFactor)
            {
                if (p1.Momentum.LengthSquared() > p2.Momentum.LengthSquared())
                    p1.Position += new Vector2(dist.X * 2, 0);
                else
                    p2.Position += new Vector2(dist.X * 2, 0);
            }
            if (Math.Abs(dist.Y) > Game.Window.ClientBounds.Height / MinZoomFactor)
            {
                if (p1.Momentum.LengthSquared() > p2.Momentum.LengthSquared())
                    p1.Position += new Vector2(0, dist.Y * 2);
                else
                    p2.Position += new Vector2(0, dist.Y * 2);
            }
        }
        #endregion
    }

}
