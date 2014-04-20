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
    /// This class represents the result of a bounding rectangle overlap test between two bounding
    /// rectangles. It stores the region of the overlap (if any) and the area of the overlap.
    /// </summary>
    //
    //############################################################################################

    public struct OverlapQueryResult
    {
        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Represents no result in an overlap query. </summary>

            public static readonly OverlapQueryResult NoResult = new OverlapQueryResult
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
            /// Center of the region where the overlap occured.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 RegionPosition; 

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the region where the overlap occured in +/- x and y directions from  
            /// the center of the region.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 RegionDimensions;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Area of the overlap region.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float RegionArea;

        //=========================================================================================
        /// <summary>
        /// Creates an overlap query result.
        /// </summary>
        /// <param name="validResult"> If this is a valid overlap query result </param>
        /// <param name="queryObject"> Object involved in the overlap query </param>
        /// <param name="regionPosition"> Center of the region where the overlap occured </param>
        /// <param name="regionDimensions"> Dimensions of the overlap region in +/- x and y directions from region center </param>
        /// <param name="regionArea"> Area of the overlap region. </param>
        //=========================================================================================

        public OverlapQueryResult
        (
            bool        validResult         ,
            GameObject  queryObject         ,
            Vector2     regionPosition      ,
            Vector2     regionDimensions    ,
            float       regionArea
        )
        {
            ValidResult         = validResult;
            QueryObject         = queryObject;
            RegionPosition      = regionPosition;
            RegionDimensions    = regionDimensions;
            RegionArea          = regionArea;
        }
    }
}
