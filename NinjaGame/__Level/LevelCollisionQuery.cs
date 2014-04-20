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

//################################################################################################
//################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary>
    /// This class allows various collision detection tests to be carried out on the objects in 
    /// the level and the results are stored in arrays for later retrieval. This class also 
    /// culls objects from the test (does an early-out) according to bounding box dimensions 
    /// of each object in the scene. Beware of this when modifying object bounding box dimensions.
    /// </summary>    
    //
    //#############################################################################################

    public class LevelCollisionQuery
    {
        //=========================================================================================
        // Properties
        //=========================================================================================
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// An array containing collision query results for the last collision test. 
            /// NOTE! The size of this array does not correspond to the number of results.
            /// Use CollisionResultCount for this instead.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public CollisionQueryResult[] CollisionResults { get { return m_collision_results; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// An array containing intersect query results for the last line intersection test. 
            /// NOTE! The size of this array does not correspond to the number of results.
            /// Use IntersectResultCount for this instead.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public IntersectQueryResult[] IntersectResults { get { return m_intersect_results; } } 

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// An array containing overlap query results for the last bounding rectangle overlap test. 
            /// NOTE! The size of this array does not correspond to the number of results.
            /// Use OverlapResultCount for this instead.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        
            public OverlapQueryResult[] OverlapResults { get { return m_overlap_results; } }

            /// <summary> The number of results obtained for the last performed collision query </summary>

            public int CollisionResultCount { get { return m_collision_result_count; } }

            /// <summary> The number of results obtained for the last performed overlap query </summary>

            public int IntersectResultCount { get { return m_intersect_result_count; } }

            /// <summary> The number of results obtained for the last performed intersection query </summary>

            public int OverlapResultCount { get { return m_overlap_result_count; } }

        //=========================================================================================
        // Constants
        //=========================================================================================   

            /// <summary> The maximum number of collision results that can be generated for any of the collison tests. </summary>

            public const int MAX_RESULTS = 256;

        //=========================================================================================
        // Variables 
        //=========================================================================================   

            /// <summary> An array containing collision query results for the last collision test. </summary>

            private CollisionQueryResult[] m_collision_results = null;

            /// <summary> An array containing intersect query results for the last line intersection test. </summary>
            
            private IntersectQueryResult[] m_intersect_results = null;

            /// <summary> An array containing overlap query results for the last bounding rectangle overlap test. </summary>

            private OverlapQueryResult[] m_overlap_results = null;

            /// <summary> The number of results obtained for the last performed collision query </summary>

            private int m_collision_result_count = 0;

            /// <summary> The number of results obtained for the last performed overlap query </summary>

            private int m_overlap_result_count = 0;

            /// <summary> The number of results obtained for the last performed intersection query </summary>

            private int m_intersect_result_count = 0;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This object can be used to speed up bounding ellipse collision detection, by figuring
            /// out the transforms needed for the test beforehand and combining them into two single 
            /// transform matrices.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static EllipseCollisionCache CollisionCache;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This object helps to speed up ray intersection testing by precomputing the minimum
            /// bounding box which will contain the ray.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static RayIntersectCache IntersectionCache;

            /// <summary> Level this query object belongs to. </summary>

            private Level m_level = null;

        //=========================================================================================
        // Structures 
        //=========================================================================================

            /// <summary> This structure contains information useful to speed up collision testing. </summary>

            public struct EllipseCollisionCache
            {
                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                /// <summary> 
                /// A transform that rotates, scales and translates so that a point is in the 
                /// coordinate system of a bounding ellipse for a collision test. It makes the 
                /// center of the ellipse the origin and undoes ellipse rotation, and applys uniform
                /// dimensions to the ellipse- reducing the collision test to a simple bounding sphere
                /// test. This matrix is used to transform lines and their points into a form that 
                /// where we do a simple bounding sphere test with the ellipse.
                /// </summary>
                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                public Matrix ToEllipseLocal;

                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                /// <summary>
                /// This matrix performs the exact opposite transform as the 'ToEllipseLocal' member. 
                /// This is used to get the results from the simplified problem coordinate system 
                /// back into the world coordinate system.
                /// </summary>
                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                public Matrix FromEllipseLocal;

                /// <summary> Top left point of a bounding box surrounding the ellipse. </summary>

                public Vector2 EllipseBoxTopLeft;

                /// <summary> Bottom right point of a bounding box surrounding the ellipse. </summary>

                public Vector2 EllipseBoxBottomRight;
            }

            /// <summary> This structure contains information useful to speed up intersection testing or raycasting. </summary>

            public struct RayIntersectCache
            {
                /// <summary> Top left point of a bounding box surrounding the ray. </summary>

                public Vector2 RayBoxTopLeft;

                /// <summary> Bottom right point of a bounding box surrounding the ray. </summary>

                public Vector2 RayBoxBottomRight;
            }

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a new level collision query object. 
        /// </summary>
        /// <param name="parentLevel"> Level this query object belongs to. </param>
        //=========================================================================================

        public LevelCollisionQuery( Level parentLevel )
        {
            // If the parent level is null then complain in windows debug:

            #if WINDOWS_DEBUG

                if ( parentLevel == null ) throw new Exception("LevelRenderer must have a valid parent Level object");

            #endif

            // Save the parent level:

            m_level = parentLevel;

            // Create the arrays of collision results:

            m_collision_results = new CollisionQueryResult[MAX_RESULTS];
            m_intersect_results = new IntersectQueryResult[MAX_RESULTS];
            m_overlap_results   = new OverlapQueryResult[MAX_RESULTS];

            for ( int i = 0 ; i < MAX_RESULTS; i++ )
            {
                m_collision_results[i]  = CollisionQueryResult.NoResult;
                m_intersect_results[i]  = IntersectQueryResult.NoResult;
                m_overlap_results[i]    = OverlapQueryResult.NoResult;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Preforms a collision query on the level for the given elipse. 
        /// </summary>
        /// <param name="ellipse_pos">   Center of the bounding elipse.                                              </param>
        /// <param name="ellipse_scale"> Dimensions of the elipse in +/- x and y directions from elipse center.      </param>
        /// <param name="ellipse_rot">   Amount the elipse is rotated by in radians.                                 </param>
        /// <param name="caller">        Object performing the query. This object will not be included in the test.  </param>
        //=========================================================================================

        public void Collide( Vector2 ellipse_pos , Vector2 ellipse_scale , float ellipse_rot , GameObject caller )
        {
            //-------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------

            // Update the collision cache:

            {
                // Figure out how much the elipse must be scaled in the y direction to make it a circle

                float circlelizing_scale = ellipse_scale.X / ellipse_scale.Y;

                // Make the to ellipse local coordinate system transform matrix:

                CollisionCache.ToEllipseLocal = 
                    
                    Matrix.CreateTranslation( - ellipse_pos.X , - ellipse_pos.Y , 0 )
                    *
                    Matrix.CreateRotationZ( - ellipse_rot )
                    *
                    Matrix.CreateScale( 1 , circlelizing_scale , 1 );
                
                // Now make the opposite transform:

                CollisionCache.FromEllipseLocal = 

                    Matrix.CreateScale( 1 , 1.0f / circlelizing_scale , 1 )                    
                    *
                    Matrix.CreateRotationZ( ellipse_rot )
                    *
                    Matrix.CreateTranslation( ellipse_pos.X , ellipse_pos.Y , 0 );

                // Rotate the four axis vectors of the ellipse:

                Vector2 v1 =   Vector2.UnitX * ellipse_scale.X;
                Vector2 v2 = - Vector2.UnitX * ellipse_scale.X;
                Vector2 v3 =   Vector2.UnitY * ellipse_scale.Y;
                Vector2 v4 = - Vector2.UnitY * ellipse_scale.Y;

                {
                    Matrix rot = Matrix.CreateRotationZ( ellipse_rot );

                    v1 = Vector2.Transform( v1 , rot );
                    v2 = Vector2.Transform( v2 , rot );
                    v3 = Vector2.Transform( v3 , rot );
                    v4 = Vector2.Transform( v4 , rot );
                }

                // Figure out the bounding box dimensions for the ellipse:

                CollisionCache.EllipseBoxTopLeft     = v1;
                CollisionCache.EllipseBoxBottomRight = v1;

                if ( v2.X < CollisionCache.EllipseBoxTopLeft.X ) CollisionCache.EllipseBoxTopLeft.X = v2.X;
                if ( v3.X < CollisionCache.EllipseBoxTopLeft.X ) CollisionCache.EllipseBoxTopLeft.X = v3.X;
                if ( v4.X < CollisionCache.EllipseBoxTopLeft.X ) CollisionCache.EllipseBoxTopLeft.X = v4.X;

                if ( v2.X > CollisionCache.EllipseBoxBottomRight.X ) CollisionCache.EllipseBoxBottomRight.X = v2.X;
                if ( v3.X > CollisionCache.EllipseBoxBottomRight.X ) CollisionCache.EllipseBoxBottomRight.X = v3.X;
                if ( v4.X > CollisionCache.EllipseBoxBottomRight.X ) CollisionCache.EllipseBoxBottomRight.X = v4.X;

                if ( v2.Y > CollisionCache.EllipseBoxTopLeft.Y ) CollisionCache.EllipseBoxTopLeft.Y = v2.Y;
                if ( v3.Y > CollisionCache.EllipseBoxTopLeft.Y ) CollisionCache.EllipseBoxTopLeft.Y = v3.Y;
                if ( v4.Y > CollisionCache.EllipseBoxTopLeft.Y ) CollisionCache.EllipseBoxTopLeft.Y = v4.Y;

                if ( v2.Y < CollisionCache.EllipseBoxBottomRight.Y ) CollisionCache.EllipseBoxBottomRight.Y = v2.Y;
                if ( v3.Y < CollisionCache.EllipseBoxBottomRight.Y ) CollisionCache.EllipseBoxBottomRight.Y = v3.Y;
                if ( v4.Y < CollisionCache.EllipseBoxBottomRight.Y ) CollisionCache.EllipseBoxBottomRight.Y = v4.Y;

                // Bring box points into world coords:

                CollisionCache.EllipseBoxTopLeft        += ellipse_pos;
                CollisionCache.EllipseBoxBottomRight    += ellipse_pos;

            }   // end of update collision cache

            //-------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------

            // Reset number of collisions that have occured:

            m_collision_result_count = 0;

            // Get enumerator for the list of collideable level objects:

            Dictionary<int,GameObject>.Enumerator e = m_level.Data.CollideableObjects.GetEnumerator();

            // Run through the list of collideable objects:

            while ( e.MoveNext() )
            {
                // If this object is the same as the caller then skip it:

                if ( caller == e.Current.Value ) continue;

                // Otherwise do a collision test:

                int num_results = e.Current.Value.OnCollisionQuery
                (
                    ellipse_pos              ,
                    ellipse_scale            ,
                    ellipse_rot              ,
                    caller                   ,
                    m_collision_results      ,
                    m_collision_result_count
                );

                // Make sure number of results is positive:

                if ( num_results < 0 ) num_results = 0;

                // Increase number of collision results:

                m_collision_result_count += num_results;

                // Make sure in range:

                if ( m_collision_result_count > MAX_RESULTS ) m_collision_result_count = MAX_RESULTS;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Preforms a intersection query on the level for the given line. 
        /// </summary>
        /// <param name="line_start"> Start of the line </param>
        /// <param name="line_end"> End of the line </param>
        /// <param name="caller"> Object performing the query. This object will not be included in the test. </param>
        //=========================================================================================

        public void Intersect( Vector2 line_start , Vector2 line_end , GameObject caller )
        {
            //-------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------

            // Update the intersection query cache:

            {
                // Figure out the bounds of the bounding box surrounding the ray:

                IntersectionCache.RayBoxTopLeft     = line_start;
                IntersectionCache.RayBoxBottomRight = line_start;

                if ( line_end.X < IntersectionCache.RayBoxTopLeft.X ) 
                {
                    IntersectionCache.RayBoxTopLeft.X = line_end.X;
                }
               
                if ( line_end.X > IntersectionCache.RayBoxBottomRight.X ) 
                {
                    IntersectionCache.RayBoxBottomRight.X = line_end.X;
                }

                if ( line_end.Y > IntersectionCache.RayBoxTopLeft.Y ) 
                {
                    IntersectionCache.RayBoxTopLeft.Y = line_end.Y;
                }

                if ( line_end.Y < IntersectionCache.RayBoxBottomRight.Y ) 
                {
                    IntersectionCache.RayBoxBottomRight.Y = line_end.Y;
                }

            }   // end of update collision cache

            //-------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------

            // Reset number of intersections that have occured:

            m_intersect_result_count = 0;

            // Get enumerator for the list of collideable level objects:

            Dictionary<int,GameObject>.Enumerator e = m_level.Data.CollideableObjects.GetEnumerator();

            // Run through the list of collideable objects:

            while ( e.MoveNext() )
            {
                // If this object is the same as the caller then skip it:

                if ( caller == e.Current.Value ) continue;

                // Otherwise do an intersection test and save the number of results:

                int num_results = e.Current.Value.OnIntersectQuery
                (
                    line_start                  ,
                    line_end                    ,
                    caller                      ,
                    m_intersect_results         ,
                    m_intersect_result_count
                );

                // Make sure it is in range:

                if ( num_results < 0 ) num_results = 0;

                // Add to current intersect result count:

                m_intersect_result_count += num_results;

                // Clamp to within range:

                if ( m_intersect_result_count > MAX_RESULTS ) m_intersect_result_count = MAX_RESULTS;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Preforms an overlap query on the level for the given rectangle. 
        /// </summary>
        /// <param name="rectangle_position"> 
        /// Center of the bounding rectangle to check for overlap with </param>
        /// <param name="rectangle_dimensions">  
        /// Dimensions of the bounding rectangle to check for overlap with, in +/- x and y directions 
        /// from rectangle center.
        /// </param>
        /// <param name="caller"> 
        /// Object making the overlap query, may be null. 
        /// </param>
        //=========================================================================================

        public void Overlap( Vector2 rectangle_position , Vector2 rectangle_dimensions , GameObject caller )
        {
            // Reset number of overlaps that have occured:

            m_overlap_result_count = 0;

            // Get enumerator for the list of overlapable level objects:

            Dictionary<int,GameObject>.Enumerator e = m_level.Data.OverlapableObjects.GetEnumerator();

            // Run through the list of overlapable objects:

            while ( e.MoveNext() )
            {
                // If this object is the same as the caller then skip it:

                if ( caller == e.Current.Value ) continue;

                // Otherwise do an overlap test and save the result:

                OverlapQueryResult result = e.Current.Value.OnOverlapQuery
                (
                    rectangle_position      ,
                    rectangle_dimensions    ,
                    caller
                );

                // See if there was a result:

                if ( result.ValidResult ) 
                {
                    // Put into the overlaps list if not full already:

                    if ( m_overlap_result_count < MAX_RESULTS )
                    {
                        // On windows debug spit an error out if the object in the query is null:

                        #if WINDOWS_DEBUG

                            if ( result.QueryObject == null ) throw new NullReferenceException("Query result must have a valid query object !");

                        #endif

                        // Put into the list

                        m_overlap_results[m_overlap_result_count] = result;

                        // Increment the number of intersections:

                        m_overlap_result_count++;
                    }
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Returns the closest intersect query result from the last intersect query.
        /// </summary>
        /// <returns> Closest ray intersect query result. </returns>
        //=========================================================================================

        public IntersectQueryResult GetClosestIntersect()
        {
            // Store the closest intersect query here:

            IntersectQueryResult closest_intersect = IntersectQueryResult.NoResult;

            // Run through all the results:

            for ( int i = 0 ; i < m_intersect_result_count ; i++ )
            {
                // See if this result is closer:

                if ( closest_intersect.ValidResult == false || m_intersect_results[i].PointDistance < closest_intersect.PointDistance )
                {
                    // Closer: save 

                    closest_intersect = m_intersect_results[i];
                }
            }

            // Return the closest intersect

            return closest_intersect;
        }

    }   // end of class

}   // end of namespace
