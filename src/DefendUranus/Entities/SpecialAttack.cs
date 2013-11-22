using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using DefendUranus.Events;
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
        #region Attributes
        private ParticleEmiter _particleEmiter;
        protected Ship _owner;
        protected Color _particleColor;
        private float _vulnerabilityTime;
        #endregion Attributes

        #region Constructor
        public SpecialAttack(GamePlay level, string texturePath, Color particleColor, Ship owner, float lifetime)
            : base(level)
        {
            Sprite = new Sprite(texturePath, new Point(16, 16), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);

            MaxRotationSpeed = 4;
            MaxSpeed = 12;

            _owner = owner;

            _vulnerabilityTime = 300f;

            #region Particle
            _particleColor = particleColor;

            List<ParticleState> particleStates = new List<ParticleState>();
            particleStates.Add(new ParticleState() { StartTime = 0f, Color = _particleColor, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 200f, Color = _particleColor * 0.8f, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 300f, Color = _particleColor * 0.6f, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 400f, Color = _particleColor * 0.3f, Scale = 1f });
            particleStates.Add(new ParticleState() { StartTime = 500f, Color = _particleColor * 0.2f, Scale = 1f });

            _particleEmiter = new ParticleEmiter("particles/spark.png", particleStates) { ParticleMaxTime = 500f, MillisecondsToEmit = 8f, OpeningAngle = 20f, ParticleSpeed = 1f };
            #endregion Particle

            Collided += OnCollided;
            AutoDestroy(TimeSpan.FromMilliseconds(lifetime));
        }
        #endregion Constructor

        #region Events
        private void OnCollided(object sender, EntityCollisionEventArgs e)
        {
            //TODO: Why the special attack hits the parent first, but only sometimes?
            if (_vulnerabilityTime < 0f)
            {                
                Destroy();
            }
        }
        #endregion Events

        #region Game Cycle

        public override void Update(GameTime gameTime)
        {
            #region Particles
            _particleEmiter.Position = this.Position;
            _particleEmiter.Direction = new Vector2(0, 1).RotateRadians(Rotation);

            _particleEmiter.Update(gameTime);
            #endregion Particles

            if (_vulnerabilityTime > 0f)
                _vulnerabilityTime -= gameTime.ElapsedGameTime.Milliseconds;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color? colorOverride = null, Vector2? scale = null)
        {
            _particleEmiter.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch, colorOverride);
        }

        #endregion Game Cycle
    }
}
