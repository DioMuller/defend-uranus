using System;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DefendUranus.Entities;
using MonoGameLib.Core.Extensions;
using DefendUranus.Helpers;

namespace DefendUranus
{
    public class ShipInputBehavior : Behavior
    {
        #region Properties
        /// <summary>
        /// The entity that this behavior is attached to.
        /// </summary>
        new public Ship Entity { get { return (Ship)base.Entity; } }

        /// <summary>
        /// The Input controlling this behavior.
        /// </summary>
        public PlayerInput Input { get; private set; }
        #endregion

        #region Constructors
        public ShipInputBehavior(PlayerIndex playerIndex, Ship parent)
            : base(parent)
        {
            Input = new PlayerInput(playerIndex);
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            Input.Update();

            Entity.Forces.Push(Vector2Extension.AngleToVector2(Entity.Rotation) * Input.Thrust * Entity.ThrotleForce);
            Entity.ApplyRotation(Input.Rotate * Entity.RotationForce);

            // TODO: Apply a small negative force when firing.
        }
        #endregion
    }
}

