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
    /// Behaviour which makes the AI do a wall run when in pursuit mode. 
    /// </summary>
    //#############################################################################################

    public class AI_Pursue_Wall_Run : AIBehaviour
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Last wall we found to the left when doing the ScoreBehaviour() routine. </summary>

            public IntersectQueryResult LastLeftWall { get { return m_last_left_wall; } } 

            /// <summary> Last wall we found to the right when doing the ScoreBehaviour() routine. </summary>
        
            public IntersectQueryResult LastRightWall { get { return m_last_right_wall; } }

            /// <summary> Current score for doing a wall run to the left </summary>

            public float RightScore { get { return m_score_right; } }

            /// <summary> Current score for doing a wall run to the right </summary>

            public float LeftScore { get { return m_score_left; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Current score for doing a wall run to the left </summary>

            private float m_score_right = 0;

            /// <summary> Current score for doing a wall run to the right </summary>

            private float m_score_left = 0;
            
            /// <summary> Last wall we found to the left when doing the ScoreBehaviour() routine. </summary>

            private IntersectQueryResult m_last_left_wall = IntersectQueryResult.NoResult;

            /// <summary> Last wall we found to the right when doing the ScoreBehaviour() routine. </summary>
        
            private IntersectQueryResult m_last_right_wall = IntersectQueryResult.NoResult;

            /// <summary> Type of the Ninja object. Used for later reference. </summary>

            private Type m_ninja_type = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> How much to smooth scores in between frames. </summary>

            private const float SCORE_SMOOTHING = 0.90f;

            /// <summary> Distance the AI casts rays out to left and right in search of walls. </summary>

            private const float RAY_CAST_DISTANCE = 512;

            /// <summary> How important the players height above the AI is when deciding to wall run. </summary>

            private const float PLAYER_HEIGHT_IMPORTANCE = 0.25f;

            /// <summary> How important the distance the AI is away from the wall when choosing a wall to run up. </summary>

            private const float AI_WALL_DISTANCE_IMPORTANCE = 50.0f;
            
            /// <summary> Maximum Y value a wall normal can have for it to be regarded as a wall by the AI </summary>

            private const float WALL_MAX_NORMAL_Y = 0.25f;

            /// <summary> Minimum Y value a wall normal can have for it to be regarded as a wall by the AI </summary>

            private const float WALL_MIN_NORMAL_Y = - 0.25f;

            /// <summary> Minimum distance the AI must be from the wall to jump up onto it. </summary>

            private const float JUMP_MIN_DIST = 40;

            /// <summary> Maximum distance the AI can be from the wall to jump up onto it. </summary>

            private const float JUMP_MAX_DIST = 128;

            /// <summary> Amount to increase the score for moving in a direction when an AI is jumping towards a wall </summary>

            private const float JUMP_SCORE_INCREASE = 75000;

            /// <summary> The minimum height above the AI the player must be for the AI to consider a wall run. </summary>

            private const float PLAYER_MIN_RELATIVE_Y = 128;

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

        public AI_Pursue_Wall_Run( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
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

            // Otherwise see if the player is currenly marked as visible to the ai:

            if ( b_sight.PlayerInSight )
            {
                // Store the previous left and right scores here:

                float prev_score_left  = m_score_left;
                float prev_score_right = m_score_right;

                // Player is in sight: see how high above the ai character the player/breacrumb is

                float player_y_relative = b_sight.PlayerBreadcrumb.Y - ControlCharacter.Position.Y;

                // If the player is not above enough then forget this behaviour:

                if ( player_y_relative > PLAYER_MIN_RELATIVE_Y )
                {
                    // Ok.. player is above. Cast two rays to the left and right of the AI to look for walls

                    Core.Level.Collision.Intersect
                    (
                        ControlCharacter.Position                                       ,
                        ControlCharacter.Position - Vector2.UnitX * RAY_CAST_DISTANCE   ,
                        null
                    );

                    IntersectQueryResult intersect_l = Core.Level.Collision.GetClosestIntersect();

                    Core.Level.Collision.Intersect
                    (
                        ControlCharacter.Position                                       ,
                        ControlCharacter.Position + Vector2.UnitX * RAY_CAST_DISTANCE   ,
                        null
                    );

                    IntersectQueryResult intersect_r = Core.Level.Collision.GetClosestIntersect();

                    // See if there is a wall to the left:

                    if ( intersect_l.ValidResult && intersect_l.Normal.Y <= WALL_MAX_NORMAL_Y && intersect_l.Normal.Y >= WALL_MIN_NORMAL_Y )
                    {
                        // Wall to the left.. get our distance to the wall and the player/breacrumb distance 

                        float c_dist = Vector2.Dot( ControlCharacter.Position - intersect_l.Point , intersect_l.Normal );
                        float p_dist = Vector2.Dot( b_sight.PlayerBreadcrumb  - intersect_l.Point , intersect_l.Normal );

                        // Make sure we are not behind that wall:

                        if ( c_dist >= 0 )
                        {
                            // Increase score by how close the player/breadcrumb is to that wall and how high up it is:

                            m_score_left += ( RAY_CAST_DISTANCE - p_dist ) * player_y_relative * PLAYER_HEIGHT_IMPORTANCE;

                            // Decrease the score by how far away we are from the wall

                            m_score_left -= c_dist * AI_WALL_DISTANCE_IMPORTANCE;

                            // Save this wall as the last left wall:

                            m_last_left_wall = intersect_l;
                        }
                        else
                        {
                            // Not behind wall: set left score to zero

                            m_score_left = 0;

                            // No last left wall:

                            m_last_left_wall.ValidResult = false;
                        }
                    }
                    else
                    {
                        // No wall to the left: set left score to zero

                        m_score_left = 0;

                        // No last left wall:

                        m_last_left_wall.ValidResult = false;
                    }

                    // See if there is a wall to the right:

                    if ( intersect_r.ValidResult && intersect_r.Normal.Y <= WALL_MAX_NORMAL_Y && intersect_r.Normal.Y >= WALL_MIN_NORMAL_Y )
                    {
                        // Wall to the left.. get our distance to the wall and the player/breadcrumb distance 

                        float c_dist = Vector2.Dot( ControlCharacter.Position - intersect_r.Point , intersect_r.Normal );
                        float p_dist = Vector2.Dot( b_sight.PlayerBreadcrumb  - intersect_r.Point , intersect_r.Normal );

                        // Make sure we are behind that wall:

                        if ( c_dist >= 0 )
                        {
                            // Increase score to the right by how close we are to the wall and how high up the player/breadcrumb is

                            m_score_right += ( RAY_CAST_DISTANCE - p_dist ) * player_y_relative * PLAYER_HEIGHT_IMPORTANCE;

                            // Decrease the score by how far away we are from the wall

                            m_score_right -= c_dist * AI_WALL_DISTANCE_IMPORTANCE;

                            // Save this wall as the last right wall:

                            m_last_right_wall = intersect_r;
                        }
                        else
                        {
                            // No wall to the right: set left score to zero

                            m_score_right = 0;

                            // No last right wall:

                            m_last_right_wall.ValidResult = false;
                        }
                    }
                    else
                    {
                        // No wall to the right: set right score to zero

                        m_score_right = 0;

                        // No last right wall:

                        m_last_right_wall.ValidResult = false;
                    }

                }
                else
                {
                    // Player/breadcrumb is not above the ai. forget the behaviour

                    m_score_right = 0;
                    m_score_left  = 0;

                    // No last left and right walls:

                    m_last_left_wall.ValidResult    = false;
                    m_last_right_wall.ValidResult   = false;
                }

                // Interploate the current left and right scores with the previous

                m_score_left  *= ( 1.0f - SCORE_SMOOTHING );
                m_score_right *= ( 1.0f - SCORE_SMOOTHING );

                m_score_left  += prev_score_left  * SCORE_SMOOTHING;
                m_score_right += prev_score_right * SCORE_SMOOTHING;

                // Return the largest wall run score:

                if ( m_score_left > m_score_right ) return m_score_left * Importance; else return m_score_right * Importance;
            }
            else
            {
                // Player is not in sight: this behaviour does not apply

                m_last_left_wall.ValidResult    = false;
                m_last_right_wall.ValidResult   = false;
                
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
            // Run towards the wall we have chosen:

            if ( m_score_left > m_score_right )
            {
                // Only do if there is a valid wall to accelerate towards:

                if ( m_last_left_wall.ValidResult )
                {
                    // Accelerate towards the wall

                    ControlCharacter.Accelerate( - ControlCharacter.Acceleration );

                    // See if we are on a valid ground surface to jump off of

                    if ( ControlCharacter.JumpSurface.ValidResult && ControlCharacter.JumpSurface.ResolveDirection.Y >= WALL_MAX_NORMAL_Y )
                    {
                        // Good: now make sure the character we are controlling is a ninja

                        if ( ControlCharacter.GetType().IsSubclassOf(m_ninja_type) )
                        {
                            // Cast the character to a ninja

                            Ninja control_ninja = (Ninja) ControlCharacter;

                            // Get our distance to the wall:

                            float dist = Vector2.Dot( m_last_left_wall.Normal , control_ninja.Position - m_last_left_wall.Point );

                            // Make sure we are within the right distance to jump:

                            if ( dist >= JUMP_MIN_DIST && dist <= JUMP_MAX_DIST )
                            {
                                // Make sure we will jump the right way, towards the surface:

                                if ( Vector2.Dot( ControlCharacter.JumpSurface.ResolveDirection , m_last_left_wall.Normal ) <= 0.1f )
                                {
                                    // Do the jump: 

                                    control_ninja.Jump(control_ninja.JumpForce);

                                    // Increase the score for this wall so we keep moving towards it:

                                    m_score_left += JUMP_SCORE_INCREASE;
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                // Only do if there is a valid wall to accelerate towards:

                if ( m_last_right_wall.ValidResult )
                {
                    // Accelerate towards the wall

                    ControlCharacter.Accelerate( ControlCharacter.Acceleration );

                    // See if we are on a valid ground surface to jump off of

                    if ( ControlCharacter.JumpSurface.ValidResult && ControlCharacter.JumpSurface.ResolveDirection.Y >= WALL_MAX_NORMAL_Y )
                    {
                        // Good: now make sure the character we are controlling is a ninja

                        if ( ControlCharacter.GetType().IsSubclassOf(m_ninja_type) )
                        {
                            // Cast the character to a ninja

                            Ninja control_ninja = (Ninja) ControlCharacter;

                            // Get our distance to the wall:

                            float dist = Vector2.Dot( m_last_right_wall.Normal , control_ninja.Position - m_last_right_wall.Point );

                            // Make sure we are within the right distance to jump: 

                            if ( dist >= JUMP_MIN_DIST && dist <= JUMP_MAX_DIST )
                            {
                                // Get the direction the player will jump in:

                                Vector2 jump_dir = Vector2.UnitY;

                                // Make sure we will jump the right way, towards the surface:

                                if ( Vector2.Dot( ControlCharacter.JumpSurface.ResolveDirection , m_last_right_wall.Normal ) <= 0.1f )
                                {
                                    // Do the jump: 

                                    control_ninja.Jump(control_ninja.JumpForce);

                                    // Increase the score for this wall so we keep moving towards it:

                                    m_score_right += JUMP_SCORE_INCREASE;
                                }
                            }
                        }
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
