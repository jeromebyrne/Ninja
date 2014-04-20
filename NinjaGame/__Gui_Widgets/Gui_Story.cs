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
    /// Widget which shows pictures and text to tell a story. Can fade through multiple pictures 
    /// and text. 
    /// </summary>
    //#############################################################################################

    public class Gui_Story : GuiWidget
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Font used by the widget to draw text with. </summary>

            private Font m_font = null;

            /// <summary> Effect used to draw the picture. </summary>

            private Effect m_effect = null;
            
            /// <summary> Size of the picture shown by the widget. </summary>

            private Vector2 m_picture_size = Vector2.One;

            /// <summary> Offset of the string from the center of the picture. </summary>

            private float m_string_offset_y = 0;

            /// <summary> Time it takes for a story part to fade in or out. </summary>

            private float m_fade_time = 0;

            /// <summary> Time a story part displays for. </summary>

            private float m_display_time = 0;

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            private string m_font_name = "";

            /// <summary> The number of pictures and accompanying text lines. </summary>

            private int m_story_part_count = 0;

            /// <summary> Strings for each part of the story. </summary>

            private string[] m_story_strings = null;

            /// <summary> Textures used for each picture in the story. </summary>

            private Texture2D[] m_pictures = null;

            /// <summary> Name of the textures used for each picture in the story. </summary>

            private string[] m_picture_names = null;

            /// <summary> Vertices used for drawing the picture part. </summary>

            private VertexPositionColorTexture[] m_vertices = new VertexPositionColorTexture[4];

            /// <summary> What state of transistion the widget is in. </summary>

            private TransitionState m_transition_state = TransitionState.SHOWING;

            /// <summary> Amount of time the widget has been in it's current state. </summary>

            private float m_current_state_time = 0;

            /// <summary> Current story part the widget is on. </summary>

            private int m_current_story_part = 0;

        //=========================================================================================
        // Enums
        //=========================================================================================

            /// <summary> What state of transistion the story widget is in. </summary>

            private enum TransitionState
            {
                /// <summary> Story part is fading out. </summary>

                FADING_OUT ,

                /// <summary> Story part is showing normally. </summary>

                SHOWING
            };

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates this widget.
        /// </summary>
        //=========================================================================================

        public Gui_Story()
        {
            // Load effect

            m_effect = Core.Graphics.LoadEffect("Effects\\dual_textured");

            // Setup vertices:

            m_vertices[0].Position.Z = -2;
            m_vertices[1].Position.Z = -2;
            m_vertices[2].Position.Z = -2;
            m_vertices[3].Position.Z = -2;

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

            data.ReadString ( "Font"            , ref m_font_name           , "Content\\Fonts\\Game_48px.xml"   );
            data.ReadFloat  ( "PictureSizeX"    , ref m_picture_size.X      , 128                               );
            data.ReadFloat  ( "PictureSizeY"    , ref m_picture_size.Y      , 128                               );
            data.ReadFloat  ( "StringOffsetY"   , ref m_string_offset_y     , 0                                 );
            data.ReadFloat  ( "FadeTime"        , ref m_fade_time           , 0.25f                             );
            data.ReadFloat  ( "DisplayTime"     , ref m_display_time        , 2                                 );
            data.ReadInt    ( "StoryPartCount"  , ref m_story_part_count    , 1                                 );

            // Read story data:

            if ( m_story_part_count > 0 )
            {
                // Declare arrays for all the story pictures and strings:

                m_pictures      = new Texture2D [ m_story_part_count ];
                m_picture_names = new string    [ m_story_part_count ];
                m_story_strings = new string    [ m_story_part_count ];

                // Read each part of the story: the string and texture

                for ( int i = 0 ; i < m_story_part_count ; i++ )
                {
                    // Story part texture

                    data.ReadTexture
                    ( 
                        "StoryPicture" + (i+1).ToString()   , 
                        ref m_picture_names[i]              , 
                        ref m_pictures[i]                   , 
                        "Graphics\\default" 
                    );

                    // Story part string

                    data.ReadString
                    ( 
                        "StoryString" + (i+1).ToString()    , 
                        ref m_story_strings[i]              ,
                        "No_String" 
                    );
                }
            }
            else
            {
                // No story:

                m_pictures      = null;
                m_picture_names = null;
                m_story_strings = null;
            }

            // Read in font:

            m_font = new Font(m_font_name);

            // Fading in to current part:

            m_transition_state      = TransitionState.SHOWING;
            m_current_state_time    = 0;
            m_current_story_part    = 0;
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
           
            data.Write( "Font"            , m_font_name         );
            data.Write( "PictureSizeX"    , m_picture_size.X    );
            data.Write( "PictureSizeY"    , m_picture_size.Y    );
            data.Write( "StringOffsetY"   , m_string_offset_y   );
            data.Write( "FadeTime"        , m_fade_time         );
            data.Write( "DisplayTime"     , m_display_time      );
            data.Write( "StoryPartCount"  , m_story_part_count  );

            // Write all story part data:

            if ( m_story_part_count > 0 )
            {
                // Read each part of the story: the string and texture

                for ( int i = 0 ; i < m_story_part_count ; i++ )
                {
                    // Story part texture

                    data.Write( "StoryPicture" + (i+1).ToString(Locale.DevelopmentCulture.NumberFormat) , m_picture_names[i] );

                    // Story part string

                    data.Write( "StoryString" + (i+1).ToString(Locale.DevelopmentCulture.NumberFormat) , m_story_strings[i] );
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Updates the story widget, changing widget states as neccessary.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Call base function

            base.OnUpdate();

            // Increase current state time:

            m_current_state_time += Core.Timing.ElapsedTime;

            // Do nothing if no story parts:

            if ( m_story_part_count <= 0 ) return;

            // See what state we are in:

            switch ( m_transition_state )
            {
                // Displaying the story part:

                case TransitionState.SHOWING:
                {
                    // See if time to start fading out:

                    if ( m_current_state_time >= m_display_time )
                    {
                        // Begin the change: but only if we are not on the last frame:

                        if ( m_current_story_part < m_story_part_count - 1 )
                        {
                            // Switch state

                            m_transition_state = TransitionState.FADING_OUT;

                            // Reset state time:

                            m_current_state_time = 0;
                        }
                    }

                }   break;


                // Fading out:

                case TransitionState.FADING_OUT:
                {
                    // See if state time is up:

                    if ( m_current_state_time >= m_fade_time )
                    {
                        // Switch to next story part:

                        m_current_story_part++;

                        // If past the end then clamp:

                        if ( m_current_story_part >= m_story_part_count )
                        {
                            // Past the end: clamp

                            m_current_story_part = m_story_part_count - 1;
                        }

                        // Switch to showing:

                        m_transition_state = TransitionState.SHOWING;

                        // Reset state time:

                        m_current_state_time = 0;
                    }

                }   break;

            }
        }

        //=========================================================================================
        /// <summary>
        /// Draws the story widget.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // If there are no story parts then abort:

            if ( m_story_part_count <= 0 ) return;

            // Make sure story part is right:

            if ( m_current_story_part >= m_story_part_count || m_current_story_part < 0 ) return;
            
            // Figure out the alpha for the story part from transition state:

            float alpha = 1;

            // See if fading out:

            if ( m_transition_state == TransitionState.FADING_OUT )
            {
                // Figure out alpha:

                alpha = 1.0f - ( m_current_state_time / m_fade_time );
            }

            // Make sure alpha is in range:

            if ( alpha < 0 ) alpha = 0;
            if ( alpha > 1 ) alpha = 1;

            //-------------------------------------------------------------------------------------
            // Draw the text for the current story part:
            //-------------------------------------------------------------------------------------

            // Only do if we have a font:

            if ( m_font != null )
            {
                // Get view projection matrix for the GUI camera:

                Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

                // Make up a custom font settings block:

                Font.CustomFontSettings settings;

                settings.CharSpacing    = m_font.CharSpacing;
                settings.Color          = m_font.Color;
                settings.Effect         = m_font.Effect;
                settings.LineSpacing    = m_font.LineSpacing;
                settings.Size           = m_font.Size;

                // Set the alpha for the custom settings:

                settings.Color.W = alpha;

                // Draw the current story part string:

                m_font.DrawCustomStringCentered
                (
                    StringDatabase.GetString( m_story_strings[m_current_story_part] )   ,
                    Position + Vector2.UnitY * m_string_offset_y                        ,
                    view_projection                                                     ,
                    settings                                                            
                );
            }

            //-------------------------------------------------------------------------------------
            // Draw a blend between the current and next story pictures (if any)
            //-------------------------------------------------------------------------------------

            if ( m_transition_state == TransitionState.FADING_OUT )
            {
                // See if there is a next picture:

                if ( m_current_story_part < m_story_part_count - 1 )
                {
                    // Cool: blend into the next pic:

                    DrawStoryPartPictures
                    (
                        m_pictures[ m_current_story_part     ]  ,
                        m_pictures[ m_current_story_part + 1 ]  ,
                        1.0f - alpha
                    );
                }
                else
                {
                    // No next pic: do no blend

                    DrawStoryPartPictures
                    (
                        m_pictures[ m_current_story_part    ] ,
                        m_pictures[ m_current_story_part    ] ,
                        0
                    );
                }
            }
            else
            {
                // Not fading: do no blend

                DrawStoryPartPictures
                (
                    m_pictures[ m_current_story_part    ] ,
                    m_pictures[ m_current_story_part    ] ,
                    0
                );
            }
        }

        //=========================================================================================
        /// <summary>
        /// Draws a mix between two story part pictures. 
        /// </summary>
        /// <param name="texture1"> 1st Texture to draw with. </param>
        /// <param name="texture2"> 2nd Texture to draw with. </param>
        /// <param name="mix"> mix amount between the two textures, from 0-1. 0 is full
        /// texture1, 1 is full texture2. 0.5 is a halfway mix.</param>
        //=========================================================================================

        private void DrawStoryPartPictures( Texture2D texture1 , Texture2D texture2 , float mix )
        {
            // Abort if no textures or effect:

            if ( m_effect == null || texture1 == null || texture2 == null ) return;

            // Get view projection matrix for the GUI camera:

            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Setup the vertices:

            m_vertices[0].Position.X = - m_picture_size.X;
            m_vertices[0].Position.Y = + m_picture_size.Y;
            m_vertices[1].Position.X = - m_picture_size.X;
            m_vertices[1].Position.Y = - m_picture_size.Y;
            m_vertices[2].Position.X = + m_picture_size.X;
            m_vertices[2].Position.Y = + m_picture_size.Y;
            m_vertices[3].Position.X = + m_picture_size.X;
            m_vertices[3].Position.Y = - m_picture_size.Y;

            m_vertices[0].Position.X += Position.X;
            m_vertices[1].Position.X += Position.X;
            m_vertices[2].Position.X += Position.X;
            m_vertices[3].Position.X += Position.X;

            m_vertices[0].Position.Y += Position.Y;
            m_vertices[1].Position.Y += Position.Y;
            m_vertices[2].Position.Y += Position.Y;
            m_vertices[3].Position.Y += Position.Y;           

            // Set vertex declaration:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

            // Set shader textures:
           
            {
                // Get the parameter:

                EffectParameter param = m_effect.Parameters["Texture1"];

                // Set:

                if ( param != null )  param.SetValue( texture1 );
            }

            {
                // Get the parameter:

                EffectParameter param = m_effect.Parameters["Texture2"];

                // Set:

                if ( param != null )  param.SetValue( texture2 );
            }


            // Set shader texture blend:
           
            {
                // Get the parameter:

                EffectParameter param = m_effect.Parameters["TextureMix"];

                // Set:

                if ( param != null )  param.SetValue( mix );
            }

            // Set shader transform:

            {
                // Get the parameter:

                EffectParameter param = m_effect.Parameters["WorldViewProjection"];

                // Set:

                if ( param != null ) 
                {
                    param.SetValue(view_projection);
                }
            }

            // Begin drawing the picture:

            m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                // Draw:

                Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                (
                    PrimitiveType.TriangleStrip     ,
                    m_vertices                      ,
                    0                               ,
                    2
                );

            // End drawing the picture:

            m_effect.End(); m_effect.CurrentTechnique.Passes[0].End();
        }

    }   // end of class

}   // end of namespace
