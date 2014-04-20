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
    //=============================================================================================
    /// <summary>
    /// This is the base class for all GUI widgets in the game. This contains common attributes that 
    /// are shared by all widgets.
    /// </summary>
    //=============================================================================================

    public abstract class GuiWidget : XmlObject
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================            

            /// <summary> Gui the widget is contained in. This should only be modified by Gui objects. </summary>

            public Gui ParentGui
            {
                get { return m_gui; } 

                // On debug windows make sure this is not called anywhere except from a gui data object

                #if WINDOWS_DEBUG

                    set { DebugAssembly.EnsureCallingClass("GuiData"); m_gui = value; }

                #else

                    set { m_gui = value; }

                #endif                
            }

            /// <summary> GuiData data object the object is encapsulated in. This should only be modified by Gui objects. </summary>

            public GuiData ParentContainer
            {
                get { return m_container; } 

                // On debug windows make sure this is not called anywhere except from a gui data object

                #if WINDOWS_DEBUG

                    set { DebugAssembly.EnsureCallingClass("GuiData"); m_container = value; }

                #else

                    set { m_container = value; }

                #endif
            }

            /// <summary> Name of the widget. This does not have to be unique or even filled in but must not be null. </summary>
        
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

            /// <summary> ID of the object in the gui. This should always be unqiue. 0 is an invalid ID. </summary>

            public int Id 
            { 
                get { return m_id; } 

                // On debug windows make sure this is not called anywhere except from a gui data object

                #if WINDOWS_DEBUG

                    set { DebugAssembly.EnsureCallingClass("GuiData"); m_id = value; }

                #else

                    set { m_id = value; }

                #endif
            } 

            /// <summary> Tells is this widget is currently in focus in the gui it is contained in. </summary>

            public bool InFocus 
            {
                get
                {
                    // See if we have a parent gui:

                    if ( m_gui != null )
                    {
                        // Parent gui: see if the focus widget is this

                        return m_gui.FocusWidget == this;
                    }
                    else
                    {
                        // No parent gui: cannot be in focus

                        return false;
                    }
                }
            }

            /// <summary> Position of the widget. </summary>
            
            public Vector2 Position { get { return m_position; } set { m_position = value; } }

            /// <summary> X Position of the widget. </summary>

            public float PositionX { get { return m_position.X; } set { m_position.X = value; } }

            /// <summary> Y Position of the widget. </summary>

            public float PositionY { get { return m_position.Y; } set { m_position.Y = value; } }

            /// <summary> Depth of the object. Used for z-ordering or layering widgets. </summary>

            public int Depth { get { return m_depth; } set { m_depth = value; }  }

            /// <summary> Is this widget allowed to have focus ? </summary>

            public bool CanFocus { get { return m_can_focus; } set { m_can_focus = true; } }

            /// <summary> Should this widget receive focus by default ? </summary>

            public bool DefaultFocus { get { if ( m_can_focus ) return m_default_focus; else return false; } }

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus to the left. </summary>

            public string LeftWidget { get { return m_left_widget; } }

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus to the right. </summary>

            public string RightWidget { get { return m_right_widget; } }

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus upwards. </summary>

            public string AboveWidget { get { return m_above_widget; } }

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus downwards. </summary>

            public string BelowWidget { get { return m_below_widget; } } 

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Gui the gui widget is contained in. </summary>

            private Gui m_gui = null;

            /// <summary> Gui container the widget is encapsulated in. </summary>

            private GuiData m_container = null;

            /// <summary> Position of the widget. </summary>

            private Vector2 m_position = Vector2.Zero;

            /// <summary> Size of the widget. </summary>

            private Vector2 m_size = Vector2.Zero;

            /// <summary> Name of the widget. </summary>

            private String m_name = ""; 

            /// <summary> Unique ID of the widget in it's Gui screen. A widget can only be a part of one GUI at a time! 0 is an invalid ID. </summary>

            private int m_id = 0;

            /// <summary> Depth of the widget. Used for z-ordering or layering of widgets.</summary>

            private int m_depth = 1;

            /// <summary> Is this widget allowed to have focus ? </summary>

            private bool m_can_focus = false;

            /// <summary> Should this widget receive focus by default ? </summary>

            private bool m_default_focus = false;

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus to the left. </summary>

            private string m_left_widget = "";

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus to the right. </summary>

            private string m_right_widget = "";

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus upwards. </summary>

            private string m_above_widget = "";

            /// <summary> Name of the widget which should receive focus next if the user attempts to shift focus downwards. </summary>

            private string m_below_widget = "";

        //=========================================================================================
        /// <summary>
        /// Called when the widget is to be drawn. 
        /// </summary>
        //=========================================================================================

        public virtual void OnDraw(){;}

        //=========================================================================================
        /// <summary>
        /// Called when the widget is removed from the gui.
        /// </summary>
        //=========================================================================================

        public virtual void OnDelete(){;}

        //=========================================================================================
        /// <summary>
        /// Called when the widget is to be updated. 
        /// </summary>
        //=========================================================================================

        public virtual void OnUpdate(){;}

        //=========================================================================================
        /// <summary>
        /// Called after a gui has completely loaded and all objects have been added to the 
        /// gui. If this gui contains references to other objects (using names) then those 
        /// references should be resolved here, since it will be guaranteed that all objects 
        /// that are to be made will have been made at this point.
        /// </summary>
        //=========================================================================================

        public virtual void OnGuiLoaded(){;}

        //=========================================================================================
        /// <summary>
        /// Called when the widget has lost focus
        /// </summary>
        //=========================================================================================

        public virtual void OnFocusLost(){;}

        //=========================================================================================
        /// <summary>
        /// Called when the widget has gained focus
        /// </summary>
        //=========================================================================================

        public virtual void OnFocusGained(){;}

        //=========================================================================================
        /// <summary>
        /// Gamepad pressed event for the widget. 
        /// </summary>
        /// <param name="button"> Button that was pressed. </param>
        //=========================================================================================

        public virtual void OnGamepadPressed( Buttons button ){}

        //=========================================================================================
        /// <summary>
        /// Gamepad released event for the widget. 
        /// </summary>
        /// <param name="button"> Button that was pressed. </param>
        //=========================================================================================

        public virtual void OnGamepadReleased( Buttons button ){}

        //=========================================================================================
        /// <summary>
        /// Keyboard pressed event for the widget. 
        /// </summary>
        /// <param name="key"> Key that was pressed. </param>
        //=========================================================================================

        public virtual void OnKeyboardPressed( Keys key ){}

        //=========================================================================================
        /// <summary>
        /// Keyboard released event for the widget. 
        /// </summary>
        /// <param name="key"> Key that was pressed. </param>
        //=========================================================================================

        public virtual void OnKeyboardReleased( Keys key ){}

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

            data.ReadInt  ( "Depth"     , ref m_depth       , 0 );
            data.ReadFloat( "PositionX" , ref m_position.X  , 0 );
            data.ReadFloat( "PositionY" , ref m_position.Y  , 0 );

            // If the object is focusable then read whether it should be in focus by default or not: also read focus travelsal info

            if ( m_can_focus ) 
            {
                data.ReadBool   ( "DefaultFocus"    , ref m_default_focus   , false );
                data.ReadString ( "LeftWidget"      , ref m_left_widget     , ""    );
                data.ReadString ( "RightWidget"     , ref m_right_widget    , ""    );
                data.ReadString ( "AboveWidget"     , ref m_above_widget    , ""    );
                data.ReadString ( "BelowWidget"     , ref m_below_widget    , ""    );
            }
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

            data.Write( "Name"      , m_name        );
            data.Write( "Depth"     , m_depth       );
            data.Write( "PositionX" , m_position.X  );
            data.Write( "PositionY" , m_position.Y  );

            // If the object is focusable then write whether it should be in focus by default or not: also write focus travelsal info

            if ( m_can_focus ) 
            {
                data.Write  ( "DefaultFocus"    , m_default_focus   );
                data.Write  ( "LeftWidget"      , m_left_widget     );
                data.Write  ( "RightWidget"     , m_right_widget    );
                data.Write  ( "AboveWidget"     , m_above_widget    );
                data.Write  ( "BelowWidget"     , m_below_widget    );
            }
        }

    }   // end of class

}   // end of namespace
