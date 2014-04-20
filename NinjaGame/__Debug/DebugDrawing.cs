#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Some helper functions to aid with debug drawing.
    /// </summary>
    //#############################################################################################

    public class DebugDrawing
    {
        //=========================================================================================
        /// <summary>
        /// Draws a line with the given world coordinates.
        /// </summary>
        /// <param name="s"> Start of the line.         </param>
        /// <param name="e"> End of the line.           </param>
        /// <param name="c"> Color to draw the line in. </param>
        //=========================================================================================

        public static void DrawWorldLine( Vector2 s , Vector2 e , Color c )
        {
           // Make sure the debug shader is active:

            if ( Core.DebugShader == null ) return;

            // Get the combined viewing and projection matrix of the camera:

            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Set the transform on the debug shader:

            {
                // Grab this param

                EffectParameter param = Core.DebugShader.Parameters["WorldViewProjection"];

                // Set transform:

                if ( param != null ) param.SetValue(view_projection);
            }

            // Set vertex declaraion on graphics device:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;            

            // Disable z buffer

            Core.Graphics.Device.RenderState.DepthBufferEnable      = false;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = false;

            // Make a list to draw the line:

            VertexPositionColor[] vertices = new VertexPositionColor[2];

            // Fill the list:

            vertices[0].Position    = new Vector3(s,0);
            vertices[1].Position    = new Vector3(e,0);
            vertices[0].Color       = c;
            vertices[1].Color       = c;

            // Begin drawing with the debug shader:

            Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.LineList  ,
                        vertices                ,
                        0                       ,
                        1
                    );

            // End drawing with the shader:

            Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End();

            // Renable z buffer

            Core.Graphics.Device.RenderState.DepthBufferEnable      = GraphicsSystem.DefaultRenderState.DepthBufferEnable;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = GraphicsSystem.DefaultRenderState.DepthBufferWriteEnable;
        }

        //=========================================================================================
        /// <summary>
        /// Draws a rectangle with the given world coordinates.
        /// </summary>
        /// <param name="p"> Position of rectangle center         </param>
        /// <param name="s"> Size of rectangle from it's center.  </param>
        /// <param name="c"> Color to draw the rectangle in.        </param>
        //=========================================================================================

        public static void DrawWorldRectangle( Vector2 p , Vector2 s , Color c )
        {
           // Make sure the debug shader is active:

            if ( Core.DebugShader == null ) return;

            // Get the combined viewing and projection matrix of the camera:

            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Set the transform on the debug shader:

            {
                // Grab this param

                EffectParameter param = Core.DebugShader.Parameters["WorldViewProjection"];

                // Set transform:

                if ( param != null ) param.SetValue(view_projection);
            }

            // Set vertex declaraion on graphics device:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;            

            // Disable z buffer

            Core.Graphics.Device.RenderState.DepthBufferEnable      = false;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = false;

            // Make a list to draw the lines in the rectangle:

            VertexPositionColor[] vertices = new VertexPositionColor[8];

            // Fill the list:

            vertices[0].Position    = new Vector3( p.X - s.X , p.Y + s.Y , -2 );
            vertices[1].Position    = new Vector3( p.X + s.X , p.Y + s.Y , -2 );
            vertices[2].Position    = new Vector3( p.X + s.X , p.Y + s.Y , -2 );
            vertices[3].Position    = new Vector3( p.X + s.X , p.Y - s.Y , -2 );
            vertices[4].Position    = new Vector3( p.X + s.X , p.Y - s.Y , -2 );
            vertices[5].Position    = new Vector3( p.X - s.X , p.Y - s.Y , -2 );
            vertices[6].Position    = new Vector3( p.X - s.X , p.Y - s.Y , -2 );
            vertices[7].Position    = new Vector3( p.X - s.X , p.Y + s.Y , -2 );

            vertices[0].Color       = c;
            vertices[1].Color       = c;
            vertices[2].Color       = c;
            vertices[3].Color       = c;
            vertices[4].Color       = c;
            vertices[5].Color       = c;
            vertices[6].Color       = c;
            vertices[7].Color       = c;

            // Begin drawing with the debug shader:

            Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.LineList  ,
                        vertices                ,
                        0                       ,
                        4
                    );

            // End drawing with the shader:

            Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End();

            // Renable z buffer

            Core.Graphics.Device.RenderState.DepthBufferEnable      = GraphicsSystem.DefaultRenderState.DepthBufferEnable;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = GraphicsSystem.DefaultRenderState.DepthBufferWriteEnable;
        }

        //=========================================================================================
        /// <summary>
        /// Draws a rectangle with the given gui coordinates.
        /// </summary>
        /// <param name="p"> Position of rectangle center         </param>
        /// <param name="s"> Size of rectangle from it's center.  </param>
        /// <param name="c"> Color to draw the rectangle in.        </param>
        //=========================================================================================

        public static void DrawGuiRectangle( Vector2 p , Vector2 s , Color c )
        {
           // Make sure the debug shader is active:

            if ( Core.DebugShader == null ) return;

            // Get the combined viewing and projection matrix of the camera:

            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Set the transform on the debug shader:

            {
                // Grab this param

                EffectParameter param = Core.DebugShader.Parameters["WorldViewProjection"];

                // Set transform:

                if ( param != null ) param.SetValue(view_projection);
            }

            // Set vertex declaraion on graphics device:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;            

            // Disable z buffer

            Core.Graphics.Device.RenderState.DepthBufferEnable      = false;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = false;

            // Make a list to draw the lines in the rectangle:

            VertexPositionColor[] vertices = new VertexPositionColor[8];

            // Fill the list:

            vertices[0].Position    = new Vector3( p.X - s.X , p.Y + s.Y , -2 );
            vertices[1].Position    = new Vector3( p.X + s.X , p.Y + s.Y , -2 );
            vertices[2].Position    = new Vector3( p.X + s.X , p.Y + s.Y , -2 );
            vertices[3].Position    = new Vector3( p.X + s.X , p.Y - s.Y , -2 );
            vertices[4].Position    = new Vector3( p.X + s.X , p.Y - s.Y , -2 );
            vertices[5].Position    = new Vector3( p.X - s.X , p.Y - s.Y , -2 );
            vertices[6].Position    = new Vector3( p.X - s.X , p.Y - s.Y , -2 );
            vertices[7].Position    = new Vector3( p.X - s.X , p.Y + s.Y , -2 );

            vertices[0].Color       = c;
            vertices[1].Color       = c;
            vertices[2].Color       = c;
            vertices[3].Color       = c;
            vertices[4].Color       = c;
            vertices[5].Color       = c;
            vertices[6].Color       = c;
            vertices[7].Color       = c;

            // Begin drawing with the debug shader:

            Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.LineList  ,
                        vertices                ,
                        0                       ,
                        4
                    );

            // End drawing with the shader:

            Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End();

            // Renable z buffer

            Core.Graphics.Device.RenderState.DepthBufferEnable      = GraphicsSystem.DefaultRenderState.DepthBufferEnable;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = GraphicsSystem.DefaultRenderState.DepthBufferWriteEnable;
        }

    }   // end of class

}   // end of namspace

#endif  // #if DEBUG