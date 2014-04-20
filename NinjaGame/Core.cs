using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary>
    /// Core game class. Contains very high level game logic and rendering code.
    /// </summary>
    // 
    //#############################################################################################

    public class Core : Microsoft.Xna.Framework.Game
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> The main game object. </summary>

            public static Game Game { get { return s_game; } }

            /// <summary> Currently active level for the game. Holds all of the objects in the level. </summary>

            public static Level Level { get { return s_level; } }

            /// <summary> Currently active gui for the game. Holds all of the widgets in the gui. The game runs under a gui. </summary>

            public static Gui Gui { get { return s_gui; } }

            /// <summary> Graphics system for the game. Contains device needed to draw and functions to load content. </summary>

            public static GraphicsSystem Graphics { get { return s_graphics_system; } }

            /// <summary> Timing system which handles the management of time in the game and consequently how fast the game should run. </summary>

            public static TimingSystem Timing { get { return s_timing_system; } }

            /// <summary> Audio system which handles the playing of game audio. This is intended for sound effects only. </summary>
            
            public static AudioSystem Audio { get { return s_effects_audio_system; } }

            /// <summary> Audio system which handles the playing of game music. This is intended for music only. </summary>
            
            public static AudioSystem Music { get { return s_music_audio_system; } }

            /// <summary> A random number generator for generating random numbers. </summary>

            public static Random Random { get { return s_random; } }

            #if DEBUG

                /// <summary> Font for debugging in windows. </summary>

                public static Font DebugFont { get { return s_debug_font; } } 

                /// <summary> Simple shader for drawing colored primitives. </summary>

                public static Effect DebugShader { get { return s_debug_shader; } }

            #endif

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> The main game object. </summary>

            private static Game s_game = null;

            /// <summary> Currently active level for the game. Holds all of the objects in the level. </summary>

            private static Level s_level = null;

            /// <summary> Currently active gui for the game. Holds all of the widgets in the gui. The game runs under a gui. </summary>

            private static Gui s_gui = null;

            /// <summary> Graphics system for the game. Contains device needed to draw and functions to load content. </summary>

            private static GraphicsSystem s_graphics_system = null;

            /// <summary> Timing system which handles the management of time in the game and consequently how fast the game should run. </summary>

            private static TimingSystem s_timing_system = null;

            /// <summary> Audio system which handles the playing of game audio. This is intended for sound effects only. </summary>

            private static AudioSystem s_effects_audio_system = null;

            /// <summary> Audio system which handles the playing of game music. This is intended for music only. </summary>

            private static AudioSystem s_music_audio_system = null;

            /// <summary> A random number generator for generating random numbers. </summary>

            private static Random s_random = new Random();

            /// <summary> Graphics device manager. Manages display modes and graphics devices. </summary>

            private static GraphicsDeviceManager s_graphics_device_manager = null;

            /// <summary> Previous state of the keyboard. </summary>

            private static KeyboardState s_previous_kb_state;

            #if DEBUG

                /// <summary> Font for debugging in: </summary>

                private static Font s_debug_font = null;

                /// <summary> Simple shader for drawing colored primitives: </summary>

                private static Effect s_debug_shader = null;

            #endif

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Name of the root content folder </summary>

            public const string CONTENT_FOLDER = "Content";

            /// <summary> Folder under user data to store high scores in </summary>

            public const string HIGH_SCORES_FOLDER = "Records";

        //=========================================================================================
        /// <summary>
        /// Game constructor. Constructs the game.
        /// </summary>
        //=========================================================================================

        public Core()
        {
            // Save ourselves as the main game object

            s_game = this;

            // Make the graphics device manager

            s_graphics_device_manager = new GraphicsDeviceManager(this);

            // Set base content directory:

            Content.RootDirectory = CONTENT_FOLDER;

            // Get current keyboard state:

            s_previous_kb_state = Keyboard.GetState();

            // Add a new gamer services component to the game

            Components.Add( new GamerServicesComponent(this) );
        }

        //=========================================================================================
        /// <summary>
        /// Initiliazes the game and loads all it's content.
        /// </summary>
        //=========================================================================================

        protected override void Initialize()
        {
            // Initialise everything

            base.Initialize();

            // Create the graphics system object

            s_graphics_system = new GraphicsSystem( s_graphics_device_manager , Content );

            // Create the audio systems:

            s_effects_audio_system = new AudioSystem
            (
                CONTENT_FOLDER + "\\Sounds\\NinjaGame_Sounds.xgs" ,
                CONTENT_FOLDER + "\\Sounds\\NinjaGame_Sounds.xwb" ,
                CONTENT_FOLDER + "\\Sounds\\NinjaGame_Sounds.xsb" ,
                false
            );

            s_music_audio_system = new AudioSystem
            (
                CONTENT_FOLDER + "\\Sounds\\NinjaGame_Music.xgs"  ,
                CONTENT_FOLDER + "\\Sounds\\NinjaGame_Music.xwb"  ,
                CONTENT_FOLDER + "\\Sounds\\NinjaGame_Music.xsb"  ,
                false
            );

            // Apply all default preferences:

            CorePreferences.ApplyAllSettings();

            // Create the timing system:

            s_timing_system = new TimingSystem();

            // Create the level and gui:

            s_level = new Level(); s_gui = new Gui();

            // On debug load the debug font and debug shader:

            #if DEBUG

                // Load the font

                s_debug_font = new Font("Content\\Fonts\\Game_24px.xml");

                // Load the shader:

                s_debug_shader = s_graphics_system.LoadEffect("Effects\\colored");

                // Windows debug specific:

                #if WINDOWS_DEBUG

                    // Also allow window to be resized:

                    Window.AllowUserResizing = true;

                    // Set to 640x480

                    Graphics.DeviceManager.PreferredBackBufferWidth     = 640;
                    Graphics.DeviceManager.PreferredBackBufferHeight    = 480;

                    Graphics.DeviceManager.ApplyChanges();

                    // Add debug console commands for core game

                    DebugCore.AddDebugCommands();

                #endif

            #endif

            // Load the main menu
            
            s_gui.Load("Content\\Guis\\Boot.xml");
        }

        //=========================================================================================
        /// <summary>
        /// Updates game logic and runs the game. 
        /// </summary>
        /// <param name="time_delta"> Time elapsed since last call to update. </param>
        //========================================================================================= 

        protected override void Update( GameTime time_delta )
        {
            // Call base class update:

            base.Update( time_delta );

            // Storage & user stuff: only do if guide is not visible

            if ( Guide.IsVisible == false )
            {
                // Check for user details:

                User.CheckUserDetails();

                // Update which storage device we are using:

                StorageSystem.ChooseStorageDevice();

                // Load all user preferences if not already done so:

                CorePreferences.LoadPreferences();

                // Get the storage container:

                StorageContainer container = StorageSystem.Container;

                // If we have a storage device and haven't load high scores then read:

                if ( container != null && LevelHighScores.ReadHighScores == false )
                {
                    // Load the scores:

                    LevelHighScores.Load( container.Path + "\\" + HIGH_SCORES_FOLDER );
                }
            }

            // Debug only code:

            #if WINDOWS_DEBUG

                // If profiling is enabled then begin profile:

                if ( DebugCore.ProfileUpdate ) DebugConsole.BeginProfile("Core.Update");

            #endif

            // Grab keyboard state

            KeyboardState kbs = Keyboard.GetState();

            // Update timing:

            s_timing_system.Update(time_delta); 
            
            // Update audio:

            s_effects_audio_system.Update();
            s_music_audio_system.Update();
            
            // Run the game:

                // Only do on windows debug if the xml object editor is not open

                #if WINDOWS_DEBUG

                    // Only do if the guide is not active and if this window is active:

                    if ( this.IsActive && Guide.IsVisible == false )
                    {
                        // Update the current gui if editor is not open:

                        if ( XmlObjectEditor.Open == false ) s_gui.Update();
                    }

                #else

                    s_gui.Update();

                #endif

            // If in windows debug then run pending console commands

            #if WINDOWS_DEBUG

                DebugConsole.RunPendingCommands();

            #endif

            // Close window if F12 pressed:

            if ( kbs.IsKeyDown(Keys.F12) ) Exit();

            // If f1 is pressed then switch into fullscreen:

            if ( kbs.IsKeyDown(Keys.F1) && s_previous_kb_state.IsKeyUp(Keys.F1) ) 
            {
                if ( Graphics.DeviceManager.IsFullScreen )
                {
                    Graphics.DeviceManager.PreferredBackBufferWidth     = 800;
                    Graphics.DeviceManager.PreferredBackBufferHeight    = 600;                    
                }
                else
                {
                    Graphics.DeviceManager.PreferredBackBufferWidth     = GraphicsDevice.DisplayMode.Width;
                    Graphics.DeviceManager.PreferredBackBufferHeight    = GraphicsDevice.DisplayMode.Height;
                }

                Graphics.DeviceManager.ToggleFullScreen(); Graphics.DeviceManager.ApplyChanges();
            }

            // Save previous keyboard state:

            s_previous_kb_state = kbs;

            // Update base class logic: but only if the window is active

            if ( this.IsActive )
            {               
                base.Update(time_delta);
            }

            #if WINDOWS_DEBUG

                // If profiling is enabled then end profile:

                if ( DebugCore.ProfileUpdate ) DebugConsole.EndProfile("Core.Update");

            #endif
        }       

        //=========================================================================================
        /// <summary>
        /// Renders the game.
        /// </summary>
        /// <param name="gameTime"> Time between the last two calls to Update() </param>
        //========================================================================================= 

        protected override void Draw( GameTime gameTime )
        {
            #if WINDOWS_DEBUG

                // If profiling is enabled then begin profile:

                if ( DebugCore.ProfileDraw ) DebugConsole.BeginProfile("Core.Draw");

            #endif

            // Clear the screen

            s_graphics_system.Device.Clear( ClearOptions.Target , Color.Black , 0 , 0 );

            // Draw the current gui:

            s_gui.Draw();

            // Do base class drawing

            base.Draw(gameTime);

            #if WINDOWS_DEBUG

                // If profiling is enabled then end profile:

                if ( DebugCore.ProfileDraw ) DebugConsole.EndProfile("Core.Draw");

            #endif
        }
    }
}
