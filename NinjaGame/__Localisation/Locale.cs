using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.IO;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// This class builds a list of locale specific files in the game's content directories. 
    /// A locale extension on a file (eg. en-en) signifies locale specific versions of files. 
    /// Every reference to a file in the game should use this system to get the locale specific 
    /// file name for a particular asset.
    /// </summary>
    //#############################################################################################

    public class Locale
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Development culture. All non-string xml data should be read and written according to this culture. </summary>

            public static CultureInfo DevelopmentCulture { get { return s_development_culture; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Development culture. All numeric xml data should be read and written according to this culture. </summary>

            private static CultureInfo s_development_culture = new CultureInfo("en-ie");

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This dictionary maps from content file names to localised versions of the files for this
            /// culture. E.g the key for a content piece might be Graphics\Default for a texture and 
            /// the localised name returned might be Graphics\Default.en-en or Graphics\Default.en-ie
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private static Dictionary<string,string> s_locale_specific_files = new Dictionary<string,string>();

        //=========================================================================================
        /// <summary>
        /// This function builds a list of locale specific files in the game's directory. It should be 
        /// called on startup before the game is initialised.
        /// </summary>
        //=========================================================================================

        public static void BuildLocalisedFileList()
        {
            // Get the current directory:

            DirectoryInfo dir = new DirectoryInfo(".\\");

            // Try to find exact language and locale versions of each file in the directory and it's sub directories:

            FindExactCultureFiles(dir);

            // Try to find language matching versions of each file in the directory and it's sub directories if no exact culture matches:

            FindSimilarCultureFiles(dir);
        }

        //=========================================================================================
        /// <summary>
        /// Finds localised files for the current culture and language that match 
        /// this culture and language exactly.
        /// </summary>
        /// <param name="dir"> 
        /// Directory to begin search for locale specific content in. All subdirectories will 
        /// be searched also.
        /// </param>
        //=========================================================================================

        private static void FindExactCultureFiles( DirectoryInfo dir )
        {
            // Make sure the directory exists:

            if ( dir.Exists == false ) return;

            // Get the current culture code:

            string culture = System.Globalization.CultureInfo.CurrentCulture.Name.ToLower();

            // If not there then abort:

            if ( culture == null || culture.Length <= 0 ) return;

            // If missing language / culture separator then abort:

            if ( culture.Contains("-") == false ) return;

            // Get a list of files in the directory matching this locale:

            FileInfo[] files = dir.GetFiles( "*." + culture + "*" );

            // Ok: run through all the files in the directory:

            foreach ( FileInfo file in files )
            {
                // Get the full name of the file:

                string file_name_relative = file.FullName.ToLower();

                // Remove the part before the content directory path from the file name if there:

                if ( file_name_relative.IndexOf( Core.CONTENT_FOLDER , StringComparison.CurrentCultureIgnoreCase) >= 0 )
                {
                    // Excellent: remove it

                    file_name_relative = file_name_relative.Substring
                    (
                        file_name_relative.IndexOf( Core.CONTENT_FOLDER ,StringComparison.CurrentCultureIgnoreCase)
                    );
                }

                // If the path starts with a backslash then remove that too:

                if ( file_name_relative.StartsWith("\\") )
                {
                    file_name_relative = file_name_relative.Remove(0,1);
                }

                // If the file ends with a .xnb (Content pipeline file) specifier then it does not need either the content folder path or the .xnb extension:

                if ( file_name_relative.EndsWith(".xnb",StringComparison.CurrentCultureIgnoreCase) )
                {
                    // Remove the .xnb specifer:

                    file_name_relative = file_name_relative.Remove( file_name_relative.LastIndexOf(".xnb") , 4 );

                    // Remove the content folder from the path:

                    if ( file_name_relative.StartsWith( Core.CONTENT_FOLDER , StringComparison.CurrentCultureIgnoreCase ) )
                    {
                        file_name_relative = file_name_relative.Remove( 0 , Core.CONTENT_FOLDER.Length );
                    }

                    // If the path starts with a backslash then remove that too:

                    if ( file_name_relative.StartsWith( "\\" , StringComparison.CurrentCultureIgnoreCase  ) )
                    {
                        file_name_relative = file_name_relative.Remove(0,1);
                    }
                }

                // Get the normal name of the file in it's culture invariant version:

                string invariant_name = file_name_relative; 

                    // Search for this in the string

                    string culture_specifier = "." + culture;

                    // Do the looking:

                    if ( invariant_name.LastIndexOf(culture_specifier) >= 0 ) 
                    {
                        // Remove this culture specifier to get the invariant name:

                        invariant_name = invariant_name.Remove( invariant_name.LastIndexOf(culture_specifier) , culture_specifier.Length );
                    }

                // Now save the invariant name as the key and the culture sensitive name as the value:

                s_locale_specific_files[invariant_name] = file_name_relative;

            }   // end foreach file

            // Get all the subdirectories in this directory and repeat the process:

            DirectoryInfo[] sub_dirs = dir.GetDirectories();

            // Run through all of them:

            for ( int i = 0 ; i < sub_dirs.Length ; i++ ) FindExactCultureFiles(sub_dirs[i]);
        }

        //=========================================================================================
        /// <summary>
        /// Similar to the FindExactCultureFiles function except this function will try to do 
        /// language matches only if an exact culture specific version of a file has not been found.
        /// For example if the file "Graphics\default.en-ie" is not present for Irish-English then 
        /// an attempt will be made to find "Graphics\default.en-en"
        /// </summary>
        /// <param name="dir"> 
        /// Directory to begin search for smiliar locale content in. All subdirectories will 
        /// be searched also.
        /// </param>
        //=========================================================================================

        private static void FindSimilarCultureFiles( DirectoryInfo dir )
        {
            // Make sure the directory exists:

            if ( dir.Exists == false ) return;

            // Get the current culture code:

            string culture = System.Globalization.CultureInfo.CurrentCulture.Name.ToLower();

            // If not there then abort:

            if ( culture == null || culture.Length <= 0 ) return;

            // If missing language / culture separator then abort:

            if ( culture.Contains("-") == false ) return;

            // Get the first part of the culture specifier (the language) :

            string[] language = culture.Split( new char[]{ '-' } );

            // Make up the new culutre in the format of lang-lang .. eg en-en , fr-fr and so on.. 

            culture = language[0] + "-" + language[0];

            // Get a list of files in the directory matching this locale:

            FileInfo[] files = dir.GetFiles( "*." + culture + "*" );

            // Ok: run through all the files in the directory:

            foreach ( FileInfo file in files )
            {
                // Get the full name of the file:

                string file_name_relative = file.FullName.ToLower();

                // Remove the part before the content directory path from the file name if there:

                if ( file_name_relative.IndexOf( Core.CONTENT_FOLDER , StringComparison.CurrentCultureIgnoreCase) >= 0 )
                {
                    // Excellent: remove it

                    file_name_relative = file_name_relative.Substring
                    (
                        file_name_relative.IndexOf( Core.CONTENT_FOLDER ,StringComparison.CurrentCultureIgnoreCase)
                    );
                }

                // If the path starts with a backslash then remove that too:

                if ( file_name_relative.StartsWith("\\") )
                {
                    file_name_relative = file_name_relative.Remove(0,1);
                }

                // If the file ends with a .xnb (Content pipeline file) specifier then it does not need either the content folder path or the .xnb extension:

                if ( file_name_relative.EndsWith(".xnb",StringComparison.CurrentCultureIgnoreCase) )
                {
                    // Remove the .xnb specifer:

                    file_name_relative = file_name_relative.Remove( file_name_relative.LastIndexOf(".xnb") , 4 );

                    // Remove the content folder from the path:

                    if ( file_name_relative.StartsWith( Core.CONTENT_FOLDER , StringComparison.CurrentCultureIgnoreCase ) )
                    {
                        file_name_relative = file_name_relative.Remove( 0 , Core.CONTENT_FOLDER.Length );
                    }

                    // If the path starts with a backslash then remove that too:

                    if ( file_name_relative.StartsWith( "\\" , StringComparison.CurrentCultureIgnoreCase  ) )
                    {
                        file_name_relative = file_name_relative.Remove(0,1);
                    }
                }

                // Get the normal name of the file in it's culture invariant version:

                string invariant_name = file_name_relative; 

                    // Search for this in the string

                    string culture_specifier = "." + culture;

                    // Do the looking:

                    if ( invariant_name.LastIndexOf(culture_specifier) >= 0 ) 
                    {
                        // Remove this culture specifier to get the invariant name:

                        invariant_name = invariant_name.Remove( invariant_name.LastIndexOf(culture_specifier) , culture_specifier.Length );
                    }

                //---------------------------------------------------------------------------------
                // Now save the invariant name as the key and the culture sensitive name as the value, 
                // but only if we did not already find an exact culture version of this file.. 
                //---------------------------------------------------------------------------------

                if ( s_locale_specific_files.ContainsKey(invariant_name) == false )
                {
                    s_locale_specific_files[invariant_name] = file_name_relative;
                }

            }   // end foreach file

            // Get all the subdirectories in this directory and repeat the process:

            DirectoryInfo[] sub_dirs = dir.GetDirectories();

            // Run through all of them:

            for ( int i = 0 ; i < sub_dirs.Length ; i++ ) FindSimilarCultureFiles(sub_dirs[i]);
        }

        //=========================================================================================
        /// <summary>
        /// Returns the appropriate locale specific version of a file if any is present. Locale 
        /// specific versions of files are denoted by locale specifiers in their file name, 
        /// such as en-en , or en-ie. For example if the this function is called 
        /// </summary>
        /// <param name="name"> 
        /// Name of the locale specific version of the file to look for. If 'Graphics\Default' 
        /// for example is given and culture is en-ie then the game will return 'Graphics\Default.en-ie' 
        /// if present. If 'Graphics\Default.en-ie' is not present but 'Graphics\Default.en-en' is 
        /// (language match) then this name will be returned instead. If no match is found then 
        /// the original name is returned instead.
        /// </param>
        /// <returns> 
        /// Name of a locale specific version of the file, or the original file name if none found. 
        /// </returns>
        //=========================================================================================

        public static string GetLocFile( string name )
        {
            // Abort if no name given:

            if ( name == null || name.Length <= 0 ) return "";

            // Make a lowercase version of the name:

            string name_lower = name.ToLower();

            // Remove the part before the content directory path from the file name if there:

            if ( name_lower.IndexOf(Core.CONTENT_FOLDER,StringComparison.CurrentCultureIgnoreCase) >= 0 )
            {
                // Excellent: remove it

                name_lower = name_lower.Substring
                (
                    name_lower.IndexOf(Core.CONTENT_FOLDER,StringComparison.CurrentCultureIgnoreCase)
                );
            }

            // See if in the dictionary:

            if ( s_locale_specific_files.ContainsKey(name_lower) )
            {
                // Bingo: return the locale specific version of the file

                return s_locale_specific_files[name_lower];
            }
            else
            {
                // Not found: just return the normal file name

                return name;
            }
        }

    }   // end of class

}   // end of namespace 
