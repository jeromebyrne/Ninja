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
    /// A widget that fades out a gui screen and then loads the next one.
    /// </summary>
    //#############################################################################################

    public class Gui_Screen_Fader : GuiWidget
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Shader the widget uses to draw itself </summary>

            private Effect m_effect = null;

            /// <summary> Time till the widget causes the screen to fade out completely </summary>

            private float m_fade_time = 2.0f; 

            /// <summary> Current amount of time elapsed since the widget was made </summary>

            private float m_time = 0.0f; 

            /// <summary> Next gui screen to load. </summary>

            private string m_next_gui = "";

            /// <summary> Vertices used to draw the fade out. </summary>

            private VertexPositionColor[] m_vertices = new VertexPositionColor[4];

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a default sprite with no texture or shader. 
        /// </summary>
        //=========================================================================================

        public Gui_Screen_Fader()
        {   
            // Set default depth:

            Depth = 0;    

            // Load the effect:

            m_effect = Core.Graphics.LoadEffect("Effects\\colored");
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

            data.ReadFloat  ( "FadeTime"    , ref m_fade_time   , 2.0f  );
            data.ReadString ( "NextGui"     , ref m_next_gui    , ""    );
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

            data.Write( "FadeTime"  , m_fade_time       );
            data.Write( "NextGui"   , m_next_gui        );
        }

        //=========================================================================================
        /// <summary>
        /// Update function. Updates the time elapsed and loads the next gui when it is time.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Call base function

            base.OnUpdate();

            // Increase time elapsed:

            m_time += Core.Timing.ElapsedTime;

            // If enough time has passed then load the next gui if possible:

            if ( m_next_gui != null && m_next_gui.Length > 0 && m_time >= m_fade_time )
            {
                // Load the next gui

                Core.Gui.Load( m_next_gui );
            }
            else if ( m_next_gui != null && m_next_gui.Length > 0 )
            {
                // If the right key was pressed then skip this screen:

                    // Check the gamepad:

                    GamePadState gps = GamePad.GetState(PlayerIndex.One);
                    
                    if 
                    ( 
                        gps.IsButtonDown( Buttons.A             ) ||
                        gps.IsButtonDown( Buttons.B             ) ||
                        gps.IsButtonDown( Buttons.X             ) ||
                        gps.IsButtonDown( Buttons.Y             ) ||
                        gps.IsButtonDown( Buttons.Start         ) ||
                        gps.IsButtonDown( Buttons.Back          ) ||
                        gps.IsButtonDown( Buttons.LeftShoulder  ) ||
                        gps.IsButtonDown( Buttons.RightShoulder ) ||
                        gps.IsButtonDown( Buttons.RightTrigger  ) ||
                        gps.IsButtonDown( Buttons.LeftTrigger   ) ||
                        gps.IsButtonDown( Buttons.LeftStick     ) ||
                        gps.IsButtonDown( Buttons.RightStick    )
                    )
                    {
                        // Load the next gui

                        Core.Gui.Load( m_next_gui );
                    }

                    // Check the keyboard:

                    KeyboardState kbs = Keyboard.GetState();

                    if 
                    (
                        kbs.IsKeyDown(Keys.Space)   ||
                        kbs.IsKeyDown(Keys.Enter)   ||
                        kbs.IsKeyDown(Keys.Escape)
                    )
                    {
                        // Load the next gui

                        Core.Gui.Load( m_next_gui );
                    }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Draws the sprite.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // If we are missing an effect then abort:

            if ( m_effect  == null ) return;

            // Pick alpha:

                // Percentage fading done:

                float t = m_time / m_fade_time;

                // Alpha:

                float a = 0.0f;

                // Pick alpha:

                if ( t < 0.5f )
                {
                    a = t * 2.0f;
                }
                else
                {
                    a = 1.0f - ( t - 0.5f ) * 2.0f;
                }

                a = 1.0f - a;

            // Set the color of all the vertices:

            m_vertices[0].Color = new Color( new Vector4(0,0,0,a) );
            m_vertices[1].Color = new Color( new Vector4(0,0,0,a) );
            m_vertices[2].Color = new Color( new Vector4(0,0,0,a) );
            m_vertices[3].Color = new Color( new Vector4(0,0,0,a) );

            // Set the position of all the vertices:

            m_vertices[0].Position.X = -2;
            m_vertices[1].Position.X = -2;
            m_vertices[2].Position.X = 2;
            m_vertices[3].Position.X = 2;

            m_vertices[0].Position.Y = 2;
            m_vertices[1].Position.Y = -2;
            m_vertices[2].Position.Y = 2;
            m_vertices[3].Position.Y = -2;

            // Set the transform on the shader:

            EffectParameter param_wvp = m_effect.Parameters[ "WorldViewProjection"   ];

            if ( param_wvp != null ) 
            {
                param_wvp.SetValue( Matrix.Identity );
            }

            // Begin drawing with the shader:

            m_effect.Begin();

                // Set vertex declaration on graphics device:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;

                // Begin pass:

                m_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.TriangleStrip ,
                        m_vertices                  ,
                        0                           ,
                        2
                    );

                // End pass:

                m_effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            m_effect.End();
        }

    }   // end of class

}   // end of namespace
