#region Using Statements
using DefendUranus.Activities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using MonoGameLib.Activities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace DefendUranus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class MainGame : ActivitiesGame
    {
        GraphicsDeviceManager graphics;

        public MainGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 640,
                PreferredBackBufferHeight = 480
            };
            Content.RootDirectory = "Content";

            Window.Title = "Defend Uranus";
        }

        /// <summary>
        /// Controls the game activities sequence, from intro to ending,
        /// including gameplay and settings activity.
        /// </summary>
        protected override async Task Play()
        {
            var res = await new StartScreen(this).Run();
        }
    }
}
