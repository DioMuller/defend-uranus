using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Helpers
{
    public class PlayerInput
    {
        #region Nested
        public struct InputState
        {
            /// <summary>
            /// Thrust input state, from -1 to +1.
            /// </summary>
            public float Thrust;
            /// <summary>
            /// Rotation input state, from -1 to +1.
            /// </summary>
            public float Rotate;
            /// <summary>
            /// Indicates if the main weapon button is active.
            /// </summary>
            public bool FireMainWeapon;
            /// <summary>
            /// Indicates whether the special power button was pressed.
            /// </summary>
            public bool UseSpecialPower;
            /// <summary>
            /// The user have confirmed a menu option.
            /// </summary>
            public bool Confirm;
            /// <summary>
            /// The user have cancelled a menu.
            /// </summary>
            public bool Cancel;
            /// <summary>
            /// The user is moving the cursor up.
            /// </summary>
            public bool Up;
            /// <summary>
            /// The user is moving the cursor down.
            /// </summary>
            public bool Down;
            /// <summary>
            /// The user is moving the cursor left.
            /// </summary>
            public bool Left;
            /// <summary>
            /// The user is moving the cursor right.
            /// </summary>
            public bool Right;
        }
        #endregion

        #region Attributes
        InputState? _lastState;
        InputState? _state;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index of the player.
        /// </summary>
        /// <value>The index of the player.</value>
        public PlayerIndex PlayerIndex { get; private set; }

        /// <summary>
        /// Gets the current input state.
        /// This value is updated after calling the Update method.
        /// </summary>
        public InputState State
        {
            get { return _state ?? default(InputState); }
        }

        public float Thrust { get { return State.Thrust; } }
        public float Rotate { get { return State.Rotate; } }
        public bool FireMainWeapon { get { return State.FireMainWeapon; } }
        public bool UseSpecialPower
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.UseSpecialPower && _state.Value.UseSpecialPower;
            }
        }
        public bool Confirm
        {
            get { return _lastState != null && !_lastState.Value.Confirm && _state.Value.Confirm; }
        }
        public bool Cancel
        {
            get { return _lastState != null && !_lastState.Value.Cancel && _state.Value.Cancel; }
        }
        public bool Up
        {
            get { return _lastState != null && !_lastState.Value.Up && _state.Value.Up; }
        }
        public bool Down
        {
            get { return _lastState != null && !_lastState.Value.Down && _state.Value.Down; }
        }
        public bool Left
        {
            get { return _lastState != null && !_lastState.Value.Left && _state.Value.Left; }
        }
        public bool Right
        {
            get { return _lastState != null && !_lastState.Value.Right && _state.Value.Right; }
        }
        #endregion

        #region Constructors
        public PlayerInput(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }
        #endregion

        #region Game Loop
        public void Update()
        {
            _lastState = _state;
            _state = GetState();
        }
        #endregion

        #region Get State
        InputState GetState()
        {
            var gamePad1 = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes);
            var gamePad2 = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.IndependentAxes);
            var kbState = Keyboard.GetState();

            if (PlayerIndex == PlayerIndex.One)
            {
                if (gamePad1.IsConnected)
                    return GetState(gamePad1);
                return GetState(kbState, PlayerIndex.One);
            }

            if (gamePad2.IsConnected)
                return GetState(gamePad2);

            return GetState(kbState, PlayerIndex.Two);
        }

        InputState GetState(GamePadState state)
        {
            const float analogToDigital = 0.4f;

            var rotDir = state.ThumbSticks.Left.X > 0 ? 1 : -1;

            return new InputState
            {
                Rotate = Math.Abs(state.ThumbSticks.Left.X) <= analogToDigital ? 0 : state.ThumbSticks.Left.X / 0.8f - 0.25f * rotDir,
                Thrust = state.Triggers.Right - state.Triggers.Left,
                UseSpecialPower = state.Buttons.X == ButtonState.Pressed,
                FireMainWeapon = state.Buttons.B == ButtonState.Pressed,

                Confirm = state.IsButtonDown(Buttons.Start),
                Cancel = state.IsButtonDown(Buttons.Back),
                Up = state.IsButtonDown(Buttons.DPadUp) || state.ThumbSticks.Left.Y > analogToDigital,
                Down = state.IsButtonDown(Buttons.DPadDown) || state.ThumbSticks.Left.Y < -analogToDigital,
                Left = state.IsButtonDown(Buttons.DPadLeft) || state.ThumbSticks.Left.X < -analogToDigital,
                Right = state.IsButtonDown(Buttons.DPadRight) || state.ThumbSticks.Left.X > analogToDigital,
            };
        }

        InputState GetState(KeyboardState state, PlayerIndex index)
        {
            if (index == Microsoft.Xna.Framework.PlayerIndex.One)
            {
                return new InputState
                {
                    Rotate = (state.IsKeyDown(Keys.Left) ? -1 : 0) +
                                (state.IsKeyDown(Keys.Right) ? 1 : 0),
                    Thrust = (state.IsKeyDown(Keys.Up) ? 1 : 0) +
                                (state.IsKeyDown(Keys.Down) ? -1 : 0),
                    FireMainWeapon = state.IsKeyDown(Keys.RightShift),
                    UseSpecialPower = state.IsKeyDown(Keys.Enter),

                    Confirm = state.IsKeyDown(Keys.Enter),
                    Cancel = state.IsKeyDown(Keys.RightShift),
                    Up = state.IsKeyDown(Keys.Up),
                    Down = state.IsKeyDown(Keys.Down),
                    Left = state.IsKeyDown(Keys.Left),
                    Right = state.IsKeyDown(Keys.Right),
                };
            }

            return new InputState
            {
                Rotate = (state.IsKeyDown(Keys.A) ? -1 : 0) +
                         (state.IsKeyDown(Keys.D) ? 1 : 0),
                Thrust = (state.IsKeyDown(Keys.W) ? 1 : 0) +
                         (state.IsKeyDown(Keys.S) ? -1 : 0),
                FireMainWeapon = state.IsKeyDown(Keys.E),
                UseSpecialPower = state.IsKeyDown(Keys.R),

                Confirm = state.IsKeyDown(Keys.R),
                Cancel = state.IsKeyDown(Keys.E),
                Up = state.IsKeyDown(Keys.W),
                Down = state.IsKeyDown(Keys.S),
                Left = state.IsKeyDown(Keys.A),
                Right = state.IsKeyDown(Keys.D),
            };
        }
        #endregion
    }
}
