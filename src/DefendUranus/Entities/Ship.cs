using DefendUranus.Activities;
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DefendUranus.Entities
{
    class Ship : GamePlayEntity
    {
        #region Constants
        /// <summary>
        /// Time between main weapon shots.
        /// </summary>
        readonly TimeSpan MainWeaponDelay = TimeSpan.FromMilliseconds(100);
        /// <summary>
        /// Time between special weapon shots.
        /// </summary>
        readonly TimeSpan SpecialWeaponDelay = TimeSpan.FromSeconds(1);
        #endregion

        #region Attributes
        AutoRegenContainer _mainWeaponAmmo = new AutoRegenContainer(20, TimeSpan.FromSeconds(2));
        AutoRegenContainer _specialWeaponAmmo = new AutoRegenContainer(3, TimeSpan.FromSeconds(60));
        #endregion

        #region Properties
        /// <summary>
        /// How much force is applied to the ship's engine.
        /// </summary>
        public float ThrotleForce { get; set; }

        /// <summary>
        /// How much force is applied to the ship's rotation.
        /// </summary>
        public float RotationForce { get; set; }

        /// <summary>
        /// Effectiveness of the ship's stabilizer.
        /// When set to 1, the ship can stop rotating immediately.
        /// </summary>
        public float RotationStabilizer { get; set; }

        /// <summary>
        /// Controls the MainWeapon usage.
        /// </summary>
        public AsyncOperation MainWeapon { get; set; }

        /// <summary>
        /// Controls the SpecialWeapon usage.
        /// </summary>
        public AsyncOperation SpecialWeapon { get; set; }

        /// <summary>
        /// The method that this ship will use as special attack.
        /// </summary>
        Action<Ship> SpecialAttack { get; set; }
        #endregion

        #region Constructors
        public Ship(GamePlay level, string texturePath, Action<Ship> specialAttack)
            : base(level, texturePath)
        {
            Health = new Container(100);
            RotationFriction = 0.1f;
            RotationForce = 10;
            MaxRotationSpeed = 3;
            MaxSpeed = 10;
            ThrotleForce = 20;
            Restitution = 0.5f;

            MainWeapon = new AsyncOperation(c => FireWeapon(_mainWeaponAmmo, FireLaser, c));
            SpecialWeapon = new AsyncOperation(c => FireWeapon(_specialWeaponAmmo, FireSpecialWeapon, c));

            SpecialAttack = specialAttack;
        }
        #endregion

        #region Actions
        public void Rotate(float force)
        {
            if (Math.Abs(force) > 0.1f)
                RotationFriction = 0;
            else
                RotationFriction = RotationStabilizer;

            ApplyRotation(force * RotationForce);
        }

        public void Accelerate(float thrust)
        {
            ApplyForce(Vector2Extension.AngleToVector2(Rotation) * thrust * ThrotleForce);
        }
        #endregion

        #region Game Loop
        #region Update
        public override void Update(GameTime gameTime)
        {
            _mainWeaponAmmo.Update(gameTime);
            _specialWeaponAmmo.Update(gameTime);
            base.Update(gameTime);
        }
        #endregion
        #endregion

        #region Private
        async Task FireWeapon(AutoRegenContainer container, Func<CancellationToken, Task> fire, CancellationToken cancellation)
        {
            if (container.IsEmpty)
                return;

            container.Regenerate = false;
            if (!container.IsEmpty)
            {
                container.Quantity--;
                await fire(cancellation);
            }
            container.Regenerate = true;
        }

        async Task FireLaser(CancellationToken cancellation)
        {
            var direction = Vector2Extension.AngleToVector2(Rotation);

            var laser = new Laser(this, Momentum, Position, direction);

            laser.ApplyForce(direction * laser.MaxSpeed, instantaneous: true);
            ApplyForce(direction * laser.MaxSpeed * -0.001f, instantaneous: true);

            Level.AddEntity(laser);

            await TaskEx.Delay(MainWeaponDelay);
        }

        async Task FireSpecialWeapon(CancellationToken cancellation)
        {
            SpecialAttack(this);
            await TaskEx.Delay(SpecialWeaponDelay);
        }
        #endregion
    }
}
