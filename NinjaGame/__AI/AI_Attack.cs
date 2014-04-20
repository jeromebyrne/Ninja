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

    public class AI_Attack : AIBehaviour
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Score for attacking. Increased by one for every frame where we can attack. </summary>

            private float m_attack_score = 0;

            /// <summary> The System.Type for a Ninja object. Stored here for later access. </summary>

            private Type m_ninja_type = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> How much to smoothly decrease score if cannot currently attack player. 0 is instant decrease. 1 means never decrease. </summary>

            private const float SCORE_SMOOTH_DECREASE = 0.95f;

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

        public AI_Attack( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
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
                // Not a ninja: cannot attack

                m_attack_score = 0;
            }
            else
            {
                // Ninja: cast to one

                Ninja control_ninja = (Ninja) ControlCharacter;

                // Get the AI sight module:

                AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

                // If not there or no player then abort:

                if ( b_sight == null || b_sight.Player == null ) return 0;

                // If we can't see the player thne 

                // See if we can attack:

                if ( control_ninja.CanAttack )
                {
                    // Good: increase the score for attacking by 1 if we can hit the player:

                    if ( CanHitPlayer() )
                    {
                        // Can hit the player: increase the score

                        m_attack_score += 1;
                    }
                    else
                    {
                        // No luck: decrease the score smoothly

                        m_attack_score *= SCORE_SMOOTH_DECREASE;
                    }
                }
                else
                {
                    // Can't attack. Smoothly decrease the score for attacking

                    m_attack_score *= SCORE_SMOOTH_DECREASE;
                }
            }

            // Return the current smoothed score for this behaviour:

            return m_attack_score * Importance;
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

            // Get the AI sight module:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            // If not there or no player then abort:

            if ( b_sight == null || b_sight.Player == null ) return;

            // Cast control character to a ninja:

            Ninja ninja = (Ninja)(ControlCharacter);

            // Attack if we can: otherwise try and reach a point where we can attack

            if ( CanHitPlayer() ) 
            {
                // Can hit the player: attack

                ninja.Attack();

                // Don't move whilst attacking:

                ControlCharacter.MoveVelocity = Vector2.Zero;
            }
            else
            {
                // Can't hit the player.. see if we are facing towards the player 

                    // Get players x position relative to ai character

                    float player_relative_x = b_sight.Player.PositionX - ControlCharacter.PositionX;

                    // Get the way we are facing:

                    float face_dir_x = -1; if ( ControlCharacter.Flipped ){ face_dir_x *= -1; }

                // Now see if we face the player:

                if ( face_dir_x * player_relative_x >= 0 )
                {
                    // We are facing player: continue moving in this direction to try and get closer to attack

                    ControlCharacter.Accelerate( ControlCharacter.Acceleration * 0.25f * face_dir_x );
                }
                else
                {
                    // We are not facing player: turn around to try and attack

                    ControlCharacter.Accelerate( ControlCharacter.Acceleration * 0.25f * - face_dir_x );
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Tells if the AI character can currently hit the player with a melee attack.
        /// </summary>
        /// <returns> True if it can, false otherwise. </returns>
        //=========================================================================================

        private bool CanHitPlayer()
        {
            // Make sure our character is a ninja:

            if ( ControlCharacter.GetType().IsSubclassOf(m_ninja_type) == false ) return false;

            // Cast control character to a ninja:

            Ninja ninja = (Ninja)(ControlCharacter);

            // Get a right vector for the player rotated by current surface rotation:

            Vector2 right = Vector2.UnitX;

                // If the character is going left then flip it:

                if ( ninja.Flipped == false ) right.X *= -1;

                // Rotate it:

                right = Vector2.Transform( right , Matrix.CreateRotationZ(ninja.SurfaceRotation) );

                // Get the true axis aligned bounding box size from the animation: the current bb for the character is distorted with rotation

                Vector2 box_dimensions = ControlCharacter.BoxDimensions;

                if ( ControlCharacter.Animation != null )
                {
                    box_dimensions = ControlCharacter.Animation.BoxDimensions;
                }

                // Increase it's size by x coordinate of box dimensions:

                right *= box_dimensions.X;

            // Make up a bounding box centered on this vector + players offset with the start and end points determining size of box

            Vector2 box_p = right * 0.5f + ninja.Position;
            Vector2 box_d = right * 0.5f;

            if ( box_d.X < 0 ) box_d.X *= -1;
            if ( box_d.Y < 0 ) box_d.Y *= -1;

            // Now do a bounding box overlap for the level:

            Core.Level.Collision.Overlap( box_p , box_d , null );

            // Run through the results:

            for ( int i = 0 ; i < Core.Level.Collision.OverlapResultCount; i++ )
            {
                // Get this object:

                GameObject game_object = Core.Level.Collision.OverlapResults[i].QueryObject;

                // See if it is a player: if so we may be able to attack

                if ( game_object.GetType().Name.Equals("PlayerNinja") ) 
                {
                    // Is a player ninja: cast to one

                    PlayerNinja player_ninja = (PlayerNinja) game_object;

                    // Ok now check precisely if we can hit when the characters bb is rotated: 

                    return player_ninja.CheckSwordHit(ControlCharacter.Position, ControlCharacter.Position + right );
                }
            }

            // Nothing to attack: return false

            return false;
        }

    }   // end of class

}   // end of namespace
