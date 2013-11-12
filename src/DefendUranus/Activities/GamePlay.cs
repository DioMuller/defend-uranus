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
using MonoGameLib.Core;
using MonoGameLib.Core.Extensions;
using MonoGameLib.GUI.Components;
using DefendUranus.Components;
#endregion

namespace DefendUranus.Activities
{
    /// <summary>
    /// The Game Play screen.
    /// It shows the players ship and finishes when one of them is destroyed.
    /// </summary>
    class GamePlay : GameActivity<GamePlay.Result>
    {
        #region Constants
        const float MaxZoomFactor = 2f;
        const float MinZoomFactor = 0.5f;

        const float MinZoomDistance = 150;
        const float MaxZoomDistance = 650;

        public const float BackgroundSlideFactor = 0.05f;
        const float BackgroundMaxScale = 0.9f;
        const float BackgroundMinScale = BackgroundMaxScale * 0.75f;
        public const float StarsSlideFactor = 0.4f;
        const float StarsMaxScale = 1f;
        const float StarsMinScale = StarsMaxScale * 0.6f;

        readonly TimeSpan SpawnAsteroidsDelay = TimeSpan.FromSeconds(30);
        #endregion

        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }
            public int Winner { get; set; }
            public TimeSpan Duration { get; set; }
        }

        public class CameraInfo
        {
            public Vector2 Position { get; private set; }
            public float ZoomFactor { get; private set; }

            public CameraInfo(Vector2 position, float zoomFactor)
            {
                Position = position;
                ZoomFactor = zoomFactor;
            }

            public Rectangle GetArea(Viewport viewport)
            {
                var screenWidth = (int)(viewport.Width / ZoomFactor / 2);
                var screenHeight = (int)(viewport.Height / ZoomFactor / 2);

                return new Rectangle(
                    x: (int)(Position.X) - screenWidth / 2,
                    y: (int)(Position.Y) - screenHeight / 2,
                    width: screenWidth,
                    height: screenHeight);
            }
        }
        #endregion

        #region Attributes
        readonly PhysicsEntity _baseEntity;
        readonly AsyncOperation _spawnAsteroids;

        readonly List<GamePlayEntity> _entities;
        readonly List<Ship> _ships;

        GameInput _gameInput;
        TimeSpan _duration;

        Texture2D _background, _stars;
        PlayerGUI _p1Gui, _p2Gui;
        #endregion

        #region Properties
        /// <summary>
        /// All the entities contained in this level.
        /// </summary>
        public IEnumerable<GamePlayEntity> Entities { get { return _entities; } }
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
            const int guiHMargin = 10;
            var guiSize = new Point(100, 140);

            var p1Ship = new Ship(this, setup.Player1Selection) { Position = new Vector2(-100, 0) };
            var p2Ship = new Ship(this, setup.Player2Selection) { Position = new Vector2(100, 0) };

            p1Ship.Behaviors.Add(new ShipInputBehavior(PlayerIndex.One, p1Ship));
            p2Ship.Behaviors.Add(new ShipInputBehavior(PlayerIndex.Two, p2Ship));

            _baseEntity = new PhysicsEntity();
            _ships = new List<Ship> { p1Ship, p2Ship };
            _entities = new List<GamePlayEntity>(_ships);
            _duration = TimeSpan.Zero;

            _spawnAsteroids = new AsyncOperation(SpawnAsteroids);

            _p1Gui = new PlayerGUI("Player 1", p1Ship, new Point(guiHMargin, 0), guiSize);
            _p2Gui = new PlayerGUI("Player 2", p2Ship, new Point(GraphicsDevice.Viewport.Width - guiSize.X - guiHMargin, 0), guiSize);
        }
        #endregion

        #region Activity Life-Cycle
        /// <summary>
        /// Prepares the activity to be started.
        /// </summary>
        protected override void Starting()
        {
            base.Starting();
            _spawnAsteroids.IsActive = true;
        }

        /// <summary>
        /// The activity is being finished.
        /// </summary>
        protected override void Completing()
        {
            base.Completing();
            _spawnAsteroids.IsActive = false;
        }

        /// <summary>
        /// Prepares the activity to be activated.
        /// </summary>
        protected override void Activating()
        {
            base.Activating();
            _gameInput = new GameInput();
            _background = Content.Load<Texture2D>("Backgrounds/Background");
            _stars = Content.Load<Texture2D>("Backgrounds/Background2");

            SoundManager.PlayBGM("Failing Defense");
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

            UpdateRelativeSpeeds();
            UpdateEntities(gameTime);
            UpdateCyclicSpace();
        }

        /// <summary>
        /// Convert all the speeds in the game to a relative value.
        /// The speed are related to the currently slower player.
        /// </summary>
        void UpdateRelativeSpeeds()
        {
            var p1Speed = _ships[0].AbsoluteSpeed();
            var p2Speed = _ships[1].AbsoluteSpeed();

            var minMomentum = _ships[p1Speed < p2Speed? 0 : 1].Momentum;

            foreach (var ent in _entities)
            {
                ent.Momentum -= minMomentum;
                ent.AbsoluteMomentum += minMomentum;
            }

            _baseEntity.Momentum += minMomentum;
        }

        /// <summary>
        /// Update all the entities and handle collisions.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        void UpdateEntities(GameTime gameTime)
        {
            _baseEntity.Update(gameTime);

            var camera = GetCamera();

            var upEnt = _entities.ToList();
            for (int i = 0; i < upEnt.Count; i++)
            {
                var ent = upEnt[i];
                if (!IsValidEntity(camera, ent))
                    continue;

                ent.Update(gameTime);

                for (int j = i + 1; j < upEnt.Count; j++)
                {
                    var cEnt = upEnt[j];
                    var dist = ent.Position - cEnt.Position;

                    var minDist = ent.Size.Y / 2 + cEnt.Size.Y / 2;

                    // Handle gravity
                    if (dist != Vector2.Zero)
                    {
                        var distSquare = Math.Max(dist.LengthSquared(), 4096);
                        var direction = Vector2.Normalize(dist);

                        var intensity = 1000 * (ent.Mass * cEnt.Mass) / distSquare;

                        ent.ApplyAcceleration(-direction * intensity);
                        cEnt.ApplyAcceleration(direction * intensity);
                    }

                    // Handle collisions
                    if (dist.Length() < minDist)
                        ResolveCollision(ent, cEnt, gameTime);
                }
            }
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
            GraphicsDevice.Clear(Color.Black);

            var camera = GetCamera();
            DrawStars(camera);
            DrawEntities(gameTime, camera);
            DrawGUI(gameTime);
        }

        /// <summary>
        /// Draw all the entities to the screen.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="camera">Current camera position.</param>
        void DrawEntities(GameTime gameTime, CameraInfo camera)
        {
            SpriteBatch.Begin(camera, GraphicsDevice.Viewport);
            foreach (var ent in _entities.ToList())
                ent.Draw(gameTime, SpriteBatch, ent.Color);
            SpriteBatch.End();
        }

        /// <summary>
        /// Draw the stars background, based on the camera position.
        /// </summary>
        /// <param name="camera">Current camera position.</param>
        void DrawStars(CameraInfo camera)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null);
            SpriteBatch.Draw(_background,
                drawRectangle: GraphicsDevice.Viewport.Bounds,
                sourceRectangle: new Rectangle(
                    (int)((_baseEntity.Position.X + camera.Position.X) * BackgroundSlideFactor),
                    (int)((_baseEntity.Position.X + camera.Position.Y) * BackgroundSlideFactor),
                    _stars.Width, _stars.Height)
                .Scale(1 / ScaleZoom(camera.ZoomFactor, BackgroundMaxScale, BackgroundMinScale)));
            SpriteBatch.Draw(_stars,
                drawRectangle: GraphicsDevice.Viewport.Bounds,
                sourceRectangle: new Rectangle(
                    (int)((_baseEntity.Position.X + camera.Position.X) * StarsSlideFactor),
                    (int)((_baseEntity.Position.Y + camera.Position.Y) * StarsSlideFactor),
                    _stars.Width, _stars.Height)
                .Scale(1 / ScaleZoom(camera.ZoomFactor, StarsMaxScale, StarsMinScale)));
            SpriteBatch.End();
        }

        /// <summary>
        /// Draws all players stats onto the screen.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        void DrawGUI(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            _p1Gui.Draw(gameTime, SpriteBatch);
            _p2Gui.Draw(gameTime, SpriteBatch);

            SpriteBatch.End();
        }

        /// <summary>
        /// Get the current camera position and zoom factor.
        /// </summary>
        /// <returns>The camera information.</returns>
        CameraInfo GetCamera()
        {
            if (_ships.Count == 0)
                throw new InvalidOperationException();

            if (_ships.Count == 1)
                return new CameraInfo(_ships[0].Position, MaxZoomFactor);

            var p1 = _ships[0];
            var p2 = _ships[1];

            return new CameraInfo
            (
                position: (p1.Position + p2.Position) / 2,
                zoomFactor: GetZoomFactor((p2.Position - p1.Position).Length())
            );
        }

        /// <summary>
        /// Gets the current zoom factor based on the ships distance.
        /// </summary>
        /// <returns>The zoom factor to be used by the game.</returns>
        float GetZoomFactor(float distance)
        {
            if (distance < MinZoomDistance)
                return MaxZoomFactor;
            if (distance > MaxZoomDistance)
                return MinZoomFactor;
            return XNATweener.Cubic.EaseOut(distance - MinZoomDistance, MaxZoomFactor, MinZoomFactor - MaxZoomFactor, MaxZoomDistance - MinZoomDistance);
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

        #region Private
        /// <summary>
        /// Continuously spawn asteroids in the screen.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that will stop the creation of asteroids.</param>
        /// <returns>The task that will create asteroids in the screen.</returns>
        async Task SpawnAsteroids(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Delay(SpawnAsteroidsDelay, cancellationToken);
                SpawnAsteroid();
            }
        }

        /// <summary>
        /// Spawn a single asteroid at the screen.
        /// </summary>
        void SpawnAsteroid()
        {
            const int AsteroidMass = 5;
            const int AsteroidMinSpeed = 1;
            const int AsteroidMaxSpeed = 10;
            const int AsteroidMaxRotationSpeed = 2;

            var camera = GetCamera();
            var area = camera.GetArea(GraphicsDevice.Viewport);

            var directionFromCamera = RandomNumberGenerator.NextDirection();
            var screenSizeRatio = Math.Min((area.Width + 16) / Math.Abs(directionFromCamera.X),
                                           (area.Height + 16) / Math.Abs(directionFromCamera.Y));

            AddEntity(new GamePlayEntity(this, "Sprites/Asteroid")
            {
                Mass = AsteroidMass,
                MaxSpeed = AsteroidMaxSpeed,
                MaxRotationSpeed = AsteroidMaxRotationSpeed,
                Position = camera.Position + directionFromCamera * screenSizeRatio,
                Momentum = -directionFromCamera * RandomNumberGenerator.Next(AsteroidMinSpeed, AsteroidMaxSpeed),
                AngularMomentum = RandomNumberGenerator.Next(-AsteroidMaxRotationSpeed, AsteroidMaxRotationSpeed)
            });
        }

        /// <summary>
        /// Verifies if a specified entity can be used in the game.
        /// </summary>
        /// <param name="camera">Current game camera.</param>
        /// <param name="entity">The entity to be checked.</param>
        /// <returns>True if the entity can be used in the game, otherwise False.</returns>
        bool IsValidEntity(CameraInfo camera, GamePlayEntity entity)
        {
            const int maxDistanceSqr = 1000000;

            // Check the entity distance from the screen.
            var distance = entity.Position - camera.Position;
            if (distance.LengthSquared() > maxDistanceSqr)
            {
                RemoveEntity(entity);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Transfers energy from one entity to another.
        /// </summary>
        /// <param name="a">Entity A</param>
        /// <param name="b">Entity B</param>
        /// <param name="gameTime">Current game time.</param>
        void ResolveCollision(GamePlayEntity a, GamePlayEntity b, GameTime gameTime)
        {
            var aDirection = Vector2.Normalize(a.Momentum);
            var bDirection = Vector2.Normalize(b.Momentum);

            var normal = a.Position - b.Position;
            var dist = normal;
            normal.Normalize();

            // Calculate relative velocity
            Vector2 rv = b.Momentum - a.Momentum;

            // Calculate relative velocity in terms of the normal direction
            float velAlongNormal = Vector2.Dot(rv, normal);

            // Do not resolve if velocities are separating
            if (velAlongNormal < 0)
                return;

            // Calculate restitution (bounciness)
            float e = MathHelper.Min(a.Restitution, b.Restitution);

            // Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal;
            j /= 1 / a.Mass + 1 / b.Mass;

            #region Apply impulse
            Vector2 impulse = j * normal;
            a.ApplyForce(-impulse, instantaneous: true);
            b.ApplyForce(impulse, instantaneous: true);
            #endregion

            #region Apply Rotation
            if (velAlongNormal > 2)
            {
                var collisionPlane = new Vector2(-normal.Y, normal.X);

                if (a.Momentum != Vector2.Zero)
                {
                    var angle = Vector2.Dot(aDirection, collisionPlane);

                    if (!b.RotateToMomentum)
                        b.ApplyRotation(angle * -j * a.Mass / b.Mass, isAcceleration: false, instantaneous: true);
                }
                if (b.Momentum != Vector2.Zero)
                {
                    var angle = -Vector2.Dot(bDirection, collisionPlane);

                    if (!a.RotateToMomentum)
                        a.ApplyRotation(angle * -j * b.Mass / a.Mass, isAcceleration: false, instantaneous: true);
                }
            }
            #endregion

            #region Stops overlapping
            var distLength = dist.Length();
            if (distLength < 16)
                a.Position += normal * (16 - distLength);
            #endregion

            a.RaiseCollision(b, gameTime, this);
            b.RaiseCollision(a, gameTime, this);
        }
        #endregion

        #region Public
        /// <summary>
        /// Adds an entity to the game.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        public void AddEntity(GamePlayEntity entity)
        {
            _entities.Add(entity);
        }

        /// <summary>
        /// Removes an entity from the game.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        public void RemoveEntity(GamePlayEntity entity)
        {
            _entities.Remove(entity);
        }
        #endregion
    }
}
