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
    class ShipInputBehavior : Behavior
    {
        #region Properties
        /// <summary>
        /// The entity that this behavior is attached to.
        /// </summary>
        public Ship Ship { get { return (Ship)base.Entity; } }

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

            Ship.Rotate(Input.Rotate);
            Ship.Accelerate(Input.Thrust);

            if(Input.FireMainWeapon)
                Ship.Fire(gameTime);

            // TODO: Ship.Fire (which shoots and applies a small negative force when firing).
        }
        #endregion
    }
}

