using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Behaviour which makes the AI run back from walls in the attempt to gain a better position 
    /// for another wall jump. 
    /// </summary>
    //#############################################################################################

    public class AI_Pursue_Wall_Clear : AIBehaviour
    {
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

        public AI_Pursue_Wall_Clear( float i , Character c , AIBehaviourSet b ) : base ( i , c , b ){}

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        { 
            // This behaviour also only applies if the character is wall running

            if ( BehaviourSet.CurrentBehaviour == null ) return 0;

                // Make sure we are wall running or doing this:

                AIBehaviour current_behaviour = BehaviourSet.CurrentBehaviour;

                if 
                ( 
                    ( ! current_behaviour.Name.Equals("AI_Pursue_Wall_Run") )
                    &&
                    ( ! current_behaviour.Name.Equals("AI_Pursue_Wall_Clear") )
                )
                {
                    return 0;
                }

            // See if we are clearing away from the wall:

            bool wall_clearing = false;

            if ( current_behaviour.Name.Equals("AI_Pursue_Wall_Clear") )
            {
                wall_clearing = true;
            }

            // Make sure we are in contact with a wall or are clearing away from it:

            if ( wall_clearing || ControlCharacter.SteepestContactSurface.ValidResult )
            {
                if ( wall_clearing == false && ControlCharacter.SteepestContactSurface.Normal.Y >= 0.25f )
                {
                    // Only touching ground: forget about this 

                    return 0;
                }
            }
            else
            {
                // Not touching a wall: behaviour doesn't apply

                return 0;
            }

            // Get the wall run behaviour:

            AI_Pursue_Wall_Run b_wall_run = (AI_Pursue_Wall_Run) BehaviourSet.GetBehaviour("AI_Pursue_Wall_Run");

            // Abort if not there:

            if ( b_wall_run == null ) return 0;

            // Ok: so we are wall running.. get the players current total velocity

            Vector2 vel = ControlCharacter.MoveVelocity + ControlCharacter.Velocity;

            // Increase the score for this behaviour as we loose velocity down the wall

            return - vel.Y * Importance;
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Get the wall run behaviour:

            AI_Pursue_Wall_Run b_wall_run = (AI_Pursue_Wall_Run) BehaviourSet.GetBehaviour("AI_Pursue_Wall_Run");

            // Abort if not there:

            if ( b_wall_run == null ) return;

            // See which wall the behaviour has been running towards:

            if ( b_wall_run.LeftScore >= b_wall_run.RightScore )
            {
                // Running towards the left wall so run away from it:

                ControlCharacter.Accelerate( ControlCharacter.Acceleration * 0.5f );
            }
            else
            {
                // Running towards the right wall so run away from it:

                ControlCharacter.Accelerate( - ControlCharacter.Acceleration * 0.5f );
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
