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
    /// Simple class that draws a tiled sky across the screen.
    /// </summary>
    //#############################################################################################

    public class Sky : Sprite
    {

        private struct Stars 
        {
            public Vector2 Position;
            public Vector4 Color;
            public Vector2 Scale;
        }

        // a list which holds the stars in the background
        private Stars[] m_stars;

        // the vertex array for stars
        private VertexPositionColorTexture[] m_starVertices;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the object.
        /// </summary>
        //=========================================================================================

        public Sky() : base(false,false,false)
        {
            // Set default depth:

            Depth = 50000;        
        }

        /// <summary>
        /// sets up vertices and position etc
        /// </summary>
        private void SetupStars()
        {
            int numStars = 20;

            m_stars = new Stars[numStars];

            m_starVertices = new VertexPositionColorTexture[numStars * 6];


           Vector2 viewArea = Core.Level.Renderer.Camera.ViewArea;

            // go through all of our stars
            for (int count = 0; count < numStars; count++)
            {
                // set our position
                m_stars[count].Position.X = Core.Random.Next(0,(int)viewArea.X);
                m_stars[count].Position.Y = Core.Random.Next(0, (int)viewArea.Y);

                // set our colour
                m_stars[count].Color.X = (float)Core.Random.NextDouble();
                m_stars[count].Color.Y = (float)Core.Random.NextDouble();
                m_stars[count].Color.Z = (float)Core.Random.NextDouble();
                m_stars[count].Color.W = 1;

                //set the size of the star, will be square shaped, we dont want elongated stars.
                int scale = Core.Random.Next(2,6);
                m_stars[count].Scale = new Vector2(scale,scale);


            }
        }

        /// <summary>
        /// update the star vertices
        /// </summary>
        private void UpdateVertices()
        {
            //set up our vertices 
            for (int i = 0, vertexCount = 0; i < m_stars.Length; i++, vertexCount += 6)
            {


                m_starVertices[vertexCount].Position.X = m_stars[i].Position.X - m_stars[i].Scale.X;
                m_starVertices[vertexCount + 1].Position.X = m_stars[i].Position.X - m_stars[i].Scale.X;
                m_starVertices[vertexCount + 2].Position.X = m_stars[i].Position.X + m_stars[i].Scale.X;
                m_starVertices[vertexCount + 3].Position.X = m_stars[i].Position.X - m_stars[i].Scale.X;
                m_starVertices[vertexCount + 4].Position.X = m_stars[i].Position.X + m_stars[i].Scale.X;
                m_starVertices[vertexCount + 5].Position.X = m_stars[i].Position.X + m_stars[i].Scale.X;


                m_starVertices[vertexCount].Position.Y = m_stars[i].Position.Y - m_stars[i].Scale.Y;
                m_starVertices[vertexCount + 1].Position.Y = m_stars[i].Position.Y + m_stars[i].Scale.Y;
                m_starVertices[vertexCount + 2].Position.Y = m_stars[i].Position.Y + m_stars[i].Scale.Y;
                m_starVertices[vertexCount + 3].Position.Y = m_stars[i].Position.Y - m_stars[i].Scale.Y;
                m_starVertices[vertexCount + 4].Position.Y = m_stars[i].Position.Y + m_stars[i].Scale.Y;
                m_starVertices[vertexCount + 5].Position.Y = m_stars[i].Position.Y - m_stars[i].Scale.Y;
            }

        }

        //=========================================================================================
        /// <summary>
        /// Draws the sprite.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // If we are missing an effect or texture then abort:

            if ( Texture == null ) return;
            if ( Effect  == null ) return;

            // Get the camera:

            Camera cam = Core.Level.Renderer.Camera; 

            // Get the area of the screen:

            Vector2 screen_area = cam.ViewArea;

            // Get view camera transforms:
            
            Matrix view_projection = cam.View * cam.Projection;

            // Disable z writes:

            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = false;

            // Disable texture clamping:

            Core.Graphics.Device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            Core.Graphics.Device.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

            // Set the position of all the sprites vertices:

            Vertices[0].Position.X = cam.Position.X;
            Vertices[1].Position.X = cam.Position.X;
            Vertices[2].Position.X = cam.Position.X;
            Vertices[3].Position.X = cam.Position.X;

            Vertices[0].Position.Y = cam.Position.Y;
            Vertices[1].Position.Y = cam.Position.Y;
            Vertices[2].Position.Y = cam.Position.Y;
            Vertices[3].Position.Y = cam.Position.Y;

            // Make the sprite twice the size of the screen area:

            Vertices[0].Position.X -= screen_area.X;
            Vertices[1].Position.X -= screen_area.X;
            Vertices[2].Position.X += screen_area.X;
            Vertices[3].Position.X += screen_area.X;

            Vertices[0].Position.Y += screen_area.Y;
            Vertices[1].Position.Y -= screen_area.Y;
            Vertices[2].Position.Y += screen_area.Y;
            Vertices[3].Position.Y -= screen_area.Y;

            // Make the sky at maximum depth

            Vertices[0].Position.Z = - Camera.Z_MAXIMUM;
            Vertices[1].Position.Z = - Camera.Z_MAXIMUM;
            Vertices[2].Position.Z = - Camera.Z_MAXIMUM;
            Vertices[3].Position.Z = - Camera.Z_MAXIMUM;

            // Fix the texture coordinates to be aspect correct:

            float aspect = screen_area.X / screen_area.Y;

            Vertices[2].TextureCoordinate.X = aspect;
            Vertices[3].TextureCoordinate.X = aspect;

            // Set the texture on the shader:

            EffectParameter param_tex = Effect.Parameters[ "Texture"               ];
            EffectParameter param_wvp = Effect.Parameters[ "WorldViewProjection"   ];

            if ( param_tex != null ) param_tex.SetValue(m_texture);
            if ( param_wvp != null ) param_wvp.SetValue(view_projection);

            // Begin drawing with the shader:

            Effect.Begin();

                // Set vertex declaration on graphics device:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

                // Begin pass:

                Effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the sky:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleStrip ,
                        Vertices                    ,
                        0                           ,
                        2
                    );

                // End pass:

                Effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            Effect.End();

            // Restore z writes:

            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = GraphicsSystem.DefaultRenderState.DepthBufferWriteEnable;

            // Renable texture clamping:

            Core.Graphics.Device.SamplerStates[0].AddressU = GraphicsSystem.DefaultSamplerState.AddressU;
            Core.Graphics.Device.SamplerStates[0].AddressV = GraphicsSystem.DefaultSamplerState.AddressV;
        }

        //=========================================================================================
        /// <summary>
        /// This function allows the game object to tell if it is visible with the given camera 
        /// view. This is used by the renderer for vis testing.
        /// </summary>
        /// <param name="c"> Camera the scene is being viewed from. </param>
        /// <returns> True if the object should be drawn, false otherwise. </returns>
        //=========================================================================================

        public override bool IsVisible(Camera c)
        {
            // The sky is always visible:

            return true;
        }

    }   // end of class

}   // end of namespace
