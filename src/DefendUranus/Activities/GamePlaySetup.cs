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
#endregion

namespace DefendUranus.Activities
{
    class GamePlaySetup : GameActivity<GamePlaySetup.Result>
    {
        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }

            public ShipDescription Player1Ship { get; set; }
            public ShipDescription Player2Ship { get; set; }
        }

        class PlayerSelectionInfo
        {
            public int DrawShift { get; set; }
            public bool ShiftingSelection { get; set; }
            //public bool SelectionConfirmed { get; set; }
            public Dictionary<ShipDescription, Vector2> IconScales { get; set; }
        }
        public class ShipDescription
        {
            public ShipDescription(ContentManager content, string texturePath, float mass, string description)
            {
                Texture = content.Load<Texture2D>(texturePath);
                TexturePath = texturePath;
                Mass = mass;
                Description = description;
            }

            public Texture2D Texture { get; private set; }
            public string TexturePath { get; set; }
            public string Description { get; set; }
            public float Mass { get; set; }

            public Ship BuildShip()
            {
                return new Ship(TexturePath) { Mass = Mass };
            }
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
            _ships = LoadShips();
            _spacing = (int)GraphicsDevice.PresentationParameters.BackBufferWidth / _ships.Count;
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Starting()
        {
            base.Starting();

            if (_result == null)
                _result = new Result();
            else
                _result.Aborted = false;


            _result.Player1Ship = _ships[0];
            _result.Player2Ship = _ships[_ships.Count / 2];
        }

        protected override void Activating()
        {
            base.Activating();
            _p1Input = new PlayerInput(PlayerIndex.One);
            _p2Input = new PlayerInput(PlayerIndex.Two);
            _gameInput = new GameInput();

            _p1Info = new PlayerSelectionInfo
            {
                IconScales = _ships.ToDictionary(i => i, i => i == _result.Player1Ship ? new Vector2(2) : Vector2.One)
            };
            _p2Info = new PlayerSelectionInfo
            {
                IconScales = _ships.ToDictionary(i => i, i => i == _result.Player2Ship ? new Vector2(2) : Vector2.One)
            };
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _p1Input.Update();
            _p2Input.Update();
            _gameInput.Update();

            if (_gameInput.Cancel)
            {
                _result.Aborted = true;
                Exit(_result);
            }
            // TODO: Start the game when both players have confirmed selection.
            else if (_gameInput.Confirm)
            {
                Exit(_result);
            }

            if (_p1Input.Right)
                ShiftSelection(_p1Info, left: false);
            else if (_p1Input.Left)
                ShiftSelection(_p1Info, left: true);

            if (_p2Input.Right)
                ShiftSelection(_p2Info, left: false);
            else if (_p2Input.Left)
                ShiftSelection(_p2Info, left: true);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.DarkGreen);

            SpriteBatch.Begin();
            DrawPlayerSelection(gameTime, _result.Player1Ship, _p1Info, 200);
            DrawPlayerSelection(gameTime, _result.Player2Ship, _p2Info, 400);
            SpriteBatch.End();
        }

        private void DrawPlayerSelection(GameTime gameTime, ShipDescription selection, PlayerSelectionInfo info, int height)
        {
            int sideShips = _ships.Count - 2;
            int width = (sideShips * 2 + 1) * _spacing;
            int x = (Game.Window.ClientBounds.Width - width) / 2 + _spacing / 2;

            var option = _ships.IndexOf(selection);
            for (int i = option - sideShips; i <= option + sideShips; i++, x += _spacing)
            {
                var ship = _ships[Mod(i, _ships.Count)];
                SpriteBatch.Draw(ship.Texture,
                    position: new Vector2(x + info.DrawShift, height),
                    scale: info.IconScales[ship],
                    origin: new Vector2(ship.Texture.Width / 2, ship.Texture.Height / 2),
                    color: Color.White);
            }
        }
        #endregion

        #region Private
        async void ShiftSelection(PlayerSelectionInfo info, bool left)
        {
            if (info.ShiftingSelection) return;
            info.ShiftingSelection = true;

            var oldOption = info == _p1Info ? _result.Player1Ship : _result.Player2Ship;
            var nextOption = _ships[Mod(_ships.IndexOf(oldOption) + (left ? -1 : 1), _ships.Count)];

            if (info == _p1Info)
                _result.Player1Ship = nextOption;
            else
                _result.Player2Ship = nextOption;

            // Anima zoom in, de nave selecionada
            var zoomIn = FloatAnimation(300, 1, 2, v => info.IconScales[nextOption] = new Vector2(v), easingFunction: Sinusoidal.EaseOut);
            // Anima zoom out, de nave desselecionada
            var zoomOut = FloatAnimation(100, 2, 1, v => info.IconScales[oldOption] = new Vector2(v));

            // Anima deslocamento para direita/esquerda
            await FloatAnimation(100, _spacing * (left ? -1 : +1), 0, v => info.DrawShift = (int)v);

            await TaskEx.WhenAll(zoomIn, zoomOut);

            info.ShiftingSelection = false;
        }

        int Mod(int value, int max)
        {
            if (value < 0)
                return value % max + max;
            if (value >= max)
                return value % max;
            return value;
        }

        List<ShipDescription> LoadShips()
        {
            // TODO: Load ships from XML
            return new List<ShipDescription>
            {
                new ShipDescription(Content, "Sprites/Avenger", 2, "Earth Avenger"),
                new ShipDescription(Content, "Sprites/Explorer", 1, "Uranus Explorer"),
                new ShipDescription(Content, "Sprites/Fatboy", 4, "Big Fatboy"),
                new ShipDescription(Content, "Sprites/Meteoroid", 3, "Meteoroid Destroyer"),
            };
        }
        #endregion
    }
}

