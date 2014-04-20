using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    // Delegates
    //#############################################################################################

        //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        /// <summary>
        /// Generic event function. For custom or non specific gui events.
        /// </summary>
        /// <param name="event_widget"> Widget involved in the event. </param>
        //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        public delegate void OnGuiGenericEventFunction( GuiWidget event_widget );         

    //#############################################################################################
    /// <summary>
    /// This class builds a list of gui events and allows the application to call one of those 
    /// events. Gui event functions are simple functions that take the name of the widget involved 
    /// in the event. These functions allow behaviour to be added to generic gui widgets.
    /// </summary>
    //#############################################################################################

    public static class GuiEvents
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Generic events list. </summary>

            private static Dictionary<string,OnGuiGenericEventFunction> s_generic_events = new Dictionary<string,OnGuiGenericEventFunction>();

        //=========================================================================================
        /// <summary>
        /// Initialises all Gui events. Runs through the assembly and builds up a list of gui events.
        /// </summary>
        //=========================================================================================

        public static void Initialize()
        {
            // Get all the class types in the assembly:

            Module[] modules = Assembly.GetCallingAssembly().GetModules();

            // Run through all the types:

            foreach ( Module module in modules )
            {
                // Get the module types:

                Type[] types = module.GetTypes();

                // Run through all the module types:

                foreach ( Type type in types )
                {
                    // Make sure not a generic type

                    if ( type.IsGenericType ) continue;

                    // Get all static methods in the class

                    MethodInfo[] methods = type.GetMethods( BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );

                    // Do nowt if this fails:

                    if ( methods == null ) continue;

                    // Build all the event lists:

                    BuildGenericEventsList( methods );
                }

            }   // end foreach module in the assembly
        }

        //=========================================================================================
        /// <summary>
        /// Builds a list of generic widget events.
        /// </summary>
        /// <param name="methods"> List of methods to check for adding into this event list. </param>
        //=========================================================================================

        private static void BuildGenericEventsList( MethodInfo[] methods )
        {
            // Run though:

            foreach ( MethodInfo method in methods )
            {
                // This might fail:

                try
                {
                    // Make sure there are no returns for the method:

                    if ( ! method.ReturnType.Name.Equals("Void") ) continue;

                    // Good: get the parameters for the method:

                    ParameterInfo[] parameters = method.GetParameters();

                    // Make sure there is 1 parameter:

                    if ( parameters.Length != 1 ) continue;

                    // Make sure that one parameter is a gui widget:

                    if ( parameters[0].ParameterType != Type.GetType("NinjaGame.GuiWidget") ) continue;

                    // Make a delegate:

                    OnGuiGenericEventFunction function = (OnGuiGenericEventFunction) Delegate.CreateDelegate
                    ( 
                        Type.GetType("NinjaGame.OnGuiGenericEventFunction") , null , method 
                    );

                    // If null then do not use:

                    if ( function == null ) continue;

                    // Save for later use:

                    s_generic_events.Add( function.Method.Name , function );
                }
                
                #if WINDOWS_DEBUG

                    catch ( Exception e )
                    {
                        // Print what happened if something went wrong on windows debug

                        DebugConsole.PrintException(e);
                    }

                #else

                    catch ( Exception ){}

                #endif

            }
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to invoke a given generic gui event function. The widget involved in the event 
        /// must be given or else this function will do nothing.
        /// </summary>
        /// <
        /// <param name="event_widget"> Widget involved in the event. </param>
        //=========================================================================================

        public static void InvokeGenericEvent( string event_name , GuiWidget event_widget )
        {
            // If no widget is given then abort:

            if ( event_widget == null ) return;

            // See if this event exists:

            if ( s_generic_events.ContainsKey(event_name) )
            {
                // Ok: get the event:

                OnGuiGenericEventFunction event_function = s_generic_events[event_name];

                // Cool: run the event. Catch any exceptions that happen

                try
                {
                    event_function(event_widget);
                }

                // If something went wrong then show it on windows debug
                
                #if WINDOWS_DEBUG

                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                #else

                    catch ( Exception ){}

                #endif
            }

        }

    }   // end of class

}   // end of namespace
