﻿#region Using Statements
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
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
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
            var startScreen = new StartScreen(this);
            var setupScreen = new GamePlaySetup(this);

            while (true)
            {
                switch (await startScreen.Run())
                {
                    case StartScreen.Options.Exit:
                        return;
                    case StartScreen.Options.HowToPlay:
                        await new HowToPlay(this).Run();
                        continue;
                    case StartScreen.Options.Play:
                        break;
                    default:
                        throw new NotImplementedException();
                }

                bool playAgain;
                do
                {
                    var setup = await setupScreen.Run();
                    if (setup.Aborted)
                        break;

                    var gamePlay = new GamePlay(this, setup);
                    var gameResult = await gamePlay.Run();

                    playAgain = gameResult.Aborted;

                    if(!gameResult.Aborted)
                        await new ShowResults(this, gameResult).Run();

                } while(playAgain);
            }
        }
    }
}