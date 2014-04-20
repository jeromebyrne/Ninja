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
    /// Causes the AI to attack the player with a sword when it can.
    /// </summary>
    //#############################################################################################

    public class AI_Throw_Shuriken : AIBehaviour
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Score for throwing. Increased every frame where we sight the player. </summary>

            private float m_throw_score = 0;

            /// <summary> The System.Type for a Ninja object. Stored here for later access. </summary>

            private Type m_ninja_type = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> How much to smoothly decrease score if cannot currently attack player. 0 is instant decrease. 1 means never decrease. </summary>

            private const float SCORE_SMOOTH_DECREASE = 0.75f;

            /// <summary> Absolulute minimum distance the AI must be from the player to throw shurikens. </summary>

            private const float THROW_DISTANCE_MIN = 96.0f;

            /// <summary> Ideally the AI will be this far away (AT MINIMUM) from the player when throwing shurikens. </summary>

            private const float IDEAL_THROW_DISTANCE_MIN = 196.0f;

            /// <summary> Ideally the AI will be this far away (AT MAXIMUM) from the player when throwing shurikens. </summary>

            private const float IDEAL_THROW_DISTANCE_MAX = 384.0f;

            /// <summary> This is the absolute maximum distance to be away when throwing shurikens. </summary>

            private const float THROW_DISTANCE_MAX = 512.0f;

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

        public AI_Throw_Shuriken( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
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
                // Not a ninja: cannot throw a shuriken

                m_throw_score = 0;
            }
            else
            {
                // Cast the controln character to a ninja

                Ninja control_ninja = (Ninja) ControlCharacter;

                // Get the AI sight module:

                AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

                // If not there then we cannot attack with shurikens:

                if ( b_sight != null && b_sight.Player != null )
                {
                    // Ok: see if the player is currently in sight and that we can attack:

                    if ( b_sight.PlayerSightedLastFrame && control_ninja.CanFireShuriken )
                    {
                        // Good: get the distance to the player:

                        float dist = Vector2.Distance( b_sight.Player.Position , ControlCharacter.Position );

                        // See what range it falls into:

                        if ( dist < THROW_DISTANCE_MIN )
                        {
                            // Absolutely cannot throw shuriken: decrease throw score

                            m_throw_score *= SCORE_SMOOTH_DECREASE;
                        }
                        else if ( dist < IDEAL_THROW_DISTANCE_MIN )
                        {
                            // Add to the throw score but begin decreasing the amount we add:

                            m_throw_score += ( dist - THROW_DISTANCE_MIN ) / (IDEAL_THROW_DISTANCE_MIN - THROW_DISTANCE_MIN);
                        }
                        else if ( dist < IDEAL_THROW_DISTANCE_MAX )
                        {
                            // Within ideal range: add 1 to score

                            m_throw_score += 1;
                        }
                        else if ( dist < THROW_DISTANCE_MAX )
                        {
                            // Outside of ideal range: begin decreasing score:

                            m_throw_score += ( dist - IDEAL_THROW_DISTANCE_MAX ) / ( THROW_DISTANCE_MAX - IDEAL_THROW_DISTANCE_MAX );
                        }
                        else
                        {
                            // Absolutely cannot throw shuriken: decrease throw score

                            m_throw_score *= SCORE_SMOOTH_DECREASE;
                        }

                    }
                    else
                    {
                        // Not currently in sight or we cannot attack: start decreasing score for this behaviour

                        m_throw_score *= SCORE_SMOOTH_DECREASE;
                    }
                }
                else
                {
                    // No AI sight module: cannot throw a shuriken

                    m_throw_score = 0;
                }
            }

            // Return the current smoothed score for this behaviour:

            return m_throw_score * Importance;
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

            // If not there then we cannot attack with shurikens:

            if ( b_sight == null || b_sight.Player == null ) return;

            // Only do the throwing if we can actually hit the player:

            if ( b_sight.PlayerSightedLastFrame == false ) 
            {
                // Yield to another behaviour:

                BehaviourSet.PerformNextMostImportantBehaviour(this);

                // Abort:
                
                return;
            }

            // Ok: attack in the direction to the player
            
            control_ninja.FireShuriken
            ( 
                new Vector2
                (
                    b_sight.Player.PositionX - control_ninja.PositionX    ,
                    b_sight.Player.PositionY - control_ninja.PositionY

                )
            );

            // Reset the throw score: we threw a shuriken

            m_throw_score = 0;
        }

    }   // end of class

}   // end of namespace
