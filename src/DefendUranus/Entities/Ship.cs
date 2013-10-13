using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities
{
    public class Ship : PhysicsEntity
    {
        public string Description { get; set; }
        public float ThrotleForce { get; set; }
        public float RotationForce { get; set; }

        public Ship(string texturePath) : base()
        {
            Sprite = new Sprite(texturePath, new Point(32, 32), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);
            Sprite.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(1)));

            RotationForce = 3;
            MaxRotationSpeed = 3;
            MaxSpeed = 10;
            ThrotleForce = 1;
        }
    }
}
