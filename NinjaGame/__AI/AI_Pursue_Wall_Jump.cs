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
    /// Behaviour which makes the AI do a wall jump when pursuing up a wall. 
    /// </summary>
    //#############################################################################################

    public class AI_Pursue_Wall_Jump : AIBehaviour
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Type of the Ninja object. Used for later reference. </summary>

            private Type m_ninja_type = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Maximum y value a wall can have for us to jump off it. </summary>

            private const float WALL_MAX_NORMAL_Y = 0.25f;

            /// <summary> Minimum y value a wall can have for us to jump off it. </summary>

            private const float WALL_MIN_NORMAL_Y = - 0.25f;

            /// <summary> When we can't see the player, this is the angle against the wall to cast downwards against when checking for ground beneath. </summary>

            private const float RAY_CAST_ANGLE = 0.8f;

            /// <summary> Length to raycast downwards to see if there's a surface beneath to jump onto when we can't see the player directly. </summary>

            private const float RAY_CAST_LENGTH = 512.0f;

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

        public AI_Pursue_Wall_Jump( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
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
            // Get the AI sight module:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            // If not there abort: 

            if ( b_sight == null ) return 0;

            // If there is no player then abort:

            if ( b_sight.Player == null ) return 0;

            // Get the current behaviour: if we are not wall running then do not consider this behaviour:

            AIBehaviour current_behaviour = BehaviourSet.CurrentBehaviour;

            if ( current_behaviour.Name == "AI_Pursue_Wall_Run" )
            {
                // Wall running: make sure we have a wall surface to jump off

                if ( ControlCharacter.JumpSurface.ResolveDirection.Y <= WALL_MAX_NORMAL_Y && ControlCharacter.JumpSurface.ResolveDirection.Y >= WALL_MIN_NORMAL_Y )
                {
                    // Get the direction to the player:

                    Vector2 to_player = b_sight.PlayerBreadcrumb - ControlCharacter.Position;

                    // Make sure we are jumping the right way:

                    if ( Vector2.Dot( to_player , ControlCharacter.JumpSurface.ResolveDirection ) < 0 ) return 0;

                    // See if the player is currently visible to us:

                    if ( b_sight.PlayerSightedLastFrame )
                    {
                        // Player is visible. Ok: we can jump off this wall. Get the characters current y velocity

                        float y_vel = ControlCharacter.Velocity.Y + ControlCharacter.MoveVelocity.Y;

                        // Get our relative y position to the player/breadcrumb:

                        float y_pos_rel = ControlCharacter.Position.Y - b_sight.PlayerBreadcrumb.Y;

                        // Get the direction of the characters up vector rotated

                        Vector2 up_dir = Vector2.Transform( Vector2.UnitY , Matrix.CreateRotationZ( ControlCharacter.SurfaceRotation ) );

                        // Get how straight we are to the wall:

                        float straightness = Vector2.Dot( up_dir , ControlCharacter.JumpSurface.ResolveDirection );

                        // Increase the importance of this action as our y velocity decreases and as we rise above the player/breadcrumb. Also by how straight we are to wall

                        return - y_vel * y_pos_rel * Importance * straightness;
                    }
                    else
                    {
                        // Player not sighted the last frame: decide on a direction to cast down:

                        Vector2 cast_dir = ControlCharacter.JumpSurface.ResolveDirection;

                        // See if the wall normal points left or right:

                        if ( cast_dir.X < 0 )
                        {
                            // Rotate the normal by the cast dir:

                            cast_dir = Vector2.Transform( cast_dir , Matrix.CreateRotationZ( RAY_CAST_ANGLE ) );
                        }
                        else
                        {
                            // Rotate the normal by the cast dir:

                            cast_dir = Vector2.Transform( cast_dir , Matrix.CreateRotationZ( -RAY_CAST_ANGLE ) );
                        }

                        // Do the raycast: 

                        Core.Level.Collision.Intersect
                        (
                            ControlCharacter.Position                               ,
                            ControlCharacter.Position + cast_dir * RAY_CAST_LENGTH  ,
                            null
                        );

                        // Get the closest intersect:

                        IntersectQueryResult closest_hit = Core.Level.Collision.GetClosestIntersect();

                        // Make sure it is ground:

                        if ( closest_hit.ValidResult && closest_hit.Normal.Y >= WALL_MAX_NORMAL_Y )
                        {
                            // There is a ground beneath us beside the wall. Ok: we can jump off this wall. Get the characters current y velocity

                            float y_vel = ControlCharacter.Velocity.Y + ControlCharacter.MoveVelocity.Y;

                            // Get our relative y position to the hit point:

                            float y_pos_rel = ControlCharacter.Position.Y - closest_hit.Point.Y;

                            // Get the direction of the characters up vector rotated

                            Vector2 up_dir = Vector2.Transform( Vector2.UnitY , Matrix.CreateRotationZ( ControlCharacter.SurfaceRotation ) );

                            // Hit the ground: we should jump. Get how straight we are to the wall:

                            float straightness = Vector2.Dot( up_dir , ControlCharacter.JumpSurface.ResolveDirection );

                            // Increase the importance of this action as our y velocity decreases and as we rise above the hit point. Also by how straight we are to wall

                            return - y_vel * y_pos_rel * Importance * straightness;
                        }
                        else
                        {
                            // Not ground: do not jump

                            return 0;
                        }
                    }

                }
                else
                {
                    // No wall to jump off - forget about it:

                    return 0;
                }
            }
            else
            {
                // Not wall running: forget about this

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
            // Make sure the control character is a ninja:

            Ninja control_ninja = null;

            if ( ControlCharacter.GetType().IsSubclassOf(m_ninja_type) )
            {
                // Controlling a ninja - save

                control_ninja = (Ninja) ControlCharacter;
            }
            else
            {
                // Not controlling a ninja - abort

                return;
            }

            // Jump !!!

            control_ninja.Jump( control_ninja.JumpForce );
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
