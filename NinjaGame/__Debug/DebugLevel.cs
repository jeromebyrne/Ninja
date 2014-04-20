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
    /// Contains console commands for level editing and debugging the level. The console functions 
    /// are only defined on windows however, and the class itself is only defined in debug mode 
    /// on all platforms.
    /// 
    /// </summary>
    //
    //#############################################################################################

    public class DebugLevel
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Debug only variable. Draws debug information for objects if this is set to true.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public static bool ShowDebugInfo { get { return s_show_debug_info; } }

            /// <summary> A debug camera that can be moved around freely. The renderer should use this when specified. </summary>
        
            public static Camera DebugCamera { get { return s_debug_camera; } }

            /// <summary> Set to true when the renderer should use the debug camera here </summary>
        
            public static bool UseDebugCamera { get { return s_use_debug_camera; } }

            /// <summary> Allow the user to step through frames ? </summary>

            public static bool FrameStep { get { return m_frame_step; } }

            /// <summary> Number of frames to step through before stopping. </summary>

            public static int FrameStepCount { get { return m_step_frame_count; } set { m_step_frame_count = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> The renderer should draw debug information for objects if this is set to true. </summary>

            private static bool s_show_debug_info = false;

            /// <summary> A debug camera that can be moved around freely </summary>
        
            private static Camera s_debug_camera = null;

            /// <summary> A debug camera that can be moved around freely. The renderer should use this when specified. </summary>

            private static bool s_use_debug_camera = false;

            /// <summary> Allow the user to step through frames ? </summary>

            private static bool m_frame_step = false;

            /// <summary> Number of frames to step through before stopping. </summary>

            private static int m_step_frame_count = 0;

            /// <summary> Last state of the keyboard when Update() was called. </summary>

            private static KeyboardState m_last_keyboard_state = new KeyboardState();

        //=========================================================================================
        /// <summary>
        /// Adds all debug commands to do with the level to the debug console.
        /// </summary>
        //=========================================================================================

        public static void Initialize()
        {
            // Add console commands on windows

            #if WINDOWS_DEBUG

                DebugConsole.AddCommand( "LInfo"        , new DebugConsole.Function_bool_bool(Con_LInfo)        );
                DebugConsole.AddCommand( "LEdit"        , new DebugConsole.Function_void_int(Con_LEdit)         );
                DebugConsole.AddCommand( "LSave"        , new DebugConsole.Function_void_string(Con_LSave)      );
                DebugConsole.AddCommand( "LLoad"        , new DebugConsole.Function_void_string(Con_LLoad)      );
                DebugConsole.AddCommand( "LNew"         , new DebugConsole.Function_void_string(Con_LNew)       );
                DebugConsole.AddCommand( "LDelete"      , new DebugConsole.Function_void_int(Con_LDelete)       );
                DebugConsole.AddCommand( "LClone"       , new DebugConsole.Function_void_int(Con_LClone)        );
                DebugConsole.AddCommand( "LMoveCamera"  , new DebugConsole.Function_bool_bool(Con_LMoveCamera)  );
                DebugConsole.AddCommand( "LFrameStep"   , new DebugConsole.Function_bool_bool(Con_LFrameStep)   );

            #endif

            // Make the debug camera:

            s_debug_camera = new Camera();
        }

        //=========================================================================================
        /// <summary>
        /// Debug console command. Turns on/off using the debug camera or free camera.
        /// </summary>
        /// <param name="arg">          Argument given from the console by user.    </param>
        /// <param name="arg_given">    If an argument was actually given.          </param>
        /// <returns>                   Value of this flag.                         </returns>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static bool Con_LMoveCamera( bool arg , bool arg_given )
            {
                if ( arg_given ) s_use_debug_camera = arg; return s_use_debug_camera;
            }

        #endif


        //=========================================================================================
        /// <summary>
        /// Debug console command. Loads the given level file.
        /// </summary>
        /// <param name="arg"> Argument given from the console by user. Level to load. </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_LLoad( string arg )
            {
                // Abort if no name given:

                if ( arg.Length == 0 ) DebugConsole.Print("\nLLoad: Must give level xml file to load from.");

                // Load the given file

                Core.Level.Load(arg);
            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Save the given level file.
        /// </summary>
        /// <param name="arg"> Argument given from the console by user. Level to save to. </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_LSave( string arg )
            {
                // Abort if no name given:

                if ( arg.Length == 0 ) DebugConsole.Print("\nLSave: Must give level xml file to save to.");

                // Load the given file

                Core.Level.Save(arg);
            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Switches on / off drawing debug information for the level.
        /// </summary>
        /// <param name="arg">          Argument given from the console by user.    </param>
        /// <param name="arg_given">    If an argument was actually given.          </param>
        /// <returns>                   Value of this flag.                         </returns>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static bool Con_LInfo ( bool arg , bool arg_given )
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

            public static void Con_LNew( string type )
            {
                // Abort if no name given:

                if ( type.Length == 0 ) DebugConsole.Print("\nLNew: Must give a valid type of game object to create.");

                // Try and make the given object:

                try
                {
                    // Make it:

                    XmlObject xml_obj = XmlFactory.CreateObject(type);

                    // See if that failed:

                    if ( xml_obj == null )
                    {
                        DebugConsole.Print("\nLNew: Not a valid GameObject sub-type."); return;
                    }

                    // Attempt to cast to a game object:

                    GameObject game_obj = (GameObject)(xml_obj);

                    // See if that failed:

                    if ( game_obj == null )
                    {
                        DebugConsole.Print("\nLNew: Not a valid GameObject sub-type."); return;
                    }

                    // Add it to the level:

                    Core.Level.Data.Add(game_obj);

                    // Make a blank xml document:

                    XmlDocument doc = new XmlDocument(); doc.AppendChild( doc.CreateElement("data") );

                    // Make an xml object data object:

                    XmlObjectData data = new XmlObjectData(doc.FirstChild);

                    // Do a fake read and allow the object to initialise some default values:

                    game_obj.ReadXml(data);

                    // Set the object's position to the debug camera position:

                    game_obj.Position = DebugCamera.Position;

                    // Open up the editor on it:

                    try
                    {
                        XmlObjectEditor.EditObject(game_obj,"Editing game object with ID '" + game_obj.Id + "'");
                    }
                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                }
                catch ( Exception e ){ DebugConsole.PrintException(e); }

            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Attempts to delete the object with the given id.
        /// </summary>
        /// <param name="object_id"> Id of the object to delete </param>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void Con_LDelete( int object_id )
            {
                // See if the object exists in the level:

                GameObject clone_obj = Core.Level.Search.FindById(object_id);

                if ( clone_obj == null )
                {
                    DebugConsole.Print("\nLDelete: given object id does not exist in level ! "); return;
                }

                // Remove the object from the level:

                Core.Level.Data.Remove( object_id );
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

            public static void Con_LClone( int object_id  )
            {
                // See if the object exists in the level:

                GameObject clone_obj = Core.Level.Search.FindById(object_id);

                if ( clone_obj == null )
                {
                    DebugConsole.Print("\nLClone: given object id does not exist in level ! "); return;
                }

                // Try and make a new copy of the object we found:

                try
                {
                    // Make it:

                    XmlObject xml_obj = XmlFactory.CreateObject( clone_obj.GetType().Name );

                    // Attempt to cast to a game object:

                    GameObject game_obj = (GameObject)(xml_obj);

                    // Add it to the level:

                    Core.Level.Data.Add(game_obj);

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
                        game_obj.ReadXml(xml_data);
                    }
                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                    // Open up the editor on it:

                    try
                    {
                        XmlObjectEditor.EditObject(game_obj,"Editing game object with ID '" + game_obj.Id + "'");
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

            public static void Con_LEdit( int object_id )
            {
                // See if the object exists in the level:

                GameObject obj = Core.Level.Search.FindById(object_id);

                if ( obj != null )
                {
                    // Edit the object:

                    XmlObjectEditor.EditObject(obj,"Editing game object with ID '" + obj.Id + "' of type: \n" + obj.GetType().Name );
                }
                else
                {
                    // Print an error: object does not exist

                    DebugConsole.Print("\nLEdit: given object id does not exist in level!");
                }

            }

        #endif

        //=========================================================================================
        /// <summary>
        /// Debug console command. Turns on/off stepping through frames. 
        /// </summary>
        /// <param name="arg"> Argument </param>
        /// <param name="arg_given"> Whether the argument was given or not. </param>
        /// <returns> Current value of this variable. </returns>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static bool Con_LFrameStep ( bool arg , bool arg_given )
            {
                // Set variable if given:

                if ( arg_given ) m_frame_step = arg;

                // Return value:

                return m_frame_step;
            }

        #endif  // #if WINDOWS_DEBUG

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

            // Get keyboard state:

            KeyboardState kbs = Keyboard.GetState();

            // If we are using the debug camera then move it if the right keys are pressed:

            if ( s_use_debug_camera )
            {
                // See what keys are pressed:

                if ( kbs.IsKeyDown(Keys.Up)          ) s_debug_camera.PositionY += MOVE_SPEED / s_debug_camera.Scale;
                if ( kbs.IsKeyDown(Keys.Down)        ) s_debug_camera.PositionY -= MOVE_SPEED / s_debug_camera.Scale;
                if ( kbs.IsKeyDown(Keys.Left)        ) s_debug_camera.PositionX -= MOVE_SPEED / s_debug_camera.Scale;
                if ( kbs.IsKeyDown(Keys.Right)       ) s_debug_camera.PositionX += MOVE_SPEED / s_debug_camera.Scale;
                if ( kbs.IsKeyDown(Keys.PageUp)      ) s_debug_camera.Scale     += ZOOM_SPEED;
                if ( kbs.IsKeyDown(Keys.PageDown)    ) s_debug_camera.Scale     -= ZOOM_SPEED;

                // Get joypad state:

                GamePadState gps = GamePad.GetState(PlayerIndex.One);

                if ( gps.IsConnected )
                {
                    s_debug_camera.PositionX += gps.ThumbSticks.Left.X * MOVE_SPEED / s_debug_camera.Scale;
                    s_debug_camera.PositionY += gps.ThumbSticks.Left.Y * MOVE_SPEED / s_debug_camera.Scale;

                    s_debug_camera.Scale += gps.Triggers.Left  * ZOOM_SPEED;
                    s_debug_camera.Scale -= gps.Triggers.Right * ZOOM_SPEED;
                }

                // Maintain a non crazy camera scaling:

                if ( s_debug_camera.Scale < 0.01f   ) s_debug_camera.Scale = 0.01f;
                if ( s_debug_camera.Scale > 100.0f  ) s_debug_camera.Scale = 100.0f;
            }

            // This key turns on and off step framing:

            if ( m_last_keyboard_state.IsKeyDown(Keys.D0) && kbs.IsKeyUp(Keys.D0) ) 
            {
                // On windows debug only do if the console is not active:

                #if WINDOWS_DEBUG

                    if ( DebugConsole.HasFocus() == false ) m_frame_step = ! m_frame_step;

                #else

                    m_frame_step = ! m_frame_step;

                #endif                
            }

            // See if step frame is on: on windows debug only do if the console is not active

            #if WINDOWS_DEBUG

                if ( m_frame_step && DebugConsole.HasFocus() == false )

            #else

                if ( m_frame_step )

            #endif
            
            {
                // See if any step frame keys were release:

                if ( m_last_keyboard_state.IsKeyDown(Keys.D1) && kbs.IsKeyUp(Keys.D1) ) m_step_frame_count += 1;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D2) && kbs.IsKeyUp(Keys.D2) ) m_step_frame_count += 2;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D3) && kbs.IsKeyUp(Keys.D3) ) m_step_frame_count += 3;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D4) && kbs.IsKeyUp(Keys.D4) ) m_step_frame_count += 4;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D5) && kbs.IsKeyUp(Keys.D5) ) m_step_frame_count += 5;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D6) && kbs.IsKeyUp(Keys.D6) ) m_step_frame_count += 6;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D7) && kbs.IsKeyUp(Keys.D7) ) m_step_frame_count += 7;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D8) && kbs.IsKeyUp(Keys.D8) ) m_step_frame_count += 8;
                if ( m_last_keyboard_state.IsKeyDown(Keys.D9) && kbs.IsKeyUp(Keys.D9) ) m_step_frame_count += 9;
            }
            else
            {
                m_step_frame_count = 0;
            }

            // Save current keyboard state

            m_last_keyboard_state = kbs;
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
                // Set the vertex declaraiton

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorDeclaration;

                // Create an array of lines:

                VertexPositionColor[] vertices = new VertexPositionColor[4];

                // Fill the array:

                vertices[0].Position.X = Core.Level.Renderer.Camera.Position.X - 8 / Core.Level.Renderer.Camera.Scale;
                vertices[0].Position.Y = Core.Level.Renderer.Camera.Position.Y;
                vertices[1].Position.X = Core.Level.Renderer.Camera.Position.X + 8 / Core.Level.Renderer.Camera.Scale;
                vertices[1].Position.Y = Core.Level.Renderer.Camera.Position.Y;

                vertices[2].Position.X = Core.Level.Renderer.Camera.Position.X;
                vertices[2].Position.Y = Core.Level.Renderer.Camera.Position.Y + 8 / Core.Level.Renderer.Camera.Scale;
                vertices[3].Position.X = Core.Level.Renderer.Camera.Position.X;
                vertices[3].Position.Y = Core.Level.Renderer.Camera.Position.Y - 8 / Core.Level.Renderer.Camera.Scale;

                vertices[0].Color = Color.White;
                vertices[1].Color = Color.White;
                vertices[2].Color = Color.White;
                vertices[3].Color = Color.White;

                // Get the viewing and projection transforms of the camera:

                Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

                // Set the transform on the debug shader:

                EffectParameter param = Core.DebugShader.Parameters["WorldViewProjection"];

                if ( param != null )
                {
                    param.SetValue(view_projection);
                }

                // Draw the lines:

                Core.DebugShader.Begin(); Core.DebugShader.CurrentTechnique.Passes[0].Begin();

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList,vertices,0,2);  

                Core.DebugShader.CurrentTechnique.Passes[0].End(); Core.DebugShader.End();

                // Draw the coordinates of the camera position:

                Core.DebugFont.DrawString
                (
                    "X:" + s_debug_camera.PositionX.ToString(Locale.DevelopmentCulture.NumberFormat),                    
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