using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

//################################################################################################
//################################################################################################

namespace NinjaGame
{
    //################################################################################################
    /// <summary>
    /// This class handles loading strings from XML files and storing them for use in the game.
    /// All strings should be read from this data base as opposed to being hardcoded into the game.
    /// This class is static and accessible from anywhere in the game.
    /// </summary>
    //################################################################################################

    public static class StringDatabase
    {
        //=========================================================================================
        // Variables 
        //=========================================================================================  

            /// <summary> This dictionary contains a mapping from string names to string values. </summary>
            
            private static Dictionary<string,string> s_strings = new Dictionary<string,string>();            

        //=========================================================================================
        /// <summary>
        /// Loads all the string xml files in a given folder and adds all their strings into the databse.
        /// This function is recursive and works with subfolders as well.
        /// </summary>
        /// <param name="folder_name"> Name of the folder to load </param>
        //=========================================================================================

        public static void LoadContentFolder( string folder_name )
        {
            // Try and open this folder:

            try
            {
                // Try and open the folder:

                DirectoryInfo folder = new DirectoryInfo(folder_name);

                // See if it exists:

                if ( folder.Exists )
                {
                    // Get a list of xml files in the folder:

                    FileInfo[] files = folder.GetFiles("*.xml");

                    // Good: run through all the files in this folder

                    foreach ( FileInfo file in files ) 
                    {
                        // Get the name of the file:

                        string file_name = file.Name.ToLower();

                        // Make a regex to see if this is a locale specific version of the file:

                        Regex r = new Regex("\\.\\w\\w-\\w\\w\\.xml");

                        // If this is a locale specific version then ignore it, we will get the locale specific version later:

                        Match match = r.Match(file_name);

                        // See if this is locale specific: if not then load and THEN translate name into locale specific version of the file

                        if ( match.Success == false ) LoadContentFile(file);
                    }

                    // Get a list of directories:

                    DirectoryInfo[] directories = folder.GetDirectories();

                    // Now run through all the subfolders in this folder and recursively call this function on it:

                    foreach ( DirectoryInfo sub_folder in directories )
                    {
                        // Call this function on the folder:

                        LoadContentFolder(sub_folder);
                    }
                }
            }
            
            // On windows debug show what went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Loads all the string xml files in a given folder and adds all their strings into the databse.
        /// This function is recursive and works with subfolders as well.
        /// </summary>
        /// <param name="folder"> Folder object containing the xml files with the strings </param>
        //=========================================================================================

        private static void LoadContentFolder( DirectoryInfo folder )
        {
            // Try and open this folder:

            try
            {
                // See if it exists:

                if ( folder.Exists )
                {
                    // Get a list of xml files in the folder:

                    FileInfo[] files = folder.GetFiles("*.xml");

                    // Good: run through all the files in this folder

                    foreach ( FileInfo file in files ) 
                    {
                        // Get the name of the file:

                        string file_name = file.Name.ToLower();

                        // Make a regex to see if this is a locale specific version of the file:

                        Regex r = new Regex("\\.\\w\\w-\\w\\w\\.xml");

                        // If this is a locale specific version then ignore it, we will get the locale specific version later:

                        Match match = r.Match(file_name);

                        // See if this is locale specific: if not then load and THEN translate name into locale specific version of the file

                        if ( match.Success == false ) LoadContentFile(file);
                    }

                    // Get a list of directories:

                    DirectoryInfo[] directories = folder.GetDirectories();

                    // Now run through all the subfolders in this folder and recursively call this function on it:

                    foreach ( DirectoryInfo sub_folder in directories )
                    {
                        // Call this function on the folder:

                        LoadContentFolder(sub_folder);
                    }
                }
            }
            
            // On windows debug show what went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary> 
        /// Loads an xml file and all it's contained strings into the databse. 
        /// </summary>
        /// <param name="file"> The file to load. </param>
        //=========================================================================================

        private static void LoadContentFile( FileInfo file )
        {
            // This might fail, Try and open this folder:

            try
            {
                // Make a new xml document:

                XmlDocument xml_doc = new XmlDocument();

                // Load it from the file:
                
                xml_doc.Load( Locale.GetLocFile(file.FullName) );

                // See if there is a root node:

                if ( xml_doc.FirstChild != null )
                {
                    // Good: run through the sub nodes of the root:

                    foreach ( XmlNode node in xml_doc.FirstChild )
                    {
                        // Make sure this node is an element type:

                        if ( node.NodeType == XmlNodeType.Element )
                        {
                            // Good: see if it has a loc id attribute and save it's id:

                            string loc_id = null;

                            // Run through all the attributes:

                            foreach ( XmlAttribute attrib in node.Attributes )
                            {
                                // See if the name of this is the loc id attribute:

                                if ( attrib.Name.ToLower().Equals("_locid") )
                                {
                                    // Excellent: save the id of the string

                                    loc_id = attrib.Value.ToLower();
                                }
                            }

                            // If the string has no loc id then ignore it:

                            if ( loc_id == null || loc_id.Length <= 0 ) continue;

                            // Good: see if its name already exists in the string database:

                            if ( s_strings.ContainsKey( loc_id ) )
                            {
                                // Already in there: throw an exception. There should be never two of the same named strings in the db

                                throw new Exception
                                (
                                    "The string called '"                                       + 
                                    loc_id                                                      + 
                                    "' in the file '"                                           + 
                                    file.FullName                                               + 
                                    "' is already present in the databse. "                     + 
                                    "Please fix this and choose another name for the string."   +
                                    "The rest of the xml file will NOT be parsed now."
                                );
                            }
                            else
                            {
                                // We're ok: add a lowercase copy of the string into the databse and replace all \n symbols with new lines

                                    // Split the line up into sub strings on the \n character:

                                    string[] lines = node.InnerText.Replace("\\n","\n").Split('\n');

                                    // Trim each string and add a newline char to the end if not the last one:

                                    for ( int i = 0 ; i < lines.Length ; i++ )
                                    {
                                        // Trim:

                                        lines[i] = lines[i].Trim();

                                        // Add new line character to end if not hte last one:

                                        if ( i < lines.Length - 1 ) lines[i] += "\n";
                                    }

                                    // Now merge all the strings into one:

                                    string combined = "";

                                    for ( int i = 0 ; i < lines.Length ; i++ )
                                    {
                                        combined += lines[i];
                                    }

                                    // Add this string into the dictionary

                                    s_strings.Add( loc_id , combined );
                            }
                        }
                    }
                }
            }
            
            // On windows debug show what went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Returns the value of the given string. If it cannot be found the expression 
        /// 'Unable to find string 'string_name'' is returned. 
        /// </summary>
        /// <param name="string_name"> Name of the string to get. </param>
        /// <returns> The value of the string. </returns>
        //=========================================================================================

        public static string GetString( string string_name )
        {
            // If the string is null then make it non null

            if ( string_name == null ) string_name = "";

            // Make a lowercase copy of the string:

            string s_lower = string_name.ToLower();

            // See if it exists:

            if ( s_strings.ContainsKey(s_lower) )
            {
                // Return the string:

                return s_strings[s_lower];
            }
            else
            {
                // Not found: return the error expression

                return "Unable to find string '" + string_name + "'";
            }
        }

    }   // end of class

}   // end of namespace
