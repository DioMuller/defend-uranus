using System;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DefendUranus.Entities;
using MonoGameLib.Core.Extensions;
using DefendUranus.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace DefendUranus
{
    class ShipInputBehavior : Behavior
    {
        #region Attributes
        readonly AsyncOperation _mainWeapon;
        #endregion

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
            _mainWeapon = new AsyncOperation(Ship.FireMainWeapon);
        }
        #endregion

        #region Game Loop
        /// <summary>
        /// Controls the entity associated with this behavior.
        /// This method is invoked for each game loop.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public override void Update(GameTime gameTime)
        {
            Input.Update();

            Ship.Rotate(Input.Rotate);
            Ship.Accelerate(Input.Thrust);

            _mainWeapon.IsActive = Input.FireMainWeapon;
        }
        #endregion
    }
}

