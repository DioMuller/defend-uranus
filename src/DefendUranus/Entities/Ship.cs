﻿using DefendUranus.Activities;
using DefendUranus.Helpers;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DefendUranus.Entities
{
    class Ship : GamePlayEntity
    {
        #region Constants
        readonly TimeSpan MainWeaponDelay = TimeSpan.FromMilliseconds(100);
        readonly TimeSpan MainWeaponRegenTime = TimeSpan.FromSeconds(2);
        const float MainWeaponMaxAmmo = 20;
        #endregion

        #region Attributes
        float _mainWeaponAmmo = MainWeaponMaxAmmo;
        TimeSpan _mainWeaponRegen;
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
        #endregion

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

            MainWeapon = new AsyncOperation(FireMainWeapon);
        }

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

        #region Game Loop
        #region Update
        public override void Update(GameTime gameTime)
        {
            AutoRefillMainWeapon(gameTime);
            base.Update(gameTime);
        }
        #endregion
        #endregion

        #region Private
        #region Main Weapon
        async Task FireMainWeapon(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested && _mainWeaponAmmo >= 1)
            {
                FireLaser();
                _mainWeaponAmmo--;
                await TaskEx.Delay(MainWeaponDelay);
            }
            _mainWeaponRegen = Easing.Quadratic.ReverseIn(_mainWeaponAmmo, 0, MainWeaponMaxAmmo, MainWeaponRegenTime);
        }

        void FireLaser()
        {
            var direction = Vector2Extension.AngleToVector2(Rotation);

            var laser = new Laser(this, Momentum, Position, direction);
            ApplyForce(direction * laser.MaxSpeed * -0.001f, instantaneous: true);
            Level.AddEntity(laser);
        }

        void AutoRefillMainWeapon(GameTime gameTime)
        {
            if (MainWeapon.IsActive || _mainWeaponAmmo >= MainWeaponMaxAmmo)
                return;

            _mainWeaponRegen += gameTime.ElapsedGameTime;
            _mainWeaponAmmo = Easing.Quadratic.In(_mainWeaponRegen, 0, MainWeaponMaxAmmo, MainWeaponRegenTime);
        }
        #endregion
        #endregion
    }
}
