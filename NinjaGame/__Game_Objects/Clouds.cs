using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Game object that draws clouds on the screen. These clouds wrap around fully.
    /// </summary>
    //#############################################################################################

    public class Clouds : GameObject
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Shader the sprite uses to draw itself </summary>

            private Effect m_effect = null;

            /// <summary> Name of the shader used by the sprite </summary>

            private string m_effect_name = "";            

            /// <summary> Number of clouds to draw. </summary>

            private int m_cloud_count = 0;

            /// <summary> Random amount of cloud speed to add / subtract to each cloud speed - 1st value </summary>

            private float m_cloud_speed_1 = 0.1f;

            /// <summary> Random amount of cloud speed to add / subtract to each cloud speed - 2nd value </summary>

            private float m_cloud_speed_2 = 0.1f;

            /// <summary> Random amount to offset clouds by on the y axis - 1st value </summary>

            private float m_cloud_y_offset_1 = 0.1f;

            /// <summary> Random amount to offset clouds by on the y axis - 2nd value </summary>

            private float m_cloud_y_offset_2 = 0.1f;

            /// <summary> Random amount to add to cloud x scale - 1st value </summary>

            private float m_cloud_x_scale_1 = 0.1f;

            /// <summary> Random amount to add to cloud x scale - 2nd value </summary>

            private float m_cloud_x_scale_2 = 0.1f;

            /// <summary> Random amount to add to cloud y scale - 1st value </summary>

            private float m_cloud_y_scale_1 = 0.1f;

            /// <summary> Random amount to add to cloud y scale - 2nd value </summary>

            private float m_cloud_y_scale_2 = 0.1f;

            /// <summary> Multiplier for cloud parallax scrolling. The position of the camera is multiplied by this. 1st value. </summary>

            private float m_parallax_multiplier_1 = 0.1f;

            /// <summary> Multiplier for cloud parallax scrolling. The position of the camera is multiplied by this. 2nd value.</summary>

            private float m_parallax_multiplier_2 = 0.1f;

            /// <summary> First cloud texture to use. </summary>

            private Texture2D m_cloud_texture_1 = null;

            /// <summary> Second cloud texture to use. </summary>

            private Texture2D m_cloud_texture_2 = null;

            /// <summary> Name of the first cloud texture to use. </summary>

            private string m_cloud_texture_1_name = "";

            /// <summary> Name of the second cloud texture to use. </summary>

            private string m_cloud_texture_2_name = "";

            /// <summary> Vertices used to render each cloud. </summary>

            private VertexPositionColorTexture[] m_vertices = null;

            /// <summary> The offset of each cloud on the screen from 0-1 </summary>

            private float[] m_cloud_x_offsets = null;

            /// <summary> Y position of each cloud in the world </summary>

            private float[] m_cloud_y_offsets = null;

            /// <summary> Speed that each cloud moves at </summary>

            private float[] m_cloud_speeds = null;

            /// <summary> Size of each cloud </summary>

            private Vector2[] m_cloud_scales = null;

            /// <summary> Parallax multiplier for each cloud to multiply the camera position by </summary>

            private float[] m_parallax_multipliers = null;
        
        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the clouds.
        /// </summary>
        //=========================================================================================

        public Clouds() : base(true,false,true,false){}

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

        public override void ReadXml(XmlObjectData data)
        {
            // Call base class function

            base.ReadXml(data);

            // Read all data:

            data.ReadEffect ( "Effect"          , ref m_effect_name             , ref m_effect          , "Effects\\textured"  );
            data.ReadTexture( "CloudTexture1"   , ref m_cloud_texture_1_name    , ref m_cloud_texture_1 , "Graphics\\default"  );
            data.ReadTexture( "CloudTexture2"   , ref m_cloud_texture_2_name    , ref m_cloud_texture_2 , "Graphics\\default"  );

            data.ReadInt   ( "CloudCount"               , ref m_cloud_count             , 16    );           
            data.ReadFloat ( "CloudScaleX_1"            , ref m_cloud_x_scale_1         , 1     );
            data.ReadFloat ( "CloudScaleY_1"            , ref m_cloud_y_scale_1         , 1     );
            data.ReadFloat ( "CloudScaleX_2"            , ref m_cloud_x_scale_2         , 1     );
            data.ReadFloat ( "CloudScaleY_2"            , ref m_cloud_y_scale_2         , 1     );
            data.ReadFloat ( "CloudOffsetY_1"           , ref m_cloud_y_offset_1        , 0     );
            data.ReadFloat ( "CloudOffsetY_2"           , ref m_cloud_y_offset_2        , 0     );
            data.ReadFloat ( "CloudSpeed_1"             , ref m_cloud_speed_1           , 0.5f  );
            data.ReadFloat ( "CloudSpeed_2"             , ref m_cloud_speed_2           , 0.5f  );
            data.ReadFloat ( "ParallaxMultiplier_1"     , ref m_parallax_multiplier_1   , 0.25f );
            data.ReadFloat ( "ParallaxMultiplier_2"     , ref m_parallax_multiplier_2   , 0.25f );

            // Create all the clouds:

            CreateClouds();
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

            data.Write( "Effect"                , m_effect_name           );
            data.Write( "CloudTexture1"         , m_cloud_texture_1_name  );
            data.Write( "CloudTexture2"         , m_cloud_texture_2_name  );
            data.Write( "CloudCount"            , m_cloud_count           );
            data.Write( "CloudScaleX_1"         , m_cloud_x_scale_1       );
            data.Write( "CloudScaleY_1"         , m_cloud_y_scale_1       );
            data.Write( "CloudScaleX_2"         , m_cloud_x_scale_2       );
            data.Write( "CloudScaleY_2"         , m_cloud_y_scale_2       );
            data.Write( "CloudOffsetY_1"        , m_cloud_y_offset_1      );
            data.Write( "CloudOffsetY_2"        , m_cloud_y_offset_2      );
            data.Write( "CloudSpeed_1"          , m_cloud_speed_1         );
            data.Write( "CloudSpeed_2"          , m_cloud_speed_2           );
            data.Write( "ParallaxMultiplier_1"  , m_parallax_multiplier_1   );
            data.Write( "ParallaxMultiplier_2"  , m_parallax_multiplier_2   );
        }

        //=========================================================================================
        /// <summary> 
        /// Creates and positions all of the clouds in the cloud collection.
        /// </summary>
        //=========================================================================================

        private void CreateClouds()
        {
            // Abort if we are missing cloud textures:

            if ( m_cloud_texture_1 == null ) return;
            if ( m_cloud_texture_2 == null ) return;

            // Make sure cloud count is not negative:

            if ( m_cloud_count < 0 ) m_cloud_count = 0;

            // If the cloud count is not zero then create the array used to render the clouds:

            if ( m_cloud_count > 0 )
            {
                // Create the array of cloud vertices

                m_vertices  = new VertexPositionColorTexture[ m_cloud_count * 12 ];

                // Create all other arrays:

                m_cloud_x_offsets       = new float     [ m_cloud_count ];
                m_cloud_y_offsets       = new float     [ m_cloud_count ];
                m_cloud_speeds          = new float     [ m_cloud_count ];
                m_cloud_scales          = new Vector2   [ m_cloud_count ];
                m_parallax_multipliers  = new float     [ m_cloud_count ];

                // Initialise data for each cloud:

                for ( int i = 0 ; i < m_cloud_count ; i++ )
                {
                    // Set x offset from 0-1

                    m_cloud_x_offsets[i] = (float) Core.Random.NextDouble();

                    // Initialise the y position

                    float t = (float) Core.Random.NextDouble();

                        m_cloud_y_offsets[i] = PositionY + t * m_cloud_y_offset_1 + ( 1.0f - t ) * m_cloud_y_offset_2;

                    // Initialise cloud speed:

                    t = (float) Core.Random.NextDouble();

                        m_cloud_speeds[i] = t * m_cloud_speed_1 + ( 1.0f - t ) * m_cloud_speed_2;

                    // Initialise x scale

                    t = (float) Core.Random.NextDouble();

                        m_cloud_scales[i].X = t * m_cloud_x_scale_1 + ( 1.0f - t ) * m_cloud_x_scale_2;

                    // Initialise y scale

                    t = (float) Core.Random.NextDouble();

                        m_cloud_scales[i].Y = t * m_cloud_y_scale_1 + ( 1.0f - t ) * m_cloud_y_scale_2;

                    // Initialise parallax multiplier

                    t = (float) Core.Random.NextDouble();

                        m_parallax_multipliers[i] = t * m_parallax_multiplier_1 + ( 1.0f - t ) * m_parallax_multiplier_2;
                }

            }
            else
            {
                // No clouds: no vertices

                m_vertices              = null; 
                m_cloud_x_offsets       = null;
                m_cloud_y_offsets       = null;
                m_cloud_speeds          = null;
                m_cloud_scales          = null;
                m_parallax_multipliers  = null;

                // Abort: no more work to do

                return;
            }

            // Right: make the vertex array for the clouds:

            m_vertices = new VertexPositionColorTexture[ m_cloud_count * 6 ];

            // Fill in the texture coordinates and colors:

            for ( int i = 0 ; i < m_cloud_count ; i++ )
            {
                // Fill in the texture coordinates:

                m_vertices[ i * 6 + 0  ].TextureCoordinate.X = 0;
                m_vertices[ i * 6 + 1  ].TextureCoordinate.X = 1;
                m_vertices[ i * 6 + 2  ].TextureCoordinate.X = 1;
                m_vertices[ i * 6 + 3  ].TextureCoordinate.X = 1;
                m_vertices[ i * 6 + 4  ].TextureCoordinate.X = 0;
                m_vertices[ i * 6 + 5  ].TextureCoordinate.X = 0;

                m_vertices[ i * 6 + 0  ].TextureCoordinate.Y = 0;
                m_vertices[ i * 6 + 1  ].TextureCoordinate.Y = 0;
                m_vertices[ i * 6 + 2  ].TextureCoordinate.Y = 1;
                m_vertices[ i * 6 + 3  ].TextureCoordinate.Y = 1;
                m_vertices[ i * 6 + 4  ].TextureCoordinate.Y = 1;
                m_vertices[ i * 6 + 5  ].TextureCoordinate.Y = 0;

                // Fill in the colors:

                m_vertices[ i * 6 + 0  ].Color = Color.White;
                m_vertices[ i * 6 + 1  ].Color = Color.White;
                m_vertices[ i * 6 + 2  ].Color = Color.White;
                m_vertices[ i * 6 + 3  ].Color = Color.White;
                m_vertices[ i * 6 + 4  ].Color = Color.White;
                m_vertices[ i * 6 + 5  ].Color = Color.White;

                // Fill in the z values for the vertices:

                const float Z = -2.0f;

                m_vertices[ i * 6 + 0  ].Position.Z = Z;
                m_vertices[ i * 6 + 1  ].Position.Z = Z;
                m_vertices[ i * 6 + 2  ].Position.Z = Z;
                m_vertices[ i * 6 + 3  ].Position.Z = Z;
                m_vertices[ i * 6 + 4  ].Position.Z = Z;
                m_vertices[ i * 6 + 5  ].Position.Z = Z;
            }

        }

        //=========================================================================================
        /// <summary>
        /// Is visible function. Tells if the object is visible from given camera view.
        /// </summary>
        /// <param name="c"> Camera the scene is being viewed from. </param>
        /// <returns> True in this case always, clouds are always visible. </returns>
        //=========================================================================================

        public override bool IsVisible( Camera c ){ return true; }

        //=========================================================================================
        /// <summary>
        /// Update function. Moves the clouds along.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Abort if no vertices or textures:

            if ( m_vertices         == null ) return;
            if ( m_cloud_texture_1  == null ) return;
            if ( m_cloud_texture_2  == null ) return;

            // Move each cloud along:

                // Do the movement:

                for ( int i = 0 ; i < m_cloud_count ; i++ )
                {
                    // Do the movement:

                    m_cloud_x_offsets[i] += Core.Timing.ElapsedTime * m_cloud_speeds[i];

                    // Keep in 0-1 range:

                    float t = (float) Math.IEEERemainder( m_cloud_x_offsets[i] , 1.0 );
                    
                    // If negative then wrap around:

                    if ( t < 0 ) t = 1.0f + t;

                    // Save new cloud offset:

                    m_cloud_x_offsets[i] = t;
                }
        }

        //=========================================================================================
        /// <summary>
        /// Draw function. Draws the clouds.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // If we are missing an effect or textures then abort:

            if ( m_cloud_texture_1  == null ) return;
            if ( m_cloud_texture_2  == null ) return;
            if ( m_effect           == null ) return;
            if ( m_vertices         == null ) return;

            // Get the view size:

            Vector2 view_size = Core.Level.Renderer.Camera.ViewArea;

            // Get the camera position:

            Vector2 cam_pos = Core.Level.Renderer.Camera.Position;

            // Setup all vertices:

            for ( int i = 0 ; i < m_cloud_count ; i++ )
            {
                // Get the width and height of this cloud:

                float cloud_w = 0;
                float cloud_h = 0;

                    if ( i > m_cloud_count / 2 )
                    {
                        cloud_w = m_cloud_texture_1.Width  * m_cloud_scales[i].X * 0.5f;
                        cloud_h = m_cloud_texture_1.Height * m_cloud_scales[i].Y * 0.5f;
                    }
                    else
                    {
                        cloud_w = m_cloud_texture_2.Width  * m_cloud_scales[i].X * 0.5f;
                        cloud_h = m_cloud_texture_2.Height * m_cloud_scales[i].Y * 0.5f;
                    }

                // Figure out the wrapped position of the of the cloud on the screen:

                float wrapped_screen_x = (float) Math.IEEERemainder
                ( 
                    ( m_cloud_x_offsets[i] * ( view_size.X + cloud_w * 2.0f ) - cam_pos.X * m_parallax_multipliers[i] ) / ( view_size.X + cloud_w * 2.0f  ), 
                    1.0f
                );

                if ( wrapped_screen_x < 0 ) wrapped_screen_x = 1.0f + wrapped_screen_x;

                wrapped_screen_x *= view_size.X + cloud_w * 2.0f;
                wrapped_screen_x -= view_size.X * 0.5f + cloud_w;

                // Translate into world coordinates:

                float wrapped_x = wrapped_screen_x + cam_pos.X;

                // Now set the vertices for this cloud:

                m_vertices[ i * 6 + 0 ].Position.X = wrapped_x - cloud_w;
                m_vertices[ i * 6 + 1 ].Position.X = wrapped_x + cloud_w;
                m_vertices[ i * 6 + 2 ].Position.X = wrapped_x + cloud_w;
                m_vertices[ i * 6 + 3 ].Position.X = wrapped_x + cloud_w;
                m_vertices[ i * 6 + 4 ].Position.X = wrapped_x - cloud_w;
                m_vertices[ i * 6 + 5 ].Position.X = wrapped_x - cloud_w;

                m_vertices[ i * 6 + 0 ].Position.Y = Position.Y + cloud_h + m_cloud_y_offsets[i];
                m_vertices[ i * 6 + 1 ].Position.Y = Position.Y + cloud_h + m_cloud_y_offsets[i];
                m_vertices[ i * 6 + 2 ].Position.Y = Position.Y - cloud_h + m_cloud_y_offsets[i];
                m_vertices[ i * 6 + 3 ].Position.Y = Position.Y - cloud_h + m_cloud_y_offsets[i];
                m_vertices[ i * 6 + 4 ].Position.Y = Position.Y - cloud_h + m_cloud_y_offsets[i];
                m_vertices[ i * 6 + 5 ].Position.Y = Position.Y + cloud_h + m_cloud_y_offsets[i];
            }

            // Set shader transform:

            {
                // Get param

                EffectParameter p = m_effect.Parameters[ "WorldViewProjection" ];

                // Set param

                if ( p != null && p.ParameterType == EffectParameterType.Single ) 
                {
                    p.SetValue( Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection );
                }
            }

            // Set vertex declaration on graphics device:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

            // Set texture

                {
                    // Get param

                    EffectParameter p = m_effect.Parameters[ "Texture" ];

                    // Set param

                    if ( p != null && p.ParameterType == EffectParameterType.Texture ) p.SetValue( m_cloud_texture_1 );
                }

            // Begin drawing with the shader: Draw the first lot of clouds

            m_effect.Begin();

                // Begin pass:

                m_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleList  ,
                        m_vertices                  ,
                        0                           ,
                        m_cloud_count
                    );

                // End pass:

                m_effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            m_effect.End();

            // Set texture

                {
                    // Get param

                    EffectParameter p = m_effect.Parameters[ "Texture" ];

                    // Set param

                    if ( p != null && p.ParameterType == EffectParameterType.Texture ) p.SetValue( m_cloud_texture_2 );
                }

            // Begin drawing with the shader: Draw the second lot of clouds

            m_effect.Begin();

                // Begin pass:

                m_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleList  ,
                        m_vertices                  ,
                        m_cloud_count * 3           ,
                        m_cloud_count
                    );

                // End pass:

                m_effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            m_effect.End();
        }

    }   // end of class

}   // end of namespace
