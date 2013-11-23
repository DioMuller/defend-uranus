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
        protected Color _particleColor;
        private float _vulnerabilityTime;

        public Ship Owner;
        #endregion Attributes

        #region Constructor
        public SpecialAttack(Ship owner, string texturePath, Color particleColor, float lifetime)
            : base(owner.Level)
        {
            Owner = owner;
            Sprite = new Sprite(texturePath, new Point(16, 16), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);

            MaxRotationSpeed = 4;
            MaxSpeed = 12;

            _vulnerabilityTime = 300f;

            #region Particle
            _particleColor = particleColor;

            List<ParticleState> particleStates = new List<ParticleState> {
                new ParticleState { Color = _particleColor, Scale = 1f, Duration = 500f },
                new ParticleState { Color = new Color(_particleColor * 0.2f, 0), Scale = 3f }
            };

            _particleEmiter = new ParticleEmiter(Level, "Particles/spark.png", particleStates) { MillisecondsToEmit = 8f, OpeningAngle = 20f, ParticleSpeed = 1f };
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

        #endregion Game Cycle
    }
}
