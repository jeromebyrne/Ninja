using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

//################################################################################################
//################################################################################################

namespace NinjaGame
{
    //############################################################################################
    //
    /// <summary>
    /// This class represents the result of a collision test between a bounding elipse and a 
    /// piece of geometry- which is normally made up of lines. Contains all the required 
    /// information about the collision including how to resolve it.
    /// </summary>
    //
    //############################################################################################

    public struct CollisionQueryResult
    {
        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Represents no result in a collision query. </summary>

            public static readonly CollisionQueryResult NoResult = new CollisionQueryResult
            (
                false           ,
                null            ,
                Vector2.Zero    ,
                Vector2.Zero    ,
                0               ,
                Vector2.Zero    , 
                false   
            );

        //=========================================================================================
        // Variables 
        //=========================================================================================   

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Tells if this is a valid result or not. If this is false then the rest of the data 
            /// should be ignored. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool ValidResult;

            /// <summary> The object which was queried to produce this result. </summary>

            public GameObject QueryObject;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// The point where the collision happened. Although the intersection area is not a point 
            /// we generally reduce this sort of collision test to moving one point away from another 
            /// in the end. This is the point we should move away from to resolve the collision.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 Point; 

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Normal or perpendicular direction to the surface at the point of collision.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 Normal;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// The amount of penetration of the elipse into the geometry. This is the amount we should
            /// move back by.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float Penetration;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This is the direction the elipse should move back in to resolve the collision. This 
            /// direction will be normalised.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 ResolveDirection;
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary>
            /// Tells if the collision is a point collision. If so then the elipse has collided with a 
            /// point at the end of a line and not the line's plane itself.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool IsPointCollision;

        //=========================================================================================
        /// <summary>
        /// Creates collision query result.
        /// </summary>
        /// <param name="validResult"> If this is a valid collision query result </param>
        /// <param name="queryObject"> Object involved in the collision query </param>
        /// <param name="point"> Point where the collision happened </param>
        /// <param name="normal"> Normal at the point of collision </param>
        /// <param name="penetration"> Amount of penetration into geometry by the elipse </param>
        /// <param name="resolveDirection"> Direction the elipse must move in to resolve collision </param>
        /// <param name="isPointCollision"> 
        /// Tells if the collision is a point collision. If so then the elipse has collided with a 
        /// point at the end of a line and not the line's plane itself.
        /// </param>
        //=========================================================================================

        public CollisionQueryResult
        (
            bool        validResult         ,
            GameObject  queryObject         ,
            Vector2     point               ,
            Vector2     normal              ,
            float       penetration         ,
            Vector2     resolveDirection    , 
            bool        isPointCollision    
        )
        {
            ValidResult         = validResult;
            QueryObject         = queryObject;
            Point               = point;
            Normal              = normal;
            Penetration         = penetration;
            ResolveDirection    = resolveDirection;
            IsPointCollision    = isPointCollision;
        }
    }
}
