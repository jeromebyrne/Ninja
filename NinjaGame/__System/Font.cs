using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{   
    //#############################################################################################
    //
    ///
    /// <summary> 
    /// 
    /// Class that stores a bitmap font and displays strings in this font type. 
    /// Expects font image to have 16 columns and 16 rows of characters, ordered 
    /// by the layout of the codepage in use.
    /// 
    /// </summary>
    //
    //#############################################################################################

    public class Font
    {
        //=========================================================================================
        // Properties
        //=========================================================================================
            
            /// <summary> Gets the texture used by the font. </summary>

            public Texture2D Texture { get { return m_texture; } }

            /// <summary> Size of the font in pixels when drawn to the screen </summary>

            public float Size { get { return m_size; } }

            /// <summary> Spacing in pixels between font characters. </summary>

            public float CharSpacing { get { return m_char_spacing; } }

            /// <summary> Spacing in pixels between font lines. </summary>

            public float LineSpacing { get { return m_line_spacing; } }

            /// <summary> RGBA color of the font </summary>
            
            public Vector4 Color { get { return m_color; } }

            /// <summary> Effect used by the font. </summary>

            public Effect Effect { get { return m_effect; } }

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Maximum Number of characters that can be rendered in one batch </summary>

            private const int RENDER_BUFFER_SIZE = 2048;

            /// <summary> Width of a space character in percentage of character size </summary>

            private const float SPACE_WIDTH_PERCENT = 0.35f;

        //=========================================================================================
        // Structs
        //=========================================================================================

            /// <summary> Structure representing custom settings for this font which allow it to be drawn exactly as desired. </summary>

            public struct CustomFontSettings
            {
                /// <summary> Size of the font with the custom settings. </summary>

                public float Size;

                /// <summary> Color of the font with the custom settings. </summary>

                public Vector4 Color;

                /// <summary> Spacing in pixels between font characters with the custom settings. </summary>

                public float CharSpacing;

                /// <summary> Spacing in pixels between font lines with the custom settings. </summary>

                public float LineSpacing;

                /// <summary> Effect to render the font with. </summary>

                public Effect Effect;

                //=================================================================================
                /// <summary> Constructor. Creates a custom font settings structure. </summary>
                /// 
                /// <param name="s">    Size of the font with the custom settings.                          </param>
                /// <param name="c">    Color of the font with the custom settings.                         </param>
                /// <param name="cs">   Spacing in pixels between font characters with the custom settings. </param>
                /// <param name="ls">   Spacing in pixels between font lines with the custom settings.      </param>
                /// <param name="e">    Effect to render the font with.                                     </param>
                //=================================================================================

                public CustomFontSettings( float s , Vector4 c , float cs , float ls , Effect e )
                {
                    Size        = s;
                    Color       = c;
                    CharSpacing = cs;
                    LineSpacing = ls;
                    Effect      = e;
                }
            };

            /// <summary> A structure representing the size and position of a character in the font. </summary>

            private struct CharacterMetrics
            {
                /// <summary> Position of the character in the texture (X). 0-1 texture coordinate. </summary>

                public float tc_x;

                /// <summary> Position of the character in the texture (Y). 0-1 texture coordinate. </summary>

                public float tc_y;

                /// <summary> Width of the character in the texture. 0-1 texture coordinate. </summary>

                public float tc_w;

                /// <summary> Height of the character in the texture. 0-1 texture coordinate. </summary>

                public float tc_h;
            }            

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Font texture to use </summary>
            
            private Texture2D m_texture = null;

            /// <summary> Shader to render the font with </summary>
            
            private Effect m_effect = null;

            /// <summary> Buffer to store characters to be rendered </summary>
            
            private VertexPositionColorTexture[] m_vertices = null;

            /// <summary> RGBA color of the font </summary>

            private Vector4 m_color = Vector4.One;  
        
            /// <summary> Size of the font in pixels when drawn to the screen </summary>

            private float m_size = 32;

            /// <summary> Spacing in pixels between font characters. </summary>

            private float m_char_spacing = 2;

            /// <summary> Spacing in pixels between font lines. </summary>

            private float m_line_spacing = 4;

            /// <summary> Metrics for each character in the char set. </summary>

            private CharacterMetrics[] m_metrics = new CharacterMetrics[256];

        //=====================================================================================
        /// <summary> 
        /// Overloaded constructor for the font. Loads the font's description from the given 
        /// xml file. 
        /// </summary>
        /// 
        /// <param name="xml_file_name"> Name of the xml file containing the font's description. </param> 
        //=====================================================================================

        public Font( String xml_file_name )
        {
            // Create the shader and buffers used for the font

            FontSetup();
            
            // Set the font texture used by the font

            LoadFontDescription(xml_file_name);
        }

        //=====================================================================================
        /// <summary> 
        /// Gets all of the settings for the font and wraps them up into the given font settings
        /// block.
        /// </summary>
        ///
        //=====================================================================================

        public CustomFontSettings GetSettings()
        {
            // Make up a new custom font settings block:

            CustomFontSettings settings;

            settings.CharSpacing    = m_char_spacing;
            settings.Color          = m_color;
            settings.Effect         = m_effect;
            settings.LineSpacing    = m_line_spacing;
            settings.Size           = m_size;

            // Now return it:

            return settings;
        }

        //=====================================================================================
        /// <summary> 
        /// Loads the given XML font descriptor for this font which contains all the sizing 
        /// and other information for this particular font. 
        /// </summary>
        /// 
        /// <param name="xml_file_name"> Name of the xml file containing the font's description. </param> 
        //=====================================================================================

        private void LoadFontDescription( String xml_file_name )
        {
            // Create the shader and buffers used for the font if not already done so

            FontSetup();

            // This might fail:

            try
            {
                // Try and open this xml file:

                XmlDocument xml_doc = new XmlDocument(); xml_doc.Load( Locale.GetLocFile(xml_file_name) );

                // Good: see if root element is there:

                if ( xml_doc.FirstChild != null )
                {
                    // Load all the information for the font:

                    foreach ( XmlNode node in xml_doc.FirstChild.ChildNodes )
                    {
                        // See what node this is:

                        if ( node.Name.Equals( "CharMetric" ) )
                        {
                            // Char metric: get all attributes

                            XmlAttribute a_code = node.Attributes["Code"];
                            XmlAttribute a_up   = node.Attributes["UP"];
                            XmlAttribute a_vp   = node.Attributes["VP"];
                            XmlAttribute a_us   = node.Attributes["US"];
                            XmlAttribute a_vs   = node.Attributes["VS"];

                            // See if the code exists firstly:

                            if ( a_code != null )
                            {
                                // Convert to char:

                                int c = Convert.ToInt32(a_code.Value,Locale.DevelopmentCulture.NumberFormat) & 0xff;

                                // Good: now that we have the character code we can save the info for the size of this char:

                                if ( a_up != null ) m_metrics[c].tc_x = Convert.ToSingle(a_up.Value, Locale.DevelopmentCulture.NumberFormat);
                                if ( a_vp != null ) m_metrics[c].tc_y = Convert.ToSingle(a_vp.Value, Locale.DevelopmentCulture.NumberFormat);
                                if ( a_us != null ) m_metrics[c].tc_w = Convert.ToSingle(a_us.Value, Locale.DevelopmentCulture.NumberFormat);
                                if ( a_us != null ) m_metrics[c].tc_h = Convert.ToSingle(a_vs.Value, Locale.DevelopmentCulture.NumberFormat);
                            }
                        }
                        else if ( node.Name.Equals("Texture") )
                        {
                            // Load the texture:

                            m_texture = Core.Graphics.LoadTexture( node.InnerText.Trim() );
                        }
                        else if ( node.Name.Equals("FontSize") )
                        {
                            // Save font size:

                            m_size = Convert.ToSingle( node.InnerText.Trim(), Locale.DevelopmentCulture.NumberFormat );
                        }
                        else if ( node.Name.Equals("FontCharSpacing") )
                        {
                            // Save font char spacing:

                            m_char_spacing = Convert.ToSingle( node.InnerText.Trim() );
                        }
                        else if ( node.Name.Equals("FontLineSpacing", StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            // Save font line spacing:

                            m_line_spacing = Convert.ToSingle( node.InnerText.Trim(), Locale.DevelopmentCulture.NumberFormat );
                        }
                        else if ( node.Name.Equals("FontColorR", StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            // Save font color r:

                            m_color.X = Convert.ToSingle( node.InnerText.Trim() );
                        }
                        else if ( node.Name.Equals("FontColorG", StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            // Save font color g:

                            m_color.Y = Convert.ToSingle( node.InnerText.Trim(), Locale.DevelopmentCulture.NumberFormat );
                        }
                        else if ( node.Name.Equals("FontColorB", StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            // Save font color b:

                            m_color.Z = Convert.ToSingle( node.InnerText.Trim(), Locale.DevelopmentCulture.NumberFormat );
                        }
                        else if ( node.Name.Equals("FontColorA", StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            // Save font color a:

                            m_color.W = Convert.ToSingle( node.InnerText.Trim(), Locale.DevelopmentCulture.NumberFormat );
                        }

                    }   // end for all root node children

                }
                
                #if WINDOWS_DEBUG

                    // Give out on win debug if root element is missing:

                    else
                    {
                        throw new Exception("Font description is missing root xml element!");
                    }

                #endif

            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened:

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif

        }

        //=========================================================================================
        /// <summary>
        /// Generates the position and size of each character in the font for later use with rendering.
        /// NOTE: THIS FUNCTION IS NOW DEPRECATED. THESE METRICS ARE NOW PRECALCULATED AND SAVED IN
        /// XML FILES. THIS IS ONLY FOR REFERENCE.
        /// </summary>
        //=========================================================================================

        private void GenerateCharacterMetrics()
        {
            // If we have no texture then abort:

            if ( m_texture == null ) return;

            // Get the array of pixels from the texture:

            Color[] pixels = new Color[ m_texture.Width * m_texture.Height ]; 
                
                // Get the data
            
                m_texture.GetData<Color>( pixels );

            // Do each character in the char set:

            for ( int i = 0 ; i < 256 ; i++ )
            {
                // Calculate the row and column of this char in the texture

                int row = ( i >> 4 ) & 0x0F;
                int col = ( i      ) & 0x0F;

                // Calculate the width of each character in the texture:

                int w = m_texture.Width  >> 4;
                int h = m_texture.Height >> 4;
                
                // Calculate the x and y position of the top left pixel in the texture:

                int x = col * w;
                int y = row * h;

                // Calculate the right and bottom pixel boundaries for this character in the texture:

                int r = x + w;
                int b = y + h;

                // Now calculate the largest and smallest x and y values for the character 

                int s_x = x + w;
                int l_x = x;
                int s_y = y + h;
                int l_y = y;

                    // Run through all the pixels for this character:
                    
                    for ( int pos_x = x ; pos_x < r ; pos_x ++ )
                    for ( int pos_y = y ; pos_y < b ; pos_y ++ )
                    {
                        // See if this is a transparent pixel:

                        if ( ( pixels[ ( pos_y * m_texture.Width ) + pos_x ].A ) > GraphicsSystem.DefaultRenderState.ReferenceAlpha )
                        {
                            // Not transparent: see if this x and y value is smaller or bigger than current:

                            if ( pos_x < s_x ) s_x = pos_x;
                            if ( pos_x > l_x ) l_x = pos_x;
                            if ( pos_y < s_y ) s_y = pos_y;
                            if ( pos_y > l_y ) l_y = pos_y;
                        }
                    }
                
                // Save the metrics of this character: add one extra pixel of spacing to be safe

                s_x--;
                l_x++;
                s_y--;
                l_y++;

                m_metrics[i].tc_x = (float)( s_x        ) / (float)( m_texture.Width     );
                m_metrics[i].tc_y = (float)( s_y        ) / (float)( m_texture.Height    );
                m_metrics[i].tc_w = (float)( l_x - s_x  ) / (float)( m_texture.Width     );
                m_metrics[i].tc_h = (float)( l_y - s_y  ) / (float)( m_texture.Height    );
            }
        }

        //=====================================================================================
        /// <summary> 
        /// Peforms basic font setup, such as creating the buffers used for rendering and 
        /// loading the shader used.
        /// </summary>
        //=====================================================================================

        private void FontSetup()
        {
            // Load the shader used to render the font:

            m_effect = Core.Graphics.LoadEffect("Effects\\textured");

            // Create the buffer of vertices used to render:

            m_vertices = new VertexPositionColorTexture[ 6 * RENDER_BUFFER_SIZE ];
        }

        //=====================================================================================
        /// <summary> 
        /// Setups the font for drawing. 
        /// </summary>
        /// 
        /// <param name="transform"> Transform matrix to use. </param>
        /// <returns> True if draw setup was successful, false if not. </returns>
        //=====================================================================================

        private bool DrawSetup( Matrix transform )
        {
            // If we don't have a shader or texture then abort:

            if ( m_texture == null || m_effect == null ) return false;

            // Set the tranform used by the shader

            {
                // Grab the model view project matrix for the shader:

                EffectParameter param = m_effect.Parameters["WorldViewProjection"];

                if ( param != null )
                {
                    param.SetValue( transform );
                }

            }

            // Set the texture used by the shader:

            {
                // Grab texture param:

                EffectParameter param = m_effect.Parameters["Texture"];

                if ( param != null )
                {
                    param.SetValue(m_texture);
                }
            }
            
            // Set the vertex declaration for the graphics device

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

            // Return true for successful setup:

            return true;
        }

        //=====================================================================================
        /// <summary>
        /// Puts the specified character into the rendering buffer at the given character 
        /// offset in the buffer.
        /// </summary>
        /// <param name="c"> Character to put in the buffer </param>
        /// <param name="offset"> Character offset in the buffer to put the data at. </param>
        /// <param name="position"> Position to draw the character at </param>
        //=====================================================================================

        private void PutBufferCharacter( char c , int offset , Vector2 position )
        {
            //------------------------------------------------------------------------------------
            // Get what row and column of the font texture the character is located at. There 
            // are 16 columns and 16 rows in a font texture. First 4 bits of character code 
            // gives the column, next 4 bits gives the row.
            //------------------------------------------------------------------------------------

            int col = ( ((int)(c)) >> 0 ) & 0xf;
            int row = ( ((int)(c)) >> 4 ) & 0xf;

            //------------------------------------------------------------------------------------
            // Now convert these to u and v texture coordinates for the top, bottom and 
            // left/right of the character's textured quad.
            //------------------------------------------------------------------------------------

            float ul = m_metrics[c&0xff].tc_x;   
            float ur = m_metrics[c&0xff].tc_x + m_metrics[c&0xff].tc_w;

            float vt = (float)( row     ) / 16.0f;
            float vb = (float)( row + 1 ) / 16.0f;

            //------------------------------------------------------------------------------------
            // Put the character's position, color and texure coordinates into the rendering 
            // buffer. We are rendering it's quad as two triangles.
            //------------------------------------------------------------------------------------

            // Get index of current position in render buffer

            int bufferPos = offset * 6;

            // Fill in the buffer positions:

            m_vertices[ bufferPos + 0 ].Position.X  = position.X;
            m_vertices[ bufferPos + 0 ].Position.Y  = position.Y;
            m_vertices[ bufferPos + 0 ].Position.Z  = 0;
            m_vertices[ bufferPos + 1 ].Position.X  = position.X + m_metrics[c&0xff].tc_w * 16 * m_size;
            m_vertices[ bufferPos + 1 ].Position.Y  = position.Y;
            m_vertices[ bufferPos + 1 ].Position.Z  = 0;
            m_vertices[ bufferPos + 2 ].Position.X  = position.X + m_metrics[c&0xff].tc_w * 16 * m_size;
            m_vertices[ bufferPos + 2 ].Position.Y  = position.Y + m_size;
            m_vertices[ bufferPos + 2 ].Position.Z  = 0;
            m_vertices[ bufferPos + 3 ].Position.X  = position.X + m_metrics[c&0xff].tc_w * 16 * m_size;
            m_vertices[ bufferPos + 3 ].Position.Y  = position.Y + m_size;
            m_vertices[ bufferPos + 3 ].Position.Z  = 0;
            m_vertices[ bufferPos + 4 ].Position.X  = position.X;
            m_vertices[ bufferPos + 4 ].Position.Y  = position.Y + m_size;
            m_vertices[ bufferPos + 4 ].Position.Z  = 0;
            m_vertices[ bufferPos + 5 ].Position.X  = position.X;
            m_vertices[ bufferPos + 5 ].Position.Y  = position.Y;
            m_vertices[ bufferPos + 5 ].Position.Z  = 0;

            // Fill in the buffer texture coordinates:

            m_vertices[ bufferPos + 0 ].TextureCoordinate.X = ul;
            m_vertices[ bufferPos + 0 ].TextureCoordinate.Y = vb;
            m_vertices[ bufferPos + 1 ].TextureCoordinate.X = ur;
            m_vertices[ bufferPos + 1 ].TextureCoordinate.Y = vb;
            m_vertices[ bufferPos + 2 ].TextureCoordinate.X = ur;
            m_vertices[ bufferPos + 2 ].TextureCoordinate.Y = vt;
            m_vertices[ bufferPos + 3 ].TextureCoordinate.X = ur;
            m_vertices[ bufferPos + 3 ].TextureCoordinate.Y = vt;
            m_vertices[ bufferPos + 4 ].TextureCoordinate.X = ul;
            m_vertices[ bufferPos + 4 ].TextureCoordinate.Y = vt;
            m_vertices[ bufferPos + 5 ].TextureCoordinate.X = ul;
            m_vertices[ bufferPos + 5 ].TextureCoordinate.Y = vb;

            // Fill in buffer colors:

            Color color = new Color(m_color); 
        
            for ( int i  = 0 ; i < 6 ; i++ )
            { 
                m_vertices[ bufferPos + i ].Color = color; 
            }
        }

        //=========================================================================================
        /// <summary> 
        /// Draws a string at the specified location.
        /// </summary>
        /// 
        /// <param name="text">      text to draw            </param>
        /// <param name="position">  where to draw the text  </param>
        //=========================================================================================

        public void DrawString ( string text , Vector2 position )
        {
            // Create an orthographic projection matrix:

            Matrix projection = Matrix.CreateOrthographic
            (
                Core.Graphics.Device.Viewport.Width         ,
                Core.Graphics.Device.Viewport.Height        ,
                0                                           ,
                1.0f
            );

            // Draw the given string using this matrix:

            DrawString( text , position , projection);
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a string centered horizontally at the specified position.
        /// </summary>
        /// 
        /// <param name="text">     text to draw        </param>
        /// <param name="box_pos">  position of text    </param>
        //=====================================================================================

        public void DrawStringCentered ( string text, Vector2 pos )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Current y offset for the text:

                float y_offset = 0;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coord to draw it at to center the string horizontally

                    float x = pos.X - ( size.X * 0.5f );

                    // Create an orthographic projection matrix:

                    Matrix projection = Matrix.CreateOrthographic
                    (
                        Core.Graphics.Device.Viewport.Width         ,
                        Core.Graphics.Device.Viewport.Height        ,
                        0                                           ,
                        1.0f
                    );

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , projection );

                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + m_size;
                }
            }
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a string centered right aligned to the specified position.
        /// </summary>
        /// 
        /// <param name="text">     text to draw        </param>
        /// <param name="box_pos">  position of text    </param>
        //=====================================================================================

        public void DrawStringRightAligned( string text, Vector2 pos )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Current y offset for the text:

                float y_offset = 0;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coord to draw it at to right align the string

                    float x = pos.X - size.X;

                    // Create an orthographic projection matrix:

                    Matrix projection = Matrix.CreateOrthographic
                    (
                        Core.Graphics.Device.Viewport.Width         ,
                        Core.Graphics.Device.Viewport.Height        ,
                        0                                           ,
                        1.0f
                    );

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , projection );

                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + m_size;
                }
            }
        }

        //=========================================================================================
        /// <summary> 
        /// Draws a string at the specified location with the given transform matrix.
        /// </summary>
        /// 
        /// <param name="text">      text to draw            </param>
        /// <param name="position">  where to draw the text  </param>
        /// <param name="transform"> transform matrix to use </param>
        //=========================================================================================

        public void DrawString ( string text , Vector2 position , Matrix transform )
        {
            // Abort on no text given:

            if ( text == null || text.Length <= 0 ) return;

            // Setup for drawing: abort on failure:

            if ( DrawSetup(transform) == false ) return;

            // Variables used when drawing:

                // Current position to draw the next character at

                Vector2 pos = position;

                // Number of characters put to the render buffer

                int buffer_chars = 0;

            // Draw the string:

            for ( int i = 0 ; i < text.Length ; )
            {   
                // Get the number of characters left:

                int charsLeft = text.Length - i;

                // See if the buffer is big enough to render the remaining chars

                if ( charsLeft <= RENDER_BUFFER_SIZE )
                {
                    //-----------------------------------------------------------------------------
                    // Can render the remaining chars in one go
                    //-----------------------------------------------------------------------------

                    // Set the number of characters put to the buffer as 0

                    buffer_chars = 0;

                    //-----------------------------------------------------------------------------
                    // Go through this part of the string that we can render in one go
                    //-----------------------------------------------------------------------------

                    for ( int j = i ; j < text.Length ; j++ )
                    {
                        // Get the character at this index:

                        char c = text[j];

                        // If the draw character is a carriage return or newline then move onto a new line:

                        if ( c == '\n' )
                        {
                            // Move onto a new line:

                            pos.Y -= m_size + m_line_spacing; pos.X = position.X; continue;
                        }

                        // If the draw character is a tab then move out to the next tab space:

                        if ( c == '\t' )
                        {
                            // Calculate x value of next tab space

                            float tab_x = (int)( ( pos.X - position.X ) / m_size * 4.0f ) * ( m_size * 4.0f ) + ( m_size * 4.0f );

                            // Set the new position to draw at

                            pos.X = tab_x; continue;
                        }

                        // If the draw character is a space then ignore:

                        if ( c == ' ' ){ pos.X += m_size * SPACE_WIDTH_PERCENT; continue; }

                        // Put this character in the render buffer:

                        PutBufferCharacter(c,buffer_chars,pos);

                        // Move onto the next character space

                        pos.X += m_char_spacing + m_metrics[c&0xff].tc_w * 16.0f * m_size;

                        // Added one more char to the rendering buffer

                        buffer_chars++;
                    }

                    //-----------------------------------------------------------------------------
                    // Draw the contents of the rendering buffer: 
                    //-----------------------------------------------------------------------------

                    // Only if there are characters to draw:

                    if ( buffer_chars > 0 )
                    {
                        // Begin drawing with the shader:

                        m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                            // Draw the text

                            Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                            ( 
                                PrimitiveType.TriangleList  , 
                                m_vertices                  , 
                                0                           , 
                                buffer_chars * 2 
                            );

                        // Finish up

                        m_effect.CurrentTechnique.Passes[0].End(); m_effect.End();
                    }

                    // Now finished: set string index to end of the string

                    i = text.Length;
                }
                else
                {
                    //---------------------------------------------------------
                    // Render buffer is not big enough to accomodate rest of string
                    //---------------------------------------------------------

                    // Set the number of characters put to the buffer as 0

                    buffer_chars = 0;

                    // Calculate the index where we will stop putting chars to the buffer in the string

                    int endIndex = i + RENDER_BUFFER_SIZE;

                    //---------------------------------------------------------
                    // Go through the remainder of the string:
                    //---------------------------------------------------------

                    for ( int j = i ; j < endIndex ; j++ )
                    {
                        // Get the character at this index:

                        char c = text[j];

                        // If the draw character is a carriage return or newline then move onto a new line:

                        if ( c == '\n' )
                        {
                            // Move onto a new line:

                            pos.Y -= m_size + m_line_spacing; pos.X = position.X; continue;
                        }

                        // If the draw character is a tab then move out to the next tab space:

                        if ( c == '\t' )
                        {
                            // Calculate x value of next tab space

                            float tab_x = (int)( ( pos.X - position.X ) / m_size * 4.0f ) * ( m_size * 4.0f ) + ( m_size * 4.0f );

                            // Set the new position to draw at

                            pos.X = tab_x; continue;
                        }

                        // If the draw character is a space then ignore:

                        if ( c == ' ' ){ pos.X += m_size * SPACE_WIDTH_PERCENT; continue; }

                        // Put this character in the render buffer:

                        PutBufferCharacter(c,buffer_chars,pos);

                        // Move onto the next character space

                        pos.X += m_line_spacing + m_metrics[c&0xff].tc_w * 16.0f * m_size;

                        // Added one more char to the rendering buffer

                        buffer_chars++;
                    }

                    //---------------------------------------------------------
                    // Draw the contents of the rendering buffer
                    //---------------------------------------------------------

                    // Only if there are characters to draw:

                    if ( buffer_chars > 0 )
                    {
                        // Begin drawing with the shader:
                        
                        m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                            // Draw the text

                            Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                            ( 
                                PrimitiveType.TriangleList  , 
                                m_vertices                  , 
                                0                           , 
                                buffer_chars * 2 
                            );

                        // End drawing with the shader:

                        m_effect.CurrentTechnique.Passes[0].End(); m_effect.End();
                    }

                    // Move on in the string by the render buffer size

                    i += RENDER_BUFFER_SIZE;

                }   // end else of if there is enough buffer space left to render remaining chars
            }
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a string centered horizontally over the given position with the given 
        /// transform matrix.
        /// </summary>
        /// 
        /// <param name="text">         text to draw                    </param>
        /// <param name="pos">          Position to draw the text at.   </param>
        /// <param name="transform">    transform to use for drawing    </param>
        //=====================================================================================

        public void DrawStringCentered ( string text, Vector2 pos , Matrix transform )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Current y offset for the text:

                float y_offset = 0;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coordinate to draw it at to center the string horizontally:

                    float x = pos.X - ( size.X * 0.5f );

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , transform );
                    
                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + m_size;
                }
            }
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a string right aligned to the given position with the given 
        /// transform matrix.
        /// </summary>
        /// 
        /// <param name="text">         text to draw                    </param>
        /// <param name="pos">          Position to draw the text at.   </param>
        /// <param name="transform">    transform to use for drawing    </param>
        //=====================================================================================

        public void DrawStringRightAligned( string text, Vector2 pos , Matrix transform )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Current y offset for the text:

                float y_offset = 0;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coordinate to draw it at to right align:

                    float x = pos.X - size.X;

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , transform );
                    
                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + m_size;
                }
            }
        }

        //=========================================================================================
        /// <summary> 
        /// Draws a string at the specified location with custom font settings.
        /// </summary>
        /// 
        /// <param name="text">      Text to draw                   </param>
        /// <param name="position">  Where to draw the text         </param>
        /// <param name="settings">  Custom font settings to use.   </param>
        //=========================================================================================

        public void DrawCustomString( string text , Vector2 position , CustomFontSettings settings )
        {
            // Create an orthographic projection matrix:

            Matrix projection = Matrix.CreateOrthographic
            (
                Core.Graphics.Device.Viewport.Width         ,
                Core.Graphics.Device.Viewport.Height        ,
                0                                           ,
                1.0f
            );

            // Save the current font settings:

            float       prev_size           = m_size;
            Vector4     prev_color          = m_color;
            float       prev_char_spacing   = m_char_spacing;
            float       prev_line_spacing   = m_line_spacing;
            Effect      prev_effect         = m_effect;

            // Apply new ones:

            m_size          = settings.Size;
            m_color         = settings.Color;
            m_char_spacing  = settings.CharSpacing;
            m_line_spacing  = settings.LineSpacing;
            m_effect        = settings.Effect;

            // Draw the given string using this matrix:

            DrawString( text , position , projection);

            // Restore old font settings:

            m_size          = prev_size;
            m_color         = prev_color;
            m_char_spacing  = prev_char_spacing;
            m_line_spacing  = prev_line_spacing;
            m_effect        = prev_effect;
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a custom formatted string centered horizontally over the given position
        /// </summary>
        /// 
        /// <param name="text">     Text to draw.                   </param>
        /// <param name="pos">      Position to draw the text at.   </param>
        /// <param name="settings"> Custom font settings to use.    </param>
        //=====================================================================================

        public void DrawCustomStringCentered( string text, Vector2 pos , CustomFontSettings settings )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Save the current font settings:

                float       prev_size           = m_size;
                Vector4     prev_color          = m_color;
                float       prev_char_spacing   = m_char_spacing;
                float       prev_line_spacing   = m_line_spacing;
                Effect      prev_effect         = m_effect;

                // Apply new ones:

                m_size          = settings.Size;
                m_color         = settings.Color;
                m_char_spacing  = settings.CharSpacing;
                m_line_spacing  = settings.LineSpacing;
                m_effect        = settings.Effect;

                // Current y offset for the text:

                float y_offset = 0;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coord to draw it at to center the string horizontally:

                    float x = pos.X - ( size.X * 0.5f );

                    // Create an orthographic projection matrix:

                    Matrix projection = Matrix.CreateOrthographic
                    (
                        Core.Graphics.Device.Viewport.Width         ,
                        Core.Graphics.Device.Viewport.Height        ,
                        0                                           ,
                        1.0f
                    );

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , projection );

                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + settings.Size;
                }

                // Restore old font settings:

                m_size          = prev_size;
                m_color         = prev_color;
                m_char_spacing  = prev_char_spacing;
                m_line_spacing  = prev_line_spacing;
                m_effect        = prev_effect;
            }
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a custom formatted string right aligned to the given position
        /// </summary>
        /// 
        /// <param name="text">     Text to draw.                   </param>
        /// <param name="pos">      Position to draw the text at.   </param>
        /// <param name="settings"> Custom font settings to use.    </param>
        //=====================================================================================

        public void DrawCustomStringRightAligned( string text, Vector2 pos , CustomFontSettings settings )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Save the current font settings:

                float       prev_size           = m_size;
                Vector4     prev_color          = m_color;
                float       prev_char_spacing   = m_char_spacing;
                float       prev_line_spacing   = m_line_spacing;
                Effect      prev_effect         = m_effect;

                // Apply new ones:

                m_size          = settings.Size;
                m_color         = settings.Color;
                m_char_spacing  = settings.CharSpacing;
                m_line_spacing  = settings.LineSpacing;
                m_effect        = settings.Effect;

                // Current y offset for the text:

                float y_offset = 0;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coord to draw it at to right align:

                    float x = pos.X - size.X;

                    // Create an orthographic projection matrix:

                    Matrix projection = Matrix.CreateOrthographic
                    (
                        Core.Graphics.Device.Viewport.Width         ,
                        Core.Graphics.Device.Viewport.Height        ,
                        0                                           ,
                        1.0f
                    );

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , projection );

                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + settings.Size;
                }

                // Restore old font settings:

                m_size          = prev_size;
                m_color         = prev_color;
                m_char_spacing  = prev_char_spacing;
                m_line_spacing  = prev_line_spacing;
                m_effect        = prev_effect;
            }
        }

        //=========================================================================================
        /// <summary> 
        /// Draws a string at the specified location with custom font settings and the given transform.
        /// </summary>
        /// 
        /// <param name="text">      Text to draw                   </param>
        /// <param name="position">  Where to draw the text         </param>
        /// <param name="transform"> transform matrix to use        </param>
        /// <param name="settings">  Custom font settings to use.   </param>
        //=========================================================================================

        public void DrawCustomString( string text , Vector2 position , Matrix transform  , CustomFontSettings settings )
        {
            // Save the current font settings:

            float       prev_size           = m_size;
            Vector4     prev_color          = m_color;
            float       prev_char_spacing   = m_char_spacing;
            float       prev_line_spacing   = m_line_spacing;
            Effect      prev_effect         = m_effect;

            // Apply new ones:

            m_size          = settings.Size;
            m_color         = settings.Color;
            m_char_spacing  = settings.CharSpacing;
            m_line_spacing  = settings.LineSpacing;
            m_effect        = settings.Effect;

            // Draw the given string using the given matrix:

            DrawString( text , position , transform );

            // Restore old font settings:

            m_size          = prev_size;
            m_color         = prev_color;
            m_char_spacing  = prev_char_spacing;
            m_line_spacing  = prev_line_spacing;
            m_effect        = prev_effect;
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a string centered horizontally at the specified position with the given 
        /// transform matrix and specified custom font settings.
        /// </summary>
        /// 
        /// <param name="text">         text to draw                    </param>
        /// <param name="box_pos">      position of box top left corner </param>
        /// <param name="box_size">     size of box                     </param>
        /// <param name="transform">    transform to use for drawing    </param>
        /// <param name="settings">     custom font settings to use     </param>
        //=====================================================================================

        public void DrawCustomStringCentered( string text, Vector2 pos , Matrix transform , CustomFontSettings settings )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Current y offset for the text:

                float y_offset = 0;

                // Save the current font settings:

                float       prev_size           = m_size;
                Vector4     prev_color          = m_color;
                float       prev_char_spacing   = m_char_spacing;
                float       prev_line_spacing   = m_line_spacing;
                Effect      prev_effect         = m_effect;

                // Apply new ones:

                m_size          = settings.Size;
                m_color         = settings.Color;
                m_char_spacing  = settings.CharSpacing;
                m_line_spacing  = settings.LineSpacing;
                m_effect        = settings.Effect;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coordinate to draw it at to center the string horizontally:

                    float x = pos.X - ( size.X * 0.5f );

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , transform );

                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + m_size;
                }

                // Restore old font settings:

                m_size          = prev_size;
                m_color         = prev_color;
                m_char_spacing  = prev_char_spacing;
                m_line_spacing  = prev_line_spacing;
                m_effect        = prev_effect;
            }
        }

        //=====================================================================================
        /// <summary> 
        /// Draws a string right aligned to the specified position with the given 
        /// transform matrix and specified custom font settings.
        /// </summary>
        /// 
        /// <param name="text">         text to draw                    </param>
        /// <param name="box_pos">      position of box top left corner </param>
        /// <param name="box_size">     size of box                     </param>
        /// <param name="transform">    transform to use for drawing    </param>
        /// <param name="settings">     custom font settings to use     </param>
        //=====================================================================================

        public void DrawCustomStringRightAligned( string text, Vector2 pos , Matrix transform , CustomFontSettings settings )
        {
            // Only do if everything is ok:

            if ( text != null && text.Length != 0 )
            {
                // Break up into lines:

                string[] lines = text.Split( new char[]{ '\n' } );

                // Current y offset for the text:

                float y_offset = 0;

                // Save the current font settings:

                float       prev_size           = m_size;
                Vector4     prev_color          = m_color;
                float       prev_char_spacing   = m_char_spacing;
                float       prev_line_spacing   = m_line_spacing;
                Effect      prev_effect         = m_effect;

                // Apply new ones:

                m_size          = settings.Size;
                m_color         = settings.Color;
                m_char_spacing  = settings.CharSpacing;
                m_line_spacing  = settings.LineSpacing;
                m_effect        = settings.Effect;

                // Draw each line:

                for ( int i = 0 ; i < lines.Length ; i++ )
                {
                    // Get the size of the string:

                    Vector2 size = GetStringSize( lines[i] );

                    // Now figure out what x coordinate to draw it to right align:

                    float x = pos.X - size.X;

                    // Now just use the regular draw string function to draw at figured out position

                    DrawString( lines[i] , new Vector2( x , pos.Y + y_offset ) , transform );

                    // Increment y offset for the next line:

                    y_offset -= m_line_spacing + m_size;
                }

                // Restore old font settings:

                m_size          = prev_size;
                m_color         = prev_color;
                m_char_spacing  = prev_char_spacing;
                m_line_spacing  = prev_line_spacing;
                m_effect        = prev_effect;
            }
        }

        //=====================================================================================
        /// <summary> 
        /// Gets the dimensions for a given string with the current font settings. 
        /// </summary>
        /// <returns> Total size that the string would take up when rendered. </returns>
        //=====================================================================================

        public Vector2 GetStringSize( string text )
        {
            // If the string is null then return zero:

            if ( text == null ) return Vector2.Zero;

            // Split the string into lines:

            string[] lines = text.Split( '\n' );

            // Store the maximum width of all the lines here:

            float max_w = 0;

            // Run through all the lines:

            for ( int i = 0 ; i < lines.Length ; i++ )
            {
                // Store the maximum width of this line here:

                float line_max_w = 0;

                // Run through all the characters in this line:

                for ( int j = 0 ; j < lines[i].Length ; j++ )
                {
                    // Get this character:

                    char c = lines[i][j];

                    // If the draw character is a tab then move out to the next tab space:

                    if ( c == '\t' )
                    {
                        // Calculate x value of next tab space

                        float tab_x = (int)( ( line_max_w ) / ( m_size * 4.0f ) ) * ( m_size * 4.0f ) + ( m_size * 4.0f );

                        // Set the new max width of the line:

                        line_max_w = tab_x; continue;
                    }

                    // If the draw character is a space then increment by a fixed size:

                    if ( c == ' ' ){ line_max_w += m_size * SPACE_WIDTH_PERCENT; continue; }

                    // Increase the max width of the string according to char metrics and font size / spacing 

                    line_max_w += m_metrics[c&0xff].tc_w * 16.0f * m_size;

                    // If this is the last character then do not apply char spacing:

                    if ( j < lines[i].Length - 1 ) line_max_w += m_char_spacing;
                }

                // If this line is longer than the current longest then save:

                if ( line_max_w > max_w ) max_w = line_max_w;
            }

            // Return the dimensions of the text:

            return new Vector2
            (
                max_w ,
                lines.Length * m_size + MathHelper.Clamp( lines.Length - 1 , 0 , 10000000 ) * m_line_spacing
            );
        }

        //=====================================================================================
        /// <summary> 
        /// Gets the dimensions for a given string with custom font settings. 
        /// </summary>
        /// <param name="text"> String to get dimensions of when rendered </param>
        /// <param name="settings"> Custom font settings to use for the calculations </param>
        /// <returns> Total size that the string would take up when rendered. </returns>
        //=====================================================================================

        public Vector2 GetCustomStringSize( string text , CustomFontSettings settings )
        {
            // If the string is null then return zero:

            if ( text == null ) return Vector2.Zero;

            // Split the string into lines:

            string[] lines = text.Split( '\n' );

            // Store the maximum width of all the lines here:

            float max_w = 0;

            // Run through all the lines:

            for ( int i = 0 ; i < lines.Length ; i++ )
            {
                // Store the maximum width of this line here:

                float line_max_w = 0;

                // Run through all the characters in this line:

                for ( int j = 0 ; j < lines[i].Length ; j++ )
                {
                    // Get this character:

                    char c = lines[i][j];

                    // If the draw character is a tab then move out to the next tab space:

                    if ( c == '\t' )
                    {
                        // Calculate x value of next tab space

                        float tab_x = (int)( ( line_max_w ) / ( settings.Size * 4.0f ) ) * ( settings.Size * 4.0f ) + ( settings.Size * 4.0f );

                        // Set the new max width of the line:

                        line_max_w = tab_x; continue;
                    }

                    // If the draw character is a space then increment by a fixed size:

                    if ( c == ' ' ){ line_max_w += settings.Size * SPACE_WIDTH_PERCENT; continue; }

                    // Increase the max width of the string according to char metrics and font size / spacing 

                    line_max_w += m_metrics[c&0xff].tc_w * 16.0f * settings.Size;

                    // If this is the last character then do not apply char spacing:

                    if ( j < lines[i].Length - 1 ) line_max_w += settings.CharSpacing;
                }

                // If this line is longer than the current longest then save:

                if ( line_max_w > max_w ) max_w = line_max_w;
            }

            // Return the dimensions of the text:

            return new Vector2
            (
                max_w ,
                lines.Length * settings.Size + MathHelper.Clamp( lines.Length - 1 , 0 , 10000000 ) * settings.LineSpacing
            );
        }

    }   // end of class BitmapFont

}   // end of namespace TerminalVelocity

