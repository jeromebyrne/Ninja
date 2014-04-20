using System;
using System.Xml;
using System.Collections;
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
    /// <summary>
    /// Similar to a regular sprite except that it is fully collideable. It loads it's collision 
    /// lines from a given XML file and allows other objects to collide with those lines. Beware 
    /// however that if the lines fall outside of the bounding box of the sprite, then they may not 
    /// be picked up in collision detection due to bounding box optimisations !!
    /// </summary>
    //#############################################################################################

    class SolidSprite : Sprite
    {
        //=========================================================================================
        // Variables
        //=========================================================================================
        
            /// <summary> Name of the file which stores the collision lines for the sprite. </summary>

            private string m_collision_lines_xml_file = "";

            /// <summary> The array of collision lines used by the object </summary>

            private Line[] m_collision_lines = null;

            /// <summary> How much to scale collision lines by when reading them in. </summary>

            private Vector2 m_collision_line_scale = Vector2.One;

            /// <summary> Reverse the normals of collision lines ?  </summary>

            private bool m_reverse_collision_normals = false; 

            /// <summary> Top left point of a bounding box surrounding all the collision lines for the sprite. </summary>

            private Vector2 m_lines_bb_top_left = Vector2.Zero;

            /// <summary> Top left point of a bounding box surrounding all the collision lines for the sprite. </summary>

            private Vector2 m_lines_bb_bottom_right = Vector2.Zero;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a solid sprite object.
        /// </summary>
        //=========================================================================================

        public SolidSprite() : base( true , false , false ){}

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
            // Call the base class function first:

            base.ReadXml(data);

            // Read the collision lines file name:

            data.ReadString( "CollisionLinesXmlFile" , ref m_collision_lines_xml_file , "Content\\CollisionLines\\default.xml" );

            // Read the scaling for the collision lines:

            data.ReadFloat( "CollisionLineScaleX" , ref m_collision_line_scale.X , 1 );
            data.ReadFloat( "CollisionLineScaleY" , ref m_collision_line_scale.Y , 1 );

            // Read this:

            data.ReadBool( "ReverseCollisionNormals" , ref m_reverse_collision_normals , false );

            // Load collision lines:

            ReadXmlCollisionLines();
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

            // Write all object data:

            data.Write( "CollisionLinesXmlFile"     , m_collision_lines_xml_file    );
            data.Write( "CollisionLineScaleX"       , m_collision_line_scale.X      );
            data.Write( "CollisionLineScaleY"       , m_collision_line_scale.Y      );
            data.Write( "ReverseCollisionNormals"   , m_reverse_collision_normals   );
        }

        //=========================================================================================
        /// <summary> 
        /// Attempts to read the collision lines file for the solid sprite.
        /// </summary>
        //=========================================================================================

        private void ReadXmlCollisionLines()
        {           
            // Clear the current array of collision lines:

            m_collision_lines = new Line[0];

            // Attempt to open the xml file:

            try
            {
                // Open the xml document and read:

                XmlDocument xml_doc = new XmlDocument(); xml_doc.Load( Locale.GetLocFile(m_collision_lines_xml_file) );

                // Store all the lines read here:

                List<Line> lines = new List<Line>();

                // Try and get the root data node:

                XmlNode xml_root = xml_doc.FirstChild;

                if ( xml_root != null )
                {
                    // Good: read each child node. Each child node should be a line

                    foreach ( XmlNode node in xml_root.ChildNodes )
                    {
                        // See if there are four child nodes: each child node should be a coordinate

                        if ( node.ChildNodes.Count >= 4 )
                        {
                            // Get all of the four nodes for the point coordinates:

                            XmlNode p1_x = node.ChildNodes[0];
                            XmlNode p1_y = node.ChildNodes[1];
                            XmlNode p2_x = node.ChildNodes[2];
                            XmlNode p2_y = node.ChildNodes[3];

                            // Convert all of those node's values to floats:

                            float x1 = Convert.ToSingle( p1_x.InnerText.Trim() , Locale.DevelopmentCulture.NumberFormat );
                            float y1 = Convert.ToSingle( p1_y.InnerText.Trim() , Locale.DevelopmentCulture.NumberFormat );
                            float x2 = Convert.ToSingle( p2_x.InnerText.Trim() , Locale.DevelopmentCulture.NumberFormat );
                            float y2 = Convert.ToSingle( p2_y.InnerText.Trim() , Locale.DevelopmentCulture.NumberFormat );

                            // Make a new line and add to the list: if the reverse normals flag is specified then reverse the line to reverse the normals:

                            if ( m_reverse_collision_normals )
                            {
                                lines.Add( new Line(x2,y2,x1,y1) );
                            }
                            else
                            {
                                lines.Add( new Line(x1,y1,x2,y2) );
                            }
                        }
                    }
                }

                // Save all of the lines:

                m_collision_lines = lines.ToArray();

                // Scale all lines by line scaling and position according to sprite position:

                for ( int i = 0 ; i < m_collision_lines.Length ; i++ )
                {
                    m_collision_lines[i].Start = m_collision_lines[i].Start * m_collision_line_scale + Position;
                    m_collision_lines[i].End   = m_collision_lines[i].End   * m_collision_line_scale + Position;
                }
            }

            // On windows debug show what happened if something went wrong
            
            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif

            // Calculate collision lines for the sprite:

            CalculateCollisionLineBoundingBox();

            // Disable points that are on the insides of concave angles:

            DisableConcavePoints();
        }

        //=========================================================================================
        /// <summary> 
        /// Calculates the bounding box for the collision lines in the sprite.
        /// </summary>
        //=========================================================================================

        private void CalculateCollisionLineBoundingBox()
        {
            // Set the bounding box points for all the lines:

            if ( m_collision_lines != null && m_collision_lines.Length > 0 )
            {
                // Have collision lines: find the minimum bounding box that will fit around all the lines

                m_lines_bb_top_left     = m_collision_lines[0].Start;
                m_lines_bb_bottom_right = m_collision_lines[0].Start;

                if ( m_collision_lines[0].End.X < m_lines_bb_top_left.X ) 
                {
                    m_lines_bb_top_left.X = m_collision_lines[0].End.X;
                }

                if ( m_collision_lines[0].End.X > m_lines_bb_bottom_right.X ) 
                {
                    m_lines_bb_bottom_right.X = m_collision_lines[0].End.X;
                }

                if ( m_collision_lines[0].End.Y > m_lines_bb_top_left.Y ) 
                {
                    m_lines_bb_top_left.Y = m_collision_lines[0].End.Y;
                }

                if ( m_collision_lines[0].End.X < m_lines_bb_bottom_right.Y ) 
                {
                    m_lines_bb_bottom_right.Y = m_collision_lines[0].End.Y;
                }

                // Run through all the lines:

                for ( int i = 1 ; i < m_collision_lines.Length ; i++ )
                {
                    if ( m_collision_lines[i].Start.X < m_lines_bb_top_left.X ) 
                    {
                        m_lines_bb_top_left.X = m_collision_lines[i].Start.X;
                    }

                    if ( m_collision_lines[i].End.X < m_lines_bb_top_left.X ) 
                    {
                        m_lines_bb_top_left.X = m_collision_lines[i].End.X;
                    }

                    if ( m_collision_lines[i].Start.X > m_lines_bb_bottom_right.X ) 
                    {
                        m_lines_bb_bottom_right.X = m_collision_lines[i].Start.X;
                    }

                    if ( m_collision_lines[i].End.X > m_lines_bb_bottom_right.X ) 
                    {
                        m_lines_bb_bottom_right.X = m_collision_lines[i].End.X;
                    }

                    if ( m_collision_lines[i].Start.Y < m_lines_bb_bottom_right.Y ) 
                    {
                        m_lines_bb_bottom_right.Y = m_collision_lines[i].Start.Y;
                    }

                    if ( m_collision_lines[i].End.Y < m_lines_bb_bottom_right.Y ) 
                    {
                        m_lines_bb_bottom_right.Y = m_collision_lines[i].End.Y;
                    }

                    if ( m_collision_lines[i].Start.Y > m_lines_bb_top_left.Y ) 
                    {
                        m_lines_bb_top_left.Y = m_collision_lines[i].Start.Y;
                    }

                    if ( m_collision_lines[i].End.Y > m_lines_bb_top_left.Y ) 
                    {
                        m_lines_bb_top_left.Y = m_collision_lines[i].End.Y;
                    }
                }

            }
            else
            {
                // There are no collision lines: set the bb points to the solid sprite's position

                m_lines_bb_bottom_right = Position;
                m_lines_bb_top_left     = Position;
            }            
        }

        //=========================================================================================
        /// <summary> 
        /// Disables points in the collision lines that are inside a concave angle. These types 
        /// of points can never be collided with.
        /// </summary>
        //=========================================================================================

        private void DisableConcavePoints()
        {
            // Do nothing if no collision lines:

            if ( m_collision_lines == null ) return;

            // Run through each line in the list:

            for ( int i = 0 ; i < m_collision_lines.Length ; i++ )
            {
                // Check against every other line in the list:

                for ( int j = i + 1 ; j < m_collision_lines.Length ; j++ )
                {
                    // Get the two lines:

                    Line line1 = m_collision_lines[i];
                    Line line2 = m_collision_lines[j];

                    // See if end of line 1 matches start of line 2:

                    if ( line1.End.Equals( line2.Start ) )
                    {
                        // Do a dot product of both of the line dir for line 1 and the normal for line 2:

                        float dot = Vector2.Dot( line1.Direction , line2.Normal );

                        // If the dot is negative then the angle between them is concave and the points at this angle should be disabled:

                        if ( dot < 0 )
                        {
                            line1.EndPointCollideable   = false;
                            line2.StartPointCollideable = false;
                        }

                        // Continue on to the next iteration:

                        continue;
                    }

                    // See if start of line 1 matches end of line 2:

                    if ( line1.Start.Equals( line2.End ) )
                    {
                        // Do a dot product of both of the line dir for line 1 and the normal for line 2:

                        float dot = Vector2.Dot( line1.Direction , line2.Normal );

                        // If the dot is negative then the angle between them is concave and the points at this angle should be disabled:

                        if ( dot < 0 )
                        {
                            line1.StartPointCollideable = false;
                            line2.EndPointCollideable   = false;
                        }
                    }
                }
            }
        }

        //=========================================================================================
        /// <summary> 
        /// Does debug drawing for the solid sprite object.
        /// </summary>
        //=========================================================================================

        #if DEBUG

            public override void OnDebugDraw()
            {
                // Call base function first

                base.OnDebugDraw();

                // Abort if no lines to draw:

                if ( m_collision_lines == null || m_collision_lines.Length <= 0 ) return;

                // Set the current vertex declaration:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;

                // Disable depth testing:

                Core.Graphics.Device.RenderState.DepthBufferEnable = false;
                Core.Graphics.Device.RenderState.DepthBufferWriteEnable = false;

                // Make an array of points for all the collision lines and their normals:

                VertexPositionColor[] vertices = new VertexPositionColor[ m_collision_lines.Length * 4 ];

                // Fill the array:

                for ( int i = 0 ; i < m_collision_lines.Length ; i++ )
                {
                    // Set line position

                    vertices[ i * 4 + 0 ].Position.X = m_collision_lines[i].StartX;
                    vertices[ i * 4 + 0 ].Position.Y = m_collision_lines[i].StartY;
                    vertices[ i * 4 + 0 ].Position.Z = - Camera.Z_MINIMUM;
                    vertices[ i * 4 + 1 ].Position.X = m_collision_lines[i].EndX;
                    vertices[ i * 4 + 1 ].Position.Y = m_collision_lines[i].EndY;
                    vertices[ i * 4 + 1 ].Position.Z = - Camera.Z_MINIMUM;

                    // Calculate line midpoint:

                    Vector2 midpoint = ( m_collision_lines[i].Start + m_collision_lines[i].End ) * 0.5f;

                    // Set normal position:

                    vertices[ i * 4 + 2 ].Position.X = midpoint.X;
                    vertices[ i * 4 + 2 ].Position.Y = midpoint.Y;
                    vertices[ i * 4 + 2 ].Position.Z = - Camera.Z_MINIMUM;
                    vertices[ i * 4 + 3 ].Position.X = midpoint.X + m_collision_lines[i].Normal.X * 6;
                    vertices[ i * 4 + 3 ].Position.Y = midpoint.Y + m_collision_lines[i].Normal.Y * 6;
                    vertices[ i * 4 + 3 ].Position.Z = - Camera.Z_MINIMUM;

                    // Set line color:

                    vertices[ i * 4 + 0 ].Color = Color.White;
                    vertices[ i * 4 + 1 ].Color = Color.White;
                    vertices[ i * 4 + 2 ].Color = Color.IndianRed;
                    vertices[ i * 4 + 3 ].Color = Color.IndianRed;
                }

                // Get the camera transforms:

                Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

                // Set the camera transforms on the debug shader:

                EffectParameter param = Core.DebugShader.Parameters["WorldViewProjection"];

                if ( param != null )
                {
                    param.SetValue( view_projection );
                }

                // Begin drawing with the debug shader:

                Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.LineList       ,
                        vertices                     ,
                        0                            ,
                        m_collision_lines.Length * 2
                    );

                Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End(); 

                // Draw the start and end points for each line:

                for ( int i = 0 ; i < m_collision_lines.Length ; i++ )
                {
                    // Set points position:

                    vertices[ i * 2 + 0 ].Position.X = m_collision_lines[i].StartX;
                    vertices[ i * 2 + 0 ].Position.Y = m_collision_lines[i].StartY;
                    vertices[ i * 2 + 1 ].Position.X = m_collision_lines[i].EndX;
                    vertices[ i * 2 + 1 ].Position.Y = m_collision_lines[i].EndY;

                    vertices[ i * 2 + 0 ].Position.Z = - Camera.Z_MINIMUM;
                    vertices[ i * 1 + 1 ].Position.Z = - Camera.Z_MINIMUM;

                    // Set color based on whether enabled or disabled:

                    if ( m_collision_lines[i].StartPointCollideable )
                    {
                        vertices[ i * 2 + 0 ].Color = Color.Green;
                    }
                    else
                    {
                        vertices[ i * 2 + 0 ].Color = Color.Red;
                    }

                    if ( m_collision_lines[i].EndPointCollideable )
                    {
                        vertices[ i * 2 + 1 ].Color = Color.Green;
                    }
                    else
                    {
                        vertices[ i * 2 + 1 ].Color = Color.Red;
                    }
                }

                // Set point size:

                Core.Graphics.Device.RenderState.PointSize = 4.0f;

                // Draw all the points:

                Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>
                    (
                        PrimitiveType.PointList      ,
                        vertices                     ,
                        0                            ,
                        m_collision_lines.Length * 2
                    );

                Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End(); 

                // Restore point size:

                Core.Graphics.Device.RenderState.PointSize = GraphicsSystem.DefaultRenderState.PointSize;

                // Restore depth testing to what it was:

                Core.Graphics.Device.RenderState.DepthBufferEnable = GraphicsSystem.DefaultRenderState.DepthBufferEnable;
                Core.Graphics.Device.RenderState.DepthBufferWriteEnable = GraphicsSystem.DefaultRenderState.DepthBufferWriteEnable;
            }

        #endif

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
            // If the index is past the end of the results array then do nothing:

            if ( results_index >= results.Length ) return 0;

            // Check out the collision cache and see if the ellipse is within bounds of our lines:

            if ( LevelCollisionQuery.CollisionCache.EllipseBoxTopLeft.X         > m_lines_bb_bottom_right.X ) return 0;
            if ( LevelCollisionQuery.CollisionCache.EllipseBoxBottomRight.X     < m_lines_bb_top_left.X     ) return 0;
            if ( LevelCollisionQuery.CollisionCache.EllipseBoxTopLeft.Y         < m_lines_bb_bottom_right.Y ) return 0;
            if ( LevelCollisionQuery.CollisionCache.EllipseBoxBottomRight.Y     > m_lines_bb_top_left.Y     ) return 0;

            // Store the number of results here:

            int num_results = 0;

            // Run through all the lines looking for a collision:

            for ( int i = 0 ; i < m_collision_lines.Length ; i++ )
            {
                // Store collision results here:

                Vector2 collide_point       = Vector2.Zero;
                Vector2 resolve_dir         = Vector2.Zero;
                Vector2 collide_normal      = Vector2.Zero;
                float   penetration         = 0;    
                bool    point_collision     = false;

                // Test against this line:                

                bool collision = m_collision_lines[i].FastCollide
                (
                    elipseDimensions.X                  ,
                    ref collide_point                   ,
                    ref resolve_dir                     ,
                    ref collide_normal                  ,
                    ref penetration                     ,
                    ref point_collision
                );

                // See if there was a collision

                if ( collision )
                {
                    // Increment number of results:

                    num_results++;

                    // Makeup the result:

                    CollisionQueryResult c;

                    c.ValidResult       = true;
                    c.QueryObject       = this;
                    c.Point             = collide_point;
                    c.Normal            = collide_normal;
                    c.Penetration       = penetration;
                    c.ResolveDirection  = resolve_dir;
                    c.IsPointCollision  = point_collision;

                    // Save the result:

                    results[results_index] = c;

                    // Increment results index:

                    results_index++;

                    // If past the end then return number of results:

                    if ( results_index >= results.Length ) return num_results;
                }

            }

            // Return the number of collision results

            return num_results;
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
            // Abort if there is no more room for results:

            if ( results_index >= results.Length ) return 0;

            // Check out the collision cache and see if the ellipse is within bounds of our lines:

            if ( LevelCollisionQuery.IntersectionCache.RayBoxTopLeft.X      > m_lines_bb_bottom_right.X     ) return 0;
            if ( LevelCollisionQuery.IntersectionCache.RayBoxBottomRight.X  < m_lines_bb_top_left.X         ) return 0;
            if ( LevelCollisionQuery.IntersectionCache.RayBoxTopLeft.Y      < m_lines_bb_bottom_right.Y     ) return 0;
            if ( LevelCollisionQuery.IntersectionCache.RayBoxBottomRight.Y  > m_lines_bb_top_left.Y         ) return 0;

            // Store the number of results here:

            int num_results = 0;

            // Make a new line to test with:

            Line line = new Line( lineStart , lineEnd );

            // Collide the line with each of this object's lines:

            for ( int i = 0 ; i < m_collision_lines.Length ; i++ )
            {
                // Store the collision point here:

                Vector2 intersection_point = Vector2.Zero;

                // Do the intersection:

                bool lines_intersect = m_collision_lines[i].Intersect( line , ref intersection_point );

                // See if we got an intersection

                if ( lines_intersect )
                {
                    // Increment number of results:

                    num_results++;

                    // Get vector from line start to point of intersection

                    Vector2 r = intersection_point - lineStart;

                    // Get the distance to the point 

                    float intersection_distance = Vector2.Dot( r , line.Direction );

                    // Make up the result:

                    IntersectQueryResult result;

                    result.ValidResult      = true;
                    result.QueryObject      = this;
                    result.Point            = intersection_point;
                    result.Normal           = m_collision_lines[i].Normal;
                    result.PointDistance    = intersection_distance;

                    // Save the result:

                    results[results_index] = result;

                    // Increment results index:

                    results_index++;

                    // If past the end then return number of results:

                    if ( results_index >= results.Length ) return num_results;
                }
            }

            // Return the number of collision results

            return num_results;
        }

    }   // end of class

}   // end of namespace 
