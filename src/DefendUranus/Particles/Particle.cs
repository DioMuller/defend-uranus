using DefendUranus.Activities;
using DefendUranus.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGameLib.Core.Particles
{
    class Particle : GamePlayEntity
    {
        #region Events
        public event EventHandler OnDecay;
        #endregion

        #region Properties
        public float Opacity { get; set; }
        #endregion Properties

        #region Constructor
        public Particle(GamePlay level, string texture, Vector2 position, List<ParticleState> states)
            : base(level, texture)
        {
            InteractWithEntities = false;
            Position = position;
            Opacity = 1;
            using (Level.DrawContext.Activate())
                Animate(states);
        }
        #endregion Constructor

        #region Particle Methods
        public virtual void CalculatePosition()
        {
            Position += (Direction * Speed);
        }
        #endregion Particle Methos

        public override void Destroy()
        {
            Level.RemoveEntity(this);
        }

        private async void Animate(List<ParticleState> states)
        {
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                var nextState = states.Count > i + 1 ? states[i + 1] : null;

                Color = state.Color * Opacity;
                Scale = new Vector2(state.Scale);

                if (state.Duration <= 0)
                    continue;

                if (nextState == null)
                    await Level.DrawContext.Delay(TimeSpan.FromMilliseconds(state.Duration));
                else
                {
                    await TaskEx.WhenAll(
                        Level.FloatAnimation((int)state.Duration, state.Scale, nextState.Scale, v => Scale = new Vector2(v)),
                        Level.FloatAnimation((int)state.Duration, 0, 1, p =>
                        {
                            Color = Color.FromNonPremultiplied(
                                r: (int)MathHelper.Lerp(state.Color.R, nextState.Color.R, p),
                                g: (int)MathHelper.Lerp(state.Color.G, nextState.Color.G, p),
                                b: (int)MathHelper.Lerp(state.Color.B, nextState.Color.B, p),
                                a: (int)(MathHelper.Lerp(state.Color.A, nextState.Color.A, p) * Opacity)
                            );
                        }));
                }
            }
            Level.RemoveEntity(this);
            if (OnDecay != null)
                OnDecay(this, EventArgs.Empty);
        }
    }
}
