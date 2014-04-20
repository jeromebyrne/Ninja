using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// A simple widget that stops all rumble if there is no Gui_Game widget present on the GUI,
    /// or if the player is dead. Stops rumble from going infinitely if the player is killed 
    /// or if the gui is invoked.
    /// </summary>
    //#############################################################################################

    public class Gui_Rumble_Stopper : GuiWidget
    {
        //=========================================================================================
        /// <summary>
        /// Update function. Stops rumbling on the gamepad where neccessary.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Grab the game widget:

            Gui_Game game_widget = (Gui_Game) ParentGui.Search.FindByType("Gui_Game");

            // See if there:

            if ( game_widget != null )
            {
                // Game widget there: see if the player is alive 

                PlayerNinja player = (PlayerNinja) Core.Level.Search.FindByType("PlayerNinja");

                // See if alive:

                if ( player == null )
                {
                    // Player is dead: stop rumble

                    GamePad.SetVibration(Microsoft.Xna.Framework.PlayerIndex.One,0,0);
                }
            }
            else
            {
                // No game widget: stop rumble

                GamePad.SetVibration(Microsoft.Xna.Framework.PlayerIndex.One,0,0);
            }

            base.OnUpdate();
        }

    }   // end of class

}   // end of namespace
