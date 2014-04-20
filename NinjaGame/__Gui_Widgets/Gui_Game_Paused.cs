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
    /// Like the game gui, but for the paused menu screen. Renders the game to a render target and 
    /// blurs the view whilst greying it out.
    /// </summary>
    //#############################################################################################

    class Gui_Game_Paused : Gui_Game
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Last state of the keyboard. </summary>

            KeyboardState m_last_keyboard_state;

            /// <summary> Last state of the gamepad. </summary>

            GamePadState m_last_gamepad_state;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the object.
        /// </summary>
        //=========================================================================================

        public Gui_Game_Paused()
        {
            // Save the current kb and gamepad state;

            m_last_gamepad_state    = GamePad.GetState(PlayerIndex.One);
            m_last_keyboard_state   = Keyboard.GetState();
        }

        //=========================================================================================
        /// <summary> 
        /// Update function for the GUI. Updates the game.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Do debug updates if in debug mode as the game will no longer do them:

            #if DEBUG

                DebugLevel.Update();

            #endif

            // Get the current gamepad and keyboard state:

            KeyboardState kbs = Keyboard.GetState();
            GamePadState  gps = GamePad.GetState(PlayerIndex.One);

            // If the unpause button is pressed then make the game gui the gui again:

            if 
            ( 
                ( m_last_keyboard_state.IsKeyUp(Keys.Pause) && kbs.IsKeyDown(Keys.Pause) )
                ||
                ( m_last_keyboard_state.IsKeyUp(Keys.P) && kbs.IsKeyDown(Keys.P) )
                ||
                ( m_last_keyboard_state.IsKeyUp(Keys.Escape) && kbs.IsKeyDown(Keys.Escape) )
                ||
                ( m_last_gamepad_state.IsButtonUp(Buttons.Start) && gps.IsButtonDown(Buttons.Start) )                    
            )
            {
                // Unpause game by loading main game gui:

                Core.Gui.Load("Content\\Guis\\Game.xml");
            }

            // Save keyboard and gamepad state:

            m_last_gamepad_state    = gps;
            m_last_keyboard_state   = kbs;
        }

    }   // end of class

}   // end of namespace
