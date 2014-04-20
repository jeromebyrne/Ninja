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
    /// Base class for all objects in the game. Contains the basic structure for a game object which 
    /// will be inherited by all in game objects. 
    /// </summary>
    // 
    //#############################################################################################

    public abstract class GameObject : XmlObject
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Level the game object is contained in. This should only be modified by LevelData objects. </summary>

            public Level ParentLevel
            {
                get { return m_level; } 

                // On debug windows make sure this is not called anywhere except from a level data object

                #if WINDOWS_DEBUG

                    set { DebugAssembly.EnsureCallingClass("LevelData"); m_level = value; }

                #else

                    set { m_level = value; }

                #endif                
            }

            /// <summary> Level data object the object is encapsulated in. This should only be modified by LevelData objects. </summary>

            public LevelData ParentContainer
            {
                get { return m_container; } 

                // On debug windows make sure this is not called anywhere except from a level data object

                #if WINDOWS_DEBUG

                    set { DebugAssembly.EnsureCallingClass("LevelData"); m_container = value; }

                #else

                    set { m_container = value; }

                #endif
            }

            /// <summary> Name of the object. This does not have to be unique or even filled in but must not be null. </summary>
        
            public string Name 
            { 
                get { return m_name; } 

                // Set method: unregister current name and register new name

                set
                {
                    // Throw exception on debug windows if a null name is given:

                    #if WINDOWS_DEBUG

                        if ( m_name == null ) throw new Exception("Object name cannot be null");

                    #endif

                    // Unregister current name and register the new one:

                    if ( m_container != null ) 
                    {
                        m_container.UnregisterName(this,m_name);
                        m_container.RegisterName(this,value);
                    }

                    // Set new name:

                    m_name = value;
                }
            }

            /// <summary> ID of the object in the level. This should always be unqiue. 0 is an invalid ID. </summary>

            public int Id 
            { 
                get { return m_id; } 

                // On debug windows make sure this is not called anywhere except from a level data object

                #if WINDOWS_DEBUG

                    set { DebugAssembly.EnsureCallingClass("LevelData"); m_id = value; }

                #else

                    set { m_id = value; }

                #endif
            } 

            /// <summary> Position of the object. </summary>
            
            public Vector2 Position { get { return m_position; } set { m_position = value; } }

            /// <summary> X Position of the object. </summary>

            public float PositionX { get { return m_position.X; } set { m_position.X = value; } }

            /// <summary> Y Position of the object. </summary>

            public float PositionY { get { return m_position.Y; } set { m_position.Y = value; } }

            /// <summary> Depth of the object. Used for z-ordering or layering 2D sprites. </summary>

            public int Depth { get { return m_depth; } set { m_depth = value; }  }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the object's bounding box in +/- x and y directions from object position
            /// or center of the object. Used in rendering to cull objects and for the overlap test. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 BoxDimensions { get { return m_box_dimensions; } set { m_box_dimensions = value; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the object's bounding box in +/- x directions from object position
            /// or center of the object. Used in rendering to cull objects and for the overlap test. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float BoxDimensionsX { get { return m_box_dimensions.X; } set { m_box_dimensions.X = value; } } 

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the object's bounding box in +/- y directions from object position
            /// or center of the object. Used in rendering to cull objects and for the overlap test. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float BoxDimensionsY { get { return m_box_dimensions.Y; } set { m_box_dimensions.Y = value; } } 

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Is the object renderable ? If false then Draw() will never be called </summary>

            public readonly bool Renderable;

            /// <summary> Can the object be updated ? If false then Update() will never be called </summary>

            public readonly bool Updateable;

            /// <summary> Is the object collideable ? If false then it will be ignored for collision and intersect queries </summary>

            public readonly bool Collideable;

            /// <summary> Is the object overlapable ? If false then it will be ignored for bounding box overlap queries </summary>
            
            public readonly bool Overlapable;

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Level the game object is contained in. </summary>

            private Level m_level = null;

            /// <summary> Level data object the object is encapsulated in. </summary>

            private LevelData m_container = null;

            /// <summary> Name of the object. This does not have to be unique or even filled in but must not be null. </summary>

            private string m_name = "";

            /// <summary> ID of the object in the level. This should always be unqiue. 0 is an invalid ID. </summary>

            private int m_id = 0;

            /// <summary> Position of the object. </summary>

            private Vector2 m_position = Vector2.Zero;

            /// <summary> Depth of the object. Used for z-ordering or layering 2D sprites.</summary>

            private int m_depth = 1;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the object's bounding box in +/- x and y directions from object position
            /// or center of the object. Used in rendering to cull objects and for the overlap test. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Vector2 m_box_dimensions = Vector2.One;

            //=====================================================================================
            /// <summary>
            /// Game object constructor. Constructs a default game object.
            /// </summary>
            /// <param name="renderable"> If the object can be drawn </param>
            /// <param name="collideable"> If the object can block the movement of other objects </param>
            /// <param name="updateable"> If the object can be updated </param>
            /// <param name="overlapable"> If the object can be used in an bounding box overlap query </param>
            //=====================================================================================

            public GameObject
            (
                bool renderable     ,
                bool collideable    ,
                bool updateable     ,
                bool overlapable    
            )
            {
                // Save the object attributes

                Renderable  = renderable;
                Collideable = collideable;
                Updateable  = updateable;
                Overlapable = overlapable;
            }

            //=====================================================================================
            /// <summary>
            /// Called when the object is removed from the level.
            /// </summary>
            //=====================================================================================

            public virtual void OnDelete(){;}

            //=====================================================================================
            /// <summary>
            /// Called when the object is to be drawn
            /// </summary>
            //=====================================================================================

            public virtual void OnDraw(){;}

            //=====================================================================================
            /// <summary>
            /// This function is called once after a level has loaded and after the OnLevelLoaded()
            /// function. It allows the class to cahche commonly loaded data for future created 
            /// instances. Example uses would be to pre-cache enemy ninja sprites once on startup 
            /// rather than loading them every time a ninja is spawned (slow!!). All a class needs to 
            /// do for this to happen is to declare this function with this exact name and format 
            /// and it will be called. 
            /// 
            /// N.B Base class versions of this function are called first; as per constructor convention.
            /// This will be called for abstract classes also.
            /// </summary>
            //=====================================================================================

            private static void BuildClassCache(){}

            //=====================================================================================
            /// <summary>
            /// This function is called once after a level has been cleared. The data that was 
            /// allocated in BuildCache() should be destroyed or references should be cleared to 
            /// it here. All a class needs to do for this to happen is to declare this function
            /// with this exact name and format and it will be called. 
            /// 
            /// N.B Derived class versions of this function are called first; as per destructor convention.
            /// This will be called for abstract classes also. If BuildCache() is not present however 
            /// this function will not be called. If BuildCache() is there however, this function 
            /// MUST be implemented or an error will be thrown and the class will be ignored for 
            /// caching.
            /// </summary>
            //=====================================================================================

            private static void ClearClassCache(){}

            //=====================================================================================
            /// <summary>
            /// Debug only function. Allows object to render information on itself. 
            /// Only on windows debug.
            /// </summary>
            //=====================================================================================
            
            #if DEBUG

                public virtual void OnDebugDraw()
                {
                    // Draw object bounding box:

                    DebugDrawing.DrawWorldRectangle( Position , BoxDimensions , Color.Red );

                    // Draw crosshair at object center:

                    DebugDrawing.DrawWorldRectangle( Position , new Vector2(4,0) , Color.Magenta );
                    DebugDrawing.DrawWorldRectangle( Position , new Vector2(0,4) , Color.Magenta );

                    // Get the world view projection matrix:

                    Matrix world_view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

                    // Draw the actual text itself:

                        // Now the id of the object with the debeug font: use camera transforms

                        Core.DebugFont.DrawString
                        (
                            m_id.ToString(Locale.DevelopmentCulture.NumberFormat)   , 
                            m_position                                              ,
                            world_view_projection
                        );

                        // Now the type of the object with the debug font: use camera transforms

                        Core.DebugFont.DrawString
                        (
                            GetType().Name                      , 
                            m_position - Vector2.UnitY * 24     ,
                            world_view_projection
                        );
                }

            #endif

            //=====================================================================================
            /// <summary>
            /// Called when the object is to have it's state updated. Objects should move according 
            /// to the values in the global timing system or do logic here.
            /// </summary>
            //=====================================================================================

            public virtual void OnUpdate(){;}

            //=====================================================================================
            /// <summary>
            /// Called after a level has completely loaded and all objects have been added to the 
            /// level. If this level contains references to other objects (using names) then those 
            /// references should be resolved here, since it will be guaranteed that all objects 
            /// that are to be made will have been made at this point.
            /// </summary>
            //=====================================================================================

            public virtual void OnLevelLoaded(){;}

            //=====================================================================================
            /// <summary>
            /// This function allows the game object to tell if it is visible with the given camera 
            /// view. This is used by the renderer for vis testing.
            /// </summary>
            /// <param name="c"> Camera the scene is being viewed from. </param>
            /// <returns> True if the object should be drawn, false otherwise. </returns>
            //=====================================================================================

            public virtual bool IsVisible( Camera c )
            {
                // Calculate the bounds of the top left and bottom right corners of the screen:

                Vector2 tl_bounds = c.Position;
                Vector2 br_bounds = c.Position;

                tl_bounds.X -= c.ViewArea.X * 0.5f;
                br_bounds.X += c.ViewArea.X * 0.5f;
                tl_bounds.Y += c.ViewArea.Y * 0.5f;
                br_bounds.Y -= c.ViewArea.Y * 0.5f;

                // Add a little to the bounds to account for error:

                tl_bounds.X -= 2.0f;
                tl_bounds.Y += 2.0f;
                br_bounds.X += 2.0f;
                br_bounds.Y -= 2.0f;

                // Check if within screen bounds:

                if ( m_position.X + m_box_dimensions.X < tl_bounds.X ) return false;
                if ( m_position.X - m_box_dimensions.X > br_bounds.X ) return false;
                if ( m_position.Y - m_box_dimensions.Y > tl_bounds.Y ) return false;
                if ( m_position.Y + m_box_dimensions.Y < br_bounds.Y ) return false;

                // If we got to here then we are visible: return true

                return true;
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

            public virtual int OnCollisionQuery
            ( 
                Vector2                 elipsePosition      ,
                Vector2                 elipseDimensions    ,
                float                   elipseRotation      ,
                GameObject              otherObject         ,
                CollisionQueryResult[]  results             ,
                int                     results_index
            )
            {
                // No collision results:

                return 0;
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

            public virtual int OnIntersectQuery
            ( 
                Vector2                 lineStart           ,
                Vector2                 lineEnd             ,
                GameObject              otherObject         ,      
                IntersectQueryResult[]  results             ,
                int                     results_index
            )
            {
                // No collision results:

                return 0;
            }

            //=====================================================================================
            /// <summary>
            /// Overlap query function that checks if a given bounding rectangle overlaps the bounding 
            /// rectangle of this object. Any objects that have flagged themselves as overlapable should 
            /// implement this test. 
            /// </summary>
            /// <param name="rectangle_position"> 
            /// Center of the bounding rectangle to check for overlap with 
            /// </param>
            /// <param name="rectangle_dimensions">  
            /// Dimensions of the bounding rectangle to check for overlap with, in +/- x and y directions 
            /// from rectangle center.
            /// </param>
            /// <param name="other_object"> Object making the overlap query, may be null. </param>
            /// <returns> Result of the collision query test. </returns>
            //=====================================================================================

            public virtual OverlapQueryResult OnOverlapQuery
            ( 
                Vector2         rectangle_position   ,
                Vector2         rectangle_dimensions ,
                GameObject      other_object       
            )
            {
                // Get the x position of the left and right sides of both rectangles:

                float r1_lx = m_position.X - m_box_dimensions.X;
                float r1_rx = m_position.X + m_box_dimensions.X;
      
                float r2_lx = rectangle_position.X - rectangle_dimensions.X;
                float r2_rx = rectangle_position.X + rectangle_dimensions.X;

                // See which has the left most point:

                if ( r1_lx < r2_lx )
                {
                    // Our rectangle has left most point: if the left most point of the other rectangle falls before our right most point then there is x overlap

                    if ( r2_lx < r1_rx )
                    {
                        // X - Overlap. Calculate the y positions of the top and bottom of both rectangles:

                        float r1_ty = m_position.Y + m_box_dimensions.Y;
                        float r1_by = m_position.Y - m_box_dimensions.Y;
              
                        float r2_ty = rectangle_position.Y + rectangle_dimensions.Y;
                        float r2_by = rectangle_position.Y - rectangle_dimensions.Y;

                        // See which rectangle has the lowest bottom point:

                        if ( r1_by < r2_by )
                        {
                            // Our rectangle has the lowest bottom point: see if the bottom point of the other rectangle is under the top point of our rectangle

                            if ( r2_by < r1_ty )
                            {
                                // Overlap: calculate the left / right and top / bottom x and y values of the overlap area

                                float ol_lx = r2_lx;
                                float ol_rx = r1_rx;
                                float ol_by = r2_by;
                                float ol_ty = r1_ty;

                                // Calculate the center of the overlap area:

                                float ol_cx = ( ol_lx + ol_rx ) * 0.5f;
                                float ol_cy = ( ol_by + ol_ty ) * 0.5f;

                                // Calculate the dimensions of the overlap region from the center

                                float ol_dx = ol_cx - ol_lx;
                                float ol_dy = ol_cy - ol_by;

                                // Calculate the area of the overlap:

                                float ol_area = ol_dx * ol_dy * 4;

                                // Makeup the overlap result and return it:

                                return new OverlapQueryResult
                                (
                                    true                        ,
                                    this                        ,
                                    new Vector2(ol_cx,ol_cy)    ,
                                    new Vector2(ol_dx,ol_dy)    ,
                                    ol_area
                                );
                            }
                            else
                            {
                                // No overlap:

                                return OverlapQueryResult.NoResult;
                            }
                        }
                        else
                        {
                            // The other rectangle has the lowest bottom point: see if the bottom point of our rectangle is under the top point of the other rectangle

                            if ( r1_by < r2_ty )
                            {
                                // Overlap: calculate the left / right and top / bottom x and y values of the overlap area

                                float ol_lx = r2_lx;
                                float ol_rx = r1_rx;
                                float ol_by = r1_by;
                                float ol_ty = r2_ty;

                                // Calculate the center of the overlap area:

                                float ol_cx = ( ol_lx + ol_rx ) * 0.5f;
                                float ol_cy = ( ol_by + ol_ty ) * 0.5f;

                                // Calculate the dimensions of the overlap region from the center

                                float ol_dx = ol_cx - ol_lx;
                                float ol_dy = ol_cy - ol_by;

                                // Calculate the area of the overlap:

                                float ol_area = ol_dx * ol_dy * 4;

                                // Makeup the overlap result and return it:

                                return new OverlapQueryResult
                                (
                                    true                        ,
                                    this                        ,
                                    new Vector2(ol_cx,ol_cy)    ,
                                    new Vector2(ol_dx,ol_dy)    ,
                                    ol_area
                                );
                            }
                            else
                            {
                                // No overlap:

                                return OverlapQueryResult.NoResult;
                            }
                        }
                    }
                    else
                    {
                        // No overlap:

                        return OverlapQueryResult.NoResult;
                    }
                }
                else
                {
                    // Other rectangle has left most point: see if our left most point falls before it's right most point

                    if ( r1_lx < r2_rx )
                    {
                        // X - Overlap. Calculate the y positions of the top and bottom of both rectangles:

                        float r1_ty = m_position.Y + m_box_dimensions.Y;
                        float r1_by = m_position.Y - m_box_dimensions.Y;
              
                        float r2_ty = rectangle_position.Y + rectangle_dimensions.Y;
                        float r2_by = rectangle_position.Y - rectangle_dimensions.Y;

                        // See which rectangle has the lowest bottom point:

                        if ( r1_by < r2_by )
                        {
                            // Our rectangle has the lowest bottom point: see if the bottom point of the other rectangle is under the top point of our rectangle

                            if ( r2_by < r1_ty )
                            {
                                // Overlap: calculate the left / right and top / bottom x and y values of the overlap area

                                float ol_lx = r1_lx;
                                float ol_rx = r2_rx;
                                float ol_by = r2_by;
                                float ol_ty = r1_ty;

                                // Calculate the center of the overlap area:

                                float ol_cx = ( ol_lx + ol_rx ) * 0.5f;
                                float ol_cy = ( ol_by + ol_ty ) * 0.5f;

                                // Calculate the dimensions of the overlap region from the center

                                float ol_dx = ol_cx - ol_lx;
                                float ol_dy = ol_cy - ol_by;

                                // Calculate the area of the overlap:

                                float ol_area = ol_dx * ol_dy * 4;

                                // Makeup the overlap result and return it:

                                return new OverlapQueryResult
                                (
                                    true                        ,
                                    this                        ,
                                    new Vector2(ol_cx,ol_cy)    ,
                                    new Vector2(ol_dx,ol_dy)    ,
                                    ol_area
                                );
                            }
                            else
                            {
                                // No overlap:

                                return OverlapQueryResult.NoResult;
                            }
                        }
                        else
                        {
                            // The other rectangle has the lowest bottom point: see if the bottom point of our rectangle is under the top point of the other rectangle

                            if ( r1_by < r2_ty )
                            {
                                // Overlap: calculate the left / right and top / bottom x and y values of the overlap area

                                float ol_lx = r1_lx;
                                float ol_rx = r2_rx;
                                float ol_by = r1_by;
                                float ol_ty = r2_ty;

                                // Calculate the center of the overlap area:

                                float ol_cx = ( ol_lx + ol_rx ) * 0.5f;
                                float ol_cy = ( ol_by + ol_ty ) * 0.5f;

                                // Calculate the dimensions of the overlap region from the center

                                float ol_dx = ol_cx - ol_lx;
                                float ol_dy = ol_cy - ol_by;

                                // Calculate the area of the overlap:

                                float ol_area = ol_dx * ol_dy * 4;

                                // Makeup the overlap result and return it:

                                return new OverlapQueryResult
                                (
                                    true                        ,
                                    this                        ,
                                    new Vector2(ol_cx,ol_cy)    ,
                                    new Vector2(ol_dx,ol_dy)    ,
                                    ol_area
                                );
                            }
                            else
                            {
                                // No overlap:

                                return OverlapQueryResult.NoResult;
                            }
                        }
                    }
                    else
                    {
                        // No overlap:

                        return OverlapQueryResult.NoResult;
                    }
                }
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

                // Read the new name of the object if it exists:

                {
                    // Store the new name here

                    string new_name = null;

                    // Read it:

                    if ( data.ReadString( "Name", ref new_name ) )
                    {
                        // Unregister old name:

                        if ( m_container != null ) m_container.UnregisterName(this,m_name);

                        // Save new name:

                        m_name = new_name;

                        // Register new name:

                        if ( m_container != null ) m_container.RegisterName(this,m_name);
                    }
                }

                // Read all other attributes:

                data.ReadInt  ( "Depth"             , ref m_depth                       );
                data.ReadFloat( "PositionX"         , ref m_position.X          , 0     );
                data.ReadFloat( "PositionY"         , ref m_position.Y          , 0     );
                data.ReadFloat( "BoxDimensionsX"    , ref m_box_dimensions.X    , 128   );
                data.ReadFloat( "BoxDimensionsY"    , ref m_box_dimensions.Y    , 128   );                
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

                data.Write( "Name"              , m_name                );
                data.Write( "Depth"             , m_depth               );
                data.Write( "PositionX"         , m_position.X          );
                data.Write( "PositionY"         , m_position.Y          );
                data.Write( "BoxDimensionsX"    , m_box_dimensions.X    );
                data.Write( "BoxDimensionsY"    , m_box_dimensions.Y    );
            }
    }
}
