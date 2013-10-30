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
#endregion

namespace DefendUranus.Activities
{
    class GamePlaySetup : GameActivity<GamePlaySetup.Result>
    {
        #region Nested
        /// <summary>
        /// Setup Result.
        /// Contains the information selected before the game starts.
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
        /// Information about the user selection.
        /// </summary>
        class PlayerSelectionInfo
        {
            public int DrawShift { get; set; }
            public bool ShiftingSelection { get; set; }
            //public bool SelectionConfirmed { get; set; }
            public IDictionary<ShipDescription, Vector2> IconScales { get; set; }
        }
        #endregion

        #region Attributes
        readonly public List<ShipDescription> _ships;
        readonly int _spacing;

        Result _result;
        PlayerSelectionInfo _p1Info, _p2Info;
        PlayerInput _p1Input, _p2Input;
        GameInput _gameInput;
        #endregion

        #region Constructors
        public GamePlaySetup(MainGame game)
            : base(game)
        {
            _ships = LoadShips().ToList();
            _spacing = GraphicsDevice.PresentationParameters.BackBufferWidth / _ships.Count;
            _result = new Result
            {
                Player1Selection = _ships[0],
                Player2Selection = _ships[_ships.Count / 2]
            };
            _p1Info = new PlayerSelectionInfo { IconScales = GetIconScales(_result.Player1Selection) };
            _p2Info = new PlayerSelectionInfo { IconScales = GetIconScales(_result.Player2Selection) };
        }
        #endregion

        #region Activity Life-Cycle
        /// <summary>
        /// Fade-in / Fade-out the screen before / after completion.
        /// </summary>
        /// <returns>A task that represents the activity execution.</returns>
        protected async override Task<GamePlaySetup.Result> RunActivity()
        {
            await FadeIn(100);
            var result = await base.RunActivity();
            await FadeOut(100);

            return result;
        }

        /// <summary>
        /// Prepares the activity to be activated.
        /// </summary>
        protected override void Activating()
        {
            base.Activating();
            _p1Input = new PlayerInput(PlayerIndex.One);
            _p2Input = new PlayerInput(PlayerIndex.Two);
            _gameInput = new GameInput();
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
            base.Update(gameTime);

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
        void CheckPlayerInput(PlayerSelectionInfo info, PlayerInput input)
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
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.DarkGreen);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawPlayerSelection(gameTime, _result.Player1Selection, _p1Info, 200);
            DrawPlayerSelection(gameTime, _result.Player2Selection, _p2Info, 400);
            SpriteBatch.End();
        }

        /// <summary>
        /// Draw the ship selection for the specified player at the specified position.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <param name="selection">Current player selection.</param>
        /// <param name="info">Current player info.</param>
        /// <param name="height">The y position on the screen where the selection will be drawn.</param>
        private void DrawPlayerSelection(GameTime gameTime, ShipDescription selection, PlayerSelectionInfo info, int height)
        {
            int sideShips = _ships.Count - 2;
            int width = (sideShips * 2 + 1) * _spacing;
            int x = (Game.Window.ClientBounds.Width - width) / 2 + _spacing / 2;

            var option = _ships.IndexOf(selection);
            for (int i = option - sideShips; i <= option + sideShips; i++, x += _spacing)
            {
                var ship = _ships[i.Mod(_ships.Count)];
                SpriteBatch.Draw(ship.Texture,
                    position: new Vector2(x + info.DrawShift, height),
                    scale: info.IconScales[ship],
                    origin: new Vector2(ship.Texture.Width / 2, ship.Texture.Height / 2));
            }
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
        async void ShiftSelection(PlayerSelectionInfo info, bool left)
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
        /// Calculates the initial scale values, based on the player selection.
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        IDictionary<ShipDescription, Vector2> GetIconScales(ShipDescription selection)
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
            yield return new ShipDescription(Content, "Sprites/Avenger", 2, "Earth Avenger", ship => DeployAttack(ship, PursuiterMissile));
            yield return new ShipDescription(Content, "Sprites/Explorer", 1, "Uranus Explorer", ship => DeployAttack(ship, WanderAttack));
            yield return new ShipDescription(Content, "Sprites/Fatboy", 4, "Big Fatboy", ship => DeployAttack(ship, FleeingFake));
            yield return new ShipDescription(Content, "Sprites/Meteoroid", 3, "Meteoroid Destroyer", ship => DeployAttack(ship, WanderAttack));
        }

        #region Special Attacks
        SpecialAttack WanderAttack(Ship target)
        {
            var specialAttack = new SpecialAttack(target.Level, "Sprites/Explorer-WandererProbe.png")
            {
                RotateToMomentum = true,
                Momentum = Vector2.One,
                Mass = 1f,
                MaxSpeed = 10f,
            };
            var steeringBehavior = new Wander(specialAttack)
            {
                Target = target,
                Jitter = 1.25f,
                WanderDistance = 50f,
                WanderRadius = 90f,
            };

            specialAttack.SteeringBehaviors.Add(steeringBehavior);

            return specialAttack;
        }

        SpecialAttack PursuiterMissile(Ship target)
        {
            var specialAttack = new SpecialAttack(target.Level, "Sprites/Avenger-PursuiterMissile.png")
            {
                RotateToMomentum = true,
                Momentum = Vector2.One,
                Mass = 1f,
                MaxSpeed = 10f,
            };
            var steeringBehavior = new Pursuit(specialAttack)
            {
                Target = target,
            };

            specialAttack.SteeringBehaviors.Add(steeringBehavior);

            return specialAttack;
        }

        SpecialAttack FleeingFake(Ship target)
        {
            var specialAttack = new SpecialAttack(target.Level, "Sprites/Avenger-PursuiterMissile.png")
            {
                RotateToMomentum = true,
                Momentum = Vector2.One,
                Mass = 1f,
                MaxSpeed = 10f,
            };
            var steeringBehavior = new Flee(specialAttack)
            {
                PanicDistance = 500f,
                Target = target,
            };

            specialAttack.SteeringBehaviors.Add(steeringBehavior);

            return specialAttack;
        }
        #endregion
        #endregion

        #region Helpers
        void DeployAttack(Ship ship, Func<Ship, SpecialAttack> createAttack)
        {
            var target = ship.Level.Entities.OfType<Ship>()
                .Where(s => s != ship)
                .OrderBy(s => (s.Position - ship.Position).LengthSquared())
                .FirstOrDefault();

            var direction = Vector2Extension.AngleToVector2(ship.Rotation);

            var item = createAttack(target);
            item.Position = ship.Position + direction * ship.Size / 2;

            item.ApplyForce(direction * item.MaxSpeed, instantaneous: true);
            ship.ApplyForce(direction * item.MaxSpeed * -0.01f, instantaneous: true);

            ship.Level.AddEntity(item);
        }
        #endregion
        #endregion
    }
}
