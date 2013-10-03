using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameLib.Core.Extensions;

namespace MonoGameLib.Core.Particles
{
    #region Delegates
    public delegate void OnDecayedDelegate();
    #endregion Delegates

    public class ParticleEmiter
    {
        #region Attributes
        private string _particleTexture;
        private List<Particle> _particles;
        private float _sinceLastEmision;
        private Random _rng;
        private bool _decaying;
        #endregion

        #region Properties
        public float MillisecondsToEmit { get; set; }
        public Vector2 Position { get; set; }

        public Vector2 Direction { get; set; }
        public float ParticleSpeed { get; set; }
        public float OpeningAngle { get; set; }
        public float ParticleMaxTime { get; set; }
        public List<ParticleState> ParticleStates { get; protected set; }
        public bool Enabled { get; set; }
        public float DecayTime { get; set; }
        #endregion Properties

        #region Delegates
        public OnDecayedDelegate OnDecay { get; set; }
        #endregion Delegates

        #region Constructor
        public ParticleEmiter(string texture, List<ParticleState> particleStates)
        {
            _particleTexture = texture;
            _rng = new Random();
            ParticleStates = particleStates;
            Enabled = true;
            _decaying = false;

            _particles = new List<Particle>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates particle emiter.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public void Update(GameTime gameTime)
        {
            if (DecayTime > 0f)
            {
                DecayTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (DecayTime <= 0f)
                {
                    Enabled = false;
                    _decaying = true;
                }
            }

            if (Enabled)
            {
                _sinceLastEmision += gameTime.ElapsedGameTime.Milliseconds;

                while (_sinceLastEmision >= MillisecondsToEmit)
                {
                    float angle = (float)(_rng.NextDouble() * (2 * OpeningAngle)) - OpeningAngle;
                    _sinceLastEmision -= MillisecondsToEmit;
                    _particles.Add(new Particle(_particleTexture, Position, Direction.Rotate(angle), ParticleStates) { Speed = ParticleSpeed });
                }
            }

            foreach (Particle p in _particles)
            {
                p.Update(gameTime);
            }

            if (ParticleMaxTime > 0f)
            {
                List<Particle> toRemove = _particles.Where((p) => p.TimeAlive > ParticleMaxTime).ToList(); ;

                foreach (Particle p in toRemove)
                {
                    _particles.Remove(p);
                }
            }

            if (_decaying && _particles.Count == 0 && OnDecay != null)
            {
                OnDecay();
            }
        }

        /// <summary>
        /// Draw the particles.
        /// </summary>
        /// <param name="gameTime">Current Game Time.</param>
        /// <param name="spriteBatch">Sprite Batch.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Particle p in _particles)
            {
                p.Draw(gameTime, spriteBatch);
            }
        }
        #endregion Methods
    }
}
