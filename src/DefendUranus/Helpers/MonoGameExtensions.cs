﻿using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
