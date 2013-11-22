using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Sprites;

namespace DefendUranus.Entities
{
    class Explosion : Entity
    {
        #region Attributes

        private int _growth;
        private float _size;
        private float _scale;
        private Color _color;
        private float _lifetime;
        private float _remainingTime;
        private GamePlay _level;
        private float _transparency;
        private List<GamePlayEntity> _alreadyHit;

        #endregion Attributes

        #region Constructor

        public Explosion(Vector2 position, GamePlay level, int growth, float lifetime, Color color)
            : base()
        {
            Position = position;

            _growth = growth;
            _size = 32;
            _scale = 1f;
            _color = color;
            _lifetime = lifetime;
            _remainingTime = lifetime;
            _level = level;
            _transparency = 1f;
            _alreadyHit = new List<GamePlayEntity>();

            #region Sprites
            Sprite = new Sprite("Sprites/Explosion", default(Point), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);
            #endregion Sprites
        }

        #endregion Constructor

        #region Game Loop

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (ExplosionFinished(gameTime))
                return;

            UpdateSize();

            DamageNearbyEntities();

            base.Update(gameTime);
        }

        /// <summary>
        /// Update the explosion life-time.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <returns>True if the explosion finished.</returns>
        bool ExplosionFinished(GameTime gameTime)
        {
            if (_lifetime > 0f)
            {
                _remainingTime -= gameTime.ElapsedGameTime.Milliseconds;

                _transparency = _remainingTime / _lifetime;

                if (_remainingTime < 0f)
                {
                    _level.RemoveEntity(this);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Increases the explosion size with time.
        /// </summary>
        void UpdateSize()
        {
            Position = new Vector2(Position.X - _growth, Position.Y - _growth);
            _size += (_growth * 2);
            _scale = (float)_size / 32f;
        }

        /// <summary>
        /// Damage all entities touching the explosion.
        /// </summary>
        void DamageNearbyEntities()
        {
            var collided = _level.Entities.Where(InCollisionArea)
                                          .Distinct()
                                          .Except(_alreadyHit)
                                          .ToList();

            foreach (var entity in collided)
            {
                if (entity.Health != null)
                {
                    entity.Health.Quantity -= Convert.ToInt32(10f * _transparency);
                }
                else
                {
                    entity.Destroy();
                }
            }

            _alreadyHit.AddRange(collided);
        }

        #endregion Update

        #region Draw

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color? colorOverride = null, Vector2? scale = null)
        {
            base.Draw(gameTime, spriteBatch, _color * _transparency, new Vector2(_scale, _scale));
        }

        #endregion Draw

        #endregion Game Loop

        #region Public

        /// <summary>
        /// Checks if a specified entity is inside the explosion range.
        /// </summary>
        /// <param name="entity">Entity to be tested.</param>
        /// <returns>True if the entity is in the explosion.</returns>
        public bool InCollisionArea(GamePlayEntity entity)
        {
            if (!entity.InteractWithEntities)
                return false;

            return entity.RotatedCollisionArea.Intersects(this.RotatedCollisionArea);
        }

        #endregion Public
    }
}
