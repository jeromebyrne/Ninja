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
    /// AI Behaviour. Keeps track of whether the player is in sight or not. Causes the AI to 
    /// stop when the player is in sight if the behaviour is marked important enough.
    /// </summary>
    //#############################################################################################

    public class AI_Sight : AIBehaviour
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Amount of time the player has been in sight for. </summary>

            public float PlayerInSightTime { get { return m_in_sight_time; } }

            /// <summary> Amount of time the player has been out of sight for. </summary>

            public float PlayerOutOfSightTime { get { return m_out_of_sight_time; } }

            /// <summary> Set to true if the player has been in sight lately enough. </summary>

            public bool PlayerInSight { get { return m_in_sight; } }

            /// <summary> Set to true if the sight test past was true for the last frame only. </summary>

            public bool PlayerSightedLastFrame { get { return m_sighted_last_frame; } }

            /// <summary> The player's character that we are tracking sight of. </summary>
        
            public Character Player { get { return m_player; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Current breadcrumb the AI should follow to reach the player. If the player is visible 
            /// or a suitable breadcrumb cannot be found, this will be the player's position itself.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 PlayerBreadcrumb { get { return m_player_breadcrumb; } }

            /// <summary> A linked list holding the last set of raycast results for when we checked if the player is visible. </summary>

            public LinkedList<IntersectQueryResult> LastRaycastResults { get { return m_last_raycast_results; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Amount of time the player has been in sight for if the player is currently marked as out of sight. </summary>

            private float m_in_sight_time = 0;

            /// <summary> Amount of time the player has been out of sight for if the player is currently marked as in sight. </summary>

            private float m_out_of_sight_time = 0;

            /// <summary> Set to true if the player has been in sight lately enough. </summary>

            private bool m_in_sight = false;

            /// <summary> Set to true if the player was in sight for the last frame. </summary>
            
            private bool m_sighted_last_frame = false;

            /// <summary> The player's character that we are tracking sight of. </summary>
        
            private Character m_player = null;

            /// <summary> Save this type for later reference. </summary>

            private Type m_ninja_type = null;

            /// <summary> Set to a value on sighting / desighting and decreased afterwords </summary>

            private float m_current_score = 0;

            /// <summary> A list of player positions taken at regular intervals. The first is the most recent. </summary>

            private LinkedList<Vector2> m_player_breadcrumbs = new LinkedList<Vector2>();

            /// <summary> A linked list holding the last set of raycast results for when we checked if the player is visible. </summary>

            private LinkedList<IntersectQueryResult> m_last_raycast_results = new LinkedList<IntersectQueryResult>();

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Current breadcrumb the AI should follow to reach the player. If the player is visible 
            /// or a suitable breadcrumb cannot be found, this will be the player's position itself.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Vector2 m_player_breadcrumb = Vector2.Zero;

            /// <summary> Time since the last breadcrumb was taken. </summary>

            private float m_time_since_last_breadcrumb = BREAD_CRUMB_INTERVAL;            

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Time in seconds the player must be visible to the ai before the player is marked as visible. </summary>

            private const float SIGHT_REGISTER_TIME = 0.25f;

            /// <summary> Time in seconds the player must be out of sight before being marked as out of sight. </summary>

            private const float OUT_OF_SIGHT_TIME = 10.0f;

            /// <summary> Angle between the two planes in the AI's vision cone and a the horizontal. </summary>

            private const float VISION_CONE_ANGLE = 45;

            /// <summary> How frequently we take snapshots of the player's position. </summary>

            private const float BREAD_CRUMB_INTERVAL = 0.125f;

            /// <summary> Maximum number of breadcrumbs the AI can keep. </summary>

            private const int MAX_BREAD_CRUMB_COUNT = 128;

            /// <summary> Distance at which a bread crumb is considered reached by the AI. </summary>

            private const float BREAD_CRUMB_REACHED_DISTANCE = 160;

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

        public AI_Sight( float i , Character c , AIBehaviourSet b ) : base ( i , c , b )
        {
            // Save this for later use

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
            // Clear the last raycast results:

            m_last_raycast_results.Clear();

            // Search for the player:
      
            m_player = (PlayerNinja) Core.Level.Search.FindByType("PlayerNinja");

            // If there is no player then return 0:

            if ( m_player == null ) 
            {
                // Reset all these variables

                m_in_sight              = false;
                m_sighted_last_frame    = false;
                m_out_of_sight_time     = 1;
                m_in_sight_time         = 0;

                // This action is not relevant

                return 0;
            }

            // Update the player's breadcrumbs:

            UpdatePlayerBreadcrumbs();

            // Update which breadcrumb we should be following:

            UpdatePlayerBreadcrumb();

            // Shorten things:

            LevelCollisionQuery c = Core.Level.Collision;

            // See if the player is inside the AI's vision cone:

            bool in_vision_cone = false;

            // If we have marked the player as being recently in sight then skip the vision cone check:

            if ( m_in_sight )
            {
                in_vision_cone = true;
            }
            else
            {
                in_vision_cone = IsPlayerInVisionCone();
            }

            // Otherwise raycast to the player: don't bother if the player is not inside the AIs vision cone:

            if ( in_vision_cone )
            {
                // Do the raycast:

                c.Intersect( ControlCharacter.Position , m_player.Position , null );

                // Save all the results for later use:

                for ( int i = 0 ; i < Core.Level.Collision.IntersectResultCount ; i++ )
                {
                    m_last_raycast_results.AddLast( Core.Level.Collision.IntersectResults[i] );
                }
            }

            // Save the previous visibility of the player here:

            bool prev_in_sight = m_in_sight;

            // See if the player is visible:

            if ( in_vision_cone == false || c.IntersectResultCount > 0 )
            {
                // Player is not in sight for this frame:

                m_sighted_last_frame = false;

                // Not visible: see if the ai has marked the player as visible ...

                if ( m_in_sight )
                {
                    // Increase the amount of time the player has been out of sight for:

                    m_out_of_sight_time += Core.Timing.ElapsedTime;

                    // If the out of sight time is past the maximum out of sight time then mark player as out of sight

                    if ( m_out_of_sight_time >= OUT_OF_SIGHT_TIME )
                    {
                        // Set the in sight time to zero: but only if the ninja falls outside of the vision cone: this is to combat a situation where the ai is rapidly turning around

                        if ( in_vision_cone == true ) m_in_sight_time = 0;

                        // No longer in sight:

                        m_in_sight = false;
                    }
                }
                else
                {
                    // Increase the out of sight time:

                    m_out_of_sight_time += Core.Timing.ElapsedTime;

                    // Set the in sight time to zero: but only if the ninja falls outside of the vision cone: this is to combat a situation where the ai is rapidly turning around

                    if ( in_vision_cone == true ) m_in_sight_time = 0;
                }
            }
            else
            {
                // Player is in sight for this frame:

                m_sighted_last_frame = true;

                // Visible: see if the ai has marked the player as visible

                if ( m_in_sight )
                {
                    // In sight: increase in sight time

                    m_in_sight_time += Core.Timing.ElapsedTime;

                    // Set the out of sight time to zero:

                    m_out_of_sight_time = 0;
                }
                else
                {
                    // Not marked in sight. Increase the time we are in sight for

                    m_in_sight_time += Core.Timing.ElapsedTime;

                    // If enough time has passed then mark the player as visible again:

                    if ( m_in_sight_time >= SIGHT_REGISTER_TIME )
                    {
                        // Set the out of sight time to zero:

                        m_out_of_sight_time = 0;

                        // Mark the player as being in sight:

                        m_in_sight = true;
                    }
                }
            }

            // See if there was a change in visibility:

            if ( m_in_sight != prev_in_sight )
            {
                // Change in visibility: set our score back to one

                m_current_score = 1;
            }
            else
            {
                // No change in visibility: decrease the importance of this module
                    
                m_current_score *= 0.95f;
            }

            // Return our score:

            return m_current_score * Importance;
        }

        //=========================================================================================
        /// <summary>
        /// Tells if the player is in the AI's vision cone
        /// </summary>
        /// <returns> True if the player is inside the vision cone of the AI, false otherwise.</returns>
        //=========================================================================================

        public bool IsPlayerInVisionCone()
        {
            // Make sure the player character is valid: if not then return false

            if ( m_player == null ) return false;

            // Construct direction vector for a line in the vision cone: point it to the left initially

            Vector2 vc_vec = new Vector2
            (
                - (float) Math.Cos( VISION_CONE_ANGLE ) , 
                + (float) Math.Sin( VISION_CONE_ANGLE )
            );

            // Rotate it according to the surface orientation of the character:

            vc_vec = Vector2.Transform( vc_vec , Matrix.CreateRotationZ( -ControlCharacter.SurfaceRotation ) );

            // Now get normals for the vision cone planes:

            Vector2 vc_n1 = new Vector2( - vc_vec.Y  ,   vc_vec.X );
            Vector2 vc_n2 = new Vector2(   vc_n1.X   , - vc_n1.Y  );

            // Flip the normals horizontally if the characte is pointing the other way:

            if ( ControlCharacter.Flipped )
            {
                vc_n1.X *= -1;
                vc_n2.X *= -1;
            }

            // Now get the players position relative to the AI player

            Vector2 r = m_player.Position - ControlCharacter.Position;

            // Do a dot product with both the vision cone plane normals to see if the player is on the positive side of these planes:

            float dot1 = Vector2.Dot( vc_n1 , r );
            float dot2 = Vector2.Dot( vc_n2 , r );

            // If either dot is negative then the player is outside of the vision cone:

            if ( dot1 < 0 ) return false;
            if ( dot2 < 0 ) return false;

            // Otherwise player is inside the vision cone: return true

            return true;
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Just do the next most important behaviour:

            BehaviourSet.PerformNextMostImportantBehaviour(this);
        }

        //=========================================================================================
        /// <summary>
        /// Updates the list of player breadcrumbs. This function drops little 'breadcrumbs' of the 
        /// players position every so often into a history.
        /// </summary>
        //=========================================================================================

        private void UpdatePlayerBreadcrumbs()
        {
            // If there is no player then clear the list and abort:

            if ( m_player == null ){ m_player_breadcrumbs.Clear(); return; }

            // See if it is time to update breadcrumbs:

            if ( m_time_since_last_breadcrumb >= BREAD_CRUMB_INTERVAL )
            {
                // Time for a breadcrumb:

                m_time_since_last_breadcrumb -= BREAD_CRUMB_INTERVAL;

                // Increment time to the next breadcrumb:

                m_time_since_last_breadcrumb += Core.Timing.ElapsedTime;

                // Drop a breadcrumb:

                m_player_breadcrumbs.AddFirst( m_player.Position );

                // If we are past the maximum number of breadcrumbs then pop the last one off the list:

                if ( m_player_breadcrumbs.Count > MAX_BREAD_CRUMB_COUNT )
                {
                    m_player_breadcrumbs.RemoveLast();
                }
            }
            else
            {
                // Increment time to the next breadcrumb:

                m_time_since_last_breadcrumb += Core.Timing.ElapsedTime;
            }

            // If the player is currently visible then clear the breadcrumbs list:

            if ( m_sighted_last_frame ) m_player_breadcrumbs.Clear();
        }

        //=========================================================================================
        /// <summary>
        /// This function updates the position that the AI should move towards to follow the player. 
        /// If the player is visible to the AI (for this frame) then it will simply return the 
        /// players position. Otherwise it will oldest breadcrumb that should be followed.
        /// </summary>
        //=========================================================================================

        public void UpdatePlayerBreadcrumb()
        {
            // See if there is a player: if not then abort

            if ( m_player == null )
            {
                m_player_breadcrumb = Vector2.Zero; return;
            }

            // Ok: see if the the player can be currently sighted: if so then player breadcrumb is just player position

            if ( m_sighted_last_frame ) 
            {
                m_player_breadcrumb = m_player.Position; return;
            }

            // If there are no breadcrumbs then breadcrumb is just the player's position:

            if ( m_player_breadcrumbs.Count <= 0 )
            {
                m_player_breadcrumb = m_player.Position; return;
            }

            // Raycast to the oldest and determine if it's visible:

            Core.Level.Collision.Intersect
            (
                ControlCharacter.Position           ,
                m_player_breadcrumbs.Last.Value     ,
                null
            );

            if ( Core.Level.Collision.IntersectResultCount > 0 )
            {
                // Not visible: use the last breadcrumb as what we should move to

                m_player_breadcrumb = m_player_breadcrumbs.Last.Value;
            }
            else
            {
                // Visible: remove it from the list:

                m_player_breadcrumbs.RemoveLast();

                // If there are no more breadcrumbs left then use the player position: otherwise use the last breadcrumb

                if ( m_player_breadcrumbs.Count <= 0 )
                {
                    // No breadcrumbs: follow player position

                    m_player_breadcrumb = m_player.Position;
                }
                else
                {
                    // More breadcrumbs. Use next oldest one for now.

                    m_player_breadcrumb = m_player_breadcrumbs.Last.Value;
                }
            }   
        }

        //=========================================================================================
        /// <summary>
        /// Causes the ai to 'wake' and be aware of the player's presence. This sets the in sight
        /// flag to true and resets the time since the player was last seen. It does not do 
        /// anything other than this however.
        /// </summary>
        //=========================================================================================

        public void AlertAI()
        {
            // Alert the ai:

            m_in_sight              = true;
            m_out_of_sight_time     = 0;
            m_in_sight_time         = SIGHT_REGISTER_TIME + 0.0001f;             
        }

    }   // end of class

}   // end of namespace
