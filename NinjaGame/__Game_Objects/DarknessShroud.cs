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
    /// This game object has a pretty simple function, to fade out the level at it's borders.
    /// </summary>
    //#############################################################################################

    public class DarknessShroud : GameObject
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Array of vertices used to draw the shroud. </summary>

            private VertexPositionColor[] m_vertices = null;

            /// <summary> Name of the shader effect used to draw the character. </summary>

            private string m_effect_name = "";
            
            /// <summary> Shader effect used to draw the character. </summary>

            private Effect m_effect = null;

            /// <summary> Distance it takes for the shroud to fade in. </summary>
        
            private float m_fade_in_distance = 64;

            /// <summary> How much to rotate the shroud about it's center by. </summary>

            private float m_rotation = 0;

            /// <summary> Color of the shroud. </summary>

            private Vector4 m_shroud_color = Vector4.Zero;

        //=========================================================================================
        ///<summary>
        /// Constructor. Creates the object.
        ///</summary>
        //=========================================================================================

        public DarknessShroud() : base(true,false,false,false)
        {
            // Set default depth:

            Depth = -1;

            // Create vertices array:

            m_vertices = new VertexPositionColor[12];
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
            // Call base function

            base.ReadXml(data);

            // Read all attributes

            data.ReadEffect ( "Effect"          , ref m_effect_name         , ref m_effect   , "Effects\\colored"   );
            data.ReadFloat  ( "FadeInDistance"  , ref m_fade_in_distance    , 64    );
            data.ReadFloat  ( "Rotation"        , ref m_rotation            , 0     );
            data.ReadFloat  ( "ShroudColorR"    , ref m_shroud_color.X      , 0     );
            data.ReadFloat  ( "ShroudColorG"    , ref m_shroud_color.Y      , 0     );
            data.ReadFloat  ( "ShroudColorB"    , ref m_shroud_color.Z      , 0     );

            // Setup our vertices:

                // Color

                m_vertices[0].Color = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1  ) );
                m_vertices[1].Color = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 0  ) );
                m_vertices[2].Color = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 0  ) );
                m_vertices[3].Color = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 0  ) );
                m_vertices[4].Color = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1  ) );
                m_vertices[5].Color = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1  ) );

                m_vertices[6].Color   = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1 ) );
                m_vertices[7].Color   = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1 ) );
                m_vertices[8].Color   = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1 ) );
                m_vertices[9].Color   = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1 ) );
                m_vertices[10].Color  = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1 ) );
                m_vertices[11].Color  = new Color( new Vector4( m_shroud_color.X , m_shroud_color.Y , m_shroud_color.Z , 1 ) );

                // Position:

                m_vertices[0].Position = new Vector3( Position , 0 ) + new Vector3( - m_fade_in_distance    , +100000 , -2 );
                m_vertices[1].Position = new Vector3( Position , 0 ) + new Vector3( 0                       , +100000 , -2 );
                m_vertices[2].Position = new Vector3( Position , 0 ) + new Vector3( 0                       , -100000 , -2 );
                m_vertices[3].Position = new Vector3( Position , 0 ) + new Vector3( 0                       , -100000 , -2 );
                m_vertices[4].Position = new Vector3( Position , 0 ) + new Vector3( - m_fade_in_distance    , -100000 , -2 );
                m_vertices[5].Position = new Vector3( Position , 0 ) + new Vector3( - m_fade_in_distance    , +100000 , -2 );

                m_vertices[6].Position  = new Vector3( Position , 0 ) + new Vector3( - 100000                , +100000 , -2 );
                m_vertices[7].Position  = new Vector3( Position , 0 ) + new Vector3( - m_fade_in_distance    , +100000 , -2 );
                m_vertices[8].Position  = new Vector3( Position , 0 ) + new Vector3( - m_fade_in_distance    , -100000 , -2 );
                m_vertices[9].Position  = new Vector3( Position , 0 ) + new Vector3( - m_fade_in_distance    , -100000 , -2 );
                m_vertices[10].Position = new Vector3( Position , 0 ) + new Vector3( - 100000                , -100000 , -2 );
                m_vertices[11].Position = new Vector3( Position , 0 ) + new Vector3( - 100000                , +100000 , -2 );

                // Rotation:

                    // Create rotation matrix:

                    Matrix rot = Matrix.CreateRotationZ( m_rotation );

                    // Do the rotation:

                    for ( int i = 0 ; i < m_vertices.Length ; i++ )
                    {
                        // Rotate about position:

                        m_vertices[i].Position = Vector3.Transform( m_vertices[i].Position  - new Vector3( Position , 0 ), rot );

                        // Restore world position:

                        m_vertices[i].Position = m_vertices[i].Position + new Vector3( Position , 0 );
                    }
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
            // Call base function

            base.WriteXml(data);

            // Write all attributes

            data.Write( "Effect"          , m_effect_name       );
            data.Write( "FadeInDistance"  , m_fade_in_distance  );
            data.Write( "Rotation"        , m_rotation          );
            data.Write( "ShroudColorR"    , m_shroud_color.X    );
            data.Write( "ShroudColorG"    , m_shroud_color.Y    );
            data.Write( "ShroudColorB"    , m_shroud_color.Z    );
        }

        //=========================================================================================
        /// <summary>
        /// Visibility function. Determines if the object is visible.
        /// </summary>
        /// <param name="c"> Camera the scene is being viewed from. </param>
        /// <returns> True because the shroud is always visible. </returns>
        //=========================================================================================

        public override bool IsVisible(Camera c){ return true; }

        //=========================================================================================
        /// <summary>
        /// Draw function, draws the shroud.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // If we are missing the effect:

            if ( m_effect == null ) return;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Set the reference alpha to zero:

            Core.Graphics.Device.RenderState.ReferenceAlpha = 0;

            // Set shader params:

            EffectParameter param_wvp = m_effect.Parameters["WorldViewProjection"];

            if ( param_wvp != null ) 
            {
                param_wvp.SetValue(view_projection);
            }

            // Begin drawing with the shader:

            m_effect.Begin();

                // Set vertex declaration on graphics device:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;

                // Begin pass:

                m_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the shroud:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.TriangleList  ,
                        m_vertices                  ,
                        0                           ,
                        4
                    );

                // End pass:

                m_effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            m_effect.End();

            // Restore reference alpha:

            Core.Graphics.Device.RenderState.ReferenceAlpha = GraphicsSystem.DefaultRenderState.ReferenceAlpha;
        }

    }   // end of class

}   // end of namespace
