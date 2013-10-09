using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Helpers
{
    class GameInput
    {
        #region Nested
        public struct InputState
        {
            /// <summary>
            /// The user have confirmed a menu option.
            /// </summary>
            public bool Confirm;
            /// <summary>
            /// The user is calling help.
            /// </summary>
            public bool Help;
            /// <summary>
            /// The user is changing the pause state.
            /// </summary>
            public bool TogglePause;
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
        /// Gets the current input state.
        /// This value is updated after calling the Update method.
        /// </summary>
        public InputState State
        {
            get { return _state ?? default(InputState); }
        }

        public bool Help
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.Help && _state.Value.Help;
            }
        }
        public bool Confirm
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.Confirm && _state.Value.Confirm;
            }
        }
        public bool TogglePause
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.TogglePause && _state.Value.TogglePause;
            }
        }
        public bool Cancel
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.Cancel && _state.Value.Cancel;
            }
        }
        public bool Up
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.Up && _state.Value.Up;
            }
        }
        public bool Down
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.Down && _state.Value.Down;
            }
        }
        public bool Left
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.Left && _state.Value.Left;
            }
        }
        public bool Right
        {
            get
            {
                return _lastState != null &&
                    !_lastState.Value.Right && _state.Value.Right;
            }
        }
        #endregion

        #region Constructors
        public GameInput()
        {
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
            var gamePad1 = GamePad.GetState(PlayerIndex.One);
            var gamePad2 = GamePad.GetState(PlayerIndex.Two);
            var kbState = Keyboard.GetState();

            if (gamePad1.IsConnected && gamePad2.IsConnected)
                return GetState(gamePad1);
            return GetState(kbState);
        }

        InputState GetState(GamePadState state)
        {
            return new InputState
            {
                Confirm = state.IsButtonDown(Buttons.A),
                TogglePause = state.IsButtonDown(Buttons.Start),
                Cancel = state.IsButtonDown(Buttons.B),
                Help = state.IsButtonDown(Buttons.BigButton),
                Up = state.IsButtonDown(Buttons.DPadUp) || state.ThumbSticks.Left.Y > 0.4,
                Down = state.IsButtonDown(Buttons.DPadDown) || state.ThumbSticks.Left.Y < -0.4,
                Left = state.IsButtonDown(Buttons.DPadLeft) || state.ThumbSticks.Left.X < -0.4,
                Right = state.IsButtonDown(Buttons.DPadRight) || state.ThumbSticks.Left.X > 0.4,
            };
        }

        InputState GetState(KeyboardState state)
        {
            return new InputState
            {
                Confirm = state.IsKeyDown(Keys.Enter),
                TogglePause = state.IsKeyDown(Keys.Pause),
                Cancel = state.IsKeyDown(Keys.Escape),
                Help = state.IsKeyDown(Keys.F1),
                Up = state.IsKeyDown(Keys.Up),
                Down = state.IsKeyDown(Keys.Down),
                Left = state.IsKeyDown(Keys.Left),
                Right = state.IsKeyDown(Keys.Right),
            };
        }
        #endregion
    }
}
