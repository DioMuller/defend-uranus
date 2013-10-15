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
using DefendUranus.SteeringBehaviors;
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

        const float BackgroundSlideFactor = 0.05f;
        const float BackgroundMaxScale = 0.9f;
        const float BackgroundMinScale = BackgroundMaxScale * 0.75f;
        const float StarsSlideFactor = 0.4f;
        const float StarsMaxScale = 1f;
        const float StarsMinScale = StarsMaxScale * 0.6f;
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
        private Texture2D _background, _stars;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new GamePlay based on the user setup.
        /// </summary>
        /// <param name="game">Current game.</param>
        /// <param name="setup">Setup configuration.</param>
        public GamePlay(MainGame game, GamePlaySetup.Result setup)
            : base(game)
        {
            var p1Ship = setup.Player1Selection.BuildShip();
            var p2Ship = setup.Player2Selection.BuildShip();

            // TODO: Set initial position based on ship size
            p1Ship.Position = new Vector2(-100, 0);
            p2Ship.Position = new Vector2(100, 0);

            p1Ship.Behaviors.Add(new ShipInputBehavior(PlayerIndex.One, p1Ship));
            p2Ship.Behaviors.Add(new ShipInputBehavior(PlayerIndex.Two, p2Ship));

            _ships = new List<Ship> { p1Ship, p2Ship };
            _entities = new List<PhysicsEntity>(_ships);
            _duration = TimeSpan.Zero;

            #region Test Entities
            var specialAttack = new SpecialAttack("Sprites/Avenger-PursuiterMissile.png")
            {
                Momentum = Vector2.One,
                Mass = 1f,
                MaxSpeed = 10f,
                Position = new Vector2(0, 0)
            };
            var steeringBehavior = new Wander(specialAttack)
            {
                // Flee
                // PanicDistance = 500f,
                Target = p1Ship,
                Jitter = 1.25f,
                WanderDistance = 50f,
                WanderRadius = 90f,
            };
            specialAttack.SteeringBehaviors.Add(steeringBehavior);
            _entities.Add(specialAttack);
            #endregion Test Entities
        }
        #endregion

        #region Activity Life-Cycle
        /// <summary>
        /// Prepares the activity to be activated.
        /// </summary>
        protected override void Activating()
        {
            base.Activating();
            _gameInput = new GameInput();
            _background = Content.Load<Texture2D>("Backgrounds/Background");
            _stars = Content.Load<Texture2D>("Backgrounds/Background2");
        }
        #endregion

        #region Game Loop
        #region Update
        /// <summary>
        /// Update the current activity's state.
        /// This method is invoked for each game loop.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
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

        /// <summary>
        /// Check if the GamePlay is complete.
        /// </summary>
        /// <returns>True if the activity has exited.</returns>
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

        /// <summary>
        /// Make the ships loop if they get out of the screen.
        /// </summary>
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
                    p2.Position -= new Vector2(dist.X * 2, 0);
            }
            if (Math.Abs(dist.Y) > Game.Window.ClientBounds.Height / MinZoomFactor)
            {
                if (p1.Momentum.LengthSquared() > p2.Momentum.LengthSquared())
                    p1.Position += new Vector2(0, dist.Y * 2);
                else
                    p2.Position -= new Vector2(0, dist.Y * 2);
            }
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw the current activity to the screen.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            Vector2 camera = (_ships.Last().Position + _ships.First().Position) / 2;
            float zoom = GetZoomFactor();

            GraphicsDevice.Clear(Color.Black);

            DrawStars(camera, zoom);
            DrawEntities(gameTime, camera, zoom);
        }

        /// <summary>
        /// Draw all the entities to the screen.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="camera">Current camera position.</param>
        /// <param name="zoom">Current zoom level.</param>
        void DrawEntities(GameTime gameTime, Vector2 camera, float zoom)
        {
            SpriteBatch.Begin(camera, zoom, Game.GraphicsDevice.Viewport);
            foreach (var ent in _entities.ToList())
                ent.Draw(gameTime, SpriteBatch);
            SpriteBatch.End();
        }

        /// <summary>
        /// Draw the stars background, based on the camera position.
        /// </summary>
        /// <param name="camera">Current camera position.</param>
        /// <param name="zoom">Current zoom level.</param>
        void DrawStars(Vector2 camera, float zoom)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            SpriteBatch.Draw(_background,
                drawRectangle: GraphicsDevice.Viewport.Bounds,
                sourceRectangle: new Rectangle(
                    (int)(camera.X * BackgroundSlideFactor),
                    (int)(camera.Y * BackgroundSlideFactor),
                    (int)(_stars.Width),
                    (int)(_stars.Height)).Scale(1 / ScaleZoom(zoom, BackgroundMaxScale, BackgroundMinScale)),
                    color: Color.White);
            SpriteBatch.Draw(_stars,
                drawRectangle: GraphicsDevice.Viewport.Bounds,
                sourceRectangle: new Rectangle(
                    (int)(camera.X * StarsSlideFactor),
                    (int)(camera.Y * StarsSlideFactor),
                    (int)(_stars.Width),
                    (int)(_stars.Height)).Scale(1 / ScaleZoom(zoom, StarsMaxScale, StarsMinScale)),
                    color: Color.White);
            SpriteBatch.End();
        }

        /// <summary>
        /// Gets the current zoom factor based on the ships distance.
        /// </summary>
        /// <returns>The zoom factor to be used by the game.</returns>
        float GetZoomFactor()
        {
            var dist = _ships.Last().Position - _ships.First().Position;
            var distLength = dist.Length();

            if (distLength < MinZoomDistance)
                return MaxZoomFactor;
            else if (distLength > MaxZoomDistance)
                return MinZoomFactor;

            return XNATweener.Cubic.EaseOut(distLength - MinZoomDistance, MaxZoomFactor, MinZoomFactor - MaxZoomFactor, MaxZoomDistance - MinZoomDistance);
        }

        /// <summary>
        /// Converts a zoom factor to another scale.
        /// This is used to minimize the zoom applied to the stars.
        /// </summary>
        /// <param name="zoom">Current zoom level.</param>
        /// <param name="max">Maximum zoom on the new scale.</param>
        /// <param name="min">Minimum zoom on the new scale.</param>
        /// <returns>The scaled zoom factor.</returns>
        float ScaleZoom(float zoom, float max, float min)
        {
            float status = (zoom - MinZoomFactor) / (MaxZoomFactor - MinZoomFactor);
            return min + status * (max - min);
        }
        #endregion
        #endregion
    }
}
