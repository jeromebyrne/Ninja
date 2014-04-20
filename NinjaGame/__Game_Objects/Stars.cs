using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that draws a series of stars for the background.
    /// </summary>
    //#############################################################################################

    public class Stars : GameObject
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> The number of stars to show. </summary>

            private int m_star_count = 0;

            /// <summary> Effect used to draw the stars. </summary>

            private Effect m_effect = null;

            /// <summary> Texture used to render the stars with. </summary>            
            
            private Texture2D m_texture = null;

            /// <summary> Name of the texture used to render the stars with. </summary>

            private string m_texture_name = "";

            /// <summary> Vertex buffer used to render the stars. </summary>

            private VertexBuffer m_vertex_buffer = null;

            /// <summary> Vertex declaration used to draw the stars with. </summary>
            
            private VertexDeclaration m_vertex_declaration = null;

            /// <summary> One of two random star sizes for the stars. </summary>

            private float m_star_size_1 = 2.0f;

            /// <summary> One of two random star sizes for the stars. </summary>

            private float m_star_size_2 = 2.0f;

            /// <summary> One of two random star colors for the stars. </summary>

            private Vector4 m_star_color_1 = Vector4.One;

            /// <summary> One of two random star colors for the stars. </summary>

            private Vector4 m_star_color_2 = Vector4.One;

            /// <summary> How much camera scrolling affects scrolling of the stars </summary>

            private float m_parallax_scale = 1.0f;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the stars.
        /// </summary>
        //=========================================================================================

        public Stars() : base( true, false, false, false )
        {
            // load the shader for drawing wrapped stars

            m_effect = Core.Graphics.LoadEffect("Effects\\stars");
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

            // Read all data:
     
            data.ReadTexture( "Texture"         , ref m_texture_name    , ref m_texture , "Graphics\\default"   );
            data.ReadInt    ( "StarCount"       , ref m_star_count      , 50                                    );
            data.ReadFloat  ( "StarSize1"       , ref m_star_size_1     , 1.0f                                  );
            data.ReadFloat  ( "StarSize2"       , ref m_star_size_2     , 2.0f                                  );
            data.ReadFloat  ( "StarColor1R"     , ref m_star_color_1.X  , 1.0f                                  );
            data.ReadFloat  ( "StarColor1G"     , ref m_star_color_1.Y  , 1.0f                                  );
            data.ReadFloat  ( "StarColor1B"     , ref m_star_color_1.Z  , 1.0f                                  );
            data.ReadFloat  ( "StarColor1A"     , ref m_star_color_1.W  , 1.0f                                  );
            data.ReadFloat  ( "StarColor2R"     , ref m_star_color_2.X  , 1.0f                                  );
            data.ReadFloat  ( "StarColor2G"     , ref m_star_color_2.Y  , 1.0f                                  );
            data.ReadFloat  ( "StarColor2B"     , ref m_star_color_2.Z  , 1.0f                                  );
            data.ReadFloat  ( "StarColor2A"     , ref m_star_color_2.W  , 1.0f                                  );
            data.ReadFloat  ( "ParallaxScale"   , ref m_parallax_scale  , 1.0f                                  );

            // Setup the stars

            SetupStars();
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

            // Write all data:
     
            data.Write( "Texture"           , m_texture_name    );
            data.Write( "StarCount"         , m_star_count      );
            data.Write( "StarSize1"         , m_star_size_1     );
            data.Write( "StarSize2"         , m_star_size_2     );
            data.Write( "StarColor1R"       , m_star_color_1.X  );
            data.Write( "StarColor1G"       , m_star_color_1.Y  );
            data.Write( "StarColor1B"       , m_star_color_1.Z  );
            data.Write( "StarColor1A"       , m_star_color_1.W  );
            data.Write( "StarColor2R"       , m_star_color_2.X  );
            data.Write( "StarColor2G"       , m_star_color_2.Y  );
            data.Write( "StarColor2B"       , m_star_color_2.Z  );
            data.Write( "StarColor2A"       , m_star_color_2.W  );
            data.Write( "ParallaxScale"     , m_parallax_scale  );
        }

        //=========================================================================================
        /// <summary>
        /// Sets up the stars for rendering.
        /// </summary>
        //=========================================================================================

        private void SetupStars()
        {
            // If the star count is negative or zero then wipe the vertex buffer and abort:

            if ( m_star_count <= 0 ){ m_vertex_buffer = null; m_vertex_declaration = null; return; }

            // Makeup the vertex declaration we will use to draw with:

                // Vertex elements:

                VertexElement[] elements = new VertexElement[]
                {
                    // Stars points local to its own center. Z dimension gives the size of the star about its center

                    new VertexElement
                    (
                        0                           , 
                        sizeof(float) * 0           , 
                        VertexElementFormat.Vector3 , 
                        VertexElementMethod.Default , 
                        VertexElementUsage.Position , 
                        0
 
                    )   ,

                    // Color of star

                    new VertexElement
                    (
                        0                           , 
                        sizeof(float) * 3           , 
                        VertexElementFormat.Vector4 , 
                        VertexElementMethod.Default , 
                        VertexElementUsage.Color    , 
                        0

                    )   ,

                    // Texture coords. X and Y for normal texture coords, Z and W give position of star on screen from 0-1

                    new VertexElement
                    (
                        0                                       , 
                        sizeof(float) * 7                       , 
                        VertexElementFormat.Vector4             , 
                        VertexElementMethod.Default             , 
                        VertexElementUsage.TextureCoordinate    , 
                        0

                    )
                };

                // Make vertex declaration:

                m_vertex_declaration = new VertexDeclaration( Core.Graphics.Device , elements );

            // Make our vertex buffer:

            m_vertex_buffer = new VertexBuffer
            ( 
                Core.Graphics.Device                    , 
                m_star_count * sizeof(float) * 11 * 6   , 
                BufferUsage.WriteOnly 
            );

            // Make up an array to fill the vertex buffer with:

            float[] data = new float[ m_star_count * 11 * 6 ];

            // Now fill the vertex buffer:

            for ( int i = 0 ; i < m_star_count ; i++ )
            {
                // Pick a size for this star:

                float t = (float) Core.Random.NextDouble();

                    // Pick size:

                    float star_size = t * m_star_size_1 + ( 1.0f - t ) * m_star_size_2;
                
                // Fill in the positions for the star:

                data[ i * 11 * 6 + 0    ] = -star_size;         // p1 x
                data[ i * 11 * 6 + 11   ] = +star_size;         // p2 x
                data[ i * 11 * 6 + 22   ] = +star_size;         // p3 x
                data[ i * 11 * 6 + 33   ] = +star_size;         // p4 x
                data[ i * 11 * 6 + 44   ] = -star_size;         // p5 x
                data[ i * 11 * 6 + 55   ] = -star_size;         // p6 x

                data[ i * 11 * 6 + 1    ] = +star_size;         // p1 y
                data[ i * 11 * 6 + 12   ] = +star_size;         // p2 y
                data[ i * 11 * 6 + 23   ] = -star_size;         // p3 y
                data[ i * 11 * 6 + 34   ] = -star_size;         // p4 y
                data[ i * 11 * 6 + 45   ] = -star_size;         // p5 y
                data[ i * 11 * 6 + 56   ] = +star_size;         // p6 y

                // Fill in the size for the star:

                data[ i * 11 * 6 + 2    ] = star_size;        // p1 size 
                data[ i * 11 * 6 + 13   ] = star_size;        // p2 size
                data[ i * 11 * 6 + 24   ] = star_size;        // p3 size
                data[ i * 11 * 6 + 35   ] = star_size;        // p4 size
                data[ i * 11 * 6 + 46   ] = star_size;        // p5 size
                data[ i * 11 * 6 + 57   ] = star_size;        // p6 size

                // Pick a color for the star:

                t = (float) Core.Random.NextDouble();

                    // Pick Color:

                    Vector4 star_color = Vector4.One;

                    star_color.X = m_star_color_1.X * t + ( 1.0f - t ) * m_star_color_2.X;              
                    star_color.Y = m_star_color_1.Y * t + ( 1.0f - t ) * m_star_color_2.Y;
                    star_color.Z = m_star_color_1.Z * t + ( 1.0f - t ) * m_star_color_2.Z;
                    star_color.W = m_star_color_1.W * t + ( 1.0f - t ) * m_star_color_2.W;

                // Fill in the color for the star:

                data[ i * 11 * 6 + 3    ] = star_color.X;       // p1 R 
                data[ i * 11 * 6 + 14   ] = star_color.X;       // p2 R
                data[ i * 11 * 6 + 25   ] = star_color.X;       // p3 R
                data[ i * 11 * 6 + 36   ] = star_color.X;       // p4 R
                data[ i * 11 * 6 + 47   ] = star_color.X;       // p5 R
                data[ i * 11 * 6 + 58   ] = star_color.X;       // p6 R

                data[ i * 11 * 6 + 4    ] = star_color.Y;       // p1 G 
                data[ i * 11 * 6 + 15   ] = star_color.Y;       // p2 G
                data[ i * 11 * 6 + 26   ] = star_color.Y;       // p3 G
                data[ i * 11 * 6 + 37   ] = star_color.Y;       // p4 G
                data[ i * 11 * 6 + 48   ] = star_color.Y;       // p5 G
                data[ i * 11 * 6 + 59   ] = star_color.Y;       // p6 G

                data[ i * 11 * 6 + 5    ] = star_color.Z;       // p1 B 
                data[ i * 11 * 6 + 16   ] = star_color.Z;       // p2 B
                data[ i * 11 * 6 + 27   ] = star_color.Z;       // p3 B
                data[ i * 11 * 6 + 38   ] = star_color.Z;       // p4 B
                data[ i * 11 * 6 + 49   ] = star_color.Z;       // p5 B
                data[ i * 11 * 6 + 60   ] = star_color.Z;       // p6 B

                data[ i * 11 * 6 + 6    ] = star_color.W;       // p1 A
                data[ i * 11 * 6 + 17   ] = star_color.W;       // p2 A
                data[ i * 11 * 6 + 28   ] = star_color.W;       // p3 A
                data[ i * 11 * 6 + 39   ] = star_color.W;       // p4 A
                data[ i * 11 * 6 + 50   ] = star_color.W;       // p5 A
                data[ i * 11 * 6 + 61   ] = star_color.W;       // p6 A

                // Fill in texture coords for the star:

                data[ i * 11 * 6 + 7    ] = 0;          // p1 u
                data[ i * 11 * 6 + 18   ] = 1;          // p2 u
                data[ i * 11 * 6 + 29   ] = 1;          // p3 u
                data[ i * 11 * 6 + 40   ] = 1;          // p4 u
                data[ i * 11 * 6 + 51   ] = 0;          // p5 u
                data[ i * 11 * 6 + 62   ] = 0;          // p6 u

                data[ i * 11 * 6 + 8    ] = 0;          // p1 v
                data[ i * 11 * 6 + 19   ] = 0;          // p2 v
                data[ i * 11 * 6 + 30   ] = 1;          // p3 v
                data[ i * 11 * 6 + 41   ] = 1;          // p4 v
                data[ i * 11 * 6 + 52   ] = 1;          // p5 v
                data[ i * 11 * 6 + 63   ] = 0;          // p6 v

                // Pick position of the star on the screen from 0-1

                float px = (float) Core.Random.NextDouble();
                float py = (float) Core.Random.NextDouble();

                // Fill in position of the star on screen:

                data[ i * 11 * 6 + 9    ] = px;         // p1 x
                data[ i * 11 * 6 + 20   ] = px;         // p2 x
                data[ i * 11 * 6 + 31   ] = px;         // p3 x
                data[ i * 11 * 6 + 42   ] = px;         // p4 x
                data[ i * 11 * 6 + 53   ] = px;         // p5 x
                data[ i * 11 * 6 + 64   ] = px;         // p6 x

                data[ i * 11 * 6 + 10   ] = py;         // p1 y
                data[ i * 11 * 6 + 21   ] = py;         // p2 y
                data[ i * 11 * 6 + 32   ] = py;         // p3 y
                data[ i * 11 * 6 + 43   ] = py;         // p4 y
                data[ i * 11 * 6 + 54   ] = py;         // p5 y
                data[ i * 11 * 6 + 65   ] = py;         // p6 y
            }

            // Upload the data onto the vertex buffer:

            m_vertex_buffer.SetData<float>( data );
        }

        //=========================================================================================
        /// <summary>
        /// Visibility determination. Stars are always visible.
        /// </summary>
        //=========================================================================================

        public override bool IsVisible( Camera c ){ return true; }

        //=========================================================================================
        /// <summary>
        /// Draw function. Draws the stars.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // Abort if nothing to draw with:

            if ( m_texture == null || m_vertex_buffer == null || m_vertex_declaration == null ) return;

            // Set the vertex declaration:

            Core.Graphics.Device.VertexDeclaration = m_vertex_declaration;

            // Set the vertex buffer on the card:

            Core.Graphics.Device.Vertices[0].SetSource
            (
                m_vertex_buffer                             , 
                0                                           , 
                m_vertex_declaration.GetVertexStrideSize(0) 
            );

            // Set the texture on the shader:

            {
                // Get the param:

                EffectParameter p = m_effect.Parameters["Texture"];

                // Set if there:
                
                if ( p != null ) p.SetValue( m_texture );
            }

            // Set parallax scale on the shader

            {
                // Get the param:

                EffectParameter p = m_effect.Parameters["ParallaxScale"];

                // Set if there:
                
                if ( p != null ) p.SetValue( m_parallax_scale );
            }

            // Set screen area and camera position:

            {
                // Get the param:

                EffectParameter p = m_effect.Parameters["ScreenSizeAndCameraPosition"];

                // Set if there:
                
                if ( p != null )
                {
                    // Make up value:

                    Vector4 v = Vector4.Zero;

                        v.X = Core.Level.Renderer.Camera.ViewArea.X;
                        v.Y = Core.Level.Renderer.Camera.ViewArea.Y;
                        v.Z = Core.Level.Renderer.Camera.PositionX;
                        v.W = Core.Level.Renderer.Camera.PositionY;

                    // Set 
                    
                    p.SetValue(v);
                }
            }

            // Now begin drawing with the shader:

            m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                // Draw:

                Core.Graphics.Device.DrawPrimitives(PrimitiveType.TriangleList,0,m_star_count*2);

            // End drawing:

            m_effect.End(); m_effect.CurrentTechnique.Passes[0].End();
        }

    }   // end of class

}   // end of namespace
