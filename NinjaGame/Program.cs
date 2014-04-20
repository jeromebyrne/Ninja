using System;
using System.Threading;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Microsoft.Xna.Framework.Input;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary>
    /// Main program class. High level bootstrap code for the game. 
    /// </summary>
    // 
    //#############################################################################################
    
    static class Program
    {
        //=========================================================================================
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"> Command line arguments to program </param>
        //=========================================================================================        

        static void Main( string[] args )
        {
            // Parse program arguments

            ParseArguments(args);

            // If windows debug create the console

            #if WINDOWS_DEBUG

                DebugConsole.Create();
                DebugConsole.SetSize(1024,200);

            #endif

            // Build a list of localised content files in the game:

            Locale.BuildLocalisedFileList();

            // Initialize all XML object types:

            XmlFactory.Initialize();

            // Initialise all GUI event code:

            GuiEvents.Initialize();

            // Load all strings:

            StringDatabase.LoadContentFolder("Content\\Strings");

            // Make a new ninja game amd run it:

            Core game = new Core(); game.Run();

            // Save all user preferences:

            CorePreferences.SavePreferences();

            // Save all high scores if we have a storage device:

            if ( StorageSystem.Container != null )
            {
                // Save high scores:

                LevelHighScores.Save( StorageSystem.Container.Path + "\\" + Core.HIGH_SCORES_FOLDER );
            }

            // Be sure to stop all joypad vibration:

            GamePad.SetVibration(Microsoft.Xna.Framework.PlayerIndex.One,0,0);

            // If windows debug shut down the console

            #if WINDOWS_DEBUG

                DebugConsole.Shutdown();

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Parses program arguments.
        /// </summary>
        /// <param name="args"> Program argumetns </param>
        //=========================================================================================

        static void ParseArguments( string[] args )
        {
            // Run through all the arguments:

            for ( int i = 0 ; i < args.Length ; i++ )
            {
                // Make lowercase copy of this arg:

                string a = args[i].ToLower();

                // See what it is:

                    // Allow culture switch in debug mode:

                    #if WINDOWS_DEBUG

                        if ( a.Equals( "-culture" , StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            // See if there is a next argument:

                            if ( i < args.Length - 1 )
                            {
                                // This is the format culture should take:

                                Regex r = new Regex("\\w\\w-\\w\\w");

                                // See if the culture string is ok:

                                Match m = r.Match(args[i+1]);

                                // See if match:

                                if ( m.Success )
                                {
                                    // Set culture:

                                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(args[i+1],false);

                                    // Move on:

                                    i++; continue;
                                }
                            }
                        }

                    #endif
            }

        }

    }   // end of class 

}   // end of namespace

