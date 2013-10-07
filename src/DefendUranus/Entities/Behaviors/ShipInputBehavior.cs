using System;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DefendUranus.Entities;
using MonoGameLib.Core.Extensions;

namespace DefendUranus
{
    public class ShipInputBehavior : Behavior
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
        }
        #endregion

        #region Attributes
        InputState? _lastState;
        InputState? _state;
        #endregion

        #region Properties
        /// <summary>
        /// The entity that this behavior is attached to.
        /// </summary>
        new public Ship Entity { get { return (Ship)base.Entity; } }

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
        #endregion

        #region Constructors
        public ShipInputBehavior(PlayerIndex playerIndex, Ship parent)
            : base(parent)
        {
            PlayerIndex = playerIndex;
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            _lastState = _state;
            _state = GetState();

            Entity.Forces.Push(Vector2Extension.AngleToVector2(Entity.Rotation) * Thrust * Entity.ThrotleForce);
            Entity.ApplyRotation(Rotate * Entity.RotationForce);

            // TODO: Apply a small negative force when firing.
        }
        #endregion

        #region Get State
        InputState GetState()
        {
            var gamePad1 = GamePad.GetState(PlayerIndex.One);
            var gamePad2 = GamePad.GetState(PlayerIndex.Two);
            var kbState = Keyboard.GetState(PlayerIndex.One);

            if (PlayerIndex == PlayerIndex.One)
            {
                if (gamePad1.IsConnected && gamePad2.IsConnected)
                    return GetState(gamePad1);
                return GetState(kbState);
            }

            if (gamePad2.IsConnected)
                return GetState(gamePad2);

            if(gamePad1.IsConnected)
                return GetState(gamePad1);

            return default(InputState);
        }

        InputState GetState(GamePadState state)
        {
            return new InputState
            {
                Rotate = state.ThumbSticks.Left.X,
                Thrust = MathHelper.Max(state.Triggers.Left, state.Triggers.Right),
                UseSpecialPower = state.Buttons.X == ButtonState.Pressed,
                FireMainWeapon = state.Buttons.B == ButtonState.Pressed
            };
        }

        InputState GetState(KeyboardState state)
        {
            return new InputState
            {
                Rotate = (state.IsKeyDown(Keys.Left)? -1 : 0) +
                         (state.IsKeyDown(Keys.Right)? 1 : 0),
                Thrust = (state.IsKeyDown(Keys.Up)? 1 : 0) + 
                         (state.IsKeyDown(Keys.Down)? -1 : 0),
                FireMainWeapon = state.IsKeyDown(Keys.Space),
                UseSpecialPower = state.IsKeyDown(Keys.F)
            };
        }
        #endregion
    }
}

