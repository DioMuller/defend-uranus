using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Explosion(Vector2 position, int growth, Color color) : base()
        {
            Position = position;

            _growth = growth;
            _size = 32;
            _scale = 1f;
            _color = color;

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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color? colorOverride = null, Vector2? scale = null)
        {
            base.Draw(gameTime, spriteBatch, _color, new Vector2(_scale, _scale));
        }
    }
}
