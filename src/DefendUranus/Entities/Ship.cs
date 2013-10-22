using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Extensions;
using MonoGameLib.Core.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities
{
    class Ship : PhysicsEntity
    {
        #region Constants
        TimeSpan MainWeaponDelay = TimeSpan.FromMilliseconds(100);
        #endregion

        #region Attributes
        GamePlay _gamePlay;
        TimeSpan _lastShot;
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
        #endregion

        public Ship(GamePlay gamePlay, string texturePath)
            : base(texturePath)
        {
            _gamePlay = gamePlay;

            RotationFriction = 0.1f;
            RotationForce = 10;
            MaxRotationSpeed = 3;
            MaxSpeed = 10;
            ThrotleForce = 20;
            Restitution = 0.5f;
        }

        public void Rotate(float force)
        {
            if (Math.Abs(force) > 0.1f)
                RotationFriction = 0;
            else
                RotationFriction = RotationStabilizer;

            ApplyRotation(force * RotationForce);
        }

        public void Fire(GameTime gameTime)
        {
            if (gameTime.TotalGameTime < _lastShot + MainWeaponDelay)
                return;

            _lastShot = gameTime.TotalGameTime;

            var direction = Vector2Extension.AngleToVector2(Rotation);

            var laser = new PhysicsEntity("Sprites/Laser")
            {
                Position = Position + direction * 12,
                Mass = 1,
                MaxSpeed = 100,
                RotateToMomentum = true
            };
            laser.ApplyForce(direction * laser.MaxSpeed, instantaneous: true);
            _gamePlay.AddEntity(laser);
        }

        public void Accelerate(float thrust)
        {
            ApplyForce(Vector2Extension.AngleToVector2(Rotation) * thrust * ThrotleForce);
        }
    }
}
