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
    /// AI Behaviour. Puts the AI to rest after a while of going on the trot. 
    /// </summary>
    //#############################################################################################

    public class AI_Rest : AIBehaviour
    {
        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Affects how time is randomly multiplied by when deciding when to rest, 0-1. 0.40f by default. Increased values mean more randomness. </summary>

            private float RestRandomness 
            {
                get { return m_rest_randomness; }

                set
                {
                    // Make sure this value is between 0 and 1

                    m_rest_randomness = MathHelper.Clamp(value,0,1);

                    // Redo the time multiplier for increasing rest need:

                    m_time_multiply = ( 1.0f - m_rest_randomness ) + (float) Core.Random.NextDouble() * m_rest_randomness;
                }

            }

            /// <summary> Number of seconds the AI will rest for with this behaviour. </summary>

            public float MaximumRestTime { get { return m_maximum_rest_time; } set { m_maximum_rest_time = value; } }

            /// <summary> How long the AI has been resting if in the rest state. </summary>

            public float RestTime { get { return m_rest_time; } set { m_rest_time = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Affects how time is randomly multiplied by when deciding when to rest, 0-1. 0.60f by default. Increased values mean more randomness. </summary>

            private float m_rest_randomness = 0.60f;

            /// <summary> Amount to multiply time by when increasing the need to rest. </summary>

            private float m_time_multiply = 1;

            /// <summary> Maximum amount of time the AI can rest. 2.0f by default. </summary>

            private float m_maximum_rest_time = 2.0f;

            /// <summary> Time since the AI character rested. </summary>

            private float m_time_since_rest = 0;

            /// <summary> How long the AI has been resting if in the rest state. </summary>

            private float m_rest_time = 0;

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

        public AI_Rest( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
        {
            // Redo the time multiplier for increasing rest need:

            m_time_multiply = ( 1.0f - m_rest_randomness ) + (float) Core.Random.NextDouble() * m_rest_randomness;
        }

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        { 
            // If we are in the air we can't rest:

            if ( ControlCharacter.JumpSurface.ValidResult == false ) return 0;

            // Get the ai sight module:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            if ( b_sight != null )
            {
                // If we are pursuing the player then forget about resting:

                if ( b_sight.PlayerInSight )
                {
                    m_rest_time         = 0;
                    m_time_since_rest   = 0;
                    
                    return 0;
                }
            }

            // If we are currently resting then increase the rest time:

            if ( BehaviourSet.CurrentBehaviour == this )
            {
                // Resting - increase the rest time:

                m_rest_time += Core.Timing.ElapsedTime;
            }
            else
            {
                // Not resting - see if there is still rest time counted:

                if ( m_rest_time > 0 )
                {
                    // Was resting previously: rest the time since last rest and rest time

                    m_rest_time = 0; m_time_since_rest = 0;
                }
            }

            // If the rest time is past the maximum then return 1 as a score:

            if ( m_rest_time >= m_maximum_rest_time ){ return 1 * Importance; }

            // See if we are not resting or are currently resting but have not finished the rest time:

            if ( m_rest_time > 0 )
            {
                // Resting: return the time since the last rest doubled as the importance

                return m_time_since_rest * 2 * Importance;
            }
            else
            {
                // Not resting: increase the time since the last rest. Do so according to randomly picked time mutliplier

                m_time_since_rest += Core.Timing.ElapsedTime * m_time_multiply;

                // Return the time since the last rest as the score:

                return m_time_since_rest * Importance;
            }        
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Don't move the character.. do nothing. 
        }

    }   // end of class

}   // end of namespace
