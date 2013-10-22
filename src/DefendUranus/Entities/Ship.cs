using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Extensions;
using MonoGameLib.Core.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DefendUranus.Entities
{
    class Ship : PhysicsEntity
    {
        #region Constants
        TimeSpan MainWeaponDelay = TimeSpan.FromMilliseconds(100);
        TimeSpan MainWeaponRecoilDelay = TimeSpan.FromMilliseconds(300);
        #endregion

        #region Attributes
        GamePlay _gamePlay;
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

        public async Task FireMainWeapon(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested /*&& tem tiro*/)
            {
                FireLaser();
                // tiro --
                await TaskEx.Delay(MainWeaponDelay);
            }
            await TaskEx.Delay(MainWeaponRecoilDelay);
        }

        public void Accelerate(float thrust)
        {
            ApplyForce(Vector2Extension.AngleToVector2(Rotation) * thrust * ThrotleForce);
        }

        #region Private
        void FireLaser()
        {
            var direction = Vector2Extension.AngleToVector2(Rotation);

            var laser = new PhysicsEntity("Sprites/Laser")
            {
                Position = Position + direction,
                Momentum = Momentum,
                Mass = 1,
                MaxSpeed = 50,
                RotateToMomentum = true
            };
            laser.ApplyForce(direction * laser.MaxSpeed, instantaneous: true);
            ApplyForce(direction * laser.MaxSpeed * -0.001f, instantaneous: true);
            _gamePlay.AddEntity(laser);
        }
        #endregion
    }
}
