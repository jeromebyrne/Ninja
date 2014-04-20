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

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary>
    /// Class representing the ground in the game. The ground is flat and composed of three layers
    /// of ground textures. Nothing can pass through the ground.
    /// </summary>
    // 
    //#############################################################################################

    class Ground : GameObject
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Top ground texture </summary>

            private Texture2D m_top_texture = null;

            /// <summary> Name of the top ground texture </summary>

            private string m_top_texture_name = "";

            /// <summary> Top ground texture normal map </summary>

            private Texture2D m_top_texture_normal_map = null;

            /// <summary> Name of the top ground texture normal map </summary>

            private string m_top_texture_normal_map_name = "";

            /// <summary> Middle ground texture </summary>

            private Texture2D m_middle_texture = null;

            /// <summary> Name of the bottom ground texture </summary>

            private string m_middle_texture_name = "";

            /// <summary> Middle ground texture normal map </summary>

            private Texture2D m_middle_texture_normal_map = null;

            /// <summary> Name of the bottom ground texture </summary>

            private string m_middle_texture_normal_map_name = "";

            /// <summary> Shader used to render the ground </summary>

            private Effect m_effect = null;

            /// <summary> Name of the shader used to render the ground </summary>

            private string m_effect_name = "";

            /// <summary> Vertices used for rendering. </summary>

            private VertexPositionColorTexture[] m_vertices = null;

            /// <summary> Number of texture repeats in the x direction for the ground </summary>
            
            private float m_tex_repeats_x = 1.0f;

            /// <summary> Line representing the plane of the ground </summary>
        
            private Line m_line = null;

        //=========================================================================================
        /// <summary>
        /// Creates a ground object.
        /// </summary>
        //=========================================================================================

        public Ground() : base(true,true,false,false)
        {
            // Make vertex declaration and vertices 

            m_vertices = new VertexPositionColorTexture[6];

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;
            m_vertices[4].Color = Color.White;
            m_vertices[5].Color = Color.White;

            // Set default depth:

            Depth = 1000;
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

            data.ReadTexture( "TopTexture"              , ref m_top_texture_name                , ref m_top_texture                 , "Graphics\\default" );
            data.ReadTexture( "TopTextureNormalMap"     , ref m_top_texture_normal_map_name     , ref m_top_texture_normal_map      , "Graphics\\default" );
            data.ReadTexture( "MiddleTexture"           , ref m_middle_texture_name             , ref m_middle_texture              , "Graphics\\default" );
            data.ReadTexture( "MiddleTextureNormalMap"  , ref m_middle_texture_normal_map_name  , ref m_middle_texture_normal_map   , "Graphics\\default" );
            data.ReadEffect ( "Effect"                  , ref m_effect_name                     , ref m_effect                      , "Effects\\default"  );

            data.ReadFloat("TextureRepeatsX" , ref m_tex_repeats_x );

            // Get height of the top texture:

            int top_tex_h = 0; 
            
            if ( m_top_texture != null )
            {
                top_tex_h = m_top_texture.Height;
            }

            // Create the line representing the plane of the ground: where the top texture ends

            m_line = new Line
            ( 
                PositionX - BoxDimensionsX              , 
                PositionY + BoxDimensionsY - top_tex_h  ,
                PositionX + BoxDimensionsX              , 
                PositionY + BoxDimensionsY - top_tex_h
            );
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
            // Call base class function first

            base.WriteXml(data);

            // Write all data:

            data.Write( "TopTexture"                , m_top_texture_name                );
            data.Write( "TopTextureNormalMap"       , m_top_texture_normal_map_name     );
            data.Write( "MiddleTexture"             , m_middle_texture_name             );
            data.Write( "MiddleTextureNormalMap"    , m_middle_texture_normal_map_name  );
            data.Write( "Effect"                    , m_effect_name                     );

            data.Write("TextureRepeatsX" , m_tex_repeats_x );
        }

        //=========================================================================================
        /// <summary>
        /// Draw function. Draws the ground.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // Abort if missing shader:

            if ( m_effect == null ) return;

            // Set vertex declaration on graphics device:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

            // Set the transform on the shader:

            EffectParameter param = m_effect.Parameters["WorldViewProjection"];

            if ( param != null )
            {
                param.SetValue( Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection );
            }

            // Set the depth of all the vertices:

            m_vertices[0].Position.Z = 0;
            m_vertices[1].Position.Z = 0;
            m_vertices[2].Position.Z = 0;
            m_vertices[3].Position.Z = 0;
            m_vertices[4].Position.Z = 0;
            m_vertices[5].Position.Z = 0;

            // Disable clamping in the x direction:

            SamplerState sampler_state1 = Core.Graphics.Device.SamplerStates[0];
            SamplerState sampler_state2 = Core.Graphics.Device.SamplerStates[1];

            sampler_state1.AddressU = TextureAddressMode.Wrap;
            sampler_state2.AddressU = TextureAddressMode.Wrap;

            // This fix is required to stop issues on the xbox: otherwise the normal map will jump around

            #if XBOX360

                sampler_state2.AddressV = GraphicsSystem.DefaultSamplerState.AddressV;
                sampler_state2.AddressW = GraphicsSystem.DefaultSamplerState.AddressW;

                sampler_state2.MinFilter = GraphicsSystem.DefaultSamplerState.MinFilter;
                sampler_state2.MagFilter = GraphicsSystem.DefaultSamplerState.MagFilter;
                sampler_state2.MipFilter = GraphicsSystem.DefaultSamplerState.MipFilter;

            #endif            

            // Draw the top layer:

            if ( m_top_texture != null )
            {
                // Set position, color and texture coordinates of all the vertices:

                m_vertices[0].Position.X = PositionX - BoxDimensionsX;
                m_vertices[1].Position.X = PositionX + BoxDimensionsX;
                m_vertices[2].Position.X = PositionX + BoxDimensionsX;
                m_vertices[3].Position.X = PositionX + BoxDimensionsX;
                m_vertices[4].Position.X = PositionX - BoxDimensionsX;
                m_vertices[5].Position.X = PositionX - BoxDimensionsX;

                m_vertices[0].Position.Y = PositionY + BoxDimensionsY;
                m_vertices[1].Position.Y = PositionY + BoxDimensionsY;
                m_vertices[2].Position.Y = PositionY + BoxDimensionsY - m_top_texture.Height;
                m_vertices[3].Position.Y = PositionY + BoxDimensionsY - m_top_texture.Height;
                m_vertices[4].Position.Y = PositionY + BoxDimensionsY - m_top_texture.Height;
                m_vertices[5].Position.Y = PositionY + BoxDimensionsY;

                m_vertices[0].TextureCoordinate.Y = 0;
                m_vertices[1].TextureCoordinate.Y = 0;
                m_vertices[2].TextureCoordinate.Y = 1;
                m_vertices[3].TextureCoordinate.Y = 1;
                m_vertices[4].TextureCoordinate.Y = 1;
                m_vertices[5].TextureCoordinate.Y = 0;

                m_vertices[0].TextureCoordinate.X = 0;
                m_vertices[1].TextureCoordinate.X = m_tex_repeats_x;
                m_vertices[2].TextureCoordinate.X = m_tex_repeats_x;
                m_vertices[3].TextureCoordinate.X = m_tex_repeats_x;                
                m_vertices[4].TextureCoordinate.X = 0;
                m_vertices[5].TextureCoordinate.X = 0;

                // Set the texture on the effect chosen:

                param = m_effect.Parameters["Texture"];

                if ( param != null ) param.SetValue( m_top_texture );

                // Set the normal map on the effect chosen:

                param = m_effect.Parameters["NormalMap"];

                if ( param != null ) param.SetValue( m_top_texture_normal_map );

                // Set the camera position on the effect chosen:

                param = m_effect.Parameters["CameraPosition"];

                if ( param != null ) param.SetValue( Core.Level.Renderer.Camera.Position );

                // Draw the ground:

                m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleList  ,
                        m_vertices                  ,
                        0                           ,
                        2
                    );

                m_effect.CurrentTechnique.Passes[0].End(); m_effect.End(); 
            }

            // Draw the middle layer:

            if ( m_middle_texture != null )
            {
                // Get the height of the top texture, if any:

                int top_tex_height = 0; 
                
                if ( m_top_texture != null ) 
                {
                    top_tex_height = m_top_texture.Height;
                }

                // Set position, color and texture coordinates of all the vertices:

                m_vertices[0].Position.X = PositionX - BoxDimensionsX;
                m_vertices[1].Position.X = PositionX + BoxDimensionsX;
                m_vertices[2].Position.X = PositionX + BoxDimensionsX;
                m_vertices[3].Position.X = PositionX + BoxDimensionsX;
                m_vertices[4].Position.X = PositionX - BoxDimensionsX;
                m_vertices[5].Position.X = PositionX - BoxDimensionsX;

                m_vertices[0].Position.Y = PositionY + BoxDimensionsY - top_tex_height;
                m_vertices[1].Position.Y = PositionY + BoxDimensionsY - top_tex_height;
                m_vertices[2].Position.Y = PositionY + BoxDimensionsY - m_middle_texture.Height - top_tex_height;
                m_vertices[3].Position.Y = PositionY + BoxDimensionsY - m_middle_texture.Height - top_tex_height;
                m_vertices[4].Position.Y = PositionY + BoxDimensionsY - m_middle_texture.Height - top_tex_height;
                m_vertices[5].Position.Y = PositionY + BoxDimensionsY - top_tex_height;

                m_vertices[0].TextureCoordinate.Y = 0;
                m_vertices[1].TextureCoordinate.Y = 0;
                m_vertices[2].TextureCoordinate.Y = 1;
                m_vertices[3].TextureCoordinate.Y = 1;
                m_vertices[4].TextureCoordinate.Y = 1;
                m_vertices[5].TextureCoordinate.Y = 0;

                m_vertices[0].TextureCoordinate.X = 0;
                m_vertices[1].TextureCoordinate.X = m_tex_repeats_x;
                m_vertices[2].TextureCoordinate.X = m_tex_repeats_x;
                m_vertices[3].TextureCoordinate.X = m_tex_repeats_x;                    
                m_vertices[4].TextureCoordinate.X = 0;
                m_vertices[5].TextureCoordinate.X = 0;

                // Set the texture on the effect chosen:

                param = m_effect.Parameters["Texture"];

                if ( param != null ) param.SetValue( m_middle_texture );

                // Set the normal map on the effect chosen:

                param = m_effect.Parameters["NormalMap"];

                if ( param != null ) param.SetValue( m_middle_texture_normal_map );

                // Set the camera position on the effect chosen:

                param = m_effect.Parameters["CameraPosition"];

                if ( param != null ) param.SetValue( Core.Level.Renderer.Camera.Position );

                // Draw the ground:

                m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleList  ,
                        m_vertices                  ,
                        0                           ,
                        2
                    );

                m_effect.CurrentTechnique.Passes[0].End(); m_effect.End();
            }

            // Restore clamping to what it was:

            sampler_state1.AddressU = GraphicsSystem.DefaultSamplerState.AddressU;
            sampler_state2.AddressU = GraphicsSystem.DefaultSamplerState.AddressU;
        }

        //=====================================================================================
        /// <summary>
        /// Intersect query function. If an object is collideable it should also implement code to 
        /// do a line / line intersection test here. It should return all of the intersections 
        /// found.
        /// </summary>
        /// <param name="lineStart">        Start point of the line involved.               </param>
        /// <param name="lineEnd">          End point of the line involved.                 </param>
        /// <param name="otherObject">      Object making the collision query, may be null. </param>
        /// <param name="results">          Array to save the results to.                   </param>
        /// <param name="results_index">    Index in the array to save the results to.      </param>
        /// <returns>                       Number of results from the intersection test.   </returns>
        //=====================================================================================

        public override int OnIntersectQuery
        ( 
            Vector2                 lineStart           ,
            Vector2                 lineEnd             ,
            GameObject              otherObject         ,      
            IntersectQueryResult[]  results             ,
            int                     results_index
        )
        {
            // Abort if no room to save new result:

            if ( results_index >= results.Length ) return 0;

            // Store the result here:

            Vector2 intersect_point = Vector2.Zero;               

            // Do it:

            bool intersection = m_line.IntersectInfinite( new Line(lineStart,lineEnd) , ref intersect_point );

            // See if there was an intersection:

            if ( intersection )
            {
                // Make up the result:

                IntersectQueryResult result;

                result.ValidResult      = true;
                result.QueryObject      = this;
                result.Point            = intersect_point;
                result.Normal           = Vector2.UnitY;
                result.PointDistance    = Vector2.Distance(lineStart,intersect_point);

                // Save the result:

                results[results_index] = result;

                // Got a result: return 1

                return 1;
            }
            else
            {
                // No result:

                return 0;
            }
        }

        //=====================================================================================
        /// <summary>
        /// Collision query function. If an object is collideable it should implement it's collision 
        /// testing code here and return the result of the collision. This collision test is for 
        /// a bounding elipse against a piece of geometry, such as a collection of lines etc. 
        /// </summary>
        /// <param name="elipsePosition">   Position of the bounding elipse                 </param>
        /// <param name="elipseDimensions"> Dimensions of the bounding elipse               </param>
        /// <param name="elipseRotation">   Amount the elipse is rotated by in radians.     </param>
        /// <param name="otherObject">      Object making the collision query, may be null. </param>
        /// <param name="results">          Array to save the results to.                   </param>
        /// <param name="results_index">    Index in the array to save the results to.      </param>
        /// <returns>                       Number of results from the collision.           </returns>
        //=====================================================================================

        public override int OnCollisionQuery
        ( 
            Vector2                 elipsePosition      ,
            Vector2                 elipseDimensions    ,
            float                   elipseRotation      ,
            GameObject              otherObject         ,
            CollisionQueryResult[]  results             ,
            int                     results_index
        )
        {
            // Get height of the top texture (if any). the ground plane is at the bottom of this.

            int top_tex_height = 0;

            if ( m_top_texture != null ) top_tex_height = m_top_texture.Height;

            // Take two points along the ground surface:

            Vector2 ground_p1 = new Vector2( elipsePosition.X - 10 , PositionY + BoxDimensionsY - top_tex_height );
            Vector2 ground_p2 = new Vector2( elipsePosition.X + 10 , PositionY + BoxDimensionsY - top_tex_height );

            // Transform into the correct form so that the ellipse is a circle, it's rotations are undone and it's center is the center of the world

            ground_p1 = Vector2.Transform( ground_p1 , LevelCollisionQuery.CollisionCache.ToEllipseLocal );
            ground_p2 = Vector2.Transform( ground_p2 , LevelCollisionQuery.CollisionCache.ToEllipseLocal );

            /* OLD UNOPTIMIZED COLLISION CODE: WHICH IS MANUALLY DOING THESE TRANSFORMS
             
                // Get both relative to the center of the elipse:

                ground_p1 = ground_p1 - elipsePosition;
                ground_p2 = ground_p2 - elipsePosition;

                // Rotate them according to the elipse rotation: we are essentially rotating the world so the elipse is not rotated anymore

                Matrix rot = Matrix.CreateRotationZ( - elipseRotation );

                ground_p1 = Vector2.Transform( ground_p1 , rot );
                ground_p2 = Vector2.Transform( ground_p2 , rot );

                // Scale the worlds y coordinates so that the elipse becomes a circle:

                float world_y_scale = elipseDimensions.X / elipseDimensions.Y;

                ground_p1.Y *= world_y_scale;
                ground_p2.Y *= world_y_scale;
             
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

            // Get the line normal:

            Vector2 ground_normal = new Vector2
            (
                - ( ground_p2.Y - ground_p1.Y  ) , 
                + ( ground_p2.X - ground_p1.X  )
            );

            ground_normal.Normalize();

            // Get distance to the ground:

            float distance = - Vector2.Dot( ground_p1 , ground_normal );

            // See if we are within range for a collision:

            if ( distance < elipseDimensions.X )
            {
                // Calculate penetration amount:

                float penetration = elipseDimensions.X - distance;

                // Calculate the collision point relative to the elipse center:

                Vector2 collision_point = - distance * ground_normal;

                // Calculate how much the elipse must move by to avoid collision:

                Vector2 resolve_vec = penetration * ground_normal;

                // Transform the collision point and resolve direction back into normal coordinates

                {
                    Vector4 v = Vector4.Zero;

                    v.X = resolve_vec.X;
                    v.Y = resolve_vec.Y;

                    v = Vector4.Transform( v , LevelCollisionQuery.CollisionCache.FromEllipseLocal );

                    resolve_vec.X = v.X;
                    resolve_vec.Y = v.Y;
                }

                collision_point = Vector2.Transform
                ( 
                    collision_point , 
                    LevelCollisionQuery.CollisionCache.FromEllipseLocal 
                );

                /* OLD UNOPTIMIZED COLLISION CODE: WHICH IS MANUALLY DOING THESE TRANSFORMS

                // Now undo the previous scaling:

                resolve_vec.Y       /= world_y_scale;
                collision_point.Y   /= world_y_scale;

                // Undo the previous rotation:

                Matrix rot_inverse = Matrix.CreateRotationZ(elipseRotation);

                    resolve_vec     = Vector2.Transform(resolve_vec,rot_inverse);
                    collision_point = Vector2.Transform(collision_point,rot_inverse);

                // Make collision point back to world coordinates

                collision_point += elipsePosition;
                 
                @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

                // Get penetration amount

                penetration = resolve_vec.Length();

                // If the penetration is very small then abort:

                if ( penetration < 0.001f ) return 0;

                // Normalise resolve direction:

                resolve_vec /= penetration;

                // Save the result:

                if ( results_index < results.Length )
                {
                    // Make up the result:

                    CollisionQueryResult c;
                    
                    c.ValidResult       = true;
                    c.QueryObject       = this;
                    c.Point             = collision_point;
                    c.Penetration       = penetration;
                    c.ResolveDirection  = resolve_vec;
                    c.Normal            = Vector2.UnitY;
                    c.IsPointCollision  = false;

                    // Save the result

                    results[results_index] = c;

                    // Got a valid result:

                    return 1;
                }
            }

            // If we got to here there was no result

            return 0;
        }
    }
}
