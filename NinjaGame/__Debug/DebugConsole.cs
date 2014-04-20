#if WINDOWS_DEBUG

using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

//#################################################################################################
//
/// <summary> 
/// 
/// The Debug console allows the programmer to create their own command 
/// line enviornment for editing game settings on the fly or debugging. 
/// You can also use the console in any .NET enabled application besides 
/// XNA Game Studio 2.0 projects. Be sure to define the symbol 
/// 'WINDOWS_DEBUG' before using the console since this tool should only
/// be used under debug windows build configurations; platforms other than
/// windows are not supported. This class is completely thread-safe and 
/// can be used without worry in a multi-threaded application.
/// 
/// Feedback is welcome. E-mail me at darraghcoy@yahoo.co.uk if you have
/// any comments or suggestions.
/// 
/// </summary>
//
//#################################################################################################

public class DebugConsole
{
    //---------------------------------------------------------------------------------------------
    // Types of functions the console can take: first type in the name
    // indicates the return type. Second part indicates the argument.
    // To add a command to the console call the DebugConsole 
    // 'addCommand' method like this:
    //
    // DebugConsole.addCommand( new DebugConsole.Function_int_void( Class.functionName ) )
    //
    // This particular function takes nothing and returns an integer to
    // the console. The function speficied must be static and must match 
    // the function types listed below. Read / write functions must
    // also take an extra boolean indicating if an argument was passed.
    //---------------------------------------------------------------------------------------------
    
    // Function with no params and no returns

    public delegate void Function_void_void();

    // Read only functions

    public delegate int     Function_int_void      ();
    public delegate bool    Function_bool_void     ();
    public delegate float   Function_float_void    ();
    public delegate string  Function_string_void   ();

    // Write only functions

    public delegate void Function_void_int       ( int       arg     );
    public delegate void Function_void_bool      ( bool      arg     );
    public delegate void Function_void_float     ( float     arg     );
    public delegate void Function_void_string    ( String    arg     );

    //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // Read / write functions: these functions can both read and write. 
    // A boolean is passed to indicate if an argument was given or not.
    // The only exception to the additonal boolean argument is the string 
    // overload, since we can tell if a string is given by seeing if the 
    // length of the string is zero.
    //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    public delegate int     Function_int_int        ( int       arg     , bool argGiven );
    public delegate bool    Function_bool_bool      ( bool      arg     , bool argGiven );
    public delegate float   Function_float_float    ( float     arg     , bool argGiven );
    public delegate string  Function_string_string  ( string    arg     );

    //---------------------------------------------------------------------------------------------
    // Imports from the console dll
    //---------------------------------------------------------------------------------------------

    [DllImport("DebugConsole.dll")] private static extern void Console_Create();                                                                    // Creates the console
    [DllImport("DebugConsole.dll")] private static extern void Console_Shutdown();                                                                  // Shuts down the console
    [DllImport("DebugConsole.dll")] private static extern void Console_Clear();                                                                     // Clears the console
    [DllImport("DebugConsole.dll")] private static extern bool Console_HasFocus();                                                                  // Tells if the console window has focus
    [DllImport("DebugConsole.dll")] private static extern bool Console_Exists();                                                                    // Tells if the console window exists

    [DllImport("DebugConsole.dll")] private static extern void Console_RunPendingCommands();                                                        // Runs any pending user commands
    [DllImport("DebugConsole.dll")] private static extern void Console_RunCommand(string command);                                                  // Run a user given command now
    

    [DllImport("DebugConsole.dll")] private static extern void Console_SetSize(int w , int h);                                                      // Sets the console position
    [DllImport("DebugConsole.dll")] private static extern void Console_SetPosition(int x , int y);                                                  // Sets the console size
    
    [DllImport("DebugConsole.dll")] private static extern void Console_Print_String     ( String    text    );                                      // Prints to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_Print_Int        ( int       num     );                                      // Prints to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_Print_Float      ( float     num     );                                      // Prints to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_Print_Boolean    ( bool      b       );                                      // Prints to the console

    [DllImport("DebugConsole.dll")] private static extern void Console_PrintTitle       ( String text );                                            // Prints to the console        
    [DllImport("DebugConsole.dll")] private static extern void Console_RemoveCommand    ( String name );                                            // Removes this command from the console
    [DllImport("DebugConsole.dll")] private static extern void Console_BeginProfile     ( String name );                                            // Begins profiling the named block of code
    [DllImport("DebugConsole.dll")] private static extern void Console_EndProfile       ( String name );                                            // Ends profiling the named block of code and prints results to the console
                                                                                    
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_void_void     ( String name , Function_void_void      function ); // Adds this type of function to the console

    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_void_int      ( String name , Function_void_int       function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_void_bool     ( String name , Function_void_bool      function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_void_float    ( String name , Function_void_float     function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_void_string   ( String name , Function_void_string    function ); // Adds this type of function to the console

    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_int_void      ( String name , Function_int_void       function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_bool_void     ( String name , Function_bool_void      function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_float_void    ( String name , Function_float_void     function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_string_void   ( String name , Function_string_void    function ); // Adds this type of function to the console

    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_int_int       ( String name , Function_int_int        function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_bool_bool     ( String name , Function_bool_bool      function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_float_float   ( String name , Function_float_float    function ); // Adds this type of function to the console
    [DllImport("DebugConsole.dll")] private static extern void Console_AddCommand_string_string ( String name , Function_string_string  function ); // Adds this type of function to the console

    //---------------------------------------------------------------------------------------------
    // Variables
    //---------------------------------------------------------------------------------------------

    /// <summary> List of delegates: must be kept here and 'alive' (not garbage collected) in order to be called </summary>
    
    private static LinkedList<Object> m_delegate_list = new LinkedList<Object>();

    /// <summary> Set to true once the console has been created </summary>
    
    private static bool m_created = false;

    /// <summary> A file that output will be logged to </summary>

    private static StreamWriter m_log_file = null;

    //=========================================================================================
    /// <summary> 
    /// Creates the debugging console.
    /// </summary>
    //=========================================================================================

    public static void Create()
    {
        try
        {
            // Create the console

            Console_Create();

            // Console has now been created:

            m_created = true;

            // Open the log file to write to

            m_log_file = new StreamWriter("ConsoleLog.txt");
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Shuts down the console
    /// </summary>
    //=========================================================================================

    public static void Shutdown()
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            // Destroy the console

            Console_Shutdown();

            // Console now not created:

            m_created = false;

            // Close the log file

            m_log_file.Close(); m_log_file = null;

        } catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Prints the given text to the console
    /// </summary>
    /// 
    /// <param name="text">text to print</param>
    //=========================================================================================

    public static void Print( String text )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            // Print to console

            Console_Print_String(text);

            // Print to the log

            m_log_file.Write(text);
        } 
        catch {;}

    }

    //=========================================================================================
    /// <summary> 
    /// Prints the given integer to the console
    /// </summary>
    /// 
    /// <param name="number">number to print</param>
    //=========================================================================================

    public static void Print( int number )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            // Print to console

            Console_Print_Int(number);

            // Print to the log

            m_log_file.Write(number);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Prints the given float to the console
    /// </summary>
    /// 
    /// <param name="number">number to print</param>
    //=========================================================================================

    public static void Print( float number )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            // Print to the console

            Console_Print_Float(number);

            // Print to the log

            m_log_file.Write(number);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Prints the given boolean to the console
    /// </summary>
    /// 
    /// <param name="boolean">boolean to print</param>
    //=========================================================================================

    public static void Print( bool boolean )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            // Print to the console

            Console_Print_Boolean(boolean);

            // Print to the log

            m_log_file.Write(boolean);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Prints the given title to the console
    /// </summary>
    /// 
    /// <param name="title">title to print</param>
    //=========================================================================================

    public static void PrintTitle( String title )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            // Print to the console

            Console_PrintTitle(title);

            // Print to the log

            m_log_file.Write("\n\n######################################################\n");
            m_log_file.Write(title);
            m_log_file.Write("\n######################################################\n\n");
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Prints an exception to the console in its entirety.
    /// </summary>
    /// 
    /// <param name="e"> Exception to print </param>
    //=========================================================================================

    public static void PrintException( Exception e )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            // Print the exception title to the console:

            Print("\n\n");
            Print("#####################################################\n");
            Print("########## AN EXCEPTION HAS OCCURED !!! #############\n");
            Print("\n");

            // Print a stack trace:

            Print("### STACK TRACE ###");
            Print("\n");

            StackTrace trace = new StackTrace();

            for ( int i = 1 ;  i < trace.FrameCount; i++ )
            {
                // Get this frame:

                StackFrame frame = trace.GetFrame(i);

                // Get the method:

                MethodBase method = frame.GetMethod();

                // Print the method name and class name:

                Print("\n" + i + ":\t" + method.DeclaringType.FullName + "." + method.Name );

                // Print the arguments to the method:

                Print("(");

                ParameterInfo[] arguments = method.GetParameters();

                for ( int j = 0 ; j < arguments.Length ; j++ )
                {
                    // Print argument type:

                    Print( arguments[j].ParameterType.Name );

                    // If not last arg then print a comma separator:

                    if ( j < arguments.Length - 1 ) Print(",");
                }

                Print(")");
            }

            // Print the full details of the exception

            Print("\n\n");
            Print("### DETAILS ###");
            Print("\n");

            while ( e != null )
            {
                // Write this exception part to the console

                Console_Print_String("\n" + e.Message );

                // Write this exception part to the log file

                m_log_file.Write("\n" + e.Message );

                // Move onto the next exception part

                e = e.InnerException;
            }

            // Print the end of exception to the console:

            Console_Print_String("\n\n");
            Console_Print_String("#####################################################\n");
            Console_Print_String("#####################################################\n");
            Console_Print_String("\n");

            // Print the end of the exception to the log file:

            m_log_file.Write("\n\n");
            m_log_file.Write("#####################################################\n");
            m_log_file.Write("#####################################################\n");
            m_log_file.Write("\n");
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Clears the console
    /// </summary>
    //=========================================================================================

    public static void Clear()
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            Console_Clear();
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Sets the console size
    /// </summary>
    /// 
    /// <param name="width">new width of console window</param>
    /// <param name="height">new height of console window</param>
    //=========================================================================================

    public static void SetSize( int width , int height )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            Console_SetSize(width,height);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Sets the console position
    /// </summary>
    /// 
    /// <param name="x"> position: x </param>
    /// <param name="y"> position: y </param>
    //=========================================================================================

    public static void SetPosition( int x , int y )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            Console_SetPosition(x,y);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Tells if the console window exists
    /// </summary>
    /// 
    /// <returns>True if the window exists, false if not</returns>
    //=========================================================================================

    public static bool Exists()
    {
        // Abort if console not created:

        if ( m_created == false ) return false;

        try
        {
            return Console_Exists();
        }
        catch { return false; }
    }

    //=========================================================================================
    /// <summary> 
    /// Tells if the console window has focus
    /// </summary>
    /// 
    /// <returns>True if the window for the console is in focus</returns>
    //=========================================================================================

    public static bool HasFocus()
    {
        // Abort if console not created:

        if ( m_created == false ) return false;

        try
        {
            return Console_HasFocus();
        }
        catch { return false;}
    }

    //=========================================================================================
    /// <summary> 
    /// Runs any pending console commands that have been input from the 
    /// user. You must call this for any pending input commands to take 
    /// effect. The reason for this call is to allow synchronisation 
    /// between the console and the main application. The console runs 
    /// entirely in its own thread, so we cannot run commands whenever we 
    /// like because it might cause threading issues in the main application.
    /// This function allows the main application to run commands when it is
    /// ready. All commands are also run on the calling thread.
    /// </summary>
    //=========================================================================================

    public static void RunPendingCommands()
    {
        try
        {
            Console_RunPendingCommands();
        }
        catch { ; }
    }

    //=========================================================================================
    /// <summary> 
    /// Runs a given string command immediately. Using the console to 
    /// parse the command and call the appropriate command function.
    /// </summary>
    //=========================================================================================

    public static void RunCommand( string command )
    {
        try
        {
            Console_RunCommand(command);
        }
        catch { ; }
    }

    //=========================================================================================
    /// <summary> 
    /// Removes a given command from the console
    /// </summary>
    /// 
    /// <param name="name">name of the command to remove</param>
    //=========================================================================================

    public static void RemoveCommand( String name )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            Console_RemoveCommand(name);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Begins profiling a block of code
    /// </summary>
    /// 
    /// <param name="name">name for the code being profiled</param>
    //=========================================================================================

    public static void BeginProfile( String name )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            Console_BeginProfile(name);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Ends profiling a block of code and prints results to the console.
    /// </summary>
    /// 
    /// <param name="name">name for the code being profiled</param>
    //=========================================================================================

    public static void EndProfile( String name )
    {
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            Console_EndProfile(name);
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_void_void function )
    {  
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_void_void(name,function);        
        }
        catch {;}
    }
    
    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_void_int function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_void_int(name,function);         
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_void_bool function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_void_bool(name,function);        
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_void_float function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_void_float(name,function);       
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_void_string function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_void_string(name,function);      
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_int_void function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_int_void(name,function);         
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_bool_void function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_bool_void(name,function);        
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_float_void function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_float_void(name,function);       
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_string_void function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_string_void(name,function);       
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_int_int function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_int_int(name,function);          
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_bool_bool function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_bool_bool(name,function);        
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_float_float function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_float_float(name,function);      
        }
        catch {;}
    }

    //=========================================================================================
    /// <summary> 
    /// Add a command to the console.
    /// </summary>
    /// 
    /// <param name="function">Delegate to command function. 
    /// Must be static. Must also match one of the pre-defined console
    /// function types. </param>
    //=========================================================================================

    public static void AddCommand( String name , Function_string_string function )
    { 
        // Abort if console not created:

        if ( m_created == false ) return;

        try
        {
            m_delegate_list.AddLast( function ); Console_AddCommand_string_string(name,function);      
        }
        catch {;}
    }

}   // end of class 'DebugConsole'

//#########################################################################
//#########################################################################

#endif  // if WINDOWS_DEBUG