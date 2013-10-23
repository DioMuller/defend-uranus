using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameLib.Core.Sprites;
using DefendUranus.Activities;

namespace DefendUranus.Entities
{
    class SpecialAttack : SteeringEntity
    {
        public SpecialAttack(GamePlay level, string texturePath)
            : base(level)
        {
            Sprite = new Sprite(texturePath, new Point(16, 16), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);

            MaxRotationSpeed = 4;
            MaxSpeed = 12;
        }
    }
}
