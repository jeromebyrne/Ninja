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
    //=============================================================================================
    /// <summary>
    /// Class representing a line. Allows for collision detection against the line.
    /// </summary>
    //=============================================================================================

    public class Line
    {
        //=========================================================================================
        // Properties
        //=========================================================================================
            
            /// <summary> Gets or sets the start point for the line </summary>

            public Vector2 Start
            {
                get { return m_start; } set { m_start = value; Update(); }
            }

            /// <summary> Gets or sets the end point for the line </summary>

            public Vector2 End
            {
                get { return m_end; } set { m_end = value; Update(); }
            }

            /// <summary> Gets or sets the line start point X coordinate </summary>

            public float StartX
            {
                get { return m_start.X; } set { m_start.X = value; Update(); }
            }

            /// <summary> Gets or sets the line start point Y coordinate </summary>

            public float StartY
            {
                get { return m_start.Y; } set { m_start.Y = value; Update(); }
            }

            /// <summary> Gets or sets the line end point X coordinate </summary>

            public float EndX
            {
                get { return m_end.X; } set { m_end.X = value; Update(); }
            }

            /// <summary> Gets or sets the line end point Y coordinate </summary>

            public float EndY
            {
                get { return m_end.Y; } set { m_end.Y = value; Update(); }
            }

            /// <summary> Tells if this is a valid line. A line is invalid if start and end points are the same. </summary>

            public bool Valid { get { return m_length > 0; } }

            /// <summary> Length of the line. </summary>

            public float Length { get { return m_length; } }

            /// <summary> Direction of the line. This will be of unit length. </summary>

            public Vector2 Direction { get { return m_direction; } }

            /// <summary> Normal for the line. This will be of unit length. </summary>

            public Vector2 Normal { get { return m_normal; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// True if the start point is collideable. Objects can set this for lines that tie up 
            /// with other lines to disable collision in concave angles. True by default.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool StartPointCollideable { get { return m_start_point_collideable; } set { m_start_point_collideable = value; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// True if the end point is collideable. Objects can set this for lines that tie up 
            /// with other lines to disable collision in concave angles. True by default.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool EndPointCollideable { get { return m_end_point_collideable; } set { m_end_point_collideable = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Start of the line. </summary>

            private Vector2 m_start = Vector2.Zero;
            
            /// <summary> End of the line. </summary>

            private Vector2 m_end = Vector2.Zero;

            /// <summary> Normal for the line. This will be normalised. </summary>

            private Vector2 m_normal = Vector2.Zero;

            /// <summary> Normalised direction of the line. </summary>
        
            private Vector2 m_direction = Vector2.Zero;

            /// <summary> Length of the line. </summary>

            private float m_length = 0;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// True if the start point is collideable. Objects can set this for lines that tie up 
            /// with other lines to disable collision in concave angles. True by default.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private bool m_start_point_collideable = true;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// True if the end point is collideable. Objects can set this for lines that tie up 
            /// with other lines to disable collision in concave angles. True by default.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private bool m_end_point_collideable = true;

        //=========================================================================================
        /// <summary>
        /// Creates a default line with both points at 0,0
        /// </summary>
        //=========================================================================================

        public Line(){}

        //=========================================================================================
        /// <summary>
        /// Creates a line with the given coordinates.
        /// </summary>
        /// <param name="start_x"> X coordinate of the first point </param>
        /// <param name="start_y"> Y coordinate of the first point </param>
        /// <param name="end_x"> X coordinate of the second point </param>
        /// <param name="end_y"> Y coordinate of the second point </param>
        //=========================================================================================

        public Line ( float start_x , float start_y , float end_x , float end_y )
        {
            // Save point

            m_start.X = start_x;
            m_start.Y = start_y;

            m_end.X = end_x;
            m_end.Y = end_y;

            // Update the lines other attributes:

            Update();
        }

        //=========================================================================================
        /// <summary>
        /// Creates a line with the given start and end points.
        /// </summary>
        //=========================================================================================

        public Line ( Vector2 start , Vector2 end )
        {
            // Save points

            m_start = start;
            m_end = end;

            // Update the lines other attributes:

            Update();
        }

        //=========================================================================================
        /// <summary>
        /// Performs a collision detection test for the given elipse with this line. returns 
        /// true on a collision occuring.
        /// </summary>
        /// <param name="elipse_position">      Position of elipse center                           </param>
        /// <param name="elipse_dimensions">    Size of elipse in x/y directions from its center    </param>
        /// <param name="elipse_rotation">      How much the elipse is rotated by.                  </param>
        /// <param name="intersect_point">      Where to save point of collision                    </param>
        /// <param name="resolve_direction">    Where to save direction to move after collision     </param>
        /// <param name="penetration">          Where to save penetration amount                    </param>
        /// <param name="normal">               Normal at the point of collision                    </param>
        /// <param name="is_point_collision"> 
        /// Tells if the collision is a point collision. If so then the elipse has collided with a 
        /// point at the end of a line and not the line's plane itself.
        /// </param>
        /// 
        /// <returns> True if a collision occured, false if not </returns>
        //=========================================================================================

        public bool Collide
        ( 
            Vector2         elipse_position     , 
            Vector2         elipse_dimensions   ,
            float           elipse_rotation     ,
            ref Vector2     collide_point       ,
            ref Vector2     resolve_direction   ,
            ref Vector2     normal              ,
            ref float       penetration         ,
            ref bool        is_point_collision  
        )
        {
            // Here's the trick to this: we will scale the elipse and the line in the y direction so that the elipse becomes a circle. Figure out the scaling first.

            float circlelizing_scale = elipse_dimensions.X / elipse_dimensions.Y;

            // Get a vector to the start and end of the line:

            Vector2 to_line_s = m_start - elipse_position;
            Vector2 to_line_e = m_end   - elipse_position;

            // Rotate the two points according to the rotation of the elipse: in essence we are rotating the world so that the elipse becomes axis aligned

            Matrix elipse_rot_m = Matrix.CreateRotationZ( - elipse_rotation );

            to_line_s = Vector2.Transform( to_line_s , elipse_rot_m );
            to_line_e = Vector2.Transform( to_line_e , elipse_rot_m );

            // Scale the world so that the elipse becomes a circle:

            to_line_s.Y *= circlelizing_scale;
            to_line_e.Y *= circlelizing_scale;

            // Get the direction, length and normal of the line:

                Vector2 line_d = to_line_e - to_line_s;

                float line_l = line_d.Length();

                line_d.X /= line_l;
                line_d.Y /= line_l;

                Vector2 line_n = Vector2.Zero;

                line_n.X = - line_d.Y;
                line_n.Y =   line_d.X; 

            // Do a dot product with the line normal to get the vertical distance from elipse center to the line:

            float v_dist = - ( (to_line_s.X * line_n.X) + (to_line_s.Y * line_n.Y) );

            // See if negative: if negative then elipse is on the negative side of the line's plane and we should ignore collison

            if ( v_dist >= 0 )
            {
                //---------------------------------------------------------------------------------
                // Elipse is on the correct side of the line's plane. 
                // See if the elipse is close enough to make a collision: use the x dimensions 
                // because we rescaled the world so that the y dimensons would match x
                //---------------------------------------------------------------------------------

                if ( v_dist < elipse_dimensions.X )
                {
                    // Get the horizontal distance that the center of the elipse is along the line when projected vertically onto it:

                    float h_dist = - ( (to_line_s.X * line_d.X) + (to_line_s.Y * line_d.Y) );

                    //-----------------------------------------------------------------------------
                    // Now there are three possibilities next.
                    // 
                    // (1) horizontal distance of the elipse center falls along the line segement.
                    //     In this case we do a planar collision with the line.
                    // (2) horizontal distance of the elipse center falls before the line segement start.
                    //     In this case we do a point collision with the line start point.
                    // (2) horizontal distance of the elipse center falls after the line segement ends.
                    //     In this case we do a point collision with the line start end.
                    //
                    // See which case we are dealing with:
                    //-----------------------------------------------------------------------------

                    if ( h_dist >= 0 )
                    {
                        //-------------------------------------------------------------------------
                        // Elipse center is after start of line:
                        //-------------------------------------------------------------------------

                        if ( h_dist <= line_l )
                        {
                            //----------------------------------------------------------------------
                            // Elipse center falls along line segment: do a planar collision with line
                            //----------------------------------------------------------------------

                            // Calculate the vector that the elipse must move back along:

                            resolve_direction = line_n * ( elipse_dimensions.X - v_dist );

                            // Calculate the point of collision:

                            collide_point = - line_n * v_dist;

                            // Undo the scaling done earlier:

                            resolve_direction.Y /= circlelizing_scale; collide_point.Y /= circlelizing_scale;

                            // Undo the previous rotations we did to the world to make the elipse axis aligned:

                            Matrix elipse_inv_rot_m = Matrix.CreateRotationZ( elipse_rotation );

                                resolve_direction = Vector2.Transform( resolve_direction , elipse_inv_rot_m );
                                collide_point = Vector2.Transform( collide_point , elipse_inv_rot_m );

                            // Bring the collision point back into world coordinates:

                            collide_point += elipse_position;

                            // Calculate it's length

                            penetration = resolve_direction.Length();

                            // Safety check: if the penetration amount is very small then ignore collision:

                            if ( penetration < 0.0001f ) return false;

                            // Normalise the resolve direction:

                            resolve_direction.X /= penetration;
                            resolve_direction.Y /= penetration;

                            // Normal is line normal

                            normal = m_normal;

                            // This is not a point collision:

                            is_point_collision = false;

                            // Return true for a collision

                            return true;
                        }
                        else
                        {
                            //---------------------------------------------------------------------
                            // Elipse center falls after end of line segment: see if close enough to 
                            // collide with line end point and if end point is collideable.
                            //---------------------------------------------------------------------

                            if ( h_dist <= line_l + elipse_dimensions.X && m_end_point_collideable )
                            {
                                //-----------------------------------------------------------------
                                // Elipse within distance of end point of line. Do collision with it.
                                //-----------------------------------------------------------------

                                // Get the distance to the end point of the line:

                                float dist_to_e = to_line_e.Length();

                                // Make sure we are within distance:

                                if ( dist_to_e >= elipse_dimensions.X ) return false;

                                // Calculate the penetration distance:

                                penetration = elipse_dimensions.X - dist_to_e;

                                // Get the direction that must be moved back along:

                                resolve_direction.X = - ( to_line_e.X / dist_to_e ) * penetration;
                                resolve_direction.Y = - ( to_line_e.Y / dist_to_e ) * penetration;

                                // Undo the scaling:

                                resolve_direction.Y /= circlelizing_scale;

                                // Undo the previous rotations we did to the world to make the elipse axis aligned:

                                Matrix elipse_inv_rot_m = Matrix.CreateRotationZ( elipse_rotation );

                                    resolve_direction = Vector2.Transform( resolve_direction , elipse_inv_rot_m );

                                // Get the real amount of penetration:

                                penetration = resolve_direction.Length();

                                // Safety check: if the penetration amount is very small then ignore collision:

                                if ( penetration < 0.0001f ) return false;

                                // Save the real resolve direction:

                                resolve_direction.X /= penetration;
                                resolve_direction.Y /= penetration;

                                // Normal is line normal

                                normal = m_normal;

                                // Point of collision is the end point:

                                collide_point = m_end;

                                // This is a point collision:

                                is_point_collision = true;

                                // Return true for a collision:

                                return true;
                            }
                            else
                            {
                                //-----------------------------------------------------------------
                                // Elipse center is too far away from line end point - no collision:
                                //-----------------------------------------------------------------
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //-------------------------------------------------------------------------
                        // Elipse center is before start of line: see if close enough and if start
                        // point is collideable.
                        //-------------------------------------------------------------------------

                        if ( h_dist >= - elipse_dimensions.X && m_start_point_collideable )
                        {
                            //---------------------------------------------------------------------
                            // Elipse within distance of start point of line. Do collision with it.
                            //---------------------------------------------------------------------

                            // Get the distance to the end point of the line:

                            float dist_to_s = to_line_s.Length();

                            // Make sure we are within distance:

                            if ( dist_to_s >= elipse_dimensions.X ) return false;

                            // Calculate the penetration distance:

                            penetration = elipse_dimensions.X - dist_to_s;

                            // Get the direction that must be moved back along:

                            resolve_direction.X = - ( to_line_s.X / dist_to_s ) * penetration;
                            resolve_direction.Y = - ( to_line_s.Y / dist_to_s ) * penetration;

                            // Undo the scaling:

                            resolve_direction.Y /= circlelizing_scale;

                            // Undo the previous rotations we did to the world to make the elipse axis aligned:

                            Matrix elipse_inv_rot_m = Matrix.CreateRotationZ( elipse_rotation );

                                resolve_direction = Vector2.Transform( resolve_direction , elipse_inv_rot_m );

                            // Get the real amount of penetration:

                            penetration = resolve_direction.Length();

                            // Safety check: if the penetration amount is very small then ignore collision:

                            if ( penetration < 0.0001f ) return false;

                            // Save the real resolve direction:

                            resolve_direction.X /= penetration;
                            resolve_direction.Y /= penetration;

                            // Normal is line normal

                            normal = m_normal;

                            // Point of collision is the start point:

                            collide_point = m_start;

                            // This is a point collision:

                            is_point_collision = true;

                            // Return true for a collision

                            return true;
                        }
                        else
                        {
                            //---------------------------------------------------------------------
                            // Elipse is not close enough to line start point: no collision
                            //---------------------------------------------------------------------

                            return false;
                        }
                    }
                }
                else
                {
                    //-----------------------------------------------------------------------------
                    // Elipse is not close enough to the line - no collision:
                    //-----------------------------------------------------------------------------

                    return false;
                }
            }
            else
            {
                //---------------------------------------------------------------------------------
                // Elipse on wrong side of the line's plane - no collision:
                //---------------------------------------------------------------------------------

                return false;
            }

        }

        //=========================================================================================
        /// <summary>
        /// This is a faster version of collide which uses cached transforms to speed up the 
        /// rotated bounding ellipse collision detection. This relies on the collision cache in the 
        /// LevelCollisionQuery class having been setup beforehand.
        /// </summary>
        /// <param name="ellipse_local_radius"> 
        /// Radius of the ellipse when it is scaled into a circle for collision testing. 
        /// </param>
        /// <param name="intersect_point">      Where to save point of collision                    </param>
        /// <param name="resolve_direction">    Where to save direction to move after collision     </param>
        /// <param name="penetration">          Where to save penetration amount                    </param>
        /// <param name="normal">               Normal at the point of collision                    </param>
        /// <param name="is_point_collision"> 
        /// Tells if the collision is a point collision. If so then the elipse has collided with a 
        /// point at the end of a line and not the line's plane itself.
        /// </param>
        /// 
        /// <returns> True if a collision occured, false if not </returns>
        //=========================================================================================

        public bool FastCollide
        ( 
            float           ellipse_local_radius    ,
            ref Vector2     collide_point           ,
            ref Vector2     resolve_direction       ,
            ref Vector2     normal                  ,
            ref float       penetration             ,
            ref bool        is_point_collision  
        )
        {
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Transform the start and end points of the line into the ellipses coordinate system,
            // so that the ellipse becomes a circle and it's rotation is undone. These two 
            // vectors are also going from the ellipse position to the points positions.
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Vector2 to_line_s = Vector2.Transform( m_start , LevelCollisionQuery.CollisionCache.ToEllipseLocal );
            Vector2 to_line_e = Vector2.Transform( m_end   , LevelCollisionQuery.CollisionCache.ToEllipseLocal );

            // Get the direction, length and normal of the line:

                Vector2 line_d = to_line_e - to_line_s;

                float line_l = line_d.Length();

                line_d.X /= line_l;
                line_d.Y /= line_l;

                Vector2 line_n = Vector2.Zero;

                line_n.X = - line_d.Y;
                line_n.Y =   line_d.X; 

            // Do a dot product with the line normal to get the vertical distance from elipse center to the line:

            float v_dist = - ( (to_line_s.X * line_n.X) + (to_line_s.Y * line_n.Y) );

            // See if negative: if negative then elipse is on the negative side of the line's plane and we should ignore collison

            if ( v_dist >= 0 )
            {
                //---------------------------------------------------------------------------------
                // Elipse is on the correct side of the line's plane. 
                // See if the elipse is close enough to make a collision: use the x dimensions 
                // because we rescaled the world so that the y dimensons would match x
                //---------------------------------------------------------------------------------

                if ( v_dist < ellipse_local_radius )
                {
                    // Get the horizontal distance that the center of the elipse is along the line when projected vertically onto it:

                    float h_dist = - ( (to_line_s.X * line_d.X) + (to_line_s.Y * line_d.Y) );

                    //-----------------------------------------------------------------------------
                    // Now there are three possibilities next.
                    // 
                    // (1) horizontal distance of the elipse center falls along the line segement.
                    //     In this case we do a planar collision with the line.
                    // (2) horizontal distance of the elipse center falls before the line segement start.
                    //     In this case we do a point collision with the line start point.
                    // (2) horizontal distance of the elipse center falls after the line segement ends.
                    //     In this case we do a point collision with the line start end.
                    //
                    // See which case we are dealing with:
                    //-----------------------------------------------------------------------------

                    if ( h_dist >= 0 )
                    {
                        //-------------------------------------------------------------------------
                        // Elipse center is after start of line:
                        //-------------------------------------------------------------------------

                        if ( h_dist <= line_l )
                        {
                            //----------------------------------------------------------------------
                            // Elipse center falls along line segment: do a planar collision with line
                            //----------------------------------------------------------------------

                            // Calculate the vector that the elipse must move back along:

                            resolve_direction = line_n * ( ellipse_local_radius - v_dist );

                            // Calculate the point of collision:

                            collide_point = - line_n * v_dist;

                            // Transform both the resolve direction and collide point back into normal coordinates:

                            {
                                Vector4 v = Vector4.Zero;

                                v.X = resolve_direction.X;
                                v.Y = resolve_direction.Y;

                                v = Vector4.Transform( v , LevelCollisionQuery.CollisionCache.FromEllipseLocal );

                                resolve_direction.X = v.X;
                                resolve_direction.Y = v.Y;
                            }

                            collide_point = Vector2.Transform
                            ( 
                                collide_point , LevelCollisionQuery.CollisionCache.FromEllipseLocal 
                            );

                            // Calculate penetration amount:

                            penetration = resolve_direction.Length();

                            // Safety check: if the penetration amount is very small then ignore collision:

                            if ( penetration < 0.0001f ) return false;

                            // Normalise the resolve direction:

                            resolve_direction.X /= penetration;
                            resolve_direction.Y /= penetration;

                            // Normal is line normal

                            normal = m_normal;

                            // This is not a point collision:

                            is_point_collision = false;

                            // Return true for a collision

                            return true;
                        }
                        else
                        {
                            //---------------------------------------------------------------------
                            // Elipse center falls after end of line segment: see if close enough to 
                            // collide with line end point. 
                            //---------------------------------------------------------------------

                            if ( h_dist <= line_l + ellipse_local_radius )
                            {
                                //-----------------------------------------------------------------
                                // Elipse within distance of end point of line. Do collision with it.
                                //-----------------------------------------------------------------

                                // Get the distance to the end point of the line:

                                float dist_to_e = to_line_e.Length();

                                // Make sure we are within distance:

                                if ( dist_to_e >= ellipse_local_radius ) return false;

                                // Calculate the penetration distance:

                                penetration = ellipse_local_radius - dist_to_e;

                                // Get the direction that must be moved back along:

                                resolve_direction.X = - ( to_line_e.X / dist_to_e ) * penetration;
                                resolve_direction.Y = - ( to_line_e.Y / dist_to_e ) * penetration;

                                // Transform the resolve direction back into normal coordinates:

                                {
                                    Vector4 v = Vector4.Zero;

                                    v.X = resolve_direction.X;
                                    v.Y = resolve_direction.Y;

                                    v = Vector4.Transform( v , LevelCollisionQuery.CollisionCache.FromEllipseLocal );

                                    resolve_direction.X = v.X;
                                    resolve_direction.Y = v.Y;
                                }

                                // Get the real amount of penetration:

                                penetration = resolve_direction.Length();

                                // Safety check: if the penetration amount is very small then ignore collision:

                                if ( penetration < 0.0001f ) return false;

                                // Save the real resolve direction:

                                resolve_direction.X /= penetration;
                                resolve_direction.Y /= penetration;

                                // Normal is line normal

                                normal = m_normal;

                                // Point of collision is the end point:

                                collide_point = m_end;

                                // This is a point collision:

                                is_point_collision = true;

                                // Return true for a collision:

                                return true;
                            }
                            else
                            {
                                //-----------------------------------------------------------------
                                // Elipse center is too far away from line end point - no collision:
                                //-----------------------------------------------------------------

                                return false;
                            }
                        }
                    }
                    else
                    {
                        //-------------------------------------------------------------------------
                        // Elipse center is before start of line: see if close enough
                        //-------------------------------------------------------------------------

                        if ( h_dist >= - ellipse_local_radius )
                        {
                            //---------------------------------------------------------------------
                            // Elipse within distance of start point of line. Do collision with it.
                            //---------------------------------------------------------------------

                            // Get the distance to the end point of the line:

                            float dist_to_s = to_line_s.Length();

                            // Make sure we are within distance:

                            if ( dist_to_s >= ellipse_local_radius ) return false;

                            // Calculate the penetration distance:

                            penetration = ellipse_local_radius - dist_to_s;

                            // Get the direction that must be moved back along:

                            resolve_direction.X = - ( to_line_s.X / dist_to_s ) * penetration;
                            resolve_direction.Y = - ( to_line_s.Y / dist_to_s ) * penetration;

                            // Transform the resolve direction back into normal coordinates:

                            {
                                Vector4 v = Vector4.Zero;

                                v.X = resolve_direction.X;
                                v.Y = resolve_direction.Y;

                                v = Vector4.Transform( v , LevelCollisionQuery.CollisionCache.FromEllipseLocal );

                                resolve_direction.X = v.X;
                                resolve_direction.Y = v.Y;
                            }

                            // Get the real amount of penetration:

                            penetration = resolve_direction.Length();

                            // Safety check: if the penetration amount is very small then ignore collision:

                            if ( penetration < 0.0001f ) return false;

                            // Save the real resolve direction:

                            resolve_direction.X /= penetration;
                            resolve_direction.Y /= penetration;

                            // Normal is line normal

                            normal = m_normal;

                            // Point of collision is the start point:

                            collide_point = m_start;

                            // This is a point collision:

                            is_point_collision = true;

                            // Return true for a collision

                            return true;
                        }
                        else
                        {
                            //---------------------------------------------------------------------
                            // Elipse is not close enough to line start point: no collision
                            //---------------------------------------------------------------------

                            return false;
                        }
                    }
                }
                else
                {
                    //-----------------------------------------------------------------------------
                    // Elipse is not close enough to the line - no collision:
                    //-----------------------------------------------------------------------------

                    return false;
                }
            }
            else
            {
                //---------------------------------------------------------------------------------
                // Elipse on wrong side of the line's plane - no collision:
                //---------------------------------------------------------------------------------

                return false;
            }

        }

        //=========================================================================================
        /// <summary>
        /// Caculates the intersection of this line with another given line. If there is no 
        /// intersection then false is returend.
        /// This follows the matrix based method described at mathworld:
        /// http://mathworld.wolfram.com/Line-LineIntersection.html
        /// </summary>
        /// <param name="other_line"> Other line to caculate the intersection with. </param>
        /// <param name="intersect_point"> Where to save the point of intersection. </param>
        /// <returns> True if there is an intersection, false if not. </returns>
        //=========================================================================================

        public bool Intersect( Line l , ref Vector2 intersect_point )
        {
            // Abort if other line is null:

            if ( l == null ) return false;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Method at mathworld:
            //
            // x = | | x1 y1 | x1 - x2 |
            //     | | x2 y2 |         |
            //     |                   |
            //     | | x3 y3 | x3 - x4 |
            //     | | x4 x4 |         |
            //
            //     _____________________
            //
            //     | x1 - x2   y1 - y2 |
            //     | x3 - x4   y3 - y4 |
            //
            // y = | | x1 y1 | y1 - y2 |
            //     | | x2 y2 |         |
            //     |                   |
            //     | | x3 y3 | y3 - y4 |
            //     | | x4 x4 |         |
            //
            //     _____________________
            //
            //     | x1 - x2   y1 - y2 |
            //     | x3 - x4   y3 - y4 |
            //
            //
            // x = | det1  x1 - x2 |
            //     | det2  x3 - x4 |
            //     _________________
            //
            //           det3
            // 
            // y = | det1  y1 - y2 |
            //     | det2  y3 - y4 |
            //     _________________
            //
            //           det3
            // 
            // 
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            // Calculate matrix determinants

            float det1 = ( m_start.X * m_end.Y ) - ( m_start.Y * m_end.X );
            float det2 = ( l.m_start.X * l.m_end.Y ) - ( l.m_start.Y * l.m_end.X );
            float det3 = (( m_start.X - m_end.X ) * ( l.m_start.Y - l.m_end.Y)) - ((m_start.Y-m_end.Y) * (l.m_start.X-l.m_end.X));

            // If third determinant is close to zero then abort: two lines are nearly parallel:

            if ( det3 >= -0.00000001f && det3 <= 0.00000001f ) return false;

            // Otherwise calculate the point of intersection:

            intersect_point.X = (det1 * (l.m_start.X - l.m_end.X)) - ((m_start.X - m_end.X ) * det2 );
            intersect_point.X /= det3;
            intersect_point.Y = (det1 * (l.m_start.Y - l.m_end.Y)) - ((m_start.Y - m_end.Y ) * det2 );
            intersect_point.Y /= det3;

            // Make sure the point is along both lines: get it relative to the start point of both lines

            Vector2 r1 = intersect_point - m_start;
            Vector2 r2 = intersect_point - l.m_start;

            // Do a dot product with both line directions to see if it is past the end of either of the two lines:

            float dot1 = Vector2.Dot( r1 , m_direction   );
            float dot2 = Vector2.Dot( r2 , l.m_direction );

            // If either dot is negative then the point is past the beginning of one of the lines:

            if ( dot1 < 0 ) return false;
            if ( dot2 < 0 ) return false;

            // If either dot exceeds the length of the line then point is past the end of the line:

            if ( dot1 > m_length   ) return false;
            if ( dot2 > l.m_length ) return false;

            // If we have got to here the intersection is ok: return true

            return true;
        }

        //=========================================================================================
        /// <summary>
        /// Caculates the intersection of this line with another given line, TREATING THIS LINE 
        /// AS IF IT EXTENDS INFINITELY IN BOTH DIRECTIONS. If there is no intersection then false 
        /// is returend. This follows the matrix based method described at mathworld:
        /// http://mathworld.wolfram.com/Line-LineIntersection.html . This variation of intersect 
        /// can be used for planar collisions.
        /// </summary>
        /// <param name="other_line"> Other line to caculate the intersection with. </param>
        /// <param name="intersect_point"> Where to save the point of intersection. </param>
        /// <returns> True if there is an intersection, false if not. </returns>
        //=========================================================================================

        public bool IntersectInfinite( Line l , ref Vector2 intersect_point )
        {
            // Abort if other line is null:

            if ( l == null ) return false;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Method at mathworld:
            //
            // x = | | x1 y1 | x1 - x2 |
            //     | | x2 y2 |         |
            //     |                   |
            //     | | x3 y3 | x3 - x4 |
            //     | | x4 x4 |         |
            //
            //     _____________________
            //
            //     | x1 - x2   y1 - y2 |
            //     | x3 - x4   y3 - y4 |
            //
            // y = | | x1 y1 | y1 - y2 |
            //     | | x2 y2 |         |
            //     |                   |
            //     | | x3 y3 | y3 - y4 |
            //     | | x4 x4 |         |
            //
            //     _____________________
            //
            //     | x1 - x2   y1 - y2 |
            //     | x3 - x4   y3 - y4 |
            //
            //
            // x = | det1  x1 - x2 |
            //     | det2  x3 - x4 |
            //     _________________
            //
            //           det3
            // 
            // y = | det1  y1 - y2 |
            //     | det2  y3 - y4 |
            //     _________________
            //
            //           det3
            // 
            // 
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            // Calculate matrix determinants

            float det1 = ( m_start.X * m_end.Y ) - ( m_start.Y * m_end.X );
            float det2 = ( l.m_start.X * l.m_end.Y ) - ( l.m_start.Y * l.m_end.X );
            float det3 = (( m_start.X - m_end.X ) * ( l.m_start.Y - l.m_end.Y)) - ((m_start.Y-m_end.Y) * (l.m_start.X-l.m_end.X));

            // If third determinant is close to zero then abort: two lines are nearly parallel:

            if ( det3 >= -0.00000001f && det3 <= 0.00000001f ) return false;

            // Otherwise calculate the point of intersection:

            intersect_point.X = (det1 * (l.m_start.X - l.m_end.X)) - ((m_start.X - m_end.X ) * det2 );
            intersect_point.X /= det3;
            intersect_point.Y = (det1 * (l.m_start.Y - l.m_end.Y)) - ((m_start.Y - m_end.Y ) * det2 );
            intersect_point.Y /= det3;

            // Make sure the point is along both lines: get it relative to the start point of both lines

            Vector2 r = intersect_point - l.m_start;

            // Do a dot product with given line direction to see if the point is on the given line:

            float dot = Vector2.Dot( r , l.m_direction );

            // If the dot is negative then the point is past the beginning of the given line:

            if ( dot < 0 ) return false;

            // If the dot exceeds length of the given line, then intersection point is past the end of the line:

            if ( dot > l.m_length ) return false;

            // If we have got to here the intersection is ok: return true

            return true;
        }

        //=========================================================================================
        /// <summary>
        /// Updates the attributes of the line- length etc. after it's coordinates have been changed.
        /// </summary>
        //=========================================================================================

        private void Update()
        {
            // Get line direction vector:

            Vector2 line_direction = m_end - m_start;

            // Get the length of the line:

            m_length = line_direction.Length();

            // If zero length then set normal and line direction to zero and abort:

            if ( m_length <= 0 )
            {
                m_normal    = Vector2.Zero;
                m_direction = Vector2.Zero;

                return;
            }

            // Save the normalised line direction:

            m_direction.X = line_direction.X / m_length;
            m_direction.Y = line_direction.Y / m_length;

            // Calculate the normal:

            m_normal.X = - m_direction.Y;
            m_normal.Y =   m_direction.X;
        }
        
    }   // end of class

}   // end of namespace
