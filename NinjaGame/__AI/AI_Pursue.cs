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
    /// Pursue AI behaviour. The AI will purse the character until it gets close enough, at 
    /// which point it will stop pursuing the player.
    /// </summary>
    //#############################################################################################

    public class AI_Pursue : AIBehaviour
    {
        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Stop pursuing the player along the x axis at this distance. </summary>

            private const float PURSUE_STOP_X_DISTANCE = 48;

            /// <summary> Controls how much to reduce the importance of this behaviour as the player get's above the ai. </summary>

            private const float Y_DISTANCE_IMPORTANCE = 0.0015f;

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

        public AI_Pursue( float i , Character c , AIBehaviourSet b ) : base ( i , c , b ){}

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        { 
            // Get the sight behaviour from the behaviour set:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");
             
            // If not there then abort: we rely on sight

            if ( b_sight == null ) return 0;

            // If the sight module has not found the player then abort:

            if ( b_sight.Player == null ) return 0;

            // If we are running towards a wall then forget this behaviour:

            if ( BehaviourSet.CurrentBehaviour != null && BehaviourSet.CurrentBehaviour.Name.Equals("AI_Pursue_Wall_Run") )
            {
                return 0;
            }

            // If the player is not marked as visible then this behaviour will not be important .. 

            if ( b_sight.PlayerInSight )
            {
                // Player in sight: get the control characters current total velocity

                Vector2 c_vel = ControlCharacter.Velocity + ControlCharacter.MoveVelocity;

                // Get that velocity relative to the players:

                c_vel -= b_sight.Player.Velocity + b_sight.Player.MoveVelocity;

                // Get the x axis distance to the player/breadcrumb:

                float x_dist = b_sight.PlayerBreadcrumb.X - ControlCharacter.Position.X;

                // If we are within the pursue distance then zero it:

                if ( Math.Abs( x_dist ) <= PURSUE_STOP_X_DISTANCE )
                {
                    x_dist = 0;
                }

                // Caculate how long it will take to reach this x position:

                float time_to_x_dist = float.PositiveInfinity;

                if ( Math.Abs(c_vel.X) > 0.00001f )
                {
                    time_to_x_dist = x_dist / c_vel.X;
                }

                // Get how high above the ai the player/breacrumb is:

                float y_dist = Math.Abs( b_sight.PlayerBreadcrumb.Y - ControlCharacter.Position.Y );

                // If the distance is below then clamp it to zero:

                if ( y_dist < 0 ) y_dist = 0;

                // Aim for an eta of one second or less:

                if ( time_to_x_dist >= 0 && time_to_x_dist < 1 )
                {
                    // We're ok... reduce importance as we draw near and as the player is higher above:

                    return Importance * ( time_to_x_dist - y_dist * Y_DISTANCE_IMPORTANCE );
                }
                else if ( time_to_x_dist < 0 )
                {
                    // We're going the wrong way.. but reduce importance as player gets higher above

                    return Importance - y_dist * Y_DISTANCE_IMPORTANCE * Importance;
                }
                else
                {
                    // We need to speed up the pursuit but reduce importance as player gets higher above

                    return Importance - y_dist * Y_DISTANCE_IMPORTANCE * Importance;
                }

            }
            else
            {
                // Player not in sight: not a relevant behaviour

                return 0;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Get the sight behaviour from the behaviour set:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");
             
            // If not there then abort: we rely on sight

            if ( b_sight == null ) return;

            // If the sight module has not found the player then abort:

            if ( b_sight.Player == null ) return;

            // If the player is not marked as visible then this behaviour will not be important .. 

            if ( b_sight.PlayerInSight )
            {
                // Player in sight: get the control characters current total velocity

                Vector2 c_vel = ControlCharacter.Velocity + ControlCharacter.MoveVelocity;

                // Get that velocity relative to the players:

                c_vel -= b_sight.Player.Velocity + b_sight.Player.MoveVelocity;

                // Get the x axis distance to the player/breadrcumb:

                float x_dist = b_sight.PlayerBreadcrumb.X - ControlCharacter.Position.X;

                // If we are within the pursue distance then zero it:

                if ( Math.Abs( x_dist ) <= PURSUE_STOP_X_DISTANCE ) x_dist = 0;

                // Caculate how long it will take to reach this x position:

                float time_to_x_dist = float.PositiveInfinity;

                if ( Math.Abs(c_vel.X) > 0.00001f )
                {
                    time_to_x_dist = x_dist / c_vel.X;
                }

                // See if the player/breadcrumb is to the left or right:

                Vector2 c_pos_rel = b_sight.PlayerBreadcrumb - ControlCharacter.Position;

                // Decide on the direction we need to move in:

                float move_dir = 1; if ( c_pos_rel.X < 0 ) move_dir = -1;

                // Aim for an eta of a second second or less:

                if ( time_to_x_dist >= 0 && time_to_x_dist < 1.0f )
                {
                    // We're ok... slow down move speed as we approach

                    ControlCharacter.Accelerate( time_to_x_dist * move_dir * ControlCharacter.Acceleration );
                }
                else 
                {
                    // Do normal pursuit:

                    ControlCharacter.Accelerate( move_dir * ControlCharacter.Acceleration );
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