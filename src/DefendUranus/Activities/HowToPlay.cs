﻿#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Input;
using System;
#endregion

namespace DefendUranus.Activities
{
    class HowToPlay : GameActivity<bool>
    {
        #region Attributes
        KeyboardWatcher _keyboard;
        #endregion

        #region Constructors
        public HowToPlay(MainGame game)
            : base(game)
        {
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _keyboard = new KeyboardWatcher();
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            if (_keyboard.IsPressed(Keys.Escape))
                Exit(false);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //SpriteBatch.Begin();
            //SpriteBatch.End();
        }
        #endregion
    }
}
