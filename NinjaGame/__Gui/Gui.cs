using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class representing a GUI. A gui cotains a collection of gui elements, such as buttons 
    /// and pictures that make up the interface. This class contains and manages all of those 
    /// elements. It is very much the GUI equivalent of the Level class.
    /// </summary>
    //#############################################################################################

    public class Gui
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Data for the gui. Contains all of the objects in the gui. </summary>

            public GuiData Data { get { return m_data; } } 

            /// <summary> Search query object used to search the gui data for widgets, according to different criteria. </summary>
            
            public GuiSearchQuery Search { get { return m_search; } } 

            /// <summary> Widget that is currently in focus, if any. </summary>

            public GuiWidget FocusWidget { get { return m_focus_widget; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Camera object used to render the gui from. If no camera is given the gui will 
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

            /// <summary> Set to true if the gui is currently loading from a file, or is dealing with newly loaded widgets through OnGuiLoaded() </summary>

            public bool Loading { get { return m_loading; } }

            /// <summary> Name of the gui file currently loaded. </summary>

            public string FileName { get { return m_file_name; } }

        //=========================================================================================
        // Enums 
        //========================================================================================= 

            /// <summary> Represents the direction a focus traversal moves in. </summary>

            public enum FocusTraversalDirection
            {
                /// <summary> Move focus to the left. </summary>

                LEFT    ,

                /// <summary> Move focus to the right. </summary>

                RIGHT    ,

                /// <summary> Move focus above. </summary>

                ABOVE    ,

                /// <summary> Move focus below. </summary>

                BELOW
            };

        //=========================================================================================
        // Variables 
        //=========================================================================================        

            /// <summary> Data for the gui. Contains all of the objects in the gui. </summary>

            private GuiData m_data = null;

            /// <summary> Search query object used to search the gui data for widgets, according to different criteria. </summary>

            private GuiSearchQuery m_search = null;

            /// <summary> A heap used for depth sorting widgets when rendering. </summary>

            private Heap<GuiWidget> m_render_heap = new Heap<GuiWidget>(16,null);

            /// <summary> Widget that is currently in focus, if any. </summary>

            private GuiWidget m_focus_widget = null;

            /// <summary> Previous state of the keyboard. </summary>

            private KeyboardState m_previous_keyboard_state;

            /// <summary> Previous state of the game pad. </summary>

            private GamePadState m_previous_gamepad_state;

            /// <summary> Set to true if the gui is currently loading from a file, or is dealing with newly loaded widgets through OnGuiLoaded() </summary>

            private bool m_loading = false;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Camera object used to render the gui from. If no camera is given the gui will 
            /// create it's own to get by with.. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Camera m_camera = null;

            /// <summary> Name of the gui file currently loaded. </summary>

            public string m_file_name = "";

            /// <summary> A list of all the previous screens we have visited. </summary>

            private LinkedList<string> m_previous_screens = new LinkedList<string>();

        //=========================================================================================
        /// <summary> 
        /// Constructor for the gui. Initialises the basic gui data structures and objects.
        /// </summary>
        //=========================================================================================

        public Gui()
        {
            // Create all required objects:

            m_data = new GuiData(this); m_search = new GuiSearchQuery(this);

            // Get previous keyboard and gamepad state:

            m_previous_gamepad_state    = GamePad.GetState(PlayerIndex.One);
            m_previous_keyboard_state   = Keyboard.GetState();

            // If debug then initialise debug gui stuff:

            #if DEBUG

                DebugGui.Initialize();

            #endif
        }

        //=========================================================================================
        /// <summary> 
        /// Clears the gui. This should be used as opposed to manually clearing gui data 
        /// when a level-wipe is required.
        /// </summary>
        //=========================================================================================

        public void Clear()
        {
            // Recreate all objects and override the old ones:

            m_data = new GuiData(this); m_search = new GuiSearchQuery(this);

            // Clear the curretnly focused widget:

            m_focus_widget = null;
        }

        //=========================================================================================
        /// <summary> 
        /// Loads the previously loaded gui, if any. This may be used multiple times if many 
        /// screens were entered.
        /// </summary>
        //=========================================================================================

        public void PopScreen()
        {
            // See if there is any previous guis:

            if ( m_previous_screens.Count > 0 )
            {
                // Good: load the first

                Load( m_previous_screens.Last.Value );

                // Pop both the last two entries off the list:

                m_previous_screens.RemoveLast();
                m_previous_screens.RemoveLast();
            }
        }
      
        //=========================================================================================
        /// <summary> 
        /// Loads a gui and all objects in the gui from the given XML file. If this process 
        /// fails then the gui will remain as it is. 
        /// </summary>
        /// 
        /// <param name="file"> Name of the file to load </param>
        //=========================================================================================

        public void Load( String file )
        {
            // Do nothing if no file is given:

            if ( file == null || file.Length <= 0 ) return;

            // Set loading flag to true:

            m_loading = true;

            // Try and read the given level:

            LinkedList<XmlObject> objects = XmlFactory.ReadXml( Locale.GetLocFile(file) );

            // See if that succeeded:

            if ( objects != null )
            {
                // Success: clear the gui

                Clear();

                // Add this file to the list of previous guis

                m_previous_screens.AddLast(m_file_name);

                // Set new gui file name:

                m_file_name = file;

                {
                    // Run through the list of objects read:

                    LinkedList<XmlObject>.Enumerator i = objects.GetEnumerator();

                    while ( i.MoveNext() )
                    {
                        // Get this object:

                        XmlObject obj = i.Current;

                        // Attempt to cast to a game object:

                        try
                        {
                            // Do the cast:

                            GuiWidget widget_obj = (GuiWidget)(obj);

                            // Save the object to the level:

                            m_data.Add(widget_obj);
                        }

                        // If something went wrong then show what on debug windows:

                        #if WINDOWS_DEBUG

                            catch ( Exception e )
                            {
                                // Print what happened: 

                                DebugConsole.Print("Gui.Load(): invalid object type encountered. " ); DebugConsole.PrintException(e);
                            }

                        #else

                            catch ( Exception ){;}

                        #endif
                        
                    }

                }

                // Lock the level gui:

                m_data.Lock();

                // Call the OnGuiLoaded() event on all objects:

                {
                    // Get all the objects:

                    Dictionary<int,GuiWidget>.Enumerator i = m_data.Objects.GetEnumerator();

                    // Run through this list:

                    while ( i.MoveNext() )
                    {
                        // Attempt to call OnGuiLoaded:

                        try
                        {
                            // Do the call:

                            i.Current.Value.OnGuiLoaded();
                        }

                        // If something went wrong then show what on debug windows:

                        #if WINDOWS_DEBUG

                            catch ( Exception e )
                            {
                                // Print what happened: 

                                DebugConsole.Print("Gui.Load(): OnGuiLoaded() failed for object. " ); DebugConsole.PrintException(e);
                            }

                        #else

                            catch ( Exception ){;}

                        #endif
                    }

                }

                // Unlock the gui data:

                m_data.Unlock();
            }

            // Choose a focus widget if possible:

            ChooseFocusWidget();

            // Set loading flag to false:

            m_loading = false;
        }

        //=========================================================================================
        /// <summary> 
        /// Saves the gui and it's objects to the given XML file.
        /// </summary>
        /// 
        /// <param name="file"> Name of the file to save to </param>
        //=========================================================================================

        public void Save( String file )
        {
            // Make a list of objects to save:

            LinkedList<XmlObject> objects = new LinkedList<XmlObject>();

            // Lock the gui data:

            m_data.Lock();

                // Put all the objects in the gui into the list:

                Dictionary<int,GuiWidget>.Enumerator e = m_data.Objects.GetEnumerator();

                while ( e.MoveNext() )
                {
                    objects.AddLast(e.Current.Value);
                }

            // Unlock the level data:

            m_data.Unlock();

            // Let the XmlFactory do the rest:

            XmlFactory.WriteXml(file,objects);
        }     

        //=========================================================================================
        /// <summary> 
        /// Updates the gui, calling the Update() function for all gui objects.
        /// </summary>
        //========================================================================================= 

        public void Update()
        {
            // If there is no focus widget then try and pick one:

            if ( m_focus_widget == null ) 
            {
                ChooseFocusWidget();
            }
            else if ( m_focus_widget.ParentGui == null )
            {
                ChooseFocusWidget();
            }

            // Do keyboard and gamepad input:

            DoKeyboardInput();
            DoGamepadInput();

            // Prevent modification of the gui data collections:

            m_data.Lock();

                // Run through the list of objects and update them all:

                Dictionary<int,GuiWidget>.Enumerator e = m_data.Objects.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Update this object:

                    e.Current.Value.OnUpdate();
                }

            // If in debug build then update debug gui stuff

            #if DEBUG

                DebugGui.Update();

            #endif

            // Ok to modify gui data again: apply all with-held changes:

            m_data.Unlock();
        }

        //=========================================================================================
        /// <summary> 
        /// Does gamepad input for the GUI to do with focus traversal.
        /// </summary>
        //========================================================================================= 

        private void DoGamepadInput()
        {
            // Get the current keyboard state:

            GamePadState gps = GamePad.GetState(PlayerIndex.One);

            // See if any of the traversal keys have been pressed:
            
            if ( m_previous_gamepad_state.IsButtonUp(Buttons.DPadUp)    && gps.IsButtonDown(Buttons.DPadUp)   ) TraverseFocus(FocusTraversalDirection.ABOVE   );
            if ( m_previous_gamepad_state.IsButtonUp(Buttons.DPadDown)  && gps.IsButtonDown(Buttons.DPadDown) ) TraverseFocus(FocusTraversalDirection.BELOW   );
            if ( m_previous_gamepad_state.IsButtonUp(Buttons.DPadLeft)  && gps.IsButtonDown(Buttons.DPadLeft) ) TraverseFocus(FocusTraversalDirection.LEFT    );
            if ( m_previous_gamepad_state.IsButtonUp(Buttons.DPadRight) && gps.IsButtonDown(Buttons.DPadRight)) TraverseFocus(FocusTraversalDirection.RIGHT   );

            // Do traversal also with the left analog stick:

            if ( m_previous_gamepad_state.ThumbSticks.Left.X > - 0.35f && gps.ThumbSticks.Left.X <= - 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.LEFT );
            }

            if ( m_previous_gamepad_state.ThumbSticks.Left.X < 0.35f && gps.ThumbSticks.Left.X >= 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.RIGHT );
            }

            if ( m_previous_gamepad_state.ThumbSticks.Left.Y > - 0.35f && gps.ThumbSticks.Left.Y <= - 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.BELOW );
            }

            if ( m_previous_gamepad_state.ThumbSticks.Left.Y < 0.35f && gps.ThumbSticks.Left.Y >= 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.ABOVE );
            }

            // Don't forget the right stick for those who prefer using it:

            if ( m_previous_gamepad_state.ThumbSticks.Right.X > - 0.35f && gps.ThumbSticks.Right.X <= - 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.LEFT );
            }

            if ( m_previous_gamepad_state.ThumbSticks.Right.X < 0.35f && gps.ThumbSticks.Right.X >= 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.RIGHT );
            }

            if ( m_previous_gamepad_state.ThumbSticks.Right.Y > - 0.35f && gps.ThumbSticks.Right.Y <= - 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.BELOW );
            }

            if ( m_previous_gamepad_state.ThumbSticks.Right.Y < 0.35f && gps.ThumbSticks.Right.Y >= 0.35f )
            {
                TraverseFocus( FocusTraversalDirection.ABOVE );
            }

            // Save the widget that's currently in focus: widgets may change this below

            GuiWidget focus_widget = m_focus_widget;

            // Dollop out gamepad events if there is a focus widget:

            if ( focus_widget != null )
            {
                // Pressed events:

                    if ( m_previous_gamepad_state.Buttons.A == ButtonState.Released && gps.Buttons.A == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.A);
                    }

                    if ( m_previous_gamepad_state.Buttons.B == ButtonState.Released && gps.Buttons.B == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.B);
                    }

                    if ( m_previous_gamepad_state.Buttons.X == ButtonState.Released && gps.Buttons.X == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.X);
                    }

                    if ( m_previous_gamepad_state.Buttons.Y == ButtonState.Released && gps.Buttons.Y == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.Y);
                    }

                    if ( m_previous_gamepad_state.Buttons.Start == ButtonState.Released && gps.Buttons.Start == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.Start);
                    }

                    if ( m_previous_gamepad_state.Buttons.Back == ButtonState.Released && gps.Buttons.Back == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.Back);
                    }

                    if ( m_previous_gamepad_state.Buttons.LeftShoulder == ButtonState.Released && gps.Buttons.LeftShoulder == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.LeftShoulder);
                    }

                    if ( m_previous_gamepad_state.Buttons.RightShoulder == ButtonState.Released && gps.Buttons.RightShoulder == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.RightShoulder);
                    }

                    if ( m_previous_gamepad_state.Buttons.LeftStick == ButtonState.Released && gps.Buttons.LeftStick == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.LeftStick);
                    }

                    if ( m_previous_gamepad_state.Buttons.RightStick == ButtonState.Released && gps.Buttons.RightStick == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.RightStick);
                    }

                    if ( m_previous_gamepad_state.DPad.Down == ButtonState.Released && gps.DPad.Down == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.DPadDown);
                    }

                    if ( m_previous_gamepad_state.DPad.Up == ButtonState.Released && gps.DPad.Up == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.DPadUp);
                    }

                    if ( m_previous_gamepad_state.DPad.Left == ButtonState.Released && gps.DPad.Left == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.DPadLeft);
                    }

                    if ( m_previous_gamepad_state.DPad.Right == ButtonState.Released && gps.DPad.Right == ButtonState.Pressed )
                    {
                        focus_widget.OnGamepadPressed(Buttons.DPadRight);
                    }

                // Released events:

                    if ( m_previous_gamepad_state.Buttons.A == ButtonState.Pressed && gps.Buttons.A == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.A);
                    }

                    if ( m_previous_gamepad_state.Buttons.B == ButtonState.Pressed && gps.Buttons.B == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.B);
                    }

                    if ( m_previous_gamepad_state.Buttons.X == ButtonState.Pressed && gps.Buttons.X == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.X);
                    }

                    if ( m_previous_gamepad_state.Buttons.Y == ButtonState.Pressed && gps.Buttons.Y == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.Y);
                    }

                    if ( m_previous_gamepad_state.Buttons.Start == ButtonState.Pressed && gps.Buttons.Start == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.Start);
                    }

                    if ( m_previous_gamepad_state.Buttons.Back == ButtonState.Pressed && gps.Buttons.Back == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.Back);
                    }

                    if ( m_previous_gamepad_state.Buttons.LeftShoulder == ButtonState.Pressed && gps.Buttons.LeftShoulder == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.LeftShoulder);
                    }

                    if ( m_previous_gamepad_state.Buttons.RightShoulder == ButtonState.Pressed && gps.Buttons.RightShoulder == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.RightShoulder);
                    }

                    if ( m_previous_gamepad_state.Buttons.LeftStick == ButtonState.Pressed && gps.Buttons.LeftStick == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.LeftStick);
                    }

                    if ( m_previous_gamepad_state.Buttons.RightStick == ButtonState.Pressed && gps.Buttons.RightStick == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.RightStick);
                    }

                    if ( m_previous_gamepad_state.DPad.Down == ButtonState.Pressed && gps.DPad.Down == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.DPadDown);
                    }

                    if ( m_previous_gamepad_state.DPad.Up == ButtonState.Pressed && gps.DPad.Up == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.DPadUp);
                    }

                    if ( m_previous_gamepad_state.DPad.Left == ButtonState.Pressed && gps.DPad.Left == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.DPadLeft);
                    }

                    if ( m_previous_gamepad_state.DPad.Right == ButtonState.Pressed && gps.DPad.Right == ButtonState.Released )
                    {
                        focus_widget.OnGamepadReleased(Buttons.DPadRight);
                    }

            }   // end if focus widget

            // Save the current keyboard state as the previous:

            m_previous_gamepad_state = gps;
        }

        //=========================================================================================
        /// <summary> 
        /// Does keyboard input for the GUI to do with focus traversal.
        /// </summary>
        //========================================================================================= 

        private void DoKeyboardInput()
        {
            // Get the current keyboard state:

            KeyboardState kbs = Keyboard.GetState();

            // See if any of the traversal keys have been pressed:

            if ( m_previous_keyboard_state.IsKeyUp(Keys.Up)     && kbs.IsKeyDown(Keys.Up)   ) TraverseFocus(FocusTraversalDirection.ABOVE   );
            if ( m_previous_keyboard_state.IsKeyUp(Keys.Down)   && kbs.IsKeyDown(Keys.Down) ) TraverseFocus(FocusTraversalDirection.BELOW   );
            if ( m_previous_keyboard_state.IsKeyUp(Keys.Left)   && kbs.IsKeyDown(Keys.Left) ) TraverseFocus(FocusTraversalDirection.LEFT    );
            if ( m_previous_keyboard_state.IsKeyUp(Keys.Right)  && kbs.IsKeyDown(Keys.Right)) TraverseFocus(FocusTraversalDirection.RIGHT   );

            if ( m_previous_keyboard_state.IsKeyUp(Keys.W) && kbs.IsKeyDown(Keys.W) ) TraverseFocus(FocusTraversalDirection.ABOVE   );
            if ( m_previous_keyboard_state.IsKeyUp(Keys.S) && kbs.IsKeyDown(Keys.S) ) TraverseFocus(FocusTraversalDirection.BELOW   );
            if ( m_previous_keyboard_state.IsKeyUp(Keys.A) && kbs.IsKeyDown(Keys.A) ) TraverseFocus(FocusTraversalDirection.LEFT    );
            if ( m_previous_keyboard_state.IsKeyUp(Keys.D) && kbs.IsKeyDown(Keys.D) ) TraverseFocus(FocusTraversalDirection.RIGHT   );

            // Save the widget that's currently in focus: widgets may change this below

            GuiWidget focus_widget = m_focus_widget;

            // Dollop out pressed and released events to the current focus widget if it is there:

            if ( focus_widget != null )
            {
                // Get the previously pressed keys:

                Keys[] p_pressed_keys = m_previous_keyboard_state.GetPressedKeys();

                // Get the currently pressed keys:

                Keys[] c_pressed_keys = kbs.GetPressedKeys();

                // See what has just been pressed:

                for ( int i = 0 ; i < c_pressed_keys.Length ; i++ )
                {
                    // Try and find this key in the array of previously pressed keys:

                    bool just_pressed = true;

                        for ( int j = 0 ; j < p_pressed_keys.Length ; j++ )
                        {
                            if ( c_pressed_keys[i] == p_pressed_keys[j] )
                            {
                                // Key has not just been pressed:

                                just_pressed = false; break;
                            }
                        }

                    // See if key has just been pressed:

                    if ( just_pressed ) focus_widget.OnKeyboardPressed(c_pressed_keys[i]);
                }

                // See what has just been released:

                for ( int i = 0 ; i < p_pressed_keys.Length ; i++ )
                {
                    // Try and find this key in the array of currently pressed keys:

                    bool just_released = true;

                        for ( int j = 0 ; j < c_pressed_keys.Length ; j++ )
                        {
                            if ( p_pressed_keys[i] == c_pressed_keys[j] )
                            {
                                // Key has not just been released:

                                just_released = false; break;
                            }
                        }

                    // See if key has just been released:

                    if ( just_released ) focus_widget.OnKeyboardReleased( p_pressed_keys[i] );
                }
            }

            // Save the current keyboard state as the previous:

            m_previous_keyboard_state = kbs;
        }

        //=========================================================================================
        /// <summary> 
        /// Renders the gui and all it's widgets.
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

                if ( DebugGui.UseDebugCamera ) m_camera = DebugGui.DebugCamera;

            #endif

            // Lock the lists of widgets before we do this:

            m_data.Lock();

                // Run through the list of widgets and add them all into the render heap:

                Dictionary<int,GuiWidget>.Enumerator e = m_data.Objects.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Add into the render heap:

                    m_render_heap.Add( e.Current.Value.Depth , e.Current.Value );
                }

            // Unlock the lists of widgets:

            m_data.Unlock();

            // Render objects in the render heap, starting with the deepest first:

            while ( m_render_heap.Count > 0 )
            {
                // Get this object:

                GuiWidget obj = m_render_heap.Remove(); 

                // Draw it:

                obj.OnDraw();
            }

            // If in debug mode then do a debug draw if enabled:

            #if DEBUG

                // If debug information is enabled, then show it:

                if ( DebugGui.ShowDebugInfo ) DebugDraw();

                // Do any debug drawing the gui debug module wants to do in debug mode:

                DebugGui.Draw();

                // See if we are to use the debug camera: if so then restore the normal camera

                if ( DebugGui.UseDebugCamera ) m_camera = previous_camera;

            #endif
        }

        //=========================================================================================
        /// <summary> 
        /// Chooses the current widget that is to be in focus. Picks the first widget that has the 
        /// 'DefaultFocus' flag set to true, or if that fails the first focusable widget.
        /// </summary>
        //=========================================================================================

        private void ChooseFocusWidget()
        {
            // Lock the objects list:

            m_data.Lock();

                // Clear the current focus widget:

                m_focus_widget = null;

                // Run through the list of widgets and try to find a default focus one:

                Dictionary<int,GuiWidget>.Enumerator e = m_data.Objects.GetEnumerator();

                    while ( e.MoveNext() )
                    {
                        // See if this is a default focus widget:

                        if ( e.Current.Value.DefaultFocus )
                        {
                            // Found our focus widget: set

                            m_focus_widget = e.Current.Value; 
                            
                            // Call the gained focus event:

                            m_focus_widget.OnFocusGained();

                            // Abort search
                            
                            m_data.Unlock(); return;
                        }
                    }

                // Ok: run through the list of widgets that can have focus and try to pick one

                e = m_data.Objects.GetEnumerator();

                    while ( e.MoveNext() )
                    {
                        // See if this is a default focus widget:

                        if ( e.Current.Value.CanFocus )
                        {
                            // Found our focus widget: set

                            m_focus_widget = e.Current.Value; 
                            
                            // Call the gained focus event:

                            m_focus_widget.OnFocusGained();

                            // Abort search
                            
                            m_data.Unlock(); return;
                        }
                    }

            // Unlock the objects list:

            m_data.Unlock();
        }

        //=========================================================================================
        /// <summary> 
        /// Makes focus shift in the direction specified if allowed.
        /// </summary>
        //=========================================================================================

        public void TraverseFocus( FocusTraversalDirection direction )
        {
            // See if there is a focus widget: we can do nowt if there is none

            if ( m_focus_widget != null )
            {
                // Ok: see what direction we have to move in

                switch ( direction )
                {
                    // Move focus to the left:

                    case FocusTraversalDirection.LEFT:
                    {
                        // Try to find the next widget to focus on:

                        GuiWidget next_focus_widget = m_search.FindByName( m_focus_widget.LeftWidget );

                        // If found then switch focus:

                        if ( next_focus_widget != null )
                        {
                            // Switch focus:

                            m_focus_widget.OnFocusLost();  m_focus_widget = next_focus_widget;

                            // Call focus gained on new widget:

                            m_focus_widget.OnFocusGained();
                        }

                    }   break;

                    case FocusTraversalDirection.RIGHT:
                    {
                        // Try to find the next widget to focus on:

                        GuiWidget next_focus_widget = m_search.FindByName( m_focus_widget.RightWidget );

                        // If found then switch focus:

                        if ( next_focus_widget != null )
                        {
                            // Switch focus:

                            m_focus_widget.OnFocusLost();  m_focus_widget = next_focus_widget;

                            // Call focus gained on new widget:

                            m_focus_widget.OnFocusGained();
                        }

                    }   break;

                    case FocusTraversalDirection.ABOVE:
                    {
                        // Try to find the next widget to focus on:

                        GuiWidget next_focus_widget = m_search.FindByName( m_focus_widget.AboveWidget );

                        // If found then switch focus:

                        if ( next_focus_widget != null )
                        {
                            // Switch focus:

                            m_focus_widget.OnFocusLost();  m_focus_widget = next_focus_widget;

                            // Call focus gained on new widget:

                            m_focus_widget.OnFocusGained();
                        }

                    }   break;

                    case FocusTraversalDirection.BELOW:
                    {
                        // Try to find the next widget to focus on:

                        GuiWidget next_focus_widget = m_search.FindByName( m_focus_widget.BelowWidget );

                        // If found then switch focus:

                        if ( next_focus_widget != null )
                        {
                            // Switch focus:

                            m_focus_widget.OnFocusLost();  m_focus_widget = next_focus_widget; 

                            // Call focus gained on new widget:

                            m_focus_widget.OnFocusGained();
                        }

                    }   break;

                }   // end switch ( direction )
            }

        }

        //=========================================================================================
        /// <summary>
        /// Does debug drawing for the gui, which includes rendering all object type names and ids as 
        /// well as a bounding boxes representing 4:3 and 16:9 / 16:10 screen area.
        /// </summary>
        //=========================================================================================

        #if DEBUG

            private void DebugDraw()
            {
                // Lock gui objects list:

                m_data.Lock();

                    // Run through the list of gui objects:

                    Dictionary<int,GuiWidget>.Enumerator e = m_data.Objects.GetEnumerator();

                    // Draw info for each object:

                    while ( e.MoveNext() )
                    {
                        // Get this object:

                        GuiWidget widget = e.Current.Value;

                        // Draw crosshair at object center:

                        DebugDrawing.DrawGuiRectangle( widget.Position , new Vector2(4,0) , Color.Magenta );
                        DebugDrawing.DrawGuiRectangle( widget.Position , new Vector2(0,4) , Color.Magenta );

                        // Get the world view projection matrix:

                        Matrix world_view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

                        // Now draw the id of the object with the debug font: use camera transforms

                        Core.DebugFont.DrawString
                        (
                            widget.Id.ToString(Locale.DevelopmentCulture.NumberFormat)      , 
                            widget.Position                                                 ,
                            world_view_projection
                        );

                        // Now draw the type of the object with the debug font: use camera transforms

                        Core.DebugFont.DrawString
                        (
                            widget.GetType().Name                   , 
                            widget.Position - Vector2.UnitY * 24    ,
                            world_view_projection
                        );
                    }

                // Unlock gui objects list:

                m_data.Unlock();

                // Draw a box around the gui showing the normal 4:3 screen area:

                DebugDrawing.DrawGuiRectangle( Vector2.Zero , new Vector2( Camera.REFERENCE_VIEW_AREA_X * 0.5f , Camera.REFERENCE_VIEW_AREA_Y * 0.5f ) , Color.Red );
            }

        #endif  // #if DEBUG

    }   // end of class 

}   // end of namespace
