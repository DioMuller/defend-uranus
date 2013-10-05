#region Using Statements
using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using MonoGameLib.Core.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
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
        const int _spacing = 72;
        KeyboardWatcher _keyboard;
        Result _result;
        List<Ship> _ships;
        int[] _option = new[] { 0, 0 };
        int[] _shift = new[] { 0, 0 };
        bool[] _shifting = new[] { false, false };
        Dictionary<int, Vector2>[] _iconScales;
        #endregion

        #region Constructors
        public GamePlaySetup(MainGame game)
            : base(game)
        {
            _result = new Result();

            _ships = new List<Ship>();
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Starting()
        {
            base.Starting();
            _result.Aborted = false;
        }

        protected override void Activating()
        {
            base.Activating();
            _keyboard = new KeyboardWatcher();
            // TODO: Load ships from XML
            _ships.AddRange(new[]
            {
                new Ship { Texture = Content.Load<Texture2D>("Sprites/Avenger"), Description = "Earth Avenger" },
                new Ship { Texture = Content.Load<Texture2D>("Sprites/Explorer"), Description = "Uranus Explorer" },
                new Ship { Texture = Content.Load<Texture2D>("Sprites/Fatboy"), Description = "Big Fatboy" },
                new Ship { Texture = Content.Load<Texture2D>("Sprites/Meteoroid"), Description = "Meteoroid Destroyer" },
            });
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
                Shift(0, left: false);
            else if (_keyboard.IsPressed(Keys.Left))
                Shift(0, left: true);

            if (_keyboard.IsPressed(Keys.D))
                Shift(1, left: false);
            else if(_keyboard.IsPressed(Keys.A))
                Shift(1, left: true);

            base.Update(gameTime);
        }

        async void Shift(int player, bool left)
        {
            if (_shifting[player]) return;

            _shifting[player] = true;
            var nextOption = Mod(_option[player] + (left ? 1 : -1), _ships.Count);

            await TaskEx.WhenAll(
                FloatAnimation(100, 0, _spacing * (left ? -1 : 1), v => _shift[player] = (int)v),
                FloatAnimation(100, 2, 1, v => _iconScales[player][_option[player]] = new Vector2(v)),
                FloatAnimation(100, 1, 2, v => _iconScales[player][nextOption] = new Vector2(v))
            );

            _shift[player] = 0;
            _option[player] = nextOption;
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

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.DarkGreen);

            SpriteBatch.Begin();
            DrawPlayerSelection(0, 200);
            DrawPlayerSelection(1, 400);
            SpriteBatch.End();
        }

        private void DrawPlayerSelection(int player, int height)
        {
            int sideShips = _ships.Count - 2;
            int width = (sideShips * 2 + 1) * _spacing;
            var option = _option[player];
            for (int i = option - sideShips, x = (800 - width) / 2 + 32; i <= option + sideShips; i++, x += _spacing)
            {
                int ship = Mod(i, _ships.Count);
                SpriteBatch.Draw(_ships[ship].Texture,
                    new Vector2(x + _shift[player], height),
                    color: Color.White,
                    scale: _iconScales[player][ship],
                    origin: new Vector2(16));
            }
        }
        #endregion
    }
}

