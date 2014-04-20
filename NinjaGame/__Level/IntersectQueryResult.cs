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
    /// This class represents the result of a collision test between a line and a 
    /// piece of geometry- which is normally made up of other lines. Contains all the required 
    /// information about the intersection. The point of intersection returned should be the 
    /// closest one along the direction of the line.
    /// </summary>
    //
    //############################################################################################

    public struct IntersectQueryResult
    {
        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Represents no result in an intersection query. </summary>

            public static readonly IntersectQueryResult NoResult = new IntersectQueryResult
            (
                false           ,
                null            ,
                Vector2.Zero    ,
                Vector2.Zero    , 
                0
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
            /// The point where the intersection happened.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 Point; 

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Normal or perpendicular direction to the surface at the point of intersection.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 Normal;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Distance that the intersection point is along the line that we are using for the 
            /// ray casting. This distance is relative to the start of the line:
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float PointDistance;

        //=========================================================================================
        /// <summary>
        /// Creates an intersect query result.
        /// </summary>
        /// <param name="validResult"> If this is a valid intersect query result </param>
        /// <param name="queryObject"> Object involved in the intersect query </param>
        /// <param name="point"> Point where the intersection happened </param>
        /// <param name="normal"> Normal at the point of intersection </param>
        /// <param name="pointDistance"> Distance from the start point of the ray cast the intersection point is </param>
        //=========================================================================================

        public IntersectQueryResult
        (
            bool        validResult             ,
            GameObject  queryObject             ,
            Vector2     point                   ,
            Vector2     normal                  ,
            float       pointDistance
        )
        {
            ValidResult     = validResult;
            QueryObject     = queryObject;
            Point           = point;
            Normal          = normal;
            PointDistance   = pointDistance;
        }

    }   // end of class

}   // end of namespace
