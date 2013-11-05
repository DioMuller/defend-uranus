#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using XNATweener;
using DefendUranus.Entities;
using Microsoft.Xna.Framework.Content;
using DefendUranus.Helpers;
using DefendUranus.SteeringBehaviors;
using MonoGameLib.Core.Extensions;
using DefendUranus.Entities.SpecialAttacks;
using MonoGameLib.GUI.Base;
#endregion

namespace DefendUranus.Activities
{
    /// <summary>
    /// The ship selecion screen.
    /// It shows the available ships with their information
    /// so the players can select their preference.
    /// </summary>
    class GamePlaySetup : GameActivity<GamePlaySetup.Result>
    {
        #region Nested
        /// <summary>
        /// Setup Result.
        /// Contains the information selected by the players before the game starts.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// True if the setup was cancelled by the user.
            /// </summary>
            public bool Aborted { get; set; }

            /// <summary>
            /// Player 1 ship selection.
            /// </summary>
            public ShipDescription Player1Selection { get; set; }
            /// <summary>
            /// Player 2 ship selection.
            /// </summary>
            public ShipDescription Player2Selection { get; set; }
        }

        /// <summary>
        /// Information about how to draw the user selection.
        /// </summary>
        class SelectionDrawInfo
        {
            public int DrawShift { get; set; }
            public bool ShiftingSelection { get; set; }
            //public bool SelectionConfirmed { get; set; }
            public IDictionary<ShipDescription, Vector2> IconScales { get; set; }
        }
        #endregion

        #region Constants
        readonly List<ShipDescription> _ships;
        readonly int _spacing;
        const int MaxVisibleShips = 6;
        #endregion

        #region Attributes
        readonly Result _result;
        readonly SelectionDrawInfo _p1Info, _p2Info;
        PlayerInput _p1Input, _p2Input;
        GameInput _gameInput;

        #region Content
        Texture2D _background, _stars;
        Vector2 _camera;

        public SpriteFont _bigFont;
        public SpriteFont _smallFont;
        #endregion

        #region Animations
        float _statsX, _descriptionOpacity;
        #endregion
        #endregion

        #region Constructors
        public GamePlaySetup(MainGame game)
            : base(game)
        {
            _ships = LoadShips().ToList();
            _spacing = GraphicsDevice.PresentationParameters.BackBufferWidth / Math.Min(_ships.Count, MaxVisibleShips);
            _result = new Result
            {
                Player1Selection = _ships[0],
                Player2Selection = _ships[_ships.Count / 2]
            };
            _p1Info = new SelectionDrawInfo { IconScales = GetDefaultScales(_result.Player1Selection) };
            _p2Info = new SelectionDrawInfo { IconScales = GetDefaultScales(_result.Player2Selection) };
        }
        #endregion

        #region Activity Life-Cycle
        /// <summary>
        /// Prepares the activity to be activated.
        /// </summary>
        protected override void Activating()
        {
            base.Activating();
            _p1Input = new PlayerInput(PlayerIndex.One);
            _p2Input = new PlayerInput(PlayerIndex.Two);
            _gameInput = new GameInput();

            _background = Content.Load<Texture2D>("Backgrounds/Background");
            _stars = Content.Load<Texture2D>("Backgrounds/Background2");
            _bigFont = Content.Load<SpriteFont>("Fonts/BigFont");
            _smallFont = Content.Load<SpriteFont>("Fonts/DefaultFont");
        }
        #endregion

        #region Activity Life-Cycle
        protected override Task IntroductionAnimation()
        {
            return TaskEx.WhenAll(
                base.IntroductionAnimation(),
                FloatAnimation(300, 0, 1, v => _descriptionOpacity = v),
                FloatAnimation(100, 200, 0, v => _statsX = v));
        }

        protected override Task ConclusionAnimation()
        {
            return TaskEx.WhenAll(
                base.ConclusionAnimation(),
                FloatAnimation(100, 0, -200, v => _statsX = v));
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
            UpdateInputs();

            if (IsSetupComplete())
                return;

            CheckPlayerInput(_p1Info, _p1Input);
            CheckPlayerInput(_p2Info, _p2Input);
        }

        /// <summary>
        /// Update all inputs.
        /// </summary>
        void UpdateInputs()
        {
            _p1Input.Update();
            _p2Input.Update();
            _gameInput.Update();
        }

        /// <summary>
        /// Check if the setup is complete.
        /// </summary>
        /// <returns>True if the activity was exited.</returns>
        bool IsSetupComplete()
        {
            if (_gameInput.Cancel)
            {
                _result.Aborted = true;
                Exit(_result);
                return true;
            }

            // TODO: Start the game when both players have confirmed selection.
            if (_gameInput.Confirm)
            {
                _result.Aborted = false;
                Exit(_result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the player input and execute its actions.
        /// </summary>
        /// <param name="info">Player info.</param>
        /// <param name="input">Player input.</param>
        void CheckPlayerInput(SelectionDrawInfo info, PlayerInput input)
        {
            if (input.Right)
                ShiftSelection(info, left: false);
            else if (input.Left)
                ShiftSelection(info, left: true);
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw the current activity to the screen.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            DrawStars(gameTime);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawPlayerSelection(_result.Player1Selection, _p1Info, 10);
            DrawPlayerSelection(_result.Player2Selection, _p2Info, 310);
            SpriteBatch.End();
        }

        /// <summary>
        /// Draw the stars background, based on the camera position.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        void DrawStars(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null);
            SpriteBatch.Draw(_background,
                drawRectangle: GraphicsDevice.Viewport.Bounds,
                sourceRectangle: new Rectangle(
                    (int)(_camera.X * GamePlay.BackgroundSlideFactor),
                    (int)(_camera.Y * GamePlay.BackgroundSlideFactor),
                    _stars.Width, _stars.Height)
                .Scale(1.1f));
            SpriteBatch.Draw(_stars,
                drawRectangle: GraphicsDevice.Viewport.Bounds,
                sourceRectangle: new Rectangle(
                    (int)(_camera.X * GamePlay.StarsSlideFactor),
                    (int)(_camera.Y * GamePlay.StarsSlideFactor),
                    _stars.Width, _stars.Height));
            SpriteBatch.End();

            Vector2 cameraDirection = new Vector2(2, 0);
            _camera += WorldHelper.MetersToPixels(cameraDirection) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Draw the ship selection for the specified player at the specified position.
        /// </summary>
        /// <param name="selection">Current player selection.</param>
        /// <param name="drawInfo">Current player draw info.</param>
        /// <param name="y">The y position on the screen where the selection will be drawn.</param>
        void DrawPlayerSelection(ShipDescription selection, SelectionDrawInfo drawInfo, int y)
        {
            SpriteBatch.DrawString(_bigFont, selection.Name, new Vector2(Game.Window.ClientBounds.Width / 2, y), new Color(Color.White, _descriptionOpacity), HorizontalAlign.Center);

            int sideShips = Math.Min(_ships.Count, MaxVisibleShips) - 2;
            int width = (sideShips * 2 + 1) * _spacing;
            int x = (Game.Window.ClientBounds.Width - width) / 2 + _spacing / 2;

            var option = _ships.IndexOf(selection);
            for (int i = option - sideShips; i <= option + sideShips; i++, x += _spacing)
            {
                var ship = _ships[i.Mod(_ships.Count)];
                SpriteBatch.Draw(ship.Texture,
                    position: new Vector2(x + drawInfo.DrawShift, y + 110),
                    scale: drawInfo.IconScales[ship],
                    origin: new Vector2(ship.Texture.Width / 2, ship.Texture.Height / 2));
            }

            #region Stats
            int statsY = y + 180;

            var leftStats = new Dictionary<string, object>
            {
                { "Special", selection.SpecialAttack.Name },
                { "Speed", selection.MaxSpeed }
            };

            var rightStats = new Dictionary<string, object>
            {
                { "Mass", selection.Mass },
                { "Fuel Duration", selection.FuelDuration.ToString("%s") },
            };

            var leftStatsString = string.Join("\r\n", leftStats.Select(k => k.Key + ": " + k.Value));
            var rightStatsString = string.Join("\r\n", rightStats.Select(k => k.Key + ": " + k.Value));

            SpriteBatch.DrawString(_smallFont, leftStatsString, new Vector2(_statsX + 40, statsY), Color.Gray);
            SpriteBatch.DrawString(_smallFont, rightStatsString, new Vector2(_statsX + 420, statsY), Color.Gray);
            #endregion
        }
        #endregion
        #endregion

        #region Private
        #region Player input actions
        /// <summary>
        /// Changes the currently selected ship.
        /// </summary>
        /// <param name="info">Info of the player changing ship selection.</param>
        /// <param name="left">True if selecting the ship to the left.</param>
        async void ShiftSelection(SelectionDrawInfo info, bool left)
        {
            if (info.ShiftingSelection) return;
            info.ShiftingSelection = true;

            // Calcula próxima nave, de acordo com direção pressionada
            var oldOption = info == _p1Info ? _result.Player1Selection : _result.Player2Selection;
            var nextOption = _ships[(_ships.IndexOf(oldOption) + (left ? -1 : 1)).Mod(_ships.Count)];

            // Troca a nave selecionada pelo jogador
            if (info == _p1Info)
                _result.Player1Selection = nextOption;
            else
                _result.Player2Selection = nextOption;

            // Realiza animação de transição
            await TaskEx.WhenAll(
                // Anima zoom in, de nave selecionada
                FloatAnimation(300, 1, 2, v => info.IconScales[nextOption] = new Vector2(v), easingFunction: Sinusoidal.EaseOut),
                // Anima zoom out, de nave desselecionada
                FloatAnimation(100, 2, 1, v => info.IconScales[oldOption] = new Vector2(v)),
                // Anima deslocamento para direita/esquerda
                FloatAnimation(100, _spacing * (left ? -1 : +1), 0, v => info.DrawShift = (int)v)
            );

            info.ShiftingSelection = false;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Calculates the initial ship scale values, based on the player selection.
        /// </summary>
        /// <param name="selection">Current player selection.</param>
        /// <returns>Dictionary containing an Scale value for each ship.</returns>
        IDictionary<ShipDescription, Vector2> GetDefaultScales(ShipDescription selection)
        {
            return _ships.ToDictionary(i => i, i => i == selection ? new Vector2(2) : Vector2.One);
        }

        /// <summary>
        /// Load all the ships available to the players.
        /// </summary>
        /// <returns>Descriptions of each ship in the game.</returns>
        IEnumerable<ShipDescription> LoadShips()
        {
            // TODO: Load ships from XML
            yield return new ShipDescription(Content, "Sprites/Avenger", "Earth Avenger", SpecialAttack("Pursuiter Missile"), mass: 2, maxSpeed: 10, fuel: TimeSpan.FromSeconds(3));
            yield return new ShipDescription(Content, "Sprites/Explorer", "Uranus Explorer", SpecialAttack("Wanderer Probe"), mass: 1, maxSpeed: 10, fuel: TimeSpan.FromSeconds(2));
            yield return new ShipDescription(Content, "Sprites/Fatboy", "Big Fatboy", SpecialAttack("Fleeing Fake"), mass: 2, maxSpeed: 10, fuel: TimeSpan.FromSeconds(5));
            yield return new ShipDescription(Content, "Sprites/Meteoroid", "Meteoroid Destroyer", SpecialAttack("Fleeing Fake"), mass: 3, maxSpeed: 13, fuel: TimeSpan.FromSeconds(4));
        }

        static ShipDescription.Special SpecialAttack(string name)
        {
            var specialCreators = new Dictionary<string, Ship.SpecialAttackCreator>
            {
                { "Pursuiter Missile", ship => new PursuiterMissile(ship) },
                { "Wanderer Probe", ship => new WandererProbe(ship) },
                { "Fleeing Fake", ship => new FleeingFake(ship) },
                //{ "Meteoroid Destroyer", ship => new WandererProbe(ship) },
            };

            if (!specialCreators.ContainsKey(name))
                throw new NotImplementedException("Special attack \"" + name + "\" could not be found.");

            return new ShipDescription.Special
            {
                Name = name,
                Creator = specialCreators[name]
            };
        }
        #endregion
        #endregion
    }
}
