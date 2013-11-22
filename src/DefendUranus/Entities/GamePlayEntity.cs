using DefendUranus.Activities;
using DefendUranus.Events;
using Microsoft.Xna.Framework;
using MonoGameLib.Activities;
using MonoGameLib.Core.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Entities
{
    /// <summary>
    /// Represents an entity used during gameplay.
    /// </summary>
    class GamePlayEntity : PhysicsEntity
    {
        #region Events
        public event EntityCollisionEventHandler Collided;
        public SyncContext UpdateContext;
        #endregion

        #region Attributes
        /// <summary>
        /// Entity size for fast access.
        /// </summary>
        public new Vector2 Size;

        /// <summary>
        /// Momentum relative to the world.
        /// </summary>
        public Vector2 AbsoluteMomentum;
        #endregion

        #region Properties
        /// <summary>
        /// Tells if this entity can interact with other entities.
        /// </summary>
        public bool InteractWithEntities { get; set; }

        /// <summary>
        /// The level in which this entity is in.
        /// </summary>
        public GamePlay Level { get; set; }

        /// <summary>
        /// How much damage this entity can endure before getting destroyed.
        /// </summary>
        public Container Health { get; protected set; }

        public new Sprite Sprite
        {
            get { return base.Sprite; }
            set
            {
                base.Sprite = value;
                Size = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y);
            }
        }

        /// <summary>
        /// Explosion Color.
        /// </summary>
        public Color ExplosionColor { get; set; }
        #endregion

        #region Constructors
        public GamePlayEntity(GamePlay level) : this(level, null) { }
        public GamePlayEntity(GamePlay level, string texturePath)
            : base(texturePath)
        {
            InteractWithEntities = true;
            UpdateContext = new SyncContext(Update);

            Level = level;
            if(texturePath != null)
                Size = base.Size;
        }
        #endregion

        #region Game Loop
        public override void Update(GameTime gameTime)
        {
            if (IsDestroyed())
                return;
            base.Update(gameTime);
        }

        protected bool IsDestroyed()
        {
            if (Health != null && Health.IsEmpty)
            {
                Level.RemoveEntity(this);
                return true;
            }

            return false;
        }
        #endregion

        #region Public
        public float AbsoluteSpeed()
        {
            return (Momentum + AbsoluteMomentum).Length();
        }

        public virtual void Destroy()
        {
            Level.RemoveEntity(this);
            Level.AddEntity(new Explosion(this.Position, Level, 1, 1000f, Color));
        }
        #endregion

        #region Internal
        internal void RaiseCollision(GamePlayEntity ent, GameTime gameTime, GamePlay level)
        {
            if(Collided != null)
                Collided(this, new EntityCollisionEventArgs(this, ent, gameTime, level));
        }
        #endregion
    }
}
