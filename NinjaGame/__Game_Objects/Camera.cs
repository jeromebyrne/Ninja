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
    /// Core game class. Contains very high level game logic and rendering code.
    /// </summary>
    // 
    //#############################################################################################

    public class Camera : GameObject
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Returns the world matrix for the camera, which is the inverse of the viewing matrix</summary>

            public Matrix World { get { return CalculateWorldMatrix(); } }

            /// <summary> Returns the viewing matrix for the camera </summary>

            public Matrix View { get { return CalculateViewMatrix(); } }

            /// <summary> Returns the projection matrix for the camera </summary>

            public Matrix Projection { get { return CalculateProjectionMatrix(); } }

            /// <summary> Gives the area of the world/gui which is shown by the current camera </summary>

            public Vector2 ViewArea { get { return GetWorldArea(); } }

            /// <summary> Scaling for the camera. Normaly 1. If this is 0.5 for example the world is zoomed out.. </summary>
        
            public float Scale { get { return m_scale; } set { m_scale = value; } }

        //=========================================================================================
        // Constants
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary>
            /// This is the world area that the screen will normally display (Width). If the displays 
            /// resolution is different to this then the game will up/downscale the graphics to fit 
            /// (maintaining the aspect ratio) as best as possible and display extra portions of the 
            /// level if the aspect ratio is different to the reference screen area. 
            /// (In widescreen for example). NOTE: This area also applys to GUI screens !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static readonly int REFERENCE_VIEW_AREA_X = 800;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary>
            /// This is the world area that the screen will normally display (Height). If the displays 
            /// resolution is different to this then the game will up/downscale the graphics to fit 
            /// (maintaining the aspect ratio) as best as possible and display extra portions of the 
            /// level if the aspect ratio is different to the reference screen area. 
            /// (In widescreen for example). NOTE: This area also applys to GUI screens !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static readonly int REFERENCE_VIEW_AREA_Y = 600;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary>
            /// Minimum value that can be in the depth buffer. Anything less and the object will not 
            /// be drawn.. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static readonly float Z_MINIMUM = 0.0f; 

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary>
            /// Maximum value that can be in the depth buffer. Anything more and the object will not 
            /// be drawn.. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static readonly float Z_MAXIMUM = 9999.0f; 

        //=========================================================================================
        // Variables
        //=========================================================================================
            
            /// <summary> Scaling for the camera. Normaly 1. If this is 0.5 for example the world is zoomed out.. </summary>

            private float m_scale = 1.0f;

        //=========================================================================================
        /// <summary>
        /// Constructor for the camera. Creates the camera.
        /// </summary>
        //=========================================================================================

        public Camera() : base( false, false , false, false ){}

        //=========================================================================================
        /// <summary>
        /// Moves the camera towards an object so that it appears to follow the object.
        /// </summary>
        /// <param name="obj"> Object to follow </param>
        /// <param name="interpolateAmount">
        /// Amount that the camera should move over to the object's position by. If this is 1 then 
        /// the camera instantly assumes the position of the object. If this is 0 then the camera 
        /// never will assume the position of the object.
        /// </param>
        //=========================================================================================

        public void Follow( GameObject obj , float interpolateAmount )
        {
            // Clamp interpolate amount from 0-1 

            if ( interpolateAmount < 0 ) interpolateAmount = 0;
            if ( interpolateAmount > 1 ) interpolateAmount = 1;

            // Make the camera follow the object:

            Position = Position * ( 1.0f - interpolateAmount ) + obj.Position * interpolateAmount;
        }

        //=========================================================================================
        /// <summary> Calculates the world matrix for the camera and returns it. </summary>
        /// <returns> World matrix for the camera. </returns>
        //=========================================================================================

        private Matrix CalculateWorldMatrix()
        {
            // Produce the world matrix for the object:

            Matrix worldMatrix = Matrix.CreateTranslation
            (
                Position.X,
                Position.Y,
                0
            );

            // Return it:

            return worldMatrix;
        }

        //=========================================================================================
        /// <summary> Calculates the viewing matrix for the camera and returns it. </summary>
        /// <returns> Viewing matrix for the camera. </returns>
        //=========================================================================================

        private Matrix CalculateViewMatrix()
        {
            // Create a translation for the camera:

            Matrix t = Matrix.CreateTranslation
            (
                - Position.X,
                - Position.Y,
                0
            );

            // Return the viewing matrix:

            return t;
        }

        //=========================================================================================
        /// <summary> Calculates the viewing matrix for the camera and returns it. </summary>
        /// <returns> Viewing matrix for the camera. </returns>
        //=========================================================================================

        public Matrix CalculateProjectionMatrix()
        {
            // Get the area of the game level shown by the screen for current display setup:

            Vector2 screen_area = GetWorldArea();
            
            // Calculate the projection matrix:

            Matrix projection = Matrix.CreateOrthographic
            (
                screen_area.X   ,
                screen_area.Y   ,
                Z_MINIMUM       ,
                Z_MAXIMUM
            );
            
            // Return the projection matrix:

            return projection;
        }

        //=========================================================================================
        /// <summary>
        /// Returns the area of the game level that will be shown according to the current 
        /// display settings of the game. On widescreen displays the area will be different 
        /// than to a 4:3 display. On widescreen, more of the level is shown.
        /// </summary>
        /// <returns> Area of the screen that will be shown for the current display settings. </returns>
        //=========================================================================================

        public Vector2 GetWorldArea()
        {
            // Get the current screen resolution:

            int w = Core.Graphics.Device.Viewport.Width;
            int h = Core.Graphics.Device.Viewport.Height;

            // Calculate the aspect ratio:

            float aspect = (float)(w) / (float)(h);      
      
            // Calculate the reference aspect ratio:

            float reference_aspect = (float)(REFERENCE_VIEW_AREA_X) / (float)(REFERENCE_VIEW_AREA_Y);

            // Calculate the area we will show when widescreen adjustments are taken into account

            float screen_area_x = ( (float)(REFERENCE_VIEW_AREA_X) / reference_aspect ) * aspect;
            float screen_area_y = REFERENCE_VIEW_AREA_Y;

            // If we have a vertical widescreen display (highly unlikely!!!) then normalise the width to reference width and resize the height:

            if ( screen_area_x < REFERENCE_VIEW_AREA_X )
            {
                // Rescale height for vertical widescreen mode:

                screen_area_y /= screen_area_x;
                screen_area_x /= screen_area_x;

                screen_area_x *= REFERENCE_VIEW_AREA_X;
                screen_area_y *= REFERENCE_VIEW_AREA_X;
            }

            // Return the area of the level that will be shown by the screen, with adjustments for scaling:

            return new Vector2
            (
                screen_area_x / m_scale ,
                screen_area_y / m_scale
            );
        }

    }
}
