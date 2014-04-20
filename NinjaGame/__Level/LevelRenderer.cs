using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
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
    /// Class that handles the rendering of the level. The renderer is owned by the level itself.
    /// </summary>
    //
    //#############################################################################################

    public class LevelRenderer
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Camera object used to render the level from. If no camera is given the renderer will 
            /// create it's own to get by with.. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                public Camera Camera 
                { 
                    get 
                    { 
                        // If there is no camera then make one:

                        if ( m_camera == null ) m_camera = new Camera(); return m_camera; 
                    } 
                    
                    set 
                    { 
                        // Set camera:

                        m_camera = value; 

                        // If null make a new one:

                        if ( m_camera == null ) m_camera = new Camera(); 
                    } 
                }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Parent level that the renderer draws </summary>

            private Level m_level = null;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Camera object used to render the level from. If no camera is given the renderer will 
            /// create it's own to get by with.. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Camera m_camera = null;

            /// <summary> A heap used for depth sorting objects when rendering. </summary>

            private Heap<GameObject> m_render_heap = new Heap<GameObject>(16,null);

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the level renderer.
        /// </summary>
        /// <param name="parentLevel"> 
        /// Parent level for the renderer. Contains the objects to be rendered.
        /// </param>
        //=========================================================================================

        public LevelRenderer( Level parentLevel )
        {
            // If the parent level is null then complain in windows debug:

            #if WINDOWS_DEBUG

                if ( parentLevel == null ) throw new Exception("LevelRenderer must have a valid parent Level object");

            #endif

            // Save the parent level:

            m_level = parentLevel;
        }

        //=========================================================================================
        /// <summary>
        /// Renders the level with the currently selected camera. If none is selected then a 
        /// default camera will be used instead.
        /// </summary>
        //=========================================================================================

        public void Draw()
        {
            // If there is no camera then make one:

            if ( m_camera == null )
            {
                // Make a camera:

                m_camera = new Camera();
            }

            // If this is debug mode and the debug camera is on then use that instead

            #if DEBUG

                // Save the previously used camera

                Camera previous_camera = m_camera;
            
                // See if we are to use the debug camera:

                if ( DebugLevel.UseDebugCamera ) m_camera = DebugLevel.DebugCamera;

            #endif

            // Lock the level data so that the data collections are not modified whilst we enumerate:

            m_level.Data.Lock();

                // Add all visible objects into the render heap:

                Dictionary<int,GameObject>.Enumerator e = m_level.Data.RenderableObjects.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Get this object:

                    GameObject obj = e.Current.Value;

                    // See if it is visible: if not then ignore

                    if ( obj.IsVisible( m_camera ) == false ) continue;

                    // Add all visible objects into the render heap:

                    m_render_heap.Add( obj.Depth , obj );
                }

            // Unlock the level data:

            m_level.Data.Unlock();

            // Render objects in the render heap, starting with the deepest first:

            while ( m_render_heap.Count > 0 )
            {
                // Get this object:

                GameObject obj = m_render_heap.Remove(); 

                // Draw it:

                obj.OnDraw();
            }

            // Let the particle emitter do it's rendering:

            m_level.Emitter.Draw();

            // If in debug mode then do a debug draw if enabled:

            #if DEBUG

                if ( DebugLevel.ShowDebugInfo ) DebugDraw();

            #endif

            // If this is debug mode and the debug camera is on then restore the previous camera used

            #if DEBUG

                // See if we are to use the debug camera: if so then restore the normal camera

                if ( DebugLevel.UseDebugCamera ) m_camera = previous_camera;

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Debug only function that draws debug information for all objects in the level.
        /// </summary>
        //=========================================================================================

        #if DEBUG

            public void DebugDraw()
            {
                // If there is no camera then make one:

                if ( m_camera == null )
                {
                    // Make a camera:

                    m_camera = new Camera();
                }

                // If this is debug mode and the debug camera is on then use that instead

                #if DEBUG

                    // Save the previously used camera

                    Camera previous_camera = m_camera;
                
                    // See if we are to use the debug camera:

                    if ( DebugLevel.UseDebugCamera ) m_camera = DebugLevel.DebugCamera;

                #endif

                // Lock the level data so that the data collections are not modified whilst we enumerate:

                m_level.Data.Lock();

                // Run through the list of renderable objects and debug draw them all:

                Dictionary<int,GameObject>.Enumerator e = m_level.Data.Objects.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Debug draw this object:

                    e.Current.Value.OnDebugDraw();
                }

                // Unlock the level data:

                m_level.Data.Unlock();

                // If this is debug mode and the debug camera is on then restore the previous camera used

                #if DEBUG

                    // See if we are to use the debug camera: if so then restore the normal camera

                    if ( DebugLevel.UseDebugCamera ) m_camera = previous_camera;

                #endif
            }

        #endif  // If DEBUG

    }
}
