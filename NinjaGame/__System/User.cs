using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Gets and maintains information for the user.
    /// </summary>
    //#############################################################################################

    public static class User
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Name of the current user of the game. </summary>
            
            public static string UserName { get { return s_user_name; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Profile for the user: only on xbox. null always on windows. </summary>

            private static GamerProfile s_user_profile = null;

            /// <summary> The user's name. In the absence of a profile we will ask the user for his/her name. </summary>

            private static string s_user_name = "-";

            /// <summary> Did we query for a user profile ? </summary>

            private static bool s_queried_user_profile = false;

            /// <summary> Did we ask the user for a name ? </summary>

            private static bool s_asked_for_user_name = false;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Maximum length for a user name. </summary>

            public const int MAX_USER_NAME_LENGTH = 15;

        //=========================================================================================
        /// <summary>
        /// Checks for user details if not done so already. If no user is signed in 
        /// (always the case on windows) then the user will be asked for his/her name.
        /// </summary>
        //=========================================================================================

        public static void CheckUserDetails()
        {
            // See if we queried for a user profile:

            if ( s_queried_user_profile == false )
            {
                // Ok: look for a user profile

                try
                {
                    // Do the query:

                    if ( Gamer.SignedInGamers.Count > 0 && Gamer.SignedInGamers[PlayerIndex.One] != null )
                    {
                        // Try and get the signed in player for player one:

                        SignedInGamer signed_in_player = Gamer.SignedInGamers[PlayerIndex.One];

                        // See if there:

                        if ( signed_in_player != null )
                        {
                            // Grab the user profile:

                            s_user_profile = signed_in_player.GetProfile();

                            // Try and get the user name:

                            s_user_name = signed_in_player.Gamertag;

                            // Make sure no leading or trailing white space:

                            s_user_name = s_user_name.Trim();

                            // If no user name given then use a dash:

                            if ( s_user_name == null || s_user_name.Length <= 0 ) s_user_name = "-";

                            // Make sure it is within the max size:

                            if ( s_user_name.Length >= MAX_USER_NAME_LENGTH )
                            {
                                s_user_name = s_user_name.Substring(0,MAX_USER_NAME_LENGTH);
                            }
                        }
                    }
                }

                // Show exceptions on windows debug

                #if WINDOWS_DEBUG

                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                #else

                    catch ( Exception ){}

                #endif

                // Now queried for a user profile:

                s_queried_user_profile = true;
            }
            else if ( s_user_profile == null && s_asked_for_user_name == false )
            {
                // If we haven't got a profile then try and ask the user for his/her name:

                if ( Guide.IsVisible == false )
                {
                    // This might fail:

                    try
                    {
                        // Only do if guide is not showing already

                        Guide.BeginShowKeyboardInput
                        (
                            PlayerIndex.One                                             ,
                            StringDatabase.GetString("user_enter_name_title")           ,
                            StringDatabase.GetString("user_enter_name_description")     ,
                            ""                                                          ,
                            new AsyncCallback(OnNameEntered)                            , 
                            null
                        );
                    }

                    // Show exceptions on windows debug

                    #if WINDOWS_DEBUG

                        catch ( Exception e ){ DebugConsole.PrintException(e); }

                    #else

                        catch ( Exception ){}

                    #endif

                    // Asked the user for his/her name

                    s_asked_for_user_name = true;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Callback. Called when the user enters its name.
        /// </summary>
        /// <param name="result"> Result of the asynchronous operation. </param>
        //=========================================================================================

        private static void OnNameEntered( IAsyncResult result )
        {
            // This might fail:

            try
            {
                // Grab the name of the user:

                s_user_name = Guide.EndShowKeyboardInput(result);

                // This has a nasty habit of returning null:

                if ( s_user_name == null ) s_user_name = "-";

                // Remove leading and trailing white space:

                s_user_name = s_user_name.Trim();

                // If the user name is longer than 15 characters then cut short:

                if ( s_user_name.Length > 15 )
                {
                    // Reduce to 15 characters

                    s_user_name = s_user_name.Substring(0,15);
                }
                else if ( s_user_name.Length <= 0 )
                {
                    // If user name is not given then adopt '-' as a user name

                    s_user_name = "-";
                }
            }

            // Show exceptions on windows debug

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif
        }

    }   // end of class

}   // end of namespace