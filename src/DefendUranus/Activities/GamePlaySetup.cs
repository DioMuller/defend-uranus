#region Using Statements
using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using MonoGameLib.Core.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace DefendUranus.Activities
{
    class GamePlaySetup : GameActivity<GamePlaySetup.Result>
    {
        #region Nested
        public class Result
        {
            public bool Aborted { get; set; }

            public string Player1Ship { get; set; }
            public string Player2Ship { get; set; }
        }
        #endregion

        #region Attributes
        KeyboardWatcher _keyboard;
        Result _result;
        List<Texture2D> _ships;
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

            _ships = new List<Texture2D>();
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
            _ships.AddRange(new[]
            {
                Content.Load<Texture2D>("Sprites/Avenger"),
                Content.Load<Texture2D>("Sprites/Explorer"),
                Content.Load<Texture2D>("Sprites/Fatboy"),
                Content.Load<Texture2D>("Sprites/Meteoroid")
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
                FloatAnimation(100, 0, 48 * (left ? -1 : 1), v => _shift[player] = (int)v),
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
            DrawPlayerSelection(0, 32);
            DrawPlayerSelection(1, 320);
            SpriteBatch.End();
        }

        private void DrawPlayerSelection(int player, int height)
        {
            int sideShips = _ships.Count - 2;
            int spacing = 48;
            int width = (sideShips * 2 + 1) * spacing;
            var option = _option[player];
            for (int i = option - sideShips, x = (800 - width) / 2 + 32; i <= option + sideShips; i++, x += spacing)
            {
                int ship = Mod(i, _ships.Count);
                SpriteBatch.Draw(_ships[ship],
                    new Vector2(x + _shift[player], height),
                    color: Color.White,
                    scale: _iconScales[player][ship],
                    origin: new Vector2(16));
            }
        }
        #endregion
    }
}

