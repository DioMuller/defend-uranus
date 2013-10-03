using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGameLib.Core.Particles
{
    public class Particle
    {
        #region Attributes
        private int _currentState;
        #endregion Attributes

        #region Properties
        public Texture2D Texture { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Vector2 Direction { get; protected set; }
        public List<ParticleState> States { get; protected set; }
        public float Speed { get; set; }
        public float TimeAlive { get; set; }

        public ParticleState CurrentState
        {
            get
            {
                if (_currentState < (States.Count - 1))
                {
                    if (States[_currentState + 1].StartTime < TimeAlive) _currentState++;
                }

                return States[_currentState];
            }
        }
        #endregion Properties

        #region Constructor
        public Particle(string texture, Vector2 position, Vector2 direction, List<ParticleState> states)
        {
            Texture = GameContent.LoadContent<Texture2D>(texture);
            Position = position;
            Direction = direction;
            States = states;
            _currentState = 0;
        }
        #endregion Constructor

        #region Particle Methods
        public virtual void CalculatePosition()
        {
            Position += (Direction * Speed);
        }
        #endregion Particle Methos

        #region Cycle Methods
        public void Update(GameTime gameTime)
        {
            TimeAlive += gameTime.ElapsedGameTime.Milliseconds;
            CalculatePosition();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, CurrentState.Color, 0f, Vector2.Zero, CurrentState.Scale, SpriteEffects.None, 0 );
        }
        #endregion Cycle Methods
    }
}
