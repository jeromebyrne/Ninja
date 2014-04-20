#if DEBUG

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
    //
    /// <summary>
    /// Test elipse that can be moved around to check collision detection. 
    /// </summary>
    // 
    //#############################################################################################

    class TestEllipse : Sprite
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

        /// <summary> Direction the test elipse should cast to for testing ray casts \ line intersections </summary>

        private Vector2 m_test_cast_dir = Vector2.Zero;

        /// <summary> Amount to rotate the test elipse by. </summary>

        private float m_rotation = 0;

        /// <summary> The last batch of collision detection results </summary>
        
        private LinkedList<CollisionQueryResult> m_last_collision_results = new LinkedList<CollisionQueryResult>();

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates this object.
        /// </summary>
        //=========================================================================================

        public TestEllipse() : base(false,true,false){ Depth = 0; }

        //=========================================================================================
        /// <summary>
        /// Called when the object is to have it's state updated. Objects should move according 
        /// to the values in the global timing system or do logic here.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Move speed for test elipse:

            const float MOVE_SPEED = 2.5f;

            // Rotation speed for test elipse:

            const float ROTATE_SPEED = 0.05f;

            // Get keyboard state:

            KeyboardState keyboard_state = Keyboard.GetState();

            // See what keys are pressed:

            if (keyboard_state.IsKeyDown(Keys.Up)       ) PositionY += MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.Down)     ) PositionY -= MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.Left)     ) PositionX -= MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.Right)    ) PositionX += MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.NumPad8)  ) m_test_cast_dir.Y += MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.NumPad2)  ) m_test_cast_dir.Y -= MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.NumPad4)  ) m_test_cast_dir.X -= MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.NumPad6)  ) m_test_cast_dir.X += MOVE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.NumPad7)  ) m_rotation += ROTATE_SPEED;
            if (keyboard_state.IsKeyDown(Keys.NumPad9)  ) m_rotation -= ROTATE_SPEED;

            // Do collision detection:

            Core.Level.Collision.Collide( Position, BoxDimensions, m_rotation , this );

            // Save all the results:

            m_last_collision_results.Clear();

            for ( int i = 0 ; i < Core.Level.Collision.CollisionResultCount ; i++ )
            {
                m_last_collision_results.AddLast( Core.Level.Collision.CollisionResults[i] );
            }

            // See if we have to move:

            if ( Core.Level.Collision.CollisionResultCount > 0 )
            {
                // Pick the greatest penetration:

                int biggest_p_index = 0; float biggest_p_distance = 0;

                for (int i = 0; i < Core.Level.Collision.CollisionResultCount; i++)
                {
                    if (Core.Level.Collision.CollisionResults[i].Penetration > biggest_p_distance)
                    {
                        // Save penetration: bigger

                        biggest_p_distance = Core.Level.Collision.CollisionResults[i].Penetration;

                        // Save index:

                        biggest_p_index = i;
                    }
                }

                // Move back along the greatest penetration:

                Position += Core.Level.Collision.CollisionResults[biggest_p_index].ResolveDirection * Core.Level.Collision.CollisionResults[biggest_p_index].Penetration;
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

            // Make the camera follow the ellipse:

            Core.Level.Renderer.Camera.Position = Position;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Make an array of vertices to draw the sprite:

            VertexPositionColorTexture[] m_vertices = new VertexPositionColorTexture[4];

            // Sprite points relative to sprite center

            m_vertices[0].Position.X = - BoxDimensions.X;
            m_vertices[1].Position.X = - BoxDimensions.X;
            m_vertices[2].Position.X = + BoxDimensions.X;
            m_vertices[3].Position.X = + BoxDimensions.X;

            m_vertices[0].Position.Y = + BoxDimensions.Y;
            m_vertices[1].Position.Y = - BoxDimensions.Y;
            m_vertices[2].Position.Y = + BoxDimensions.Y;
            m_vertices[3].Position.Y = - BoxDimensions.Y;

            // Rotate the sprite:

            Matrix rot = Matrix.CreateRotationZ( m_rotation );

            m_vertices[0].Position = Vector3.Transform( m_vertices[0].Position , rot );
            m_vertices[1].Position = Vector3.Transform( m_vertices[1].Position , rot );
            m_vertices[2].Position = Vector3.Transform( m_vertices[2].Position , rot );
            m_vertices[3].Position = Vector3.Transform( m_vertices[3].Position , rot );

            // Set the texture coordinates for each vertex:

            m_vertices[0].TextureCoordinate = Vector2.Zero;
            m_vertices[1].TextureCoordinate = Vector2.UnitY;
            m_vertices[2].TextureCoordinate = Vector2.UnitX;
            m_vertices[3].TextureCoordinate = Vector2.One;

            // Set the colors for each vertex:

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;     

            // Move all vertices into z position:

            m_vertices[0].Position.X += Position.X;
            m_vertices[1].Position.X += Position.X;
            m_vertices[2].Position.X += Position.X;
            m_vertices[3].Position.X += Position.X;

            m_vertices[0].Position.Y += Position.Y;
            m_vertices[1].Position.Y += Position.Y;
            m_vertices[2].Position.Y += Position.Y;
            m_vertices[3].Position.Y += Position.Y;

            // Set the z component of the vertices according to the depth of the sprite:

            m_vertices[0].Position.Z = -Depth;
            m_vertices[1].Position.Z = -Depth;
            m_vertices[2].Position.Z = -Depth;
            m_vertices[3].Position.Z = -Depth;

            // Set the texture on the shader:

            EffectParameter param_tex = Effect.Parameters[ "Texture"               ];
            EffectParameter param_wvp = Effect.Parameters[ "WorldViewProjection"   ];

            if ( param_tex != null ) param_tex.SetValue(Texture);
            if ( param_wvp != null ) param_wvp.SetValue(view_projection);

            // Begin drawing with the shader:

            Effect.Begin();

                // Set vertex declaration on graphics device:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

                // Begin pass:

                Effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleStrip ,
                        m_vertices                  ,
                        0                           ,
                        2
                    );

                // End pass:

                Effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            Effect.End();

            // Make sure there are collision results before proceeding:

            if ( m_last_collision_results.Count <= 0 ) return;
            
            // Make sure the debug shader is active:

            if ( Core.DebugShader == null ) return;

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

            Core.Graphics.Device.RenderState.DepthBufferEnable = false;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = false;

            // Set point size:

            Core.Graphics.Device.RenderState.PointSize = 6;

            // Make a list of collision contact points to draw:

            VertexPositionColor[] collision_vertices = new VertexPositionColor[ m_last_collision_results.Count ];

            // Fill the list:

            {
                int point_num = 0; 

                foreach ( CollisionQueryResult result in m_last_collision_results )
                {
                    collision_vertices[point_num].Position = new Vector3( result.Point , 0 );
                    collision_vertices[point_num].Color = Color.Magenta;

                    point_num++;
                }
            }

            // Begin drawing with the debug shader:

            Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.PointList         ,
                        collision_vertices              ,
                        0                               ,
                        m_last_collision_results.Count
                    );

            // End drawing with the shader:

            Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End();

            // Renable z buffer

            Core.Graphics.Device.RenderState.DepthBufferEnable = GraphicsSystem.DefaultRenderState.DepthBufferEnable;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable = GraphicsSystem.DefaultRenderState.DepthBufferWriteEnable;

            // Restore point size:

            Core.Graphics.Device.RenderState.PointSize = GraphicsSystem.DefaultRenderState.PointSize;
        }

        //=========================================================================================
        /// <summary>
        /// Does debug drawing for this object
        /// </summary>
        //=========================================================================================

        #if DEBUG

        public override void OnDebugDraw()
        {
            // Call the base function first:

            base.OnDebugDraw();

            // Do a raycast:

            Core.Level.Collision.Intersect( Position , Position + m_test_cast_dir , this );

            // The closest point in the raycast:

            Vector2 closest_point = Vector2.Zero; 

            // The closest distance in the raycast:

            float closest_distance = float.PositiveInfinity;

            // See if there were any raycast results:

            if ( Core.Level.Collision.IntersectResultCount > 0 )
            {
                for ( int i = 0 ; i < Core.Level.Collision.IntersectResultCount ; i++ )
                {
                    // See if this intersect point is closer:

                    if ( Core.Level.Collision.IntersectResults[i].PointDistance < closest_distance )
                    {
                        // Closer intersect point: save it's distance

                        closest_distance = Core.Level.Collision.IntersectResults[i].PointDistance;

                        // Save the point

                        closest_point = Core.Level.Collision.IntersectResults[i].Point;
                    }
                }
            }
            else
            {
                // No intersection: set the closest point to where the ray normally ends:

                closest_point = Position + m_test_cast_dir;
            }

            // Do nothing if no debug shader:

            if ( Core.DebugShader == null ) return;

            // Array of vertices used for drawing:

            VertexPositionColor[] vertices = new VertexPositionColor[2];

            // Set the colors of the vertices:

            vertices[0].Color = Color.White;
            vertices[1].Color = Color.White;

            // Draw the start and end points of the test ray:

            vertices[0].Position = new Vector3 ( Position , 0 );
            vertices[1].Position = new Vector3 ( closest_point , 0 );

            // Get the world view projection matrix:

            Matrix world_view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Set the transform matrix on the shader:

            EffectParameter param = Core.DebugShader.Parameters["WorldViewProjection"];

            if ( param != null ) 
            {
                param.SetValue( world_view_projection );
            }

            // Set graphics device vertex declaration

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;

            // Disable depth tests and writes:

            Core.Graphics.Device.RenderState.DepthBufferEnable          = false;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable     = false;

            // Draw all the lines:

            Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList,vertices,0,1);

            Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End(); 

            // Restore depth tests and writes to the default settings

            Core.Graphics.Device.RenderState.DepthBufferEnable          = GraphicsSystem.DefaultRenderState.DepthBufferEnable;
            Core.Graphics.Device.RenderState.DepthBufferWriteEnable     = GraphicsSystem.DefaultRenderState.DepthBufferWriteEnable;

        }

        #endif

    }   // end of class

}   // end of namespace

#endif  // #if DEBUG