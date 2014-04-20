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
    /// This particular behaviour makes the AI try and find the end of a platform either to the 
    /// right or left when the player is above or below the AI.
    /// </summary>
    //#############################################################################################

    public class AI_Pursue_Move_Around_Platform : AIBehaviour
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Score for moving around platforms. </summary>

            private float m_score = 0;

            /// <summary> Direction to try and move around a platform. -1 is left, 1 is right. </summary>

            private int m_move_dir = 0;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> How much to smoothly decrease score if we don't think we should move around platforms. </summary>

            private const float SCORE_SMOOTH_DECREASE = 0.98f;

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

        public AI_Pursue_Move_Around_Platform( float i , Character c , AIBehaviourSet b ) : base ( i , c , b ){}

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        {
            // If we are not the current behaviour then pick a random direction to try and move to find a gap in the platform

            if ( BehaviourSet.CurrentBehaviour != this )
            {
                // Pick a random direction to move in:

                if ( ( Core.Random.Next() & 1 ) == 0 ) m_move_dir = -1; else m_move_dir = +1;
            }

            // Get the AI sight module:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            // If not there or no player then abort:

            if ( b_sight == null || b_sight.Player == null ) return 0;

            // If we are not pursuing the player then forget this beheaviour:

            if ( b_sight.PlayerInSight == false ) return 0;

            // If the player is currently in sight then begin to forget this behaviour:

            if ( b_sight.PlayerSightedLastFrame ) 
            {
                m_score *= SCORE_SMOOTH_DECREASE; return m_score * Importance;
            }

            // If we are not a sufficient distance away from the player on the y axis then begin to forget this behaviour:

            if ( Math.Abs( b_sight.Player.PositionY - ControlCharacter.PositionY ) <= 128 ) 
            {
                m_score *= SCORE_SMOOTH_DECREASE; return m_score * Importance;
            }

            // Ok: run though the raycast results and see if there is a ceiling / floor between us and the player:

            bool flat_between = false; bool wall_between = false;

                // Run through the list:

                LinkedList<IntersectQueryResult>.Enumerator e = b_sight.LastRaycastResults.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // See if this is a ceiling or floor:

                    if ( Math.Abs( e.Current.Normal.Y ) > 0.25f  )
                    {
                        // Floor / ceiling between us and the player..

                        flat_between = true;
                    }
                    else
                    {
                        // Wall between us and possibly platforms: abort search

                        wall_between = true;
                    }
                }

            // If there's any ceiling / floor between us (but no wall) and the player then begin to increase the score for this behaviour

            if ( flat_between && wall_between == false )
            {
                // Something between us: increase the score

                m_score += 1;
            }
            else
            {
                // Nothing between us: decrease the score

                m_score *= SCORE_SMOOTH_DECREASE;
            }

            // Return the current smoothed score for this behaviour:

            return m_score * Importance;
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Begin moving in the direction we are supposed to move:

            ControlCharacter.Accelerate( m_move_dir * ControlCharacter.Acceleration );

            // Do a ray cast out in front of our move direction: if we detect a wall then begin turing around

            Core.Level.Collision.Intersect
            ( 
                ControlCharacter.Position                                                                           , 
                ControlCharacter.Position + Vector2.UnitX * ControlCharacter.EllipseDimensionsX * 2 * m_move_dir    ,
                null
            );

            // Run through the results:

            for ( int i = 0 ; i < Core.Level.Collision.IntersectResultCount ; i++ )
            {
                // Grab this result:

                IntersectQueryResult result = Core.Level.Collision.IntersectResults[i];

                // See if it is a wall:

                if ( result.Normal.Y < 0.25f )
                {
                    // Wall in the direction we are moving: change direction

                    m_move_dir *= -1; break;
                }
            }

        }

    }   // end of class

}   // end of namespace
