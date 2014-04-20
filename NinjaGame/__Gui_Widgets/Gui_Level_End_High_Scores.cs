using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// This class draws the high scores for the level after it has finished. If the player has 
    /// got a new high score then this is shown to the player. This class also records the players 
    /// current score in the level with the high scores database on creation.
    /// </summary>
    //#############################################################################################

    public class Gui_Level_End_High_Scores : GuiWidget
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            public Font Font { get { return m_font; } set { m_font = value; } } 

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Font used by the widget to draw text with. </summary>

            private Font m_font = null;

            /// <summary> Name of the xml font descriptor used by the widget's font. </summary>

            private string m_font_name = "";

            /// <summary> This is the time it takes for each of the three scores to fade in. </summary>

            private float m_score_fade_in_time = 0.25f;

            /// <summary> Time in seconds since screen creation. </summary>

            private float m_time_since_creation = 0;

            /// <summary> Name of the string used for '1st' </summary>

            private string m_string_1st = "";

            /// <summary> Name of the string used for '2nd' </summary>

            private string m_string_2nd = "";

            /// <summary> Name of the string used for '3rd' </summary>

            private string m_string_3rd = "";

            /// <summary> Index of the new high score the player has just got. -1 for no score, 0 for 1st, 1 for 2nd etc..  </summary>

            private int m_new_high_score = -1;

            /// <summary> Time it takes for a score to size up and down when we are displaying a new high score </summary>

            private float m_new_high_score_throb_time = 0.25f;

            /// <summary> Amount to expand text by when displaying a throbbing new high score </summary>

            private float m_new_high_score_throb_expansion = 8;

            /// <summary> Color to throb in and out to when displaying a new high score </summary>

            private Vector4 m_new_high_score_throb_color = Vector4.One;

            /// <summary> Are we throbbing the new high score up or down ? </summary>

            private bool m_new_high_score_throb_up = true;

            /// <summary> Time since the current throb with the new high score began. </summary>

            private float m_new_high_score_time_throb_time_elapsed = 0;

        //=========================================================================================
        /// <summary>
        /// Constructor. Records the players current score with the high scores records.
        /// </summary>
        //=========================================================================================

        public Gui_Level_End_High_Scores()
        {
            // Get the level rules object:

            LevelRules rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

            // Do this only if there:

            if ( rules != null )
            {
                // Record the players score for the level and see if there is a high score:

                m_new_high_score = LevelHighScores.RecordLevelScore
                (
                    Core.Level.FileName ,
                    rules.PlayerScore
                );
            }
        }

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should read its own data from
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// read from here.
        /// </param>
        //=========================================================================================

        public override void ReadXml( XmlObjectData data )
        {
            // Call base class function

            base.ReadXml(data);

            // Read font:

            data.ReadString ( "Font" , ref m_font_name , "Content\\Fonts\\Game_16px.xml" );  
          
            // Read all other data

            data.ReadString ( "String1st"                   , ref m_string_1st                      , "no_string"   );
            data.ReadString ( "String2nd"                   , ref m_string_2nd                      , "no_string"   );
            data.ReadString ( "String3rd"                   , ref m_string_3rd                      , "no_string"   );
            data.ReadFloat  ( "ScoreFadeInTime"             , ref m_score_fade_in_time              , 0.25f         );
            data.ReadFloat  ( "NewHighScoreThrobTime"       , ref m_new_high_score_throb_time       , 0.25f         );
            data.ReadFloat  ( "NewHighScoreThrobExpansion"  , ref m_new_high_score_throb_expansion  , 8             );
            data.ReadFloat  ( "NewHighScoreThrobColorR"     , ref m_new_high_score_throb_color.X    , 1             );
            data.ReadFloat  ( "NewHighScoreThrobColorG"     , ref m_new_high_score_throb_color.Y    , 1             );
            data.ReadFloat  ( "NewHighScoreThrobColorB"     , ref m_new_high_score_throb_color.Z    , 1             );
            data.ReadFloat  ( "NewHighScoreThrobColorA"     , ref m_new_high_score_throb_color.W    , 1             );

            // Create font:

            m_font = new Font(m_font_name);

            // Reset this

            m_time_since_creation = 0;
        }

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should write its own data to
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// written to here.
        /// </param>
        //=========================================================================================

        public override void WriteXml( XmlObjectData data )
        {
            // Call base class function

            base.WriteXml(data);

            // Write all data:
           
            data.Write( "Font"                          , m_font_name                       );
            data.Write( "ScoreFadeInTime"               , m_score_fade_in_time              );
            data.Write( "String1st"                     , m_string_1st                      );
            data.Write( "String2nd"                     , m_string_2nd                      );
            data.Write( "String3rd"                     , m_string_3rd                      );
            data.Write( "NewHighScoreThrobTime"         , m_new_high_score_throb_time       );
            data.Write( "NewHighScoreThrobExpansion"    , m_new_high_score_throb_expansion  );
            data.Write( "NewHighScoreThrobColorR"       , m_new_high_score_throb_color.X    );
            data.Write( "NewHighScoreThrobColorG"       , m_new_high_score_throb_color.Y    );
            data.Write( "NewHighScoreThrobColorB"       , m_new_high_score_throb_color.Z    );
            data.Write( "NewHighScoreThrobColorA"       , m_new_high_score_throb_color.W    );
        }

        //=========================================================================================
        /// <summary>
        /// Update function. Updates logic for this widget.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Call the base class function

            base.OnUpdate();

            // Increase time since the screen was created:

            m_time_since_creation += Core.Timing.ElapsedTime;

            // Do high score throbbing:

            m_new_high_score_time_throb_time_elapsed += Core.Timing.ElapsedTime;

            // See if we need to switch throb:

            if ( m_new_high_score_time_throb_time_elapsed >= m_new_high_score_throb_time )
            {
                // Switch between throb up/down

                m_new_high_score_throb_up = ! m_new_high_score_throb_up;

                // Reset time elapsed

                m_new_high_score_time_throb_time_elapsed = 0;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Draws the scores for the level.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // Abort if we do not have a font:

            if ( m_font == null ) return;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Get the high scores for the current level:

            LevelHighScores.HighScoreRecord high_score = LevelHighScores.GetLevelRecords(Core.Level.FileName);

            // Get the length outwards from the the 1st,2nd,3rd labels we should draw user names at:

            float position_label_length = GetPositionLabelLength();

            // Figure out the maximum length for player names:

            float name_label_length = GetNameLabelLength(high_score);

            // Draw each of the three scores:

            DrawScore
            (
                StringDatabase.GetString( m_string_1st )    ,
                0                                           ,
                high_score.Score1                           ,
                high_score.Name1                            ,
                position_label_length                       ,
                name_label_length                           
            );

            DrawScore
            (
                StringDatabase.GetString( m_string_2nd )    ,
                1                                           ,
                high_score.Score2                           ,
                high_score.Name2                            ,
                position_label_length                       ,
                name_label_length                           
            );

            DrawScore
            (
                StringDatabase.GetString( m_string_3rd )    ,
                2                                           ,
                high_score.Score3                           ,
                high_score.Name3                            ,
                position_label_length                       ,
                name_label_length                           
            );
        }

        //=========================================================================================
        /// <summary>
        /// Gets the index of a new high score the player has just got, if any.
        /// </summary>
        /// <param name="high_scores"> Current high scores records</param>
        /// <returns> 0 if the player has obtained the new 1st high score, 1 if the 
        /// 2nd best score was got and so on. -1 is returned if no high score was got. </returns>
        //=========================================================================================

        private int GetNewHighScoreIndex( LevelHighScores.HighScoreRecord high_scores )
        {
            // Get the level rules object:

            LevelRules rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

            // If not there then abort:

            if ( rules == null ) return -1;

            // Otherwise return the first score the players score exceeds: 

            if ( rules.PlayerScore > high_scores.Score1 ) return 0;
            if ( rules.PlayerScore > high_scores.Score2 ) return 1;
            if ( rules.PlayerScore > high_scores.Score3 ) return 2;

            // Player didn't get a new high score:

            return -1;
        }

        //=========================================================================================
        /// <summary>
        /// Draws one score in the high scores table.
        /// </summary>
        /// <param name="position_label"> Label for the position. E.G 1st etc.. </param>
        /// <param name="index"> Index in the table, 0 for 1st, 1 for 2nd and so on.</param>
        /// <param name="score"> Score obtained. </param>
        /// <param name="name_label"> Name of the user who got the score. </param>
        /// <param name="position_label_length"> Amount of spacing to get from position label to name label </param>
        /// <param name="name_label_length"> Amount of spacing to get from name label to score label</param>
        //=========================================================================================

        private void DrawScore
        ( 
            string  position_label          , 
            int     index                   ,
            float   score                   ,
            string  name_label              ,
            float   position_label_length   ,
            float   name_label_length
        )
        {
            // Abort if no font:

            if ( m_font == null ) return;

            // Get the view projection matrix for the gui:

            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Calculate the time at which this score will start to become visible:

            float visible_time = index * m_score_fade_in_time;

            // If we have not reached this time then the score is completely invisible: do not draw

            if ( m_time_since_creation < visible_time ) return;

            // Right: calculate the time at which the score will be fully opaque:

            float opaque_time = visible_time + m_score_fade_in_time;

            // Calculate the opacity of the score:

            float alpha = ( m_time_since_creation - visible_time ) / m_score_fade_in_time;

            // Clamp:

            alpha = MathHelper.Clamp( alpha , 0 , 1 );

            // Calculate the y offset for this score:

            float y_offset = - ( m_font.Size + m_font.LineSpacing ) * index;

            // Make up custom font settings block for this piece of text:

            Font.CustomFontSettings settings = m_font.GetSettings();

            // Set alpha on font:

            settings.Color.W = alpha;

            // See if this is a high score: if so then change settings.. 

            if ( m_new_high_score == index )
            {
                // Figure out t value for interpolation:

                float t = m_new_high_score_time_throb_time_elapsed / m_new_high_score_throb_time;

                // New high score: do throbbing

                if ( m_new_high_score_throb_up )
                {
                    // Expand:

                    settings.Size += t * m_new_high_score_throb_expansion;

                    // Set color:

                    settings.Color = new Vector4
                    (
                        t * settings.Color.X + ( 1.0f - t ) * m_new_high_score_throb_color.X ,  
                        t * settings.Color.Y + ( 1.0f - t ) * m_new_high_score_throb_color.Y ,
                        t * settings.Color.Z + ( 1.0f - t ) * m_new_high_score_throb_color.Z ,
                        t * settings.Color.W + ( 1.0f - t ) * m_new_high_score_throb_color.W
                    );
                }
                else
                {
                    // Expand:

                    settings.Size += ( 1.0f - t ) * m_new_high_score_throb_expansion;

                    // Set color:

                    settings.Color = new Vector4
                    (
                        ( 1.0f - t ) * settings.Color.X + t * m_new_high_score_throb_color.X ,  
                        ( 1.0f - t ) * settings.Color.Y + t * m_new_high_score_throb_color.Y ,
                        ( 1.0f - t ) * settings.Color.Z + t * m_new_high_score_throb_color.Z ,
                        ( 1.0f - t ) * settings.Color.W + t * m_new_high_score_throb_color.W
                    );

                }
            }

            // Draw the position label:

            m_font.DrawCustomString
            ( 
                position_label                      , 
                Position + new Vector2(0,y_offset)  , 
                view_projection                     , 
                settings 
            );

            // Draw the name label:

            m_font.DrawCustomString
            ( 
                name_label                                              , 
                Position + new Vector2(position_label_length,y_offset)  , 
                view_projection                                         , 
                settings 
            );

            // Draw the score label:

            m_font.DrawCustomString
            ( 
                ((int)(score)).ToString()                                                       ,
                Position + new Vector2(position_label_length + name_label_length , y_offset )   , 
                view_projection                                                                 , 
                settings 
            );
        }

        //=========================================================================================
        /// <summary>
        /// Gets the length that the rendering code should move out along the x-axis from where the 
        /// position labels are being drawn to the names.
        /// </summary>
        /// <returns>Length along the x axis that the rendering code should move </returns>
        //=========================================================================================

        private float GetPositionLabelLength()
        {
            // If we have no font return 0:

            if ( m_font == null ) return 0;

            // Get all three position labels:

            string label1 = StringDatabase.GetString( m_string_1st ).Trim();
            string label2 = StringDatabase.GetString( m_string_2nd ).Trim();
            string label3 = StringDatabase.GetString( m_string_3rd ).Trim();

            // Add two spaces in between them and character names:

            label1 += "  ";
            label2 += "  ";
            label3 += "  ";

            // Get the length of them all:

            float l1 = m_font.GetStringSize( label1 ).X;
            float l2 = m_font.GetStringSize( label2 ).X;
            float l3 = m_font.GetStringSize( label3 ).X;

            // Get the longest of the lot:

            float longest = l1;

            if ( l2 > longest ) longest = l2;
            if ( l3 > longest ) longest = l3;

            // Return the longest string:

            return longest;
        }

        //=========================================================================================
        /// <summary>
        /// Gets the length that the rendering code should move out along the x-axis from where the 
        /// names are being drawn to the scores.
        /// </summary>
        /// <param name="record"> High scores record containing names for all the scores </param>
        /// <returns>Length along the x axis that the rendering code should move </returns>
        //=========================================================================================

        private float GetNameLabelLength( LevelHighScores.HighScoreRecord record )
        {
            // If we have no font return 0:

            if ( m_font == null ) return 0;

            // Get the size of all three names in the high scores: add 4 spaces to each also

            float s1 = m_font.GetStringSize( record.Name1 + "    " ).X;
            float s2 = m_font.GetStringSize( record.Name2 + "    " ).X;
            float s3 = m_font.GetStringSize( record.Name3 + "    " ).X;

            // Get the longest of the lot:

            float longest = s1;

            if ( s2 > longest ) longest = s2;
            if ( s3 > longest ) longest = s3;

            // Return the longest string:

            return longest;
        }

    }   // end of class

}   // end of namespace
