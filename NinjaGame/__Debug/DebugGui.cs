#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
    /// 
    /// Contains console commands for gui editing and debugging the gui. The console functions 
    /// are only defined on windows however, and the class itself is only defined in debug mode 
    /// on all platforms. Practically the same as the DebugLevel module. Most of the Level stuff 
    /// has been repeated to do Guis.
    /// 
    /// </summary>
    //
    //#############################################################################################

    public class DebugGui
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Debug only variable. Draws debug information for widgets if this is set to true.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static bool ShowDebugInfo { get { return s_show_debug_info; } }

            /// <summary> A debug camera that can be moved around freely. The current gui should use this when specified. </summary>
        
            public static Camera DebugCamera { get { return s_debug_camera; } }

            /// <summary> Set to true when the gui rendering widgets should use the debug camera here </summary>
        
            public static bool UseDebugCamera { get { return s_use_debug_camera; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> The gui should draw debug information for objects if this is set to true. </summary>

            private static bool s_show_debug_info = false;

            /// <summary> A debug camera that can be moved around freely </summary>
        
            private static Camera s_debug_camera = null;

            /// <summary> A debug camera that can be moved around freely. The current gui should use this when specified. </summary>

            private static bool s_use_debug_camera = false;

        //=========================================================================================
        /// <summary>
        /// Adds all debug commands to do with the gui to the debug console.
        /// </summary>
        //=========================================================================================

        public static void Initialize()
        {
            // Add console commands on windows

            #if WINDOWS_DEBUG

                DebugConsole.AddCommand( "GInfo"        , new DebugConsole.Function_bool_bool(Con_GInfo)        );
                DebugConsole.AddCommand( "GEdit"        , new DebugConsole.Function_void_int(Con_GEdit)         );
                DebugConsole.AddCommand( "GSave"        , new DebugConsole.Function_void_string(Con_GSave)      );
                DebugConsole.AddCommand( "GLoad"        , new DebugConsole.Function_void_string(Con_GLoad)      );
                DebugConsole.AddCommand( "GNew"         , new DebugConsole.Function_void_string(Con_GNew)       );
                DebugConsole.AddCommand( "GDelete"      , new DebugConsole.Function_void_int(Con_GDelete)       );
                DebugConsole.AddCommand( "GClone"       , new DebugConsole.Function_void_int(Con_GClone)        );
                DebugConsole.AddCommand( "GMoveCamera"  , new DebugConsole.Function_bool_bool(Con_GMoveCamera)  );

            #endif

            // Make the debug camera:

            s_debug_camera = new Camera();
        }

        //=========================================================================================
        /// <summary>
        /// Debug console command. Turns on/off using the debug camera or free camera in the gui.
        /// </summary>
        /// <param name="arg">          Argument given from the console by user.    </param>
        /// <param name="arg_given">    If an argument was actually given.          </param>
        /// <returns>                   Value of this flag.                         </returns>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static bool Con_GMoveCamera( bool arg , bool arg_given )
            {
                if ( arg_given ) s_use_debug_camera = arg; return s_use_debug_camera;
            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Loads the given gui file.
        /// </summary>
        /// <param name="arg"> Argument given from the console by user. Level to load. </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_GLoad( string arg )
            {
                // Abort if no name given:

                if ( arg.Length == 0 ) DebugConsole.Print("\nGLoad: Must give gui xml file to load from.");

                // Load the given file

                Core.Gui.Load(arg);
            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Save the given gui file.
        /// </summary>
        /// <param name="arg"> Argument given from the console by user. Level to save to. </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_GSave( string arg )
            {
                // Abort if no name given:

                if ( arg.Length == 0 ) DebugConsole.Print("\nGSave: Must give gui xml file to save to.");

                // Load the given file

                Core.Gui.Save(arg);
            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Switches on / off drawing debug information for the gui.
        /// </summary>
        /// <param name="arg">          Argument given from the console by user.    </param>
        /// <param name="arg_given">    If an argument was actually given.          </param>
        /// <returns>                   Value of this flag.                         </returns>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static bool Con_GInfo ( bool arg , bool arg_given )
            {
                // Enable / disable debug drawing

                if ( arg_given ) s_show_debug_info = arg;

                // Return current value of debug drawing

                return s_show_debug_info;
            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Attempts to create an object of the given type and 
        /// opens up the editor on it.
        /// </summary>
        /// <param name="type"> Argument given from the console by user. Type of object to make. </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_GNew( string type )
            {
                // Abort if no name given:

                if ( type.Length == 0 ) DebugConsole.Print("\nGNew: Must give a valid type of gui widget object to create.");

                // Try and make the given object:

                try
                {
                    // Make it:

                    XmlObject xml_obj = XmlFactory.CreateObject(type);

                    // See if that failed:

                    if ( xml_obj == null )
                    {
                        DebugConsole.Print("\nGNew: Not a valid GuiWidget sub-type."); return;
                    }

                    // Attempt to cast to a gui widget:

                    GuiWidget widget_obj = (GuiWidget)(xml_obj);

                    // See if that failed:

                    if ( widget_obj == null )
                    {
                        DebugConsole.Print("\nGNew: Not a valid GuiWidget sub-type."); return;
                    }

                    // Add it to the current gui:

                    Core.Gui.Data.Add(widget_obj);

                    // Make a blank xml document:

                    XmlDocument doc = new XmlDocument(); doc.AppendChild( doc.CreateElement("data") );

                    // Make an xml object data object:

                    XmlObjectData data = new XmlObjectData(doc.FirstChild);

                    // Do a fake read and allow the object to initialise some default values:

                    widget_obj.ReadXml(data);

                    // Open up the editor on it:

                    try
                    {
                        XmlObjectEditor.EditObject(widget_obj,"Editing gui widget with ID '" + widget_obj.Id + "' of type " + widget_obj.GetType().Name );
                    }
                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                }
                catch ( Exception e ){ DebugConsole.PrintException(e); }

            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Attempts to delete the widget with the given id.
        /// </summary>
        /// <param name="object_id"> Id of the object to delete </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_GDelete( int object_id )
            {
                // See if the object exists in the level:

                GuiWidget clone_obj = Core.Gui.Search.FindById(object_id);

                if ( clone_obj == null )
                {
                    DebugConsole.Print("\nGDelete: given object id does not exist in current gui ! "); return;
                }

                // Remove the object from the current gui:

                Core.Gui.Data.Remove( object_id );
            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Attempts to clone an object with the given id and opens up 
        /// the xml editor on it.
        /// </summary>
        /// <param name="type"> Argument given from the console by user. Object to clone and edit. </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_GClone( int object_id  )
            {
                // See if the object exists in the current gui:

                GuiWidget clone_obj = Core.Gui.Search.FindById(object_id);

                if ( clone_obj == null )
                {
                    DebugConsole.Print("\nGClone: given object id does not exist in current Gui ! "); return;
                }

                // Try and make a new copy of the object we found:

                try
                {
                    // Make it:

                    XmlObject xml_obj = XmlFactory.CreateObject( clone_obj.GetType().Name );

                    // Attempt to cast to a gui widget:

                    GuiWidget widget_obj = (GuiWidget)(xml_obj);

                    // Add it to the current gui:

                    Core.Gui.Data.Add(widget_obj);

                    // Create an xml document with a root data node:

                    XmlDocument xml_doc = new XmlDocument(); XmlNode xml_node = xml_doc.CreateElement("data");

                    xml_doc.AppendChild(xml_node);

                    // Create an xml data object:

                    XmlObjectData xml_data = new XmlObjectData(xml_node);

                    // Read the data from the object we are cloning:

                    try
                    {
                        clone_obj.WriteXml(xml_data);
                    }
                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                    // Write the data to the object we are creating:

                    try
                    {
                        widget_obj.ReadXml(xml_data);
                    }
                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                    // Open up the editor on it:

                    try
                    {
                        XmlObjectEditor.EditObject(widget_obj,"Editing gui widget with ID '" + widget_obj.Id + "' of type " + widget_obj.GetType().Name);
                    }
                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                }
                catch ( Exception e ){ DebugConsole.PrintException(e); }

            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Edits the object with the given id. 
        /// </summary>
        /// <param name="object_id"> Argument given from the console by user. Id of object to edit. </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_GEdit( int object_id )
            {
                // See if the object exists in the level:

                GuiWidget obj = Core.Gui.Search.FindById(object_id);

                if ( obj != null )
                {
                    // Edit the object:

                    XmlObjectEditor.EditObject(obj,"Editing gui widget with ID '" + obj.Id + "' of type " + obj.GetType().Name );
                }
                else
                {
                    // Print an error: object does not exist

                    DebugConsole.Print("\nGEdit: given object id does not exist in the current gui!");
                }

            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Does time based debug behaviour.
        /// </summary>
        //=========================================================================================

        public static void Update()
        {
            // Movement speeds for the debug camera:

            const float MOVE_SPEED = 20.0f;
            const float ZOOM_SPEED = 0.02f;

            // If we are using the debug camera then move it if the right keys are pressed:

            if ( s_use_debug_camera )
            {
                // Get keyboard state:

                KeyboardState keyboard_state = Keyboard.GetState();

                // See what keys are pressed:

                if ( keyboard_state.IsKeyDown(Keys.Up)          ) s_debug_camera.PositionY += MOVE_SPEED / s_debug_camera.Scale;
                if ( keyboard_state.IsKeyDown(Keys.Down)        ) s_debug_camera.PositionY -= MOVE_SPEED / s_debug_camera.Scale;
                if ( keyboard_state.IsKeyDown(Keys.Left)        ) s_debug_camera.PositionX -= MOVE_SPEED / s_debug_camera.Scale;
                if ( keyboard_state.IsKeyDown(Keys.Right)       ) s_debug_camera.PositionX += MOVE_SPEED / s_debug_camera.Scale;
                if ( keyboard_state.IsKeyDown(Keys.PageUp)      ) s_debug_camera.Scale     += ZOOM_SPEED;
                if ( keyboard_state.IsKeyDown(Keys.PageDown)    ) s_debug_camera.Scale     -= ZOOM_SPEED;

                // Get joypad state:

                GamePadState gamepad_state = GamePad.GetState(PlayerIndex.One);

                if ( gamepad_state.IsConnected )
                {
                    s_debug_camera.PositionX += gamepad_state.ThumbSticks.Left.X * MOVE_SPEED / s_debug_camera.Scale;
                    s_debug_camera.PositionY += gamepad_state.ThumbSticks.Left.Y * MOVE_SPEED / s_debug_camera.Scale;

                    s_debug_camera.Scale += gamepad_state.Triggers.Left  * ZOOM_SPEED;
                    s_debug_camera.Scale -= gamepad_state.Triggers.Right * ZOOM_SPEED;
                }

                // Maintain a non crazy camera scaling:

                if ( s_debug_camera.Scale < 0.01f   ) s_debug_camera.Scale = 0.01f;
                if ( s_debug_camera.Scale > 100.0f  ) s_debug_camera.Scale = 100.0f;
            }

        }

        //=========================================================================================
        /// <summary>
        /// Does overview debug drawing. Draws a crosshair at camera position etc..
        /// </summary>
        //=========================================================================================

        public static void Draw()
        {
            // Draw the center of the camera as a crosshair: but only if debug drawing is on

            if ( s_use_debug_camera )
            {
                // Draw crosshair at center of screen:

                DebugDrawing.DrawGuiRectangle( Core.Gui.Camera.Position , new Vector2(8,0) , Color.White );
                DebugDrawing.DrawGuiRectangle( Core.Gui.Camera.Position , new Vector2(0,8) , Color.White );

                // Get the viewing and projection transforms of the camera:

                Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

                // Draw the coordinates of the camera position:

                Core.DebugFont.DrawString
                (
                    "X:" +  s_debug_camera.PositionX.ToString(Locale.DevelopmentCulture.NumberFormat),                    
                    new Vector2(0,-16)
                );

                Core.DebugFont.DrawString
                (
                    "Y:" + s_debug_camera.PositionY.ToString(Locale.DevelopmentCulture.NumberFormat),                    
                    new Vector2(0,-32)
                );
            }

        }

    }   // end of class

}   // end of namespace

#endif  // if DBEUG