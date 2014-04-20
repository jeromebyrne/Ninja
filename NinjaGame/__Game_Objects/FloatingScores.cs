using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// This object allows floating scores to be drawn in the level. Used for when an enemy is 
    /// killed.
    /// </summary>
    //#############################################################################################

    public class FloatingScores : GameObject
    {
        //=========================================================================================
        // Structs
        //=========================================================================================

            /// <summary> Represents a floating score in the game. </summary>

            private struct FloatingScore
            {
                /// <summary> Position of the score.</summary>

                public Vector2 Position;

                /// <summary> Rounded up score value. </summary>

                public int Score;

                /// <summary> How long the score has been floating for. </summary>

                public float TimeAlive;
            }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Font we use to draw all the scores with. </summary>
            
            private Font m_font = null;

            /// <summary> Name of the font used to draw all the scores with. </summary>

            private string m_font_name = "";

            /// <summary> How long scores are shown until fading into nothing. </summary>

            private float m_score_live_time = 2;

            /// <summary> How fast the scores float upwards. </summary>

            private float m_float_speed = 0.25f;

            /// <summary> List of scores currently active. </summary>

            private LinkedList<FloatingScore> m_scores = new LinkedList<FloatingScore>();

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the object.
        /// </summary>
        //=========================================================================================

        public FloatingScores() : base(true,false,true,false){}

        //=========================================================================================
        /// <summary>
        /// Visibilty test function. This object is always visible.
        /// </summary>
        //=========================================================================================

        public override bool IsVisible(Camera c) { return true; }

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
            // Call base function

            base.ReadXml(data);

            // Read all attributes

            data.ReadString( "Font"            , ref m_font_name       , "Content\\Fonts\\Game_16px.xml"    );
            data.ReadFloat ( "FloatSpeed"      , ref m_float_speed     , 0.25f                              );
            data.ReadFloat ( "ScoreLiveTime"   , ref m_score_live_time , 2.0f                               );

            // Load the given font:

            m_font = new Font(m_font_name);
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
            // Call base function

            base.WriteXml(data);

            // Write all attributes

            data.Write( "Font"            , m_font_name       );
            data.Write( "FloatSpeed"      , m_float_speed     );
            data.Write( "ScoreLiveTime"   , m_score_live_time );
        }

        //=========================================================================================
        /// <summary>
        /// Adds a floating score into the level at the given position.
        /// </summary>
        /// <param name="score"> Score amount to add </param>
        /// <param name="position"> Position to add it at </param>
        //=========================================================================================

        public void AddScore( float score , Vector2 position )
        {
            // Make a new score

            FloatingScore s = new FloatingScore();

            s.TimeAlive = 0;
            s.Score     = (int) Math.Ceiling(score);
            s.Position  = position;

            // Add it into the list

            m_scores.AddLast(s);
        }

        //=========================================================================================
        /// <summary>
        /// Update function. Updates all the scores that are floating.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // If no scores to update then abort:

            if ( m_scores.Count <= 0 ) return;

            // Run through all the scores:

            LinkedListNode<FloatingScore> node = m_scores.First;

                // Run through the list

                while ( node != null )
                {
                    // Get the node's value:

                    FloatingScore s = node.Value;

                    // Move upwards:

                    s.Position += Vector2.UnitY * m_float_speed;

                    // Increase time alive:

                    s.TimeAlive += Core.Timing.ElapsedTime;

                    // Set the new value for the node

                    node.Value = s;

                    // If past the time to live then kill:

                    if ( node.Value.TimeAlive > m_score_live_time )
                    {
                        // Get the next node:

                        LinkedListNode<FloatingScore> next = node.Next;
 
                        // Remove the current node:

                        m_scores.Remove(node);

                        // Set next node as the current:

                        node = next;
                    }
                    else
                    {
                        // Just move on in the list as normal

                        node = node.Next;
                    }
                }

        }

        //=========================================================================================
        /// <summary>
        /// Draw function. Draws all the floating scores.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // If we have no font then abort:

            if ( m_font == null ) return;

            // Make a custom font settings struct for drawing:

            Font.CustomFontSettings settings = new Font.CustomFontSettings
            (
                m_font.Size         ,
                m_font.Color        ,
                m_font.CharSpacing  ,
                m_font.LineSpacing  ,
                m_font.Effect
            );

            // Get the view projection matrix of the camera:

            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Run through all the nodes:

            LinkedList<FloatingScore>.Enumerator e = m_scores.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Get the score:

                    FloatingScore s = e.Current;

                    // Calculate the alpha value we will give this score depending on. fade it out as it lives longer.

                    float a = 1.0f - ( s.TimeAlive / m_score_live_time );

                    // Set the alpha for the font:

                    settings.Color.W = a;

                    // Good: now draw the score in it's position:

                    m_font.DrawCustomStringCentered
                    (
                        s.Score.ToString()  ,
                        s.Position          ,
                        view_projection     ,
                        settings                                    
                    );
                }
        }

    }   // end of class

}   // end of namespace