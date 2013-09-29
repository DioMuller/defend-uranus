#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace DefendUranus
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        static void SetupLibraries()
        {
            #region Include Libraries to path
            string dllDir = @".\lib\" + (Environment.Is64BitProcess ? "x64" : "x86");
            Environment.SetEnvironmentVariable("PATH", dllDir + ";" + Environment.GetEnvironmentVariable("PATH"));
            #endregion
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetupLibraries();

            using (var game = new MainGame())
                game.Run();
        }
    }
#endif
}
