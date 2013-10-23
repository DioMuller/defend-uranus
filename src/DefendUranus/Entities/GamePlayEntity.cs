using DefendUranus.Activities;
using DefendUranus.Events;
using Microsoft.Xna.Framework;
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
        #endregion

        #region Properties
        /// <summary>
        /// The level in which this entity is in.
        /// </summary>
        public GamePlay Level { get; set; }

        /// <summary>
        /// How much damage this entity can endure before getting destroyed.
        /// </summary>
        public Container Health { get; protected set; }
        #endregion

        #region Constructors
        public GamePlayEntity(GamePlay level) : this(level, null) { }
        public GamePlayEntity(GamePlay level, string texturePath)
            : base(texturePath)
        {
            Level = level;
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

        #region Internal
        internal void RaiseCollision(GamePlayEntity ent, GameTime gameTime, GamePlay level)
        {
            if(Collided != null)
                Collided(this, new EntityCollisionEventArgs(this, ent, gameTime, level));
        }
        #endregion
    }
}
