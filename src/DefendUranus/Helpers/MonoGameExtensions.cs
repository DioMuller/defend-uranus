using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.GUI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Helpers
{
    public static class MonoGameExtensions
    {
        internal static void Begin(this SpriteBatch spriteBatch, GamePlay.CameraInfo camera, Viewport gameViewport = default(Viewport), SamplerState sampler = null)
        {
            var transformation = Matrix.CreateTranslation(new Vector3(-camera.Position.X, -camera.Position.Y, 0)) *
                //Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(camera.ZoomFactor, camera.ZoomFactor, 1)) *
                Matrix.CreateTranslation(new Vector3(gameViewport.Width * 0.5f, gameViewport.Height * 0.5f, 0));

            spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.NonPremultiplied,
                        null,
                        null,
                        null,
                        null,
                        transformation);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, Vector2 percentOrigin)
        {
            var measure = font.MeasureString(text);
            spriteBatch.DrawString(font, text, position, color, 0, measure * percentOrigin, 1, SpriteEffects.None, 0);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, HorizontalAlign hAlign, VerticalAlign vAlign = VerticalAlign.Top)
        {
            spriteBatch.DrawString(font, text, position, color, AlignExtensions.ToVector(hAlign, vAlign));
        }

        public static Point CenterPoint(this Viewport viewPort)
        {
            return new Point((int)(viewPort.Width / 2), (int)(viewPort.Height / 2));
        }

        public static Vector2 LimitSize(this Vector2 vector, float maxSize)
        {
            var length = vector.Length();
            if (length > maxSize)
                return vector * (maxSize / length);
            return vector;
        }

        public static Rectangle Scale(this Rectangle rect, float factor)
        {
            int newWidth = (int)(rect.Width * factor);
            int newHeight = (int)(rect.Height * factor);
            return new Rectangle (
                rect.X - (newWidth - rect.Width) / 2,
                rect.Y - (newHeight - rect.Height) / 2,
                newWidth,
                newHeight);
        }

        public static Vector2 Truncate(this Vector2 vector, float maxLength)
        {
            var lengthSquared = vector.LengthSquared();
            if (lengthSquared <= maxLength * maxLength)
                return vector;
            vector.Normalize();
            return  vector * maxLength;
        }
    }
}
