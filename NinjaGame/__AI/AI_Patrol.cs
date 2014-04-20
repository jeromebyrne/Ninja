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
    /// AI Behaviour. Patrols the AI back and forth along the ground.
    /// </summary>
    //#############################################################################################

    public class AI_Patrol : AIBehaviour
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================  

            /// <summary> Direction to patrol. If true the character patrols to the right. </summary>

            public bool PatrolRight { get { return m_patrol_right; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Direction to patrol. If true the character patrols to the right. </summary>

            private bool m_patrol_right = false;

            /// <summary> Current score for moving right. </summary>

            private float m_score_right = 0;

            /// <summary> Current score for moving left. </summary>

            private float m_score_left = 0;

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

        public AI_Patrol( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
        {
            // Pick a random direction to patrol in:

            m_patrol_right = ( new Random().Next() & 1 ) == 1;

            // Give either left or right a big score to start off with:

            if ( m_patrol_right )
            {
                m_score_right = 1000;
            }
            else
            {
                m_score_left = 1000;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        {
            // If we're in the air then theres no patrolling:

            if ( ControlCharacter.JumpSurface.ValidResult == false ) return 0;

            // Get the player sight behaviour

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            if ( b_sight != null )
            {
                // If the player is currently marked as visible to the AI then don't do patrols

                if ( b_sight.PlayerInSight ) return 0;
            }

            // Return a constant value for this particular behaviour otherwise

            return Importance;
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Shorten things:

            LevelCollisionQuery c = Core.Level.Collision;

            // Save the previous left and right movement scores for later interpolation:

            float prev_score_right = m_score_right;
            float prev_score_left = m_score_left;

            // Give the current movement direction a little score because we do not want to turn too suddenly:

            if ( m_patrol_right ){ m_score_right += 50; } else { m_score_left += 50; }

            // Do collision detection with the level to the right:

            c.Collide
            ( 
                new Vector2
                ( 
                    ControlCharacter.Position.X + ControlCharacter.EllipseDimensionsX * 3 , 
                    ControlCharacter.Position.Y 
                ) , 

                ControlCharacter.EllipseDimensions * 2   , 
                ControlCharacter.SurfaceRotation        , 
                null 
            );

            // Run through the results:

            for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
            {
                // See if this thing is not ground:

                if ( c.CollisionResults[i].Normal.Y < 0.35f )
                {
                    // Not ground: determine if to left or right

                    Vector2 point_r = ControlCharacter.Position - c.CollisionResults[i].Point; 

                    if ( point_r.X < - 1 )
                    {
                        // Thing is to the right of the character: use its normal to determine how much we need to turn left by

                        m_score_left += Math.Abs( c.CollisionResults[i].Normal.X ) * 350;

                        // Assign even more severity to the point if it is below the player:

                        if ( point_r.Y > ControlCharacter.Position.Y + 2 ) m_score_left += 900 * Math.Abs( c.CollisionResults[i].Normal.X );

                    }
                    else if ( point_r.X > 1 )
                    {
                        // Thing is to the right of the character: use its normal to determine how much we need to turn left by

                        m_score_right += Math.Abs( c.CollisionResults[i].Normal.X ) * 350;

                        // Assign even more severity to the point if it is below the player:

                        if ( point_r.Y > ControlCharacter.Position.Y + 2 ) m_score_right += 900 * Math.Abs( c.CollisionResults[i].Normal.X );
                    }
                }
            }

            // Do collision detection with the level to the left:

            c.Collide
            ( 
                new Vector2
                ( 
                    ControlCharacter.Position.X - ControlCharacter.EllipseDimensionsX * 3 , 
                    ControlCharacter.Position.Y 
                ) , 

                ControlCharacter.EllipseDimensions * 2   , 
                ControlCharacter.SurfaceRotation        , 
                null 
            );

            // Run through the results:

            for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
            {
                // See if this thing is not ground:

                if ( c.CollisionResults[i].Normal.Y < 0.35f )
                {
                    // Not ground: determine if to left or right

                    Vector2 point_r = ControlCharacter.Position - c.CollisionResults[i].Point; 

                    if ( point_r.X < - 1 )
                    {
                        // Thing is to the right of the character: use its normal to determine how much we need to turn left by

                        m_score_left += Math.Abs( c.CollisionResults[i].Normal.X ) * 350;

                        // Assign even more severity to the point if it is below the player:

                        if ( point_r.Y > ControlCharacter.Position.Y + 2 ) m_score_left += 900 * Math.Abs( c.CollisionResults[i].Normal.X );

                    }
                    else if ( point_r.X > 1 )
                    {
                        // Thing is to the right of the character: use its normal to determine how much we need to turn left by

                        m_score_right += Math.Abs( c.CollisionResults[i].Normal.X ) * 350;

                        // Assign even more severity to the point if it is below the player:

                        if ( point_r.Y > ControlCharacter.Position.Y + 2 ) m_score_right += 900 * Math.Abs( c.CollisionResults[i].Normal.X );
                    }
                }
            }

            // Do collision detection where the character currently stands:

            c.Collide
            ( 
                ControlCharacter.Position               ,
                ControlCharacter.EllipseDimensions * 2   , 
                ControlCharacter.SurfaceRotation        , 
                null 
            );

            // Run through the results:

            for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
            {
                // See if this thing is not ground:

                if ( c.CollisionResults[i].Normal.Y < 0.35f )
                {
                    // Not ground: determine if to left or right

                    Vector2 point_r = ControlCharacter.Position - c.CollisionResults[i].Point; 

                    if ( point_r.X < - 1 )
                    {
                        // Thing is to the right of the character: use its normal to determine how much we need to turn left by

                        m_score_left += Math.Abs( c.CollisionResults[i].Normal.X ) * 350;

                        // Assign even more severity to the point if it is below the player:

                        if ( point_r.Y > ControlCharacter.Position.Y + 2 ) m_score_left += 900 * Math.Abs( c.CollisionResults[i].Normal.X );

                    }
                    else if ( point_r.X > 1 )
                    {
                        // Thing is to the right of the character: use its normal to determine how much we need to turn left by

                        m_score_right += Math.Abs( c.CollisionResults[i].Normal.X ) * 350;

                        // Assign even more severity to the point if it is below the player:

                        if ( point_r.Y > ControlCharacter.Position.Y + 2 ) m_score_right += 900 * Math.Abs( c.CollisionResults[i].Normal.X );
                    }
                }
            }

            // Do gradual diminishing of scores:

            m_score_left  *= 0.98f;
            m_score_right *= 0.98f;

            // Interpolate current score with previous:

            float t = 0.25f;

            m_score_right   *= ( 1.0f - t );
            m_score_left    *= ( 1.0f - t );
            m_score_right   += prev_score_right * t;
            m_score_left    += prev_score_left  * t;

            // Now change patrol direction depending on scores:

            if ( m_score_right >= m_score_left )
            {
                m_patrol_right = true;
            }
            else
            {
                m_patrol_right = false;
            }

            // Accelerate at one quarter speed in the correct direction:

            if ( m_patrol_right )
            {
                ControlCharacter.Accelerate( 0.25f * ControlCharacter.Acceleration );
            }
            else
            {
                ControlCharacter.Accelerate( -0.25f * ControlCharacter.Acceleration );
            }
        }

    }   // end of class

}   // end of namespace
