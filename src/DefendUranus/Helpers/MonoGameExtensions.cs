﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefendUranus.Helpers
{
    public static class MonoGameExtensions
    {
        public static void Begin(this SpriteBatch spriteBatch, Vector2 camera = default(Vector2), float zoom = 1, Viewport gameViewport = default(Viewport), SamplerState sampler = null)
        {
            var transformation = Matrix.CreateTranslation(new Vector3(-camera.X, -camera.Y, 0)) *
                //Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(gameViewport.Width * 0.5f, gameViewport.Height * 0.5f, 0));

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        transformation);
        }

        public static Vector2 Center(this Viewport viewPort)
        {
            return new Vector2(viewPort.Width / 2, viewPort.Height / 2);
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
    }
}