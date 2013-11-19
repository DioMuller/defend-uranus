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
        private int _growth;
        private float _size;
        private float _scale;
        private Color _color;
        private float _lifetime;
        private float _remainingTime;
        private GamePlay _level;
        private float _transparency;
        private List<PhysicsEntity> _alreadyHit; 

        public Explosion(Vector2 position, GamePlay level, int growth, float lifetime, Color color) : base()
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
            _alreadyHit = new List<PhysicsEntity>();

            #region Sprites
            Sprite = new Sprite("Sprites/Explosion", default(Point), 0);
            Sprite.Animations.Add(new Animation("default", 0, 0, 0));
            Sprite.Origin = new Vector2(Sprite.FrameSize.X, Sprite.FrameSize.Y) / 2;
            Sprite.ChangeAnimation(0);
            #endregion Sprites
        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(Position.X - _growth, Position.Y - _growth);
            _size += (_growth * 2);
            _scale = (float) _size / 32f;

            #region Lifetime control
            if (_lifetime > 0f )
            {
                _remainingTime -= gameTime.ElapsedGameTime.Milliseconds;

                _transparency = _remainingTime/_lifetime;

                if (_remainingTime < 0f)
                {
                    _level.RemoveEntity(this);
                }
            }
            #endregion Lifetime control

            #region Damage
            IEnumerable<PhysicsEntity> collided =
                _level.Entities.Where((e => e.RotatedCollisionArea.Intersects(this.RotatedCollisionArea)));

            foreach (PhysicsEntity entity in collided)
            {
                if (!_alreadyHit.Contains(entity))
                {
                    if (entity is Ship)
                    {
                        Ship ship = entity as Ship;
                        ship.Health.Quantity -= Convert.ToInt32(10f*_transparency);
                    }
                    else
                    {
                        if (entity is GamePlayEntity)
                        {
                            (entity as GamePlayEntity).Destroy();
                        }
                        else
                        {
                            _level.RemoveEntity(entity);
                        }
                    }

                    _alreadyHit.Add(entity);
                }
            }
            #endregion Damage

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color? colorOverride = null, Vector2? scale = null)
        {
            base.Draw(gameTime, spriteBatch, _color * _transparency, new Vector2(_scale, _scale));
        }
    }
}
