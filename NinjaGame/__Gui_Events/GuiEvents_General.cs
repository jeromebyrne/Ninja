using System;
using System.Collections.Generic;
using System.Text;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Collection of general gui events.
    /// </summary>
    //#############################################################################################

    public class GuiEvents_Main_Menu
    {
        //=========================================================================================
        /// <summary>
        /// Gui event. Causes the game to end.
        /// </summary>
        /// <param name="widget"> Widget that the event is happening on. </param>
        //=========================================================================================

        public static void Event_Quit( GuiWidget widget ){ Core.Game.Exit(); }

        //=========================================================================================
        /// <summary>
        /// Gui event. Restarts the level.
        /// </summary>
        /// <param name="widget"> Widget that the event is happening on. </param>
        //=========================================================================================

        public static void Event_Restart_Level( GuiWidget widget )
        { 
            // Reload the level:

            Core.Level.Load( Core.Level.FileName );
        }

        //=========================================================================================
        /// <summary>
        /// Gui event. Opens up the previously open screen.
        /// </summary>
        /// <param name="widget"> Widget that the event is happening on. </param>
        //=========================================================================================

        public static void Event_Pop_Screen( GuiWidget widget )
        { 
            // Load the previous screen:

            Core.Gui.PopScreen();
        }

    }   // end of namespace

}   // end of class 
