using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame.__Gui_Widgets
{
    //#############################################################################################
    /// <summary>
    /// Specialised button that goes to a new screen when clicked on.
    /// </summary>
    //#############################################################################################

    public class Gui_Screen_Change_Button : Gui_Button
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Screen the button will go to when activated next. </summary>

            private string m_next_screen = "";

            /// <summary> Screen the button will go back to when the back button is pressed. </summary>

            private string m_previous_screen = "";

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
            // Call base class function

            base.ReadXml(data);

            // Read all data:

            data.ReadString ( "NextScreen"     , ref m_next_screen      , ""    );
            data.ReadString ( "PreviousScreen" , ref m_previous_screen  , ""    );
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
            // Call base class function

            base.WriteXml(data);

            // Write all data:

            data.Write( "NextScreen"     , m_next_screen        );
            data.Write( "PreviousScreen" , m_previous_screen    );
        }

        //=========================================================================================
        /// <summary>
        /// On keyboard pressed event for the widget.
        /// </summary>
        /// <param name="key"> Key just pressed. </param>
        //=========================================================================================

        public override void OnKeyboardPressed( Keys key )
        {
            // Call events if the right buttons are pressed:

            switch ( key )
            {
                case Keys.Enter:    Core.Gui.Load(m_next_screen);       break;
                case Keys.Space:    Core.Gui.Load(m_next_screen);       break;
                case Keys.Escape:   Core.Gui.Load(m_previous_screen);   break;
                case Keys.Back:     Core.Gui.Load(m_previous_screen);   break;
            }

            // Call base function:

            base.OnKeyboardPressed(key);
        }

        //=========================================================================================
        /// <summary>
        /// On gamepad pressed event for the widget.
        /// </summary>
        /// <param name="button"> Button just pressed. </param>
        //=========================================================================================

        public override void OnGamepadPressed( Buttons button )
        {
            // Call events if the right buttons are pressed:

            switch ( button )
            {
                case Buttons.A: Core.Gui.Load(m_next_screen);       break;
                case Buttons.B: Core.Gui.Load(m_previous_screen);   break;
            }

            // Call base function:

            base.OnGamepadPressed(button);
        }

    }   // end of class

}   // end of namespace
