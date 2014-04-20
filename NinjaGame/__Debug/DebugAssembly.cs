#if WINDOWS_DEBUG

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary> 
    /// 
    /// Contains helpful functions that can be used in assembly debugging. Only available in debug 
    /// build on windows.
    /// 
    /// </summary>
    //
    //#############################################################################################

    class DebugAssembly
    {
        //=========================================================================================
        /// <summary>
        /// Ensures that the function that called this function, was called from a given class name.
        /// </summary>
        /// <param name="className"> Name of the class to ensure </param>
        //=========================================================================================

        public static void EnsureCallingClass( string className )
        {
            // Get the stack trace:

            StackTrace trace = new StackTrace();

            if ( trace.GetFrame(2).GetMethod().DeclaringType.Name != className )
            {
                throw new Exception
                (
                    "DebugAssembly: EnsureCallingClass failed. Expected class " +
                    className + 
                    " only to call function " + 
                    trace.GetFrame(1).GetMethod().Name
                );
            }
        }

    }   // end of class

}   // end of namespace 

//#################################################################################################
//#################################################################################################

#endif  // if WINDOWS_DEBUG