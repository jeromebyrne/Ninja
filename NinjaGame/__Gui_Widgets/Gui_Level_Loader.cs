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
    /// <summary>
    /// This widget simply loads a level when it is ready. It allows the GUI screen to do one 
    /// draw before commencing loading. It also switches to a specified in game GUI.
    /// </summary>
    //#############################################################################################

    public class Gui_Level_Loader : GuiWidget
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Name of the level to load. </summary>

            public string LevelName
            {
                get { return m_level_name; } 
                
                set 
                { 
                    // Set level name

                    m_level_name = value; 

                    // Don't allow null values
                    
                    if ( m_level_name == null ) 
                    {
                        m_level_name = "";
                    }
                }
            }

            /// <summary> Name of the game gui to load. </summary>

            public string GameGui
            {
                get { return m_game_gui_name; } 
                
                set 
                { 
                    // Set game gui name

                    m_game_gui_name = value; 

                    // Don't allow null values
                    
                    if ( m_game_gui_name == null ) 
                    {
                        m_game_gui_name = "";
                    }
                }
            }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Did the screen refresh ? The level is not loaded until the gui is drawn </summary>

            private bool m_screen_drawn = false;

            /// <summary> Name of the level to load. </summary>

            private string m_level_name = "";

            /// <summary> Name of the game gui to load. </summary>

            private string m_game_gui_name = "";

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should read its own data from
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// read from here.
        /// </param>
        //=========================================================================================

        public override void ReadXml( XmlObjectData data )
        {
            // Call base class function

            base.ReadXml(data);

            // Read all data

            data.ReadString ( "Level"   , ref m_level_name      );
            data.ReadString ( "GameGui" , ref m_game_gui_name   );
        }

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should write its own data to
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// written to here.
        /// </param>
        //=========================================================================================

        public override void WriteXml( XmlObjectData data )
        {
            // Call base class function

            base.WriteXml(data);

            // Write all data:
           
            data.Write( "Level"     , m_level_name      );
            data.Write( "GameGui"   , m_game_gui_name   );
        }

        //=========================================================================================
        /// <summary>
        /// Draw function for this widget. Sets the screen drawn flag to true which allows level 
        /// loading to commence.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // We have no drawn a screen:

            m_screen_drawn = true;
        }

        //=========================================================================================
        /// <summary>
        /// Update function for the widget. Loads the given level if the screen has been drawn and 
        /// if a level name was given to load.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Abort if the screen has not been drawn yet:

            if ( m_screen_drawn == false ) return;

            // Abort if no level name or game gui was given:

            if ( m_level_name == null || m_level_name.Length <= 0 ) 
            {
                return;
            }

            if ( m_game_gui_name == null || m_game_gui_name.Length <= 0 ) 
            {
                return;
            }

            // Load the level:

            Core.Level.Load(m_level_name); 

            // Clear the level to load:

            m_level_name = "";

            // Load the game gui:

            Core.Gui.Load(m_game_gui_name);

            // Clear the gui to load:

            m_game_gui_name = "";
        }

    }   // end of class

}   // end of namespace
