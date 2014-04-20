using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that looks after accquiring storage devices for saving user preferences as well as 
    /// other user data. Also accquires user profile information.
    /// </summary>
    //#############################################################################################

    public static class StorageSystem
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Returns a storage container to save games to. Returns null if none is available. </summary>

            public static StorageContainer Container
            {
                get
                {
                    // Return our storage container:

                    return s_storage_container;
                }
            }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Did we already prompt the user for storage devices ? </summary>

            private static bool s_prompted_user_for_storage = false;

            /// <summary> Storage device we have to save to, if any. </summary>

            private static StorageDevice s_storage_device = null;

            /// <summary> Storage container for the game's files. </summary>

            private static StorageContainer s_storage_container = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Folder/title to store game user data under. </summary>

            public const string TITLE_NAME = "Ninja";

        //=========================================================================================
        /// <summary>
        /// Prompts the user where to save their information if not already done so.
        /// </summary>        
        //=========================================================================================

        public static void ChooseStorageDevice()
        {
            // This might fail:

            try
            {
                // Show the memory dialog if not already done so:

                if ( Guide.IsVisible == false && s_prompted_user_for_storage == false )
                {
                    Guide.BeginShowStorageDeviceSelector
                    (
                        new AsyncCallback(OnStorageDeviceSelect)    ,
                        null 
                    );
                }
            }
            catch ( Exception ){}

            // Prompted the user already

            s_prompted_user_for_storage = true;
        }

        //=========================================================================================
        /// <summary>
        /// Called when a storage device is selected.
        /// </summary>
        /// <param name="result"> Result of the operation. </param>
        //=========================================================================================

        private static void OnStorageDeviceSelect( IAsyncResult result )
        {
            // This might fail:

            try
            {
                // Save the selected storage device:

                s_storage_device = Guide.EndShowStorageDeviceSelector(result);

                // Choose a storage container:

                if ( s_storage_device != null )
                {
                    // Open seasame:

                    s_storage_container = s_storage_device.OpenContainer( TITLE_NAME );
                }

            }
            catch ( Exception ){}
        }

    }   // end of class

}   // end of namespace
