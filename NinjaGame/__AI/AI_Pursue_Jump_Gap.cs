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
    /// Behaviour which makes the AI jump over a gap ahead when pursuing the player.
    /// </summary>
    //#############################################################################################

    public class AI_Pursue_Jump_Gap : AIBehaviour
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Type of a ninja. Stored here for later access. </summary>

            private Type m_ninja_type = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Distance to cast a ray out when checking for ground. </summary>

            private const float RAY_CAST_DISTANCE = 128;

            /// <summary> Angle against the horizontal to cast the ray down at. </summary>

            private const float RAY_CAST_ANGLE = -1.45f;

            /// <summary> Minimum y value the ground normal must have for it to be considered ground </summary>

            private const float MIN_GROUND_NORMAL_Y = 0.35f;

            /// <summary> Y distance above the ai the player must be for the ai to do a big gap jump </summary>

            private const float BIG_JUMP_PLAYER_Y_DIST = 16;

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

        public AI_Pursue_Jump_Gap( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
        {
            // Get the ninja type:

            m_ninja_type = Type.GetType("NinjaGame.Ninja");
        }

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        { 
            // Get the current behaviour:

            AIBehaviour current_behaviour = BehaviourSet.CurrentBehaviour;

            // If the behaviour is not the pursue/wall run behaviour then abort: not a relevant action

            if 
            ( 
                ( ! current_behaviour.Name.Equals("AI_Pursue") )
                &&
                ( ! current_behaviour.Name.Equals("AI_Pursue_Wall_Run") )
            ) 
            {
                // Action not relevant

                return 0;
            }

            // Otherwise see if we are moving left or right:

            bool moving_right = true;

            if ( ControlCharacter.MoveVelocity.X < 0 ) 
            {
                moving_right = false;
            }

            // Now figure out the direction to cast a ray out in to check for ground:

            Vector2 cast_dir = Vector2.Transform( Vector2.UnitX , Matrix.CreateRotationZ(RAY_CAST_ANGLE) );

            // If the character is not moving right then flip the cast direction horizontally:

            if ( moving_right == false ) cast_dir.X = - cast_dir.X;

            // Do the raycast:

            Core.Level.Collision.Intersect
            (
                ControlCharacter.Position                                   ,
                ControlCharacter.Position + cast_dir * RAY_CAST_DISTANCE    ,
                null
            );

            // Get the closest thing hit:

            IntersectQueryResult ground_hit = Core.Level.Collision.GetClosestIntersect();

            // If nothing was hit then we should jump

            if ( ground_hit.ValidResult == false ) return Importance;

            // If something was hit then determine if it is ground or not:

            if ( ground_hit.Normal.Y >= MIN_GROUND_NORMAL_Y ) 
            {
                // Ground: we're ok

                return 0;
            }
            else
            {
                // Not ground: should jump. But only if we're on a ground surface

                if ( ControlCharacter.JumpSurface.ValidResult && ControlCharacter.JumpSurface.Normal.Y >= MIN_GROUND_NORMAL_Y )
                {
                    return Importance;
                }
                else
                {
                    return 0;
                }
            }                               
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Make sure the character we're controlling is a ninja

            if ( ControlCharacter.GetType().IsSubclassOf( m_ninja_type ) )
            {
                // Cast to ninja:

                Ninja ninja = (Ninja) ControlCharacter;

                // Get the sight module:

                AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

                // Make sure it's there and there is a player:

                if ( b_sight != null && b_sight.Player != null )
                {
                    // See if the playe/breadcrumb is above or below us: if below do not jump

                    if ( b_sight.PlayerBreadcrumb.Y - ControlCharacter.Position.Y >= BIG_JUMP_PLAYER_Y_DIST )
                    {
                        // Player/breadcrumb is above us: jump at full force

                        ninja.Jump( ninja.JumpForce );
                    }
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Does debug drawing for this behaviour.
        /// </summary>
        //=========================================================================================

        #if DEBUG

            public override void OnDebugDraw()
            {
                // Call base function

                base.OnDebugDraw();

                // Get the sight behaviour from the behaviour set:

                AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");
                 
                // If not there then abort: we rely on sight

                if ( b_sight == null ) return;

                // Draw a line from the ai to the current bread crumb:

                DebugDrawing.DrawWorldLine
                (
                    ControlCharacter.Position   ,
                    b_sight.PlayerBreadcrumb    , 
                    Color.Magenta
                );
            }

        #endif  // #if DEBUG

    }   // end of class

}   // end of namespace
