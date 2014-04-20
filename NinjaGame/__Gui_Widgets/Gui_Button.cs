using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame.__Gui_Widgets
{
    //#############################################################################################
    /// <summary>
    /// Button widget that performs an action when the user 
    /// </summary>
    //#############################################################################################

    public class Gui_Button : Gui_Text
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Name of the GuiEventFunction type function to call then the button is clicked. </summary>

            private string m_use_event = "";

            /// <summary> Name of the GuiEventFunction type function to call when the button to go back to the previous screen is pressed. </summary>

            private string m_back_event = "";

            /// <summary> Time it takes for the button to expand / contract in font size when loosing / gaining focus. </summary>

            private float m_expand_time = 1.0f;

            /// <summary> Maximum amount of expansion for the button font size. </summary>

            private float m_expansion = 4.0f;

            /// <summary> Current amount of expansion for the button font size. </summary>

            private float m_current_expansion = 1.0f;

            /// <summary> Shader used to cause text to wobble and distort slightly when highlighted. </summary>

            private Effect m_highlight_wobble_effect = null;

            /// <summary> Current time value for the wobble shader. </summary>

            private float m_highlight_wobble_time = 0;

            /// <summary> Speed of the wobble effect when highlighted. </summary>

            private float m_highlight_wobble_speed = 0.0001f;

            /// <summary> Intensity of the wobble effect when highlighted. </summary>

            private float m_highlight_wobble_intensity = 4.0f;

            /// <summary> Sound to play when the button gains focus. </summary>

            private string m_focus_sound = "";

            /// <summary> Sound to play when the button is used. </summary>

            private string m_use_sound = "";

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates this widget.
        /// </summary>
        //=========================================================================================

        public Gui_Button()
        {
            // This widget can be focused:

            CanFocus = true;

            // Load the wobble shader:

            m_highlight_wobble_effect = Core.Graphics.LoadEffect("Effects\\textured_wobble");
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

            data.ReadString ( "UseEvent"                    , ref m_use_event                   , ""        );
            data.ReadString ( "BackEvent"                   , ref m_back_event                  , ""        );
            data.ReadFloat  ( "ExpandTime"                  , ref m_expand_time                 , 1         );
            data.ReadFloat  ( "Expansion"                   , ref m_expansion                   , 4         );
            data.ReadFloat  ( "HighlightWobbleSpeed"        , ref m_highlight_wobble_speed      , 0.01f     );
            data.ReadFloat  ( "HighlightWobbleIntensity"    , ref m_highlight_wobble_intensity  , 8         );
            data.ReadString ( "FocusSound"                  , ref m_focus_sound                 , ""        );
            data.ReadString ( "UseSound"                    , ref m_use_sound                   , ""        );

            // Make sure expansion time is not near zero:

            if ( m_expand_time < 0.001f ) m_expand_time = 0.001f;
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

            data.Write( "UseEvent"                  , m_use_event                   );
            data.Write( "BackEvent"                 , m_back_event                  );
            data.Write( "ExpandTime"                , m_expand_time                 );
            data.Write( "Expansion"                 , m_expansion                   );
            data.Write( "HighlightWobbleSpeed"      , m_highlight_wobble_speed      );
            data.Write( "HighlightWobbleIntensity"  , m_highlight_wobble_intensity  );
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

                float expansion = Core.Timing.ElapsedTime * m_expansion * ( 1.0f / m_expand_time );

                // Expand:

                m_current_expansion += expansion;

                // If past the limit then stop:

                m_current_expansion = MathHelper.Clamp( m_current_expansion , 0 , m_expansion );
            }
            else
            {
                // Contract: figure out rate of contraction for this frame

                float contraction = Core.Timing.ElapsedTime * m_expansion * ( 1.0f / m_expand_time );

                // Contract:

                m_current_expansion -= contraction;

                // If past the limit then stop:

                m_current_expansion = MathHelper.Clamp( m_current_expansion , 0 , m_expansion );
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
            // Abort if we do not have a font or shader:

            if ( Font == null || m_highlight_wobble_effect == null ) return;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Set the wobble shader settings:

                // Time:

                {
                    EffectParameter p = m_highlight_wobble_effect.Parameters["WobbleTime"];

                    if ( p != null )
                    {
                        p.SetValue( m_highlight_wobble_time );
                    }
                }

                // Intensity: set to zero if not in focus

                {
                    EffectParameter p = m_highlight_wobble_effect.Parameters["WobbleIntensity"];

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
                    EffectParameter p = m_highlight_wobble_effect.Parameters["WobbleSpeed"];

                    if ( p != null )
                    {
                        p.SetValue( m_highlight_wobble_speed );
                    }
                }

            // Make up a custom font settings block:

            Font.CustomFontSettings custom_settings = new Font.CustomFontSettings
            (
                Font.Size + m_current_expansion ,
                Font.Color                      ,
                Font.CharSpacing                ,
                Font.LineSpacing                ,
                m_highlight_wobble_effect
            );

            // Draw the font string: see what it's alignment is first though
             
            if ( TextAlignment == Alignment.CENTER )
            {
                // Draw centered at its position:
            
                Font.DrawCustomStringCentered
                ( 
                    StringDatabase.GetString(StringName)    , 
                    Position                                ,
                    view_projection                         ,
                    custom_settings
                );
            }
            else if ( TextAlignment == Alignment.RIGHT )
            {
                // Draw right aligned at its position:
            
                Font.DrawCustomStringRightAligned
                ( 
                    StringDatabase.GetString(StringName)    , 
                    Position                                ,
                    view_projection                         ,
                    custom_settings
                );
            }
            else
            {
                // Draw left aligned:
            
                Font.DrawCustomString
                ( 
                    StringDatabase.GetString(StringName)    , 
                    Position                                , 
                    view_projection                         ,
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

            // Play the focus sound if not loading or just loaded:

            if ( ParentGui.Loading == false ) Core.Audio.Play(m_focus_sound);
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
                case Keys.Enter:    GuiEvents.InvokeGenericEvent(m_use_event,this);     Core.Audio.Play(m_use_sound);   break;
                case Keys.Space:    GuiEvents.InvokeGenericEvent(m_use_event,this);     Core.Audio.Play(m_use_sound);   break;
                case Keys.Escape:   GuiEvents.InvokeGenericEvent(m_back_event,this);    Core.Audio.Play(m_use_sound);   break;
                case Keys.Back:     GuiEvents.InvokeGenericEvent(m_back_event,this);    Core.Audio.Play(m_use_sound);   break;
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
                case Buttons.A: GuiEvents.InvokeGenericEvent(m_use_event,this);     Core.Audio.Play(m_use_sound);   break;
                case Buttons.B: GuiEvents.InvokeGenericEvent(m_back_event,this);    Core.Audio.Play(m_use_sound);   break;
            }
        }

    }   // end of class

}   // end of namespace
