using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// When many AI characters are bunched up together this behaviour will cause the AI to try 
    /// and find some space for itself. This also moves the AI away from the player.
    /// </summary>
    //#############################################################################################

    class AI_Decluster : AIBehaviour
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Current score for moving right. </summary>

            private float m_score_right = 0;

            /// <summary> Current score for moving left. </summary>

            private float m_score_left = 0;

            /// <summary> The System.Type for a Character object. Stored here for later access. </summary>

            private Type m_character_type = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> How much to smooth scores in between frames. </summary>

            private const float SCORE_SMOOTHING = 0.6f;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the behaviour. A valid control character MUST be passed in.
        /// </summary>
        /// <param name="i"> 
        /// Importance of the behaviour to this character. Behaviour scores are scaled by this. 
        /// </param>
        /// <param name="c"> Character the behaviour is controlling. </param>
        /// <param name="b"> Behaviour set the AIBehaviour belongs to. </param>
        //=========================================================================================

        public AI_Decluster( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
        {
            // Get and save the character type:

            m_character_type = Type.GetType("NinjaGame.Character");
        }

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        {
            // Do an overlap query with the level:

            Core.Level.Collision.Overlap
            ( 
                ControlCharacter.Position       , 
                ControlCharacter.BoxDimensions  ,  
                null 
            );

            // Produce a 'current' score for left and right movement

            float l_score = 0;
            float r_score = 0;

            // Shorten code:

            LevelCollisionQuery c = Core.Level.Collision;

            // Run through the overlap results:

            for ( int i = 0 ; i < c.OverlapResultCount ; i++ )
            {               
                // Save the result: 

                OverlapQueryResult result = c.OverlapResults[i];

                // Make sure this object is a character:

                if ( result.QueryObject.GetType().IsSubclassOf(m_character_type) )
                {
                    // Cast the object to a character:

                    Character other_object = (Character) result.QueryObject;

                    // Is a character: make sure it is not our own:

                    if ( other_object != ControlCharacter )
                    {
                        // Good: now see if the overlap occured to the left or right

                        if ( result.RegionPosition.X - ControlCharacter.Position.X >= 0 )
                        {
                            // If the other character is travelling in the direction we want to move in then invert the score and set our movment velocity to zero

                            float invert = 1;

                            if ( other_object.VelocityX + other_object.MoveVelocity.X > 0 ) 
                            {
                                invert = -1;
                            }

                            // Collision occured to the right: add the area to the current right score

                            r_score += c.OverlapResults[i].RegionArea * invert;
                        }
                        else
                        {
                            // If the other character is travelling in the direction we want to move in then invert the score

                            float invert = 1;

                            if ( other_object.VelocityX + other_object.MoveVelocity.X < 0 ) 
                            {
                                invert = -1;
                            }

                            // Collision occured to the left: add the area to the current left score

                            l_score += c.OverlapResults[i].RegionArea * invert;
                        }
                    }
                }
            }

            // Smooth the current scores with the previous:

            l_score *= ( 1.0f - SCORE_SMOOTHING ); 
            r_score *= ( 1.0f - SCORE_SMOOTHING );

            m_score_left    *= SCORE_SMOOTHING;
            m_score_right   *= SCORE_SMOOTHING;

            m_score_left    += l_score;
            m_score_right   += r_score;

            // Now return the largest multiplied by the importance of this behaviour:

            if ( m_score_right >= m_score_left )
            {
                return m_score_right * Importance;
            }
            else
            {
                return m_score_left * Importance;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // See if we have to move left or right:

            if ( m_score_right >= m_score_left )
            {
                // If the right score is very small then don't bother:

                if ( m_score_right < 0.001f ) return;

                // Move right:

                ControlCharacter.Accelerate( - ControlCharacter.Acceleration * 0.25f );
            }
            else
            {
                // If the left score is very small then don't bother:

                if ( m_score_left < 0.001f ) return;

                // Move left:

                ControlCharacter.Accelerate( ControlCharacter.Acceleration * 0.25f );
            }
        }

    }   // end of class

}   // end of namespace
