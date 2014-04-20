using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

//################################################################################################
//################################################################################################

namespace NinjaGame
{    
    //############################################################################################
    /// <summary>
    /// Class that tracks and maintains user high scores for each level.
    /// </summary>
    //############################################################################################

    public static class LevelHighScores
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Have high scores been read from a file yet ? </summary>

            public static bool ReadHighScores { get { return s_read_high_scores; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> High scores for each level in the game. Indexed by level file name in lower case. </summary>

            private static Dictionary<string,HighScoreRecord> s_scores = new Dictionary<string,HighScoreRecord>();

            /// <summary> Have high scores been read from a file yet ? </summary>

            private static bool s_read_high_scores = false;

        //=========================================================================================
        // Structs
        //=========================================================================================

            /// <summary> Holds high scores for one particular level. </summary>

            public struct HighScoreRecord
            {
                /// <summary> Top high score. Anything zero or below means no high score. </summary>

                public float Score1;

                /// <summary> 2nd best high score. Anything zero or below means no high score. </summary>

                public float Score2;

                /// <summary> 3rd best high score. Anything zero or below means no high score. </summary>

                public float Score3;

                /// <summary> Name of the user that obtained Score1. Never longer than that defined in User.MAX_USER_NAME_LENGTH. </summary>

                public string Name1;

                /// <summary> Name of the user that obtained Score2. Never longer than that defined in User.MAX_USER_NAME_LENGTH. </summary>
                
                public string Name2;

                /// <summary> Name of the user that obtained Score3. Never longer than that defined in User.MAX_USER_NAME_LENGTH. </summary>

                public string Name3;
            };

        //#########################################################################################
        /// <summary>
        /// Reads the high scores for each level in the game.
        /// </summary>
        /// <param name="folder"> 
        /// Folder name containing the xml files holding the high scores. Each xml file corresponds to 
        /// the high scores for one level and is named the same as the level file.
        /// </param>
        //#########################################################################################

        public static void Load( string folder )
        {
            // We have read high scores from a file now:

            s_read_high_scores = true;

            // Clear high scores:

            s_scores.Clear();

            // This might fail:

            try
            {
                // Try and open the given directory:

                DirectoryInfo dir = new DirectoryInfo(folder);

                // Only do if it exists:

                if ( dir.Exists )
                {
                    // Cool: get a list of files in the dir

                    FileInfo[] files = dir.GetFiles( "*.xml" );

                    // Run through the list of files:

                    foreach ( FileInfo file in files )
                    {
                        // Make an IO stream:

                        Stream stream = file.Open(FileMode.Open,FileAccess.Read);

                        // Attempt to read its data:

                        XmlObjectData data = new XmlObjectData(stream);

                        // Make up a new high score record

                        HighScoreRecord h; 

                        h.Score1 = 0;
                        h.Score2 = 0;
                        h.Score3 = 0;

                        h.Name1 = "";
                        h.Name2 = "";
                        h.Name3 = "";

                        // If any high score is negative then zero it:

                        if ( h.Score1 < 0 ) h.Score1 = 0;
                        if ( h.Score2 < 0 ) h.Score2 = 0;
                        if ( h.Score3 < 0 ) h.Score3 = 0;

                        // Attempt to read all three scores

                        data.ReadFloat( "Score1" , ref h.Score1 );
                        data.ReadFloat( "Score2" , ref h.Score2 );
                        data.ReadFloat( "Score3" , ref h.Score3 );

                        // Load user names:

                        data.ReadString( "Name1" , ref h.Name1 );
                        data.ReadString( "Name2" , ref h.Name2 );
                        data.ReadString( "Name3" , ref h.Name3 );

                        // Trim the names:

                        h.Name1 = h.Name1.Trim();
                        h.Name2 = h.Name2.Trim();
                        h.Name3 = h.Name3.Trim();

                        // If no user name given then use the '-' monkier

                        if ( h.Name1.Length <= 0 ) h.Name1 = "-";
                        if ( h.Name2.Length <= 0 ) h.Name2 = "-";
                        if ( h.Name3.Length <= 0 ) h.Name3 = "-";

                        // Ensure names are within limits:

                        if ( h.Name1.Length > User.MAX_USER_NAME_LENGTH ) h.Name1 = h.Name1.Substring( 0 , User.MAX_USER_NAME_LENGTH );
                        if ( h.Name2.Length > User.MAX_USER_NAME_LENGTH ) h.Name2 = h.Name2.Substring( 0 , User.MAX_USER_NAME_LENGTH );
                        if ( h.Name3.Length > User.MAX_USER_NAME_LENGTH ) h.Name3 = h.Name3.Substring( 0 , User.MAX_USER_NAME_LENGTH );

                        // Sort the scores if not in order:

                        if ( h.Score3 > h.Score2 ){ float t = h.Score3; h.Score3 = h.Score2; h.Score2 = t; }
                        if ( h.Score2 > h.Score1 ){ float t = h.Score2; h.Score2 = h.Score1; h.Score1 = t; }
                        if ( h.Score3 > h.Score2 ){ float t = h.Score3; h.Score3 = h.Score2; h.Score2 = t; }

                        // Save the high scores record:

                        s_scores[ file.Name.ToLower() ] = h;
                    }
                }
            }

            // On windows debug display errors:

            #if WINDOWS_DEBUG

                catch ( Exception e )
                {
                    // Show what happened:

                    DebugConsole.PrintException(e);

                    // Clear high scores:

                    s_scores.Clear();
                }
            
            #else
                
                catch ( Exception )
                {
                    // Clear high scores:

                    s_scores.Clear();
                }

            #endif

        }

        //#########################################################################################
        /// <summary>
        /// Saves the high scores for each level in the game.
        /// </summary>
        /// <param name="folder"> 
        /// Folder name containing the xml files holding the high scores. Each xml file corresponds to 
        /// the high scores for one level and is named the same as the level file.
        /// </param>
        //#########################################################################################

        public static void Save( string folder )
        {
            // This might fail:

            try
            {
                // Make sure the folder exists, if not then try and make it:

                DirectoryInfo dir = new DirectoryInfo( folder );

                // Make it if doesn't exist:

                if ( dir.Exists == false ) dir.Create();

                // Run through all the high scores in the list:

                Dictionary<string,HighScoreRecord>.Enumerator e = s_scores.GetEnumerator();

                // Run through the list:

                while ( e.MoveNext() )
                {
                    // Make up the file:

                    FileInfo file = new FileInfo( folder + "\\" + e.Current.Key );

                    // Ok: open a write stream

                    Stream stream = file.Open( FileMode.Create , FileAccess.Write );

                    // Write to the file:

                    try
                    {
                        // Makeup an xml data object:

                        XmlObjectData data = new XmlObjectData();

                        // Write all high scores for this level to it:

                        data.Write( "Score1" , e.Current.Value.Score1 );
                        data.Write( "Score2" , e.Current.Value.Score2 );
                        data.Write( "Score3" , e.Current.Value.Score3 );

                        data.Write( "Name1" , e.Current.Value.Name1 );
                        data.Write( "Name2" , e.Current.Value.Name2 );
                        data.Write( "Name3" , e.Current.Value.Name3 );

                        // Write the data to the stream:

                        data.Save( stream );

                        // Close the stream:

                        stream.Close();
                    }
                    catch ( Exception )
                    {
                        // Close the stream:

                        try { stream.Close(); } catch ( Exception ){}

                        // Try and delete the file, it may be corrupt:
                      
                        try { file.Delete(); } catch ( Exception ){}
                    }
                }
            }

            // On windows debug display errors:

            #if WINDOWS_DEBUG

                catch ( Exception e )
                {
                    // Show what happened:

                    DebugConsole.PrintException(e);

                    // Clear high scores:

                    s_scores.Clear();
                }
            
            #else
                
                catch ( Exception )
                {
                    // Clear high scores:

                    s_scores.Clear();
                }

            #endif

        }

        //#########################################################################################
        /// <summary>
        /// Gets the high score record for a particular level. If there is no records then 0 scores
        /// will simply be returned. If the level name contains a path then the path will be ignored.
        /// </summary>
        /// <param name="level_name"> Name of the level file to get the high scores for. </param>
        //#########################################################################################

        public static HighScoreRecord GetLevelRecords( string level_name )
        {
            // Make a blank record structure

            HighScoreRecord blank_record;

            blank_record.Score1 = 0;
            blank_record.Score2 = 0;
            blank_record.Score3 = 0;
            blank_record.Name1  = "-";
            blank_record.Name2  = "-";
            blank_record.Name3  = "-";

            // If null or no name is given then return the blank record

            if ( level_name == null || level_name.Length <= 0 ) return blank_record;

            // If there are folder names in the level name then remove them:

            if ( level_name.Contains( "\\" ) )
            {
                // Has path in the name: remove it

                level_name = level_name.Substring( level_name.LastIndexOf('\\') + 1 );
            }

            // Abort if level name is now zero length:

            if ( level_name.Length <= 0 ) return blank_record;

            // Make level name lowercase

            level_name = level_name.ToLower();

            // See if the level exists in our records:

            if ( s_scores.ContainsKey(level_name) )
            {
                // Cool: found it- return the record

                return s_scores[level_name];
            }
            else
            {
                // Didn't find it: return a blank record

                return blank_record;
            }
        }

        //#########################################################################################
        /// <summary>
        /// Records the given high score for the given level. If this is a high score (it made it 
        /// into the list of 3 high scores in the level) then 0 is returned if it is the top score,
        /// 1 is returned for 2nd, and 2 is returned for 3rd. If no high score is attained then -1 
        /// is returned.
        /// </summary>
        /// <param name="level_name"> Name of the level file to record the score for. </param>
        /// <param name="score"> Score to record </param>
        //#########################################################################################

        public static int RecordLevelScore( string level_name , float score )
        {
            // If the score given is invalid or zero then abort:

            if ( score <= 0 ) return -1;

            // If no level is given then abort:

            if ( level_name == null || level_name.Length <= 0 ) return -1;

            // If there are folder names in the level name then remove them:

            if ( level_name.Contains( "\\" ) )
            {
                // Has path in the name: remove it

                level_name = level_name.Substring( level_name.LastIndexOf('\\') + 1 );
            }

            // Make level name lowercase

            level_name = level_name.ToLower().Trim();

            // Abort if level name is now zero length:

            if ( level_name.Length <= 0 ) return -1;

            // See if a record exists for this level already:

            if ( s_scores.ContainsKey( level_name ) == false )
            {
                // No records for this level: make a new record

                HighScoreRecord record; 

                record.Score1   = score;
                record.Score2   = 0;
                record.Score3   = 0;
                record.Name1    = User.UserName;
                record.Name2    = "-";
                record.Name3    = "-";

                // Save the record

                s_scores[level_name] = record;

                // Got a first place ranking:

                return 0;
            }
            else
            {
                // Already have a record: get it

                HighScoreRecord record = s_scores[level_name];

                // See if this score is any better than the rest of them:

                if ( score > record.Score1 )
                {
                    // 1st place score: save
                    
                    record.Score3   = record.Score2;
                    record.Score2   = record.Score1;
                    record.Score1   = score;
                                        
                    record.Name3    = record.Name2;
                    record.Name2    = record.Name1;
                    record.Name1    = User.UserName;

                    s_scores[level_name] = record;

                    // Return placing in the high scores

                    return 0;
                }
                else if ( score > record.Score2 )
                {
                    // 2nd place score: save
                    
                    record.Score3 = record.Score2;
                    record.Score2 = score;

                    record.Name3 = record.Name2;
                    record.Name2 = User.UserName;

                    s_scores[level_name] = record;

                    // Return placing in the high scores

                    return 1;
                }
                else if ( score > record.Score3 )
                {
                    // 3rd place score: save
                    
                    record.Score3 = score; 
                    record.Name3  = User.UserName;
                    
                    s_scores[level_name] = record;

                    // Return placing in the high scores

                    return 2;
                }
                else
                {
                    // Didn't get a score

                    return -1;
                }
            }
        }

    }   // end of class

}   // end of namespace
