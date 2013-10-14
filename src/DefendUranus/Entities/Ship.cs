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
    public class Ship : PhysicsEntity
    {
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

        public Ship(string texturePath) : base()
        {
            Sprite = new Sprite(texturePath, new Point(32, 32), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);

            RotationFriction = 0.1f;
            RotationForce = 10;
            MaxRotationSpeed = 3;
            MaxSpeed = 10;
            ThrotleForce = 1;
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
            Forces.Push(Vector2Extension.AngleToVector2(Rotation) * thrust * ThrotleForce);
        }
    }
}
