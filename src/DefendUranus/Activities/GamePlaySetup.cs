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
#endregion

namespace DefendUranus.Activities
{
    class GamePlaySetup : GameActivity<GamePlaySetup.Result>
    {
        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }

            public Ship Player1Ship { get; set; }
            public Ship Player2Ship { get; set; }
        }
        #endregion

        #region Attributes
        int _spacing;
        KeyboardWatcher _keyboard;
        Result _result;
        List<Ship> _ships;
        int[] _option;
        int[] _shift = new[] { 0, 0 };
        bool[] _shifting = new[] { false, false };
        Dictionary<int, Vector2>[] _iconScales;
        #endregion

        #region Constructors
        public GamePlaySetup(MainGame game)
            : base(game)
        {
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
        }

        protected override void Activating()
        {
            base.Activating();
            _keyboard = new KeyboardWatcher();

            // TODO: Load ships from XML
            // TODO: Change to ship definitions, and create the ships on gameplay.
            // this should fix the bug when 2 players select the same ship
            _ships = new List<Ship>
            {
                new Ship("Sprites/Avenger") { Mass = 2, Description = "Earth Avenger" },
                new Ship("Sprites/Explorer") { Mass = 1, Description = "Uranus Explorer" },
                new Ship("Sprites/Fatboy") { Mass = 4, Description = "Big Fatboy" },
                new Ship("Sprites/Meteoroid") { Mass = 3, Description = "Meteoroid Destroyer" },
            };
            _spacing = (int)Game.Window.ClientBounds.Width / _ships.Count;
            _result.Player1Ship = _ships[0];
            _result.Player2Ship = _ships[_ships.Count / 2];


            _option = new[] {
                _ships.IndexOf(_result.Player1Ship),
                _ships.IndexOf(_result.Player2Ship)
            };

            _iconScales = new[]{
                Enumerable.Range(0, _ships.Count).ToDictionary(i => i, i => Vector2.One),
                Enumerable.Range(0, _ships.Count).ToDictionary(i => i, i => Vector2.One)
            };
            _iconScales[0][_option[0]] = new Vector2(2);
            _iconScales[1][_option[1]] = new Vector2(2);
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            if (_keyboard.IsPressed(Keys.Escape))
            {
                _result.Aborted = true;
                Exit(_result);
            }
            else if (_keyboard.IsPressed(Keys.Enter))
            {
                Exit(_result);
            }

            if (_keyboard.IsPressed(Keys.Right))
                ShiftSelection(0, left: false);
            else if (_keyboard.IsPressed(Keys.Left))
                ShiftSelection(0, left: true);

            if (_keyboard.IsPressed(Keys.D))
                ShiftSelection(1, left: false);
            else if (_keyboard.IsPressed(Keys.A))
                ShiftSelection(1, left: true);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.DarkGreen);

            SpriteBatch.Begin();
            DrawPlayerSelection(gameTime, 0, 200);
            DrawPlayerSelection(gameTime, 1, 400);
            SpriteBatch.End();
        }

        private void DrawPlayerSelection(GameTime gameTime, int player, int height)
        {
            int sideShips = _ships.Count - 2;
            int width = (sideShips * 2 + 1) * _spacing;
            int x = (Game.Window.ClientBounds.Width - width) / 2 + _spacing / 2;

            var option = _option[player];
            for (int i = option - sideShips; i <= option + sideShips; i++, x += _spacing)
            {
                int ship = Mod(i, _ships.Count);
                _ships[ship].Sprite.Draw(gameTime, SpriteBatch,
                    position: new Vector2(x + _shift[player], height),
                    color: Color.White,
                    scale: _iconScales[player][ship]);
            }
        }
        #endregion

        #region Private
        async void ShiftSelection(int player, bool left)
        {
            if (_shifting[player]) return;
            _shifting[player] = true;

            var oldOption = _option[player];
            var nextOption = Mod(_option[player] + (left ? -1 : 1), _ships.Count);

            if (player == 0)
                _result.Player1Ship = _ships[nextOption];
            else if (player == 1)
                _result.Player2Ship = _ships[nextOption];

            // Anima zoom in, de nave selecionada
            var zoomIn = FloatAnimation(300, 1, 2, v => _iconScales[player][nextOption] = new Vector2(v), easingFunction: Sinusoidal.EaseOut);
            // Anima zoom out, de nave desselecionada
            var zoomOut = FloatAnimation(100, 2, 1, v => _iconScales[player][oldOption] = new Vector2(v));

            // Anima deslocamento para direita/esquerda
            await FloatAnimation(100, 0, _spacing * (left ? 1 : -1), v => _shift[player] = (int)v);

            _shift[player] = 0;
            _option[player] = nextOption;

            await TaskEx.WhenAll(zoomIn, zoomOut);

            _shifting[player] = false;
        }

        int Mod(int value, int max)
        {
            if (value < 0)
                return value % max + max;
            if (value >= max)
                return value % max;
            return value;
        }
        #endregion
    }
}

