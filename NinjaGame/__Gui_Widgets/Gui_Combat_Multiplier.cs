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
    /// This widget displays the player's combat multiplier as a number.
    /// </summary>
    //#############################################################################################

    public class Gui_Combat_Multiplier : GuiWidget
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            public Font Font { get { return m_font; } }

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

            data.ReadString ( "Font"    , ref m_font_name   , "Content\\Fonts\\Game_16px.xml"   );
            data.ReadBool   ( "Center"  , ref m_center      , false                             );

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
           
            data.Write( "Font"      , m_font_name   );
            data.Write( "Center"    , m_center      );
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

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Get the player's combat multiplier:

            float multiplier = 1;

                {
                    // Try to find the level rules component:

                    LevelRules rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

                    // See if found: if so then get multiplier

                    if ( rules != null ) multiplier = rules.PlayerCombatMultiplier;
                }

            // Draw the font string: see if it is to be centered about it's position first though
             
            if ( m_center )
            {
                // Draw centered at its position:
            
                m_font.DrawStringCentered( "x" + ((int)(multiplier)).ToString(Locale.DevelopmentCulture.NumberFormat) , Position , view_projection );
            }
            else
            {
                // Draw non centered:
            
                m_font.DrawString( "x" + ((int)(multiplier)).ToString(Locale.DevelopmentCulture.NumberFormat) , Position , view_projection );
            }
        }

    }   // end of class

}   // end of namespace
