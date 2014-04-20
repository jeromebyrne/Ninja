using System;
using System.Collections.Generic;
using System.Text;
using System.Xml; 
using System.IO; 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Simple class holding user preferences for the game.
    /// </summary>
    //#############################################################################################

    public static class CorePreferences
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Volume of the game sound, from 0-1. </summary>

            public static float SoundVolume { get { return s_sound_volume; } set { s_sound_volume = MathHelper.Clamp(value,0,1); UpdateSoundVolume(); } }

            /// <summary> Volume of the game music, from 0-1. </summary>

            public static float MusicVolume { get { return s_music_volume; } set { s_music_volume = MathHelper.Clamp(value,0,1); UpdateMusicVolume(); } }

            /// <summary> Brightness of the display, from 0.5-1.5 </summary>
        
            public static float Brightness { get { return s_brightness; } set { s_brightness = MathHelper.Clamp(value,0.5f,1.5f); UpdateBrightness(); } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Volume of the game sound, from 0-1. </summary>

            private static float s_sound_volume = 0.75f;

            /// <summary> Volume of the game music, from 0-1. </summary>

            private static float s_music_volume = 0.65f;

            /// <summary> Brightness of the display, from 0.5-1.5 </summary>
        
            private static float s_brightness = 1.0f;

            /// <summary> Did we load user preferences already for the game ? </summary>

            private static bool s_loaded_preferences = false;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> File and folder to store game preferences in. </summary>

            public const string PREFERENCES_FILE = "Preferences.xml";

        //=========================================================================================
        /// <summary>
        /// Attempts to load user preferences from the preferences xml file
        /// </summary>        
        //=========================================================================================

        public static void LoadPreferences()
        {
            // If we already loaded preferences then abort:

            if ( s_loaded_preferences == true ) return;

            // This might fail:

            try
            {
                // Get current storage container

                StorageContainer container = StorageSystem.Container;

                // See if we got a storage container:

                if ( container != null )
                {
                    // Set the loaded preferences flag to true:

                    s_loaded_preferences = true;

                    // Try and get the preferences file:

                    FileInfo file = new FileInfo( container.Path + "\\" + PREFERENCES_FILE );
                    
                    // If it doesn't exist then abort:

                    if ( file.Exists == false ) return;

                    // Open an input stream:

                    Stream input_stream = file.Open(FileMode.Open);

                    // Create a new xml data object:

                    XmlObjectData data = null;

                    // Try and read the xml:

                    try { data = new XmlObjectData( input_stream ); } catch ( Exception ){}

                    // Close the input stream:

                    input_stream.Close();

                    // Ok: try and read the attributes:

                    float sound_volume  = 0.75f;
                    float music_volume  = 0.65f;
                    float brightness    = 1.0f;

                    data.ReadFloat( "SoundVolume"   , ref sound_volume  );
                    data.ReadFloat( "MusicVolume"   , ref music_volume  );
                    data.ReadFloat( "Brightness"    , ref brightness    );

                    // Now set them:

                    SoundVolume = sound_volume;
                    MusicVolume = music_volume;
                    Brightness  = brightness;
                }

            }
            
            // In windows debug show what happened if something went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }
            
            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to save user preferences to the preferences xml file
        /// </summary>        
        //=========================================================================================

        public static void SavePreferences()
        {
            // This might fail:

            try
            {                    
                // Get the current storage container:

                StorageContainer container = StorageSystem.Container;

                // Only do if we have a storage device:

                if ( container != null )
                {
                    // Try and get the preferences file:

                    FileInfo file = new FileInfo( container.Path + "\\" + PREFERENCES_FILE );

                    // Create a new xml data object:

                    XmlObjectData data = new XmlObjectData();

                    // Ok: write all attributes

                    data.Write( "SoundVolume"   , s_sound_volume  );
                    data.Write( "MusicVolume"   , s_music_volume  );
                    data.Write( "Brightness"    , s_brightness    );

                    // Open an output stream:

                    Stream output_stream = file.Open(FileMode.Create,FileAccess.Write);

                    // Now save the data:

                    try 
                    { 
                        // Save:

                        data.Save( output_stream ); 
                    } 

                    // Show what happened on windows debug if something went wrong
                    
                    #if WINDOWS_DEBUG
                    
                        catch ( Exception e ){ DebugConsole.PrintException(e); }

                    #else

                        catch ( Exception ){}

                    #endif

                    // Close the output stream

                    output_stream.Close();
                }

            }
            
            // In windows debug show what happened if something went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }
            
            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Applys all the current game settings so they take effect.
        /// </summary>        
        //=========================================================================================

        public static void ApplyAllSettings()
        {
            // Apply all settings:

            UpdateSoundVolume();
            UpdateMusicVolume();
            UpdateBrightness();
        }

        //=========================================================================================
        /// <summary>
        /// Updates the actual sound volume of the game with the current settings.
        /// </summary>        
        //=========================================================================================

        private static void UpdateSoundVolume()
        {
            // Set the sound volume:

            if ( Core.Audio != null ) Core.Audio.SetGlobal( "MasterVolume" , s_sound_volume );
        }

        //=========================================================================================
        /// <summary>
        /// Updates the actual music volume of the game with the current settings.
        /// </summary>        
        //=========================================================================================

        private static void UpdateMusicVolume()
        {
            // Set the music volume:

            if ( Core.Music != null ) Core.Music.SetGlobal( "MasterVolume" , s_music_volume );
        }

        //=========================================================================================
        /// <summary>
        /// Updates the actual brightness of the game with the current settings.
        /// </summary>        
        //=========================================================================================

        private static void UpdateBrightness()
        {
            // Make a gamma ramp:

            GammaRamp g = new GammaRamp();

            // Calculate the gamma ramp:

            short[] ramp_table = new short[256];
            
            for ( int i = 0 ; i < 256 ; i++ )
            {
                // Calculate the normal intensity of this value:

                double l = (double)(i) / 255.0;

                // Adjust by gamma setting:

                l = Math.Pow( l , 1.0f - ( s_brightness - 1.0f ) );

                // Clamp:

                if ( l > 1.0f ) l = 1.0f;

                // Back to integer:

                l *= 65535;

                // Save to table:

                ramp_table[i] = (short) l;
            }

            // Set tables:

            g.SetRed(ramp_table);
            g.SetGreen(ramp_table);
            g.SetBlue(ramp_table);

            // Set on graphics device:

            try
            {
                if ( Core.Graphics != null ) Core.Graphics.Device.SetGammaRamp(false,g);
            }
            catch ( Exception ){}
        }

    }   // end of class

}   // end of namespace
