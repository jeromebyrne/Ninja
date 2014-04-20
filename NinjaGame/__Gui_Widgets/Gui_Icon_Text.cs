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
    /// Special version of the text class that allows icons to be inserted in between portions of 
    /// text. All icons are fixed size. Icon texture names are specified in between [] brackets.
    /// </summary>
    //#############################################################################################

    public class Gui_Icon_Text : Gui_Text
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> String from the string database to display in the widget. </summary>

            public override string StringName 
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

                    // Generate the portions that we will render

                    CreatePortions();
                }
            }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Size of all the icons in the x and y dimensions. </summary>

            private float m_icon_size = 32;

            /// <summary> Amount to offset each icon on the y axis. </summary>

            private float m_icon_offset_y = 0;

            /// <summary> Portions for output. The icon text is split up into different pieces of text and icons for rendering. </summary>

            private OutputPortion[] m_portions = new OutputPortion[0];

            /// <summary> Vertices used for rendering the icons. </summary>

            private VertexPositionColorTexture[] m_vertices = new VertexPositionColorTexture[4];

            /// <summary> Effect to render the icons with. </summary>

            private Effect m_icon_effect = null;

            /// <summary> Name of the effect to render the icons with. </summary>

            private string m_icon_effect_name = "";

        //=========================================================================================
        // Structs
        //=========================================================================================

            /// <summary> Represents a part of the iconified text, either an icon or just text itself. </summary>

            private struct OutputPortion
            {
                /// <summary> Output text. If this is null then this is an icon part. </summary>

                public string Text;

                /// <summary> Icon. If this is null then this is just normal text part. </summary>

                public Texture2D Icon;
            };

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates this object.
        /// </summary>
        //=========================================================================================

        public Gui_Icon_Text()
        {
            // Set the texture coordinates and colors on our vertices

            m_vertices[0].TextureCoordinate = Vector2.Zero;
            m_vertices[1].TextureCoordinate = Vector2.UnitY;
            m_vertices[2].TextureCoordinate = Vector2.UnitX;
            m_vertices[3].TextureCoordinate = Vector2.One;

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;
        }

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

            // Read all data:

            data.ReadEffect( "IconEffect" , ref m_icon_effect_name , ref m_icon_effect , "Effects\\textured" );

            data.ReadFloat( "IconSize"     , ref m_icon_size       , 32    );
            data.ReadFloat( "IconOffsetY"  , ref m_icon_offset_y   , 0     );

            // Generate the portions that we will render

            CreatePortions();
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
           
            data.Write( "IconEffect"    , m_icon_effect_name    );
            data.Write( "IconSize"      , m_icon_size           );
            data.Write( "IconOffsetY"   , m_icon_offset_y       );
        }

        //=========================================================================================
        /// <summary>
        /// Sets the effect used for the icons.
        /// </summary>
        /// <param name="name"> Name of the effect resource. </param>
        //=========================================================================================

        public void SetIconEffect( string name )
        {
            // If null then blank:

            if ( name == null ) name = "";

            // Load the effect

            m_icon_effect = Core.Graphics.LoadEffect(name);

            // Save its name:

            m_icon_effect_name = name;
        }

        //=========================================================================================
        /// <summary>
        /// Draw function. Draws the text.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // Abort if we do not have a font:

            if ( Font == null ) return;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Get total length of the text and icons:

            float text_length = CalculateTextAndIconLength();

            // Right: set x position where we will draw at

            float x = Position.X;

                // See which way we are aligned:

                if ( TextAlignment == Alignment.CENTER )
                {
                    // Center over position:

                    x -= text_length * 0.5f;
                }
                else if ( TextAlignment == Alignment.RIGHT )
                {
                    // Right align the text:

                    x -= text_length;
                }

            // Now draw each part:

            for ( int i = 0 ; i < m_portions.Length ; i++ )
            {
                // See if this is a text or icon portion:

                if ( m_portions[i].Text == null )
                {
                    // Is icon: draw it

                    DrawIcon( m_portions[i].Icon , x );

                    // Move on the x position:

                    x += m_icon_size;
                }
                else
                {
                    // Text: draw it at current position:

                        // Make up position to draw at:

                        Vector2 pos = Position; pos.X = x;

                        // Draw:

                        Font.DrawString( m_portions[i].Text , pos , view_projection );

                    // Move on the x position:

                    x += Font.GetStringSize( m_portions[i].Text ).X;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Draws the given icon at the given x position.
        /// </summary>
        /// <param name="icon_texture"> Icon to draw </param>
        /// <param name="x_pos"> X position to draw the icon at </param>
        //=========================================================================================

        private void DrawIcon( Texture2D icon_texture , float x_pos )
        {
            // Abort if no icon effect or texture:

            if ( m_icon_effect == null || Font == null ) return;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Take into account size of icon

            m_vertices[0].Position.X = 0;
            m_vertices[1].Position.X = 0;
            m_vertices[2].Position.X = m_icon_size;
            m_vertices[3].Position.X = m_icon_size;

            m_vertices[0].Position.Y = 0;
            m_vertices[1].Position.Y = - m_icon_size;
            m_vertices[2].Position.Y = 0;
            m_vertices[3].Position.Y = - m_icon_size;

            // Move icon into it's world position:

            m_vertices[0].Position.X += Position.X + x_pos;
            m_vertices[1].Position.X += Position.X + x_pos;
            m_vertices[2].Position.X += Position.X + x_pos;
            m_vertices[3].Position.X += Position.X + x_pos;

            m_vertices[0].Position.Y += Position.Y + m_icon_offset_y;
            m_vertices[1].Position.Y += Position.Y + m_icon_offset_y;
            m_vertices[2].Position.Y += Position.Y + m_icon_offset_y;
            m_vertices[3].Position.Y += Position.Y + m_icon_offset_y;

            // Set shader params:

            EffectParameter param_tex = m_icon_effect.Parameters[ "Texture"               ];
            EffectParameter param_wvp = m_icon_effect.Parameters[ "WorldViewProjection"   ];

            if ( param_tex != null ) param_tex.SetValue( icon_texture               );
            if ( param_wvp != null ) param_wvp.SetValue( view_projection    );

            // Begin drawing with the shader:

            m_icon_effect.Begin();

                // Set vertex declaration on graphics device:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

                // Begin pass:

                m_icon_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleStrip ,
                        m_vertices                  ,
                        0                           ,
                        2
                    );

                // End pass:

                m_icon_effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            m_icon_effect.End();
        }

        //=========================================================================================
        /// <summary> 
        /// Creates the portions of text and icons used for rendering. 
        /// </summary>
        //=========================================================================================

        private void CreatePortions()
        {
            // No string name means no portions: 

            if ( StringName == null || StringName.Length <= 0 )
            {
                // Abort:

                m_portions = new OutputPortion[0]; return;
            }

            // Grab the text for this widget:

            string text = StringDatabase.GetString( StringName );

            // Store a list of portions here:

            List<OutputPortion> portions = new List<OutputPortion>();

            // Run through the text:

            for ( int i = 0 ; i < text.Length ; )
            {
                // Search for the the next opening bracket:

                int open_index = text.IndexOf( '[' , i );

                // See if there: 

                if ( open_index >= 0 )
                {
                    // Great: now try and find the closing bracket:

                    int close_index = text.IndexOf( ']' , open_index );

                    // See if found:

                    if ( close_index >= 0 )
                    {
                        // Found closing bracket: make a text portion for before the bracketed part

                        {
                            // Calculate the length of the text we want:

                            int length = open_index - i;

                            // Only do if there is text:

                            if ( length > 0 )
                            {
                                // Get the text we want

                                string t = text.Substring( i , open_index - i );

                                // Make the portion:

                                OutputPortion p;

                                p.Text = t;
                                p.Icon = null;

                                // Add into the list of portions:

                                portions.Add(p);
                            }
                        }

                        // Make an icon portion for inside the bracketed part:

                        {
                            // Get the length of the text inside the bracket:

                            int bracket_text_length = close_index - open_index - 1;

                            // Make a new icon portion if the text is not zero length:

                            if ( bracket_text_length > 0 )
                            {
                                // Get the text inside the bracket:

                                string t = text.Substring( open_index + 1 , bracket_text_length );

                                // Now: load a texture from this

                                Texture2D tex = Core.Graphics.LoadTexture(t);

                                // See if that succeeded:

                                if ( tex != null )
                                {
                                    // Loaded the texture successfully: make an icon part

                                    OutputPortion p;

                                    p.Text = null;
                                    p.Icon = tex;

                                    // Add into the list of portions:

                                    portions.Add(p);
                                }
                                else
                                {
                                    // Loading the texture failed: make a text part with an invalid characer, it will be shown as a square by the font

                                    string c = ((char)(0x01)).ToString();

                                    // Make the portion:

                                    OutputPortion p;

                                    p.Text = c;
                                    p.Icon = null;

                                    // Add into the list of portions:

                                    portions.Add(p);
                                }
                            }
                            else
                            {
                                // Nothing inside the bracket: make a text part with an invalid characer, it will be shown as a square by the font

                                string c = ((char)(0x01)).ToString();

                                // Make the portion:

                                OutputPortion p;

                                p.Text = c;
                                p.Icon = null;

                                // Add into the list of portions:

                                portions.Add(p);
                            }
                        }

                        // Move on past the closing index

                        i = close_index + 1;
                    }
                    else
                    {
                        // Couldn't find a closing bracket to correspond with opening one.. We are done with this text: make a text portion from here to the end

                        OutputPortion p; 

                        p.Text = text.Substring(i);
                        p.Icon = null;

                        // Add into the list of portions:

                        portions.Add(p);

                        // Now done:

                        i = text.Length;
                    }
                }
                else
                {
                    // Couldn't find next opening bracket.. we are done with this text: make a text portion from here to the end

                    OutputPortion p; 

                    p.Text = text.Substring(i);
                    p.Icon = null;

                    // Add into the list of portions:

                    portions.Add(p);

                    // Now done:

                    i = text.Length;
                }
            }

            // Now save the list of portions:

            m_portions = portions.ToArray();
        }

        //=========================================================================================
        /// <summary> 
        /// Calculates the length of the text with icons and text
        /// </summary>
        //=========================================================================================

        public float CalculateTextAndIconLength()
        {
            // Store total length here:

            float length = 0;

            // Run through all the portions:

            for ( int i = 0 ; i < m_portions.Length ; i++ )
            {
                // See if this portion is an icon or not:

                if ( m_portions[i].Icon == null )
                {
                    // Text: get the length of it

                    length += Font.GetStringSize(m_portions[i].Text).X;
                }
                else
                {
                    // Icon: add to the length by icon size

                    length += m_icon_size;
                }
            }

            // Return the length:

            return length;
        }

    }   // end of class

}   // end of namespace
