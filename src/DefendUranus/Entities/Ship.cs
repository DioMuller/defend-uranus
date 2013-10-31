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
        #region Nested
        public delegate void SpecialAttackMethod(Ship owner);
        #endregion

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
        public SpecialAttackMethod SpecialAttack { get; set; }
        #endregion

        #region Constructors
        public Ship(GamePlay level, string texturePath)
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

        public void DeployAttack(SpecialAttack attack)
        {
            var direction = Vector2Extension.AngleToVector2(Rotation);

            attack.Position = Position + direction * Size / 2;

            attack.ApplyForce(direction * attack.MaxSpeed, instantaneous: true);
            ApplyForce(-direction * attack.Mass, instantaneous: true);

            Level.AddEntity(attack);
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
        async Task FireWeapon(AutoRegenContainer container, Func<CancellationToken, Task<bool>> fire, CancellationToken cancellation)
        {
            if (container.IsEmpty)
                return;

            container.Regenerate = false;

            while (!cancellation.IsCancellationRequested)
                if (!await fire(cancellation))
                    break;

            container.Regenerate = true;
        }

        public async Task<bool> FireLaser(CancellationToken cancellation)
        {
            if (_mainWeaponAmmo.IsEmpty)
                return false;

            var direction = Vector2Extension.AngleToVector2(Rotation);

            var laser = new Laser(this, Momentum, Position, direction);

            laser.ApplyAcceleration(direction * laser.MaxSpeed, instantaneous: true);
            ApplyForce(-direction * laser.Mass, instantaneous: true);

            _mainWeaponAmmo.Quantity--;
            Level.AddEntity(laser);

            await Level.Delay(MainWeaponDelay);
            return true;
        }

        public async Task<bool> FireSpecialWeapon(CancellationToken cancellation)
        {
            if (SpecialAttack == null || _specialWeaponAmmo.IsEmpty)
                return false;

            _specialWeaponAmmo.Quantity--;
            SpecialAttack(this);
            await Level.Delay(SpecialWeaponDelay);
            return true;
        }
        #endregion
    }
}
