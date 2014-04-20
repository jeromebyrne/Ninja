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
    /// Simple class that allows text to be drawn in the GUI.
    /// </summary>
    //#############################################################################################

    public class Gui_Text : GuiWidget
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            public Font Font { get { return m_font; } set { m_font = value; } } 

            /// <summary> String from the string database to display in the widget. </summary>

            public virtual string StringName 
            {
                get { return m_string_name; }

                // On debug complain if the text is null:

                set
                {
                    #if DEBUG

                        if ( value == null ) throw new ArgumentNullException("String name for Gui_Text widget cannot be null !");

                    #endif

                    // Save the value

                    m_string_name = value;
                }
            }

            /// <summary> Which way to align the text. </summary>

            public Alignment TextAlignment { get { return m_alignment; } set { m_alignment = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Font used by the widget to draw text with. </summary>

            private Font m_font = null;

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            private string m_font_name = "";

            /// <summary> Name of the string from the database to display in the widget. </summary>

            protected string m_string_name = "";

            /// <summary> Which way to align the text. </summary>

            private Alignment m_alignment = Alignment.LEFT;

        //=========================================================================================
        // Enums
        //=========================================================================================

            /// <summary> Structure specifying how the text should be aligned. </summary>

            public enum Alignment
            {
                /// <summary> Draw text normally from left of text position. </summary>

                LEFT ,

                /// <summary> Center text over text position. </summary>

                CENTER ,

                /// <summary> Draw text so it ends at text position. </summary>

                RIGHT
            };

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

            data.ReadString ( "Font"        , ref m_font_name   , "Content\\Fonts\\Game_16px.xml"   );
            data.ReadString ( "StringName"  , ref m_string_name , "No_String_Specified"             );

            // Read alignment:

            string alignment = ""; data.ReadString("Alignment" , ref alignment );

            // See what alignment we have:

            alignment = alignment.ToLower();

            if ( alignment.Equals("center") )
            {
                // Text is centered

                m_alignment = Alignment.CENTER;
            }
            else if ( alignment.Equals("right") )
            {
                // Right align

                m_alignment = Alignment.RIGHT;
            }
            else
            {
                // Left align

                m_alignment = Alignment.LEFT;
            }

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
           
            data.Write( "Font"          , m_font_name       );
            data.Write( "StringName"    , m_string_name     );

            // Write text alignment:

            if      ( m_alignment == Alignment.LEFT   ) { data.Write("Alignment" , "left"  ); }
            else if ( m_alignment == Alignment.RIGHT  ) { data.Write("Alignment" , "Right" ); }
            else                                        { data.Write("Alignment" , "Center"); }
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

            // Draw the font string: see what alignment it has first though
             
            if ( m_alignment == Alignment.CENTER )
            {
                // Draw centered at its position:
            
                m_font.DrawStringCentered( StringDatabase.GetString(m_string_name) , Position , view_projection );
            }
            else if ( m_alignment == Alignment.RIGHT )
            {
                // Draw right aligned:
            
                m_font.DrawStringRightAligned( StringDatabase.GetString(m_string_name) , Position , view_projection );
            }
            else
            {
                // Draw left aligned:
            
                m_font.DrawString( StringDatabase.GetString(m_string_name) , Position , view_projection );
            }
        }

    }   // end of class

}   // end of namespace
