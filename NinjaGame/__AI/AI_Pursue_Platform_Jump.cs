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
    /// Causes the AI to jump up onto a platform if the player is above. This behaviour is only 
    /// done if there are no ceilings in the way and if the AI cannot see the player.
    /// </summary>
    //#############################################################################################

    public class AI_Pursue_Platform_Jump : AIBehaviour
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Score for jumping up. Increased every frame where we do not see the player and where there is a platform above. </summary>

            private float m_score = 0;

            /// <summary> The System.Type for a Ninja object. Stored here for later access. </summary>

            private Type m_ninja_type = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> How much to smoothly decrease score if cannot currently attack player. 0 is instant decrease. 1 means never decrease. </summary>

            private const float SCORE_SMOOTH_DECREASE = 0.55f;

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

        public AI_Pursue_Platform_Jump( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
        {
            // Get and save this character types:

            m_ninja_type = Type.GetType("NinjaGame.Ninja");

            // Complain if neither can be found:

            #if DEBUG

                if ( m_ninja_type == null ) throw new Exception("No ninja type exists");

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        {
            // Make sure our character is a ninja: 

            if ( ControlCharacter.GetType().IsSubclassOf(m_ninja_type) == false ) 
            {
                // Not a ninja: cannot jump up

                m_score = 0;
            }
            else
            {
                // Cast the control character to a ninja

                Ninja control_ninja = (Ninja) ControlCharacter;

                // Get the AI sight module:

                AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

                // If this does not exist then return 0:

                if ( b_sight == null ) 
                {
                    m_score *= SCORE_SMOOTH_DECREASE; return m_score * Importance;
                }

                // Forget this behaviour if we are not pursuing the player:

                if ( b_sight.PlayerInSight == false ) 
                {
                    m_score *= SCORE_SMOOTH_DECREASE; return m_score * Importance;
                }

                // If we can see the player there is no need for this behaviour:

                if ( b_sight.PlayerSightedLastFrame )
                {
                    m_score *= SCORE_SMOOTH_DECREASE; return m_score * Importance;
                }

                // If we are in the air then this behaviour does not matter:

                if ( control_ninja.JumpSurface.ValidResult == false )
                {
                    m_score *= SCORE_SMOOTH_DECREASE; return m_score * Importance;
                }

                // Get the direction to the player: if the player is below us or we are nearing the players y position then don't jump

                if ( b_sight.Player.PositionY - ControlCharacter.PositionY <= 128 )
                {
                    m_score *= SCORE_SMOOTH_DECREASE; return m_score * Importance;
                }

                // Ok: we cannot see the player. See if there is a floor between us but no ceiling

                bool floor_between      = false;
                bool ceiling_between    = false;

                    // Run through all the raycast results:
                    
                    LinkedList<IntersectQueryResult>.Enumerator e = b_sight.LastRaycastResults.GetEnumerator();

                    while ( e.MoveNext() )
                    {
                        // Get this result:

                        IntersectQueryResult result = e.Current;

                        // See if it is a celing:

                        if ( result.Normal.Y <= - 0.25f )
                        {
                            // Ceiling. Abort search. We cannot jump.

                            ceiling_between = true; break;
                        }
                        else if ( result.Normal.Y >= 0.25f )
                        {
                            // Floor:

                            floor_between = true;
                        }
                    }

                // Right if there is a floor between us but no ceiling then we can jump up:

                if ( ceiling_between == false && floor_between == true )
                {
                    // Can jump: increase our score

                    m_score += 1;
                }
                else
                {
                    // Can't jump. Reduce our score

                    m_score *= SCORE_SMOOTH_DECREASE;
                }
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
            // Make sure our character is a ninja: 

            if ( ControlCharacter.GetType().IsSubclassOf(m_ninja_type) == false ) return;

            // Cast to ninja:

            Ninja control_ninja = (Ninja) ControlCharacter;

            // Get the AI sight module:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            // If not there then we cannot jump up:

            if ( b_sight == null ) return;

            // Ok: see if we can jump.. if not then let another behaviour take over

            if ( control_ninja.JumpSurface.ValidResult )
            {
                // Can jump: do it

                control_ninja.Jump( control_ninja.JumpForce );
            }
            else
            {
                // Can't jump: let another behaviour take over

                BehaviourSet.PerformNextMostImportantBehaviour(this);
            }
        }

    }   // end of class

}   // end of namespace
