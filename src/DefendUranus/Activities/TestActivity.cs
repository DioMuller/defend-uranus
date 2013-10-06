﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefendUranus.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Core.Entities;
using MonoGameLib.Core.Input;

namespace DefendUranus.Activities
{
    class TestActivity : GameActivity<bool>
    {
        #region Attributes
        KeyboardWatcher _keyboard;
        List<Entity> _entities;
        #endregion

        #region Constructors
        public TestActivity(MainGame game)
            : base(game)
        {
        }
        #endregion

        #region Activity Life-Cycle
        protected override void Activating()
        {
            base.Activating();
            _keyboard = new KeyboardWatcher();

            _entities = new List<Entity>();
            //TODO: Create Entities
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
