using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Slider widget that can be used to set some value using a sliding scale. 
    /// </summary>
    //#############################################################################################

    public class Gui_Slider : GuiWidget
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Get/sets the minimum value of the bar. </summary>

            public float MinimumValue
            {
                get 
                {
                    return m_minimum_value;
                }

                set
                {
                    // Set the value:

                    m_minimum_value = value;

                    // If the minimum value is bigger the max then swap:

                    if ( m_minimum_value > m_maximum_value )
                    {
                        float t = m_minimum_value; m_minimum_value = m_maximum_value; m_maximum_value = t;
                    }

                    // Ensure current value is in range:

                    m_current_value = MathHelper.Clamp( m_current_value , m_minimum_value , m_maximum_value );
                }
            }

            /// <summary> Get/sets the maximum value of the bar. </summary>

            public float MaximumValue
            {
                get 
                {
                    return m_maximum_value;
                }

                set
                {
                    // Set the value:

                    m_maximum_value = value;

                    // If the minimum value is bigger the max then swap:

                    if ( m_minimum_value > m_maximum_value )
                    {
                        float t = m_minimum_value; m_minimum_value = m_maximum_value; m_maximum_value = t;
                    }

                    // Ensure current value is in range:

                    m_current_value = MathHelper.Clamp( m_current_value , m_minimum_value , m_maximum_value );
                }
            }

            /// <summary> Gets/sets the current vaue of the bar. </summary>

            public float CurrentValue
            {
                get 
                {
                    return m_current_value;
                }

                set
                {
                    // Set the value:

                    m_current_value = value;

                    // Ensure current value is in range:

                    m_current_value = MathHelper.Clamp( m_current_value , m_minimum_value , m_maximum_value );
                }
            }          

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Effect used to render the slider. </summary>

            private Effect m_effect = null;

            /// <summary> Name of the effect used to render the slider. </summary>

            private string m_effect_name = "";

            /// <summary> Wobble effect used to render the slider string. </summary>

            private Effect m_string_wobble_effect = null;

            /// <summary> Name of the wobble effect used to render the slider string. </summary>

            private string m_string_wobble_effect_name = "";

            /// <summary> Texture to use for the fill of the slider bar. </summary>

            private Texture2D m_fill_texture = null;

            /// <summary> Name of the texture to use for the fill of the slider bar. </summary>

            private string m_fill_texture_name = "";

            /// <summary> Texture for the bar underneath the fill </summary>

            private Texture2D m_bar_texture = null;

            /// <summary> Name of the texture for the bar underneath the fill. </summary>

            private string m_bar_texture_name = "";

            /// <summary> Font used by the widget to draw text with. </summary>

            private Font m_font = null;

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            private string m_font_name = "";

            /// <summary> Name of the string from the database to display in the widget. </summary>

            private string m_string_name = "";

            /// <summary> Y Offset of the slider string from the slider center </summary>

            private float m_string_offset_y = 0;

            /// <summary> Minimum value of the bar. </summary>

            private float m_minimum_value = 0;

            /// <summary> Maximum value of the bar. </summary>

            private float m_maximum_value = 0;

            /// <summary> Current value of the bar. </summary>

            private float m_current_value = 0;

            /// <summary> Speed that the bar's value changes at. </summary>

            private float m_change_speed = 0;

            /// <summary> Size of the bar. </summary>

            private Vector2 m_size = Vector2.Zero;

            /// <summary> Amount of padding for the fill inside the bar </summary>

            private Vector2 m_fill_padding = Vector2.Zero;

            /// <summary> Vertices used to render the bar. </summary>

            private VertexPositionColorTexture[] m_vertices = new VertexPositionColorTexture[12];

            /// <summary> Time it takes for the slider to expand / contract in font size when loosing / gaining focus. </summary>

            private float m_expand_time = 1.0f;

            /// <summary> Maximum amount of expansion for the slider. </summary>

            private Vector2 m_expansion = Vector2.Zero;

            /// <summary> Maximum amount of expansion for the slider string. </summary>

            private float m_string_expansion = 4.0f;

            /// <summary> Current percent of expansion for the slider from 0-1. </summary>

            private float m_current_expansion_pc = 0.0f;

            /// <summary> Current time value for the wobble shader on the text. </summary>

            private float m_highlight_wobble_time = 0;

            /// <summary> Speed of the wobble effect when highlighted on the text. </summary>

            private float m_highlight_wobble_speed = 0.0001f;

            /// <summary> Intensity of the wobble effect when highlighted on the text. </summary>

            private float m_highlight_wobble_intensity = 4.0f;

            /// <summary> Sound to play when the slider gains focus. </summary>

            private string m_focus_sound = "";

            /// <summary> Sound to play when the button is used. </summary>

            private string m_use_sound = "";

            /// <summary> Generic gui event to invoke when the slider value is changed </summary>

            private string m_value_changed_event = "";

            /// <summary> Name of the GuiEventFunction type function to call when the button to go back to the previous screen is pressed. </summary>

            private string m_back_event = "";


        //=========================================================================================
        /// <summary>
        /// Constructor. Creates this widget.
        /// </summary>
        //=========================================================================================

        public Gui_Slider()
        {
            // This widget can be focused:

            CanFocus = true;
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

            data.ReadEffect ( "Effect"              , ref m_effect_name                 , ref m_effect                  , "Effects\\textured"   );
            data.ReadEffect ( "StringWobbleEffect"  , ref m_string_wobble_effect_name   , ref m_string_wobble_effect    , "Effects\\textured"   );
            data.ReadTexture( "FillTexture"         , ref m_fill_texture_name           , ref m_fill_texture            , "Graphics\\default"   );
            data.ReadTexture( "BarTexture"          , ref m_bar_texture_name            , ref m_bar_texture             , "Graphics\\default"   );
            
            data.ReadString ( "Font"                        , ref m_font_name                   , "Content\\Fonts\\Game_16px.xml"   );
            data.ReadString ( "StringName"                  , ref m_string_name                 , "No_String"                       );
            data.ReadFloat  ( "StringOffsetY"               , ref m_string_offset_y             , 32                                );
            data.ReadFloat  ( "FillPaddingX"                , ref m_fill_padding.X              , 0                                 );
            data.ReadFloat  ( "FillPaddingY"                , ref m_fill_padding.Y              , 0                                 );
            data.ReadFloat  ( "MinimumValue"                , ref m_minimum_value               , 0.0f                              );
            data.ReadFloat  ( "MaximumValue"                , ref m_maximum_value               , 1.0f                              );
            data.ReadFloat  ( "CurrentValue"                , ref m_current_value               , 0.5f                              );
            data.ReadFloat  ( "ChangeSpeed"                 , ref m_change_speed                , 1.0f                              );
            data.ReadFloat  ( "SizeX"                       , ref m_size.X                      , 128                               );
            data.ReadFloat  ( "SizeY"                       , ref m_size.Y                      , 32                                );
            data.ReadFloat  ( "ExpandTime"                  , ref m_expand_time                 , 1                                 );
            data.ReadFloat  ( "ExpansionX"                  , ref m_expansion.X                 , 16                                );
            data.ReadFloat  ( "ExpansionY"                  , ref m_expansion.Y                 , 4                                 );
            data.ReadFloat  ( "StringExpansion"             , ref m_string_expansion            , 0                                 );
            data.ReadFloat  ( "HighlightWobbleSpeed"        , ref m_highlight_wobble_speed      , 0.01f                             );
            data.ReadFloat  ( "HighlightWobbleIntensity"    , ref m_highlight_wobble_intensity  , 8                                 );
            data.ReadString ( "ValueChangedEvent"           , ref m_value_changed_event         , ""                                );
            data.ReadString ( "BackEvent"                   , ref m_back_event                  , ""                                );
            data.ReadString ( "FocusSound"                  , ref m_focus_sound                 , ""                                );
            data.ReadString ( "UseSound"                    , ref m_use_sound                   , ""                                );
            

            // If the minimum value is bigger the max then swap:

            if ( m_minimum_value > m_maximum_value )
            {
                float t = m_minimum_value; m_minimum_value = m_maximum_value; m_maximum_value = t;
            }

            // Fill in the color and texture coordinates of the vertices for rendering:

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;
            m_vertices[4].Color = Color.White;
            m_vertices[5].Color = Color.White;

            m_vertices[0].TextureCoordinate = Vector2.Zero;
            m_vertices[1].TextureCoordinate = Vector2.UnitY;
            m_vertices[2].TextureCoordinate = Vector2.UnitX;
            m_vertices[3].TextureCoordinate = Vector2.One;
            m_vertices[4].TextureCoordinate = Vector2.Zero;
            m_vertices[5].TextureCoordinate = Vector2.UnitY;

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

            data.Write( "Effect"                    , m_effect_name                 );
            data.Write( "StringWobbleEffect"        , m_string_wobble_effect_name   );
            data.Write( "FillTexture"               , m_fill_texture_name           );
            data.Write( "BarTexture"                , m_bar_texture_name            );
            data.Write( "Font"                      , m_font_name                   );
            data.Write( "StringName"                , m_string_name                 );
            data.Write( "StringOffsetY"             , m_string_offset_y             );
            data.Write( "FillPaddingX"              , m_fill_padding.X              );
            data.Write( "FillPaddingY"              , m_fill_padding.Y              );
            data.Write( "MinimumValue"              , m_minimum_value               );
            data.Write( "MaximumValue"              , m_maximum_value               );
            data.Write( "CurrentValue"              , m_current_value               );
            data.Write( "ChangeSpeed"               , m_change_speed                );
            data.Write( "SizeX"                     , m_size.X                      );
            data.Write( "SizeY"                     , m_size.Y                      );
            data.Write( "ExpandTime"                , m_expand_time                 );
            data.Write( "ExpansionX"                , m_expansion.X                 );
            data.Write( "ExpansionY"                , m_expansion.Y                 );
            data.Write( "StringExpansion"           , m_string_expansion            );
            data.Write( "HighlightWobbleSpeed"      , m_highlight_wobble_speed      );
            data.Write( "HighlightWobbleIntensity"  , m_highlight_wobble_intensity  );
            data.Write( "ValueChangedEvent"         , m_value_changed_event         );
            data.Write( "BackEvent"                 , m_back_event                  );
            data.Write( "FocusSound"                , m_focus_sound                 );
            data.Write( "UseSound"                  , m_use_sound                   );            
        }

        //=========================================================================================
        /// <summary> 
        /// Update function for the widget. Expands / contracts the text if in or out of focus.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Call base function:

            base.OnUpdate();

            // See if we are in focus:

            if ( ParentGui != null && ParentGui.FocusWidget == this )
            {
                // Expand: figure out rate of expansion for this frame

                float expansion = Core.Timing.ElapsedTime * ( 1.0f / m_expand_time );

                // Expand:

                m_current_expansion_pc += expansion;

                // If past the limit then stop:

                m_current_expansion_pc = MathHelper.Clamp( m_current_expansion_pc , 0 , 1 );
            }
            else
            {
                // Contract: figure out rate of contraction for this frame

                float contraction = Core.Timing.ElapsedTime * ( 1.0f / m_expand_time );

                // Contract:

                m_current_expansion_pc -= contraction;

                // If past the limit then stop:

                m_current_expansion_pc = MathHelper.Clamp( m_current_expansion_pc , 0 , 1 );
            }

            // Slider movement: but only if in focus

            if ( ParentGui.FocusWidget == this )
            {
                // Get keyboard and gamepad state:

                GamePadState gps = GamePad.GetState(PlayerIndex.One); KeyboardState kbs = Keyboard.GetState();

                // Get the current time delta:

                float t = Core.Timing.ElapsedTime;

                // Save the old value of the slider:

                float old_value = m_current_value;

                // Do slider movement:

                if ( kbs.IsKeyDown( Keys.Left   ) ) m_current_value -= t * m_change_speed;
                if ( kbs.IsKeyDown( Keys.Right  ) ) m_current_value += t * m_change_speed;

                if ( gps.IsButtonDown( Buttons.DPadLeft ) ) m_current_value -= t * m_change_speed;
                if ( gps.IsButtonDown( Buttons.DPadRight) ) m_current_value += t * m_change_speed;

                m_current_value += t * gps.ThumbSticks.Left.X  * m_change_speed;
                m_current_value += t * gps.ThumbSticks.Right.X * m_change_speed;

                // Clamp slider value to within range:

                m_current_value = MathHelper.Clamp( m_current_value , m_minimum_value , m_maximum_value );

                // See if the value has changed:

                if ( m_current_value != old_value )
                {
                    // Value change: see if event for this:

                    if ( m_value_changed_event != null && m_value_changed_event.Length > 0 )
                    {
                        GuiEvents.InvokeGenericEvent(m_value_changed_event,this);
                    }
                }
            }

            // Increase the highlight wobble time:

            m_highlight_wobble_time += Core.Timing.ElapsedTime;
        }

        //=========================================================================================
        /// <summary> 
        /// Draws the text for the button.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            //-------------------------------------------------------------------------------------
            // Abort if we have no effect:
            //-------------------------------------------------------------------------------------

            if ( m_effect == null || m_fill_texture == null ) return;

            //-------------------------------------------------------------------------------------
            // GD Setup
            //-------------------------------------------------------------------------------------
            
            // Set vertex declaration:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Set the transform on the shader:

            {
                // Get the parameter:

                EffectParameter param_wvp = m_effect.Parameters[ "WorldViewProjection" ];

                // Set it's value:

                if ( param_wvp != null ) param_wvp.SetValue( view_projection );
            }

            //-------------------------------------------------------------------------------------
            // Figure out the expanded size of the bar:
            //-------------------------------------------------------------------------------------

            Vector2 s = Vector2.Zero;

            s.X = m_size.X + m_expansion.X * m_current_expansion_pc;
            s.Y = m_size.Y + m_expansion.Y * m_current_expansion_pc;

            //-------------------------------------------------------------------------------------
            // Draw the middle of the bar:
            //-------------------------------------------------------------------------------------

            // Only do if we have the texture:

            if ( m_bar_texture != null )
            {
                // Fill in the vertices for the underneath of the bar:

                    m_vertices[0].Position = new Vector3( Position.X - s.X  , Position.Y + s.Y , -2 );
                    m_vertices[1].Position = new Vector3( Position.X - s.X  , Position.Y - s.Y , -2 );
                    m_vertices[2].Position = new Vector3( Position.X        , Position.Y + s.Y , -2 );
                    m_vertices[3].Position = new Vector3( Position.X        , Position.Y - s.Y , -2 );
                    m_vertices[4].Position = new Vector3( Position.X + s.X  , Position.Y + s.Y , -2 ); 
                    m_vertices[5].Position = new Vector3( Position.X + s.X  , Position.Y - s.Y , -2 );

                // Set the texture on the shader:

                EffectParameter param_tex = m_effect.Parameters[ "Texture" ];            

                if ( param_tex != null ) 
                {
                    param_tex.SetValue( m_bar_texture );
                }
            
                // Begin drawing with shader:

                m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the bar:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleStrip     ,
                        m_vertices                      ,
                        0                               ,
                        4
                    );

                // End drawing with shader:

                m_effect.CurrentTechnique.Passes[0].End(); m_effect.End();
            }

            //-------------------------------------------------------------------------------------
            // Figure out how much padding to apply to the bar:
            //-------------------------------------------------------------------------------------

            float xp = m_fill_padding.X * ( 1.0f + ( m_current_expansion_pc * m_expansion.X ) / m_size.X );
            float yp = m_fill_padding.Y * ( 1.0f + ( m_current_expansion_pc * m_expansion.Y ) / m_size.Y );

            //-------------------------------------------------------------------------------------
            // Figure out the width of the bar fill:
            //-------------------------------------------------------------------------------------

            // Get fully filled width of the bar:

            float fill_s = s.X * 2 - xp * 2;
            float fill_w = fill_s * (( m_current_value - m_minimum_value ) / ( m_maximum_value - m_minimum_value ));

            //-------------------------------------------------------------------------------------
            // Now set the positions of the vertices for the fill:
            //-------------------------------------------------------------------------------------

            m_vertices[0].Position = new Vector3( Position.X - s.X + xp             , Position.Y + s.Y - yp , -2 );
            m_vertices[1].Position = new Vector3( Position.X - s.X + xp             , Position.Y - s.Y + yp , -2 );
            m_vertices[2].Position = new Vector3( Position.X - s.X + xp + fill_w    , Position.Y + s.Y - yp , -2 );
            m_vertices[3].Position = new Vector3( Position.X - s.X + xp + fill_w    , Position.Y - s.Y + yp , -2 );

            //-------------------------------------------------------------------------------------
            // Draw the fill:
            //-------------------------------------------------------------------------------------

            if ( m_fill_texture != null )
            {
                // Set the texture on the shader:

                EffectParameter param_tex = m_effect.Parameters[ "Texture" ];            

                if ( param_tex != null ) 
                {
                    param_tex.SetValue( m_fill_texture );
                }
            
                // Begin drawing with shader:

                m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the bar:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleStrip     ,
                        m_vertices                      ,
                        0                               ,
                        2
                    );

                // End drawing with shader:

                m_effect.CurrentTechnique.Passes[0].End(); m_effect.End();
            }

            //-------------------------------------------------------------------------------------
            // Draw the font string:
            //-------------------------------------------------------------------------------------

            if ( m_font != null && m_string_wobble_effect != null )
            {
                // Set the wobble shader settings:

                    // Time:
                
                    {
                        EffectParameter p = m_string_wobble_effect.Parameters["WobbleTime"];

                        if ( p != null )
                        {
                            p.SetValue( m_highlight_wobble_time );
                        }
                    }

                    // Intensity: set to zero if not in focus

                    {
                        EffectParameter p = m_string_wobble_effect.Parameters["WobbleIntensity"];

                        if ( p != null )
                        {
                            if ( InFocus )
                            {       
                                p.SetValue( m_highlight_wobble_intensity );
                            }
                            else
                            {
                                p.SetValue( 0 );
                            }
                        }
                    }

                    // Intensity: fade as we contract

                    {
                        EffectParameter p = m_string_wobble_effect.Parameters["WobbleSpeed"];

                        if ( p != null )
                        {
                            p.SetValue( m_highlight_wobble_speed );
                        }
                    }

                // Make up a custom font settings block:

                Font.CustomFontSettings custom_settings = new Font.CustomFontSettings
                (
                
                    m_font.Size + m_expansion.Y * m_current_expansion_pc    ,
                    m_font.Color                                            ,
                    m_font.CharSpacing                                      ,
                    m_font.LineSpacing                                      ,
                    m_string_wobble_effect
                );

                // Draw the string
            
                m_font.DrawCustomStringCentered
                ( 
                    StringDatabase.GetString(m_string_name)         , 
                    Position + Vector2.UnitY * m_string_offset_y    ,
                    view_projection                                 ,
                    custom_settings
                );
            }
        }

        //=========================================================================================
        /// <summary>
        /// Focus gained event for the button. Plays the focus gained sound.
        /// </summary>
        //=========================================================================================

        public override void OnFocusGained()
        {
            // Call base function

            base.OnFocusGained();

            // Play the focus sound:

            Core.Audio.Play( m_focus_sound );
        }

        //=========================================================================================
        /// <summary>
        /// On keyboard pressed event for the widget.
        /// </summary>
        /// <param name="key"> Key just pressed. </param>
        //=========================================================================================

        public override void OnKeyboardPressed( Keys key )
        {
            // Call events if the right buttons are pressed:

            switch ( key )
            {
                case Keys.Escape:   GuiEvents.InvokeGenericEvent(m_back_event,this); Core.Audio.Play(m_use_sound); break;
                case Keys.Back:     GuiEvents.InvokeGenericEvent(m_back_event,this); Core.Audio.Play(m_use_sound); break;
            }
        }

        //=========================================================================================
        /// <summary>
        /// On gamepad pressed event for the widget.
        /// </summary>
        /// <param name="button"> Button just pressed. </param>
        //=========================================================================================

        public override void OnGamepadPressed( Buttons button )
        {
            // Call events if the right buttons are pressed:

            switch ( button )
            {
                case Buttons.B: GuiEvents.InvokeGenericEvent(m_back_event,this); Core.Audio.Play(m_use_sound); break;
            }
        }

    }   // end of class

}   // end of namespace
