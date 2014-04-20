#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#####################################################################################################
//#####################################################################################################

namespace NinjaGame
{
    //#################################################################################################
    //
    /// <summary> Contains debug functions to do with the game core. </summary>
    //
    //#################################################################################################

    public class DebugCore
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Profile update code ? </summary>

            public static bool ProfileUpdate { get { return m_profile_update; } }

            /// <summary> Profile rendering code ? </summary>

            public static bool ProfileDraw { get { return m_profile_draw; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Profile update code ? </summary>

            private static bool m_profile_update = false;

            /// <summary> Profile rendering code ? </summary>

            private static bool m_profile_draw = false;

        //=========================================================================================
        /// <summary>
        /// Adds core debug commands into the game.
        /// </summary>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static void AddDebugCommands()
            {
                DebugConsole.AddCommand( "PUpdate"  , new DebugConsole.Function_bool_bool( Con_PUpdate  ) );
                DebugConsole.AddCommand( "PDraw"    , new DebugConsole.Function_bool_bool( Con_PDraw    ) );
            }

        #endif  // #if WINDOWS_DEBUG

        //=========================================================================================
        /// <summary>
        /// Debug console command. Turns on/off profiling of the level update.
        /// </summary>
        /// <param name="arg"> Argument </param>
        /// <param name="arg_given"> Whether the argument was given or not. </param>
        /// <returns> Current value of this variable. </returns>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static bool Con_PUpdate ( bool arg , bool arg_given )
            {
                // Set variable if given:

                if ( arg_given ) m_profile_update = arg;

                // Return value:

                return m_profile_update;
            }

        #endif  // #if WINDOWS_DEBUG

        //=========================================================================================
        /// <summary>
        /// Debug console command. Turns on/off profiling of the game drawing.
        /// </summary>
        /// <param name="arg"> Argument </param>
        /// <param name="arg_given"> Whether the argument was given or not. </param>
        /// <returns> Current value of this variable. </returns>
        //=========================================================================================

        #if WINDOWS_DEBUG

            public static bool Con_PDraw ( bool arg , bool arg_given )
            {
                // Set variable if given:

                if ( arg_given ) m_profile_draw = arg;

                // Return value:

                return m_profile_draw;
            }

        #endif  // #if WINDOWS_DEBUG

    }   // end of class

}   // end of namespace

#endif  // #if DEBUG