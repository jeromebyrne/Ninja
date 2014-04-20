
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
    /// Class that draws messages to the screen as the current phase of the game is changing.
    /// </summary>
    //#############################################################################################

    public class Gui_Phase_Change_Drawer : GuiWidget
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            public Font Font { get { return m_font; } } 

            /// <summary> String from the string database to display for the phase number message. </summary>

            public string PhaseStringName 
            {
                get { return m_phase_string_name; }

                // On debug complain if the text is null:

                set
                {
                    #if DEBUG

                        if ( value == null ) throw new ArgumentNullException("String name cannot be null !");

                    #endif

                    // Save the value

                    m_phase_string_name = value;
                }
            }

            /// <summary> String from the string database to display for the ready message. </summary>

            public string ReadyStringName 
            {
                get { return m_ready_string_name; }

                // On debug complain if the text is null:

                set
                {
                    #if DEBUG

                        if ( value == null ) throw new ArgumentNullException("String name cannot be null !");

                    #endif

                    // Save the value

                    m_ready_string_name = value;
                }
            }

            /// <summary> Center the text about it's position ? </summary>

            public bool Center { get { return m_center; } set { m_center = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Font used by the widget to draw text with. </summary>

            private Font m_font = null;

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            private string m_font_name = "";

            /// <summary> Center the text about it's position ? </summary>

            private bool m_center = false;

            /// <summary> String from the string database to display for the phase number message. </summary>

            private string m_phase_string_name = "";

            /// <summary> String from the string database to display for the ready message. </summary>

            private string m_ready_string_name = "";

            /// <summary> How much to make the font bigger as the phase change message fades out </summary>
            
            private float m_font_enlarge = 32.0f;

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

            data.ReadString ( "Font"            , ref m_font_name           , "Content\\Fonts\\Game_16px.xml"   );
            data.ReadString ( "PhaseStringName" , ref m_phase_string_name   , "No_String_Specified"             );
            data.ReadString ( "ReadyStringName" , ref m_ready_string_name   , "No_String_Specified"             );
            data.ReadBool   ( "Center"          , ref m_center              , false                             );
            data.ReadFloat  ( "FontEnlarge"     , ref m_font_enlarge        , 64.0f                             );

            // Read in font:

            m_font = new Font(m_font_name);
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
           
            data.Write( "Font"              , m_font_name           );
            data.Write( "PhaseStringName"   , m_phase_string_name   );
            data.Write( "ReadyStringName"   , m_ready_string_name   );
            data.Write( "Center"            , m_center              );
            data.Write( "FontEnlarge"       , m_font_enlarge        );
        }

        //=========================================================================================
        /// <summary>
        /// Draws the text.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // Abort if we do not have a font:

            if ( m_font == null ) return;

            // Grab the level rules object from the game:

            LevelRules level_rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

            // If it's not there then abort:

            if ( level_rules == null ) return;

            // Now see if a phase change is underway, if not then abort:

            if ( level_rules.TimeSincePhaseChange >= LevelRules.PHASE_CHANGE_TIME ) return;

            // Decide on the phase change message, alpha, and font enlargen amountfrom the time since the phase change:

            string phase_change_message     = null; 
            float  phase_change_message_a   = 0.0f;
            float  font_enlarge             = 0.0f;

                if ( level_rules.TimeSincePhaseChange >= LevelRules.PHASE_CHANGE_TIME * 0.5f ) 
                {
                    // Show the ready message:

                    phase_change_message = StringDatabase.GetString(m_ready_string_name);

                    // Decide on alpha:

                    if ( level_rules.TimeSincePhaseChange >= LevelRules.PHASE_CHANGE_TIME * 0.75f )
                    {
                        phase_change_message_a  = level_rules.TimeSincePhaseChange - LevelRules.PHASE_CHANGE_TIME * 0.75f;
                        phase_change_message_a /= LevelRules.PHASE_CHANGE_TIME * 0.25f;
                        phase_change_message_a  = 1.0f - phase_change_message_a;
                    }
                    else
                    {
                        phase_change_message_a  = level_rules.TimeSincePhaseChange - LevelRules.PHASE_CHANGE_TIME * 0.50f;
                        phase_change_message_a /= LevelRules.PHASE_CHANGE_TIME * 0.25f;
                    }
                }
                else
                {
                    // Show the phase message and the current phase number:

                    phase_change_message = StringDatabase.GetString(m_phase_string_name) + " " + (level_rules.CurrentPhase + 1 ).ToString();

                    // Decide on alpha:

                    if ( level_rules.TimeSincePhaseChange >= LevelRules.PHASE_CHANGE_TIME * 0.25f )
                    {
                        phase_change_message_a  = level_rules.TimeSincePhaseChange - LevelRules.PHASE_CHANGE_TIME * 0.25f;
                        phase_change_message_a /= LevelRules.PHASE_CHANGE_TIME * 0.25f;
                        phase_change_message_a  = 1.0f - phase_change_message_a;
                    }
                    else
                    {
                        phase_change_message_a  = level_rules.TimeSincePhaseChange;
                        phase_change_message_a /= LevelRules.PHASE_CHANGE_TIME * 0.25f;
                    }
                }
            
                // Make the alpha have a non linear ramp up:

                phase_change_message_a = (float) Math.Sqrt( phase_change_message_a );

                // Decide on font enlargening amount:

                font_enlarge = m_font_enlarge * ( 1.0f - phase_change_message_a );

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Make up a custom font settings block:

            Font.CustomFontSettings settings = new Font.CustomFontSettings
            (
                m_font.Size + font_enlarge  ,
                m_font.Color                ,
                m_font.CharSpacing          ,
                m_font.LineSpacing          ,
                m_font.Effect
            );

            // Change the settings:

            settings.Color.W = phase_change_message_a;

            // Draw the font string: see if it is to be centered about it's position first though
             
            if ( m_center )
            {
                // Draw centered at its position:
            
                m_font.DrawCustomStringCentered
                ( 
                    phase_change_message    , 
                    Position                ,
                    view_projection         ,
                    settings        
                );
            }
            else
            {
                // Draw non centered:
            
                m_font.DrawCustomString
                ( 
                    phase_change_message    , 
                    Position                , 
                    view_projection         ,
                    settings
                );
            }
        }

    }   // end of class

}   // end of namespace
