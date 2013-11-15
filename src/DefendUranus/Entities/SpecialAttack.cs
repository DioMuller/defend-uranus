using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Extensions;
using MonoGameLib.Core.Particles;
using MonoGameLib.Core.Sprites;
using DefendUranus.Activities;

namespace DefendUranus.Entities
{
    class SpecialAttack : SteeringEntity
    {
        private ParticleEmiter _particleEmiter;
        private Color _particleColor;

        public SpecialAttack(GamePlay level, string texturePath, Color particleColor)
            : base(level)
        {
            Sprite = new Sprite(texturePath, new Point(16, 16), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);

            MaxRotationSpeed = 4;
            MaxSpeed = 12;

            #region Particle
            _particleColor = particleColor;

            List<ParticleState> particleStates = new List<ParticleState>();
            particleStates.Add(new ParticleState() { StartTime = 0f, Color = _particleColor, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 200f, Color = _particleColor * 0.8f, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 300f, Color = _particleColor * 0.6f, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 400f, Color = _particleColor * 0.3f, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 500f, Color = _particleColor * 0.2f, Scale = 11f });


            _particleEmiter = new ParticleEmiter("particles/spark.png", particleStates) { ParticleMaxTime = 500f, MillisecondsToEmit = 8f, OpeningAngle = 20f, ParticleSpeed = 1f };
            #endregion Particle
        }

        #region Game Cycle

        public override void Update(GameTime gameTime)
        {
            #region Particles
            _particleEmiter.Position = this.Position;
            _particleEmiter.Direction = new Vector2(0, 1).RotateRadians(Rotation);

            _particleEmiter.Update(gameTime);
            #endregion Particles

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color? colorOverride = null)
        {
            _particleEmiter.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch, colorOverride);
        }

        #endregion Game Cycle
    }
}
