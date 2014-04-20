using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary>
    /// This object monitors the rules for the level. It spawns enemies and powerups and ends the 
    /// level when either the player wins or looses.
    /// </summary>
    // 
    //#############################################################################################

    public class LevelRules : GameObject
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Current score for the player. </summary>
        
            public float PlayerScore { get { return m_player_score; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Combat multiplier for the player. Score increments are multiplied by this and 
            /// it is increased with each kill
            /// and decreased upon taking damage.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float PlayerCombatMultiplier { get { return m_player_combat_multiplier; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// The current number of enemies that are currently in the level
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public int ActiveEnemyCount
            {
                get
                {
                    // Store the count here:

                    int count = 0;

                    // Add all the enemy types to the count:

                    count += Core.Level.Search.GetTypeCount("EnemyNinja");

                    // Return the count:

                    return count;
                }
            }

            /// <summary> Number of enemies the player has killed for the current phase. </summary>

            public int CurrentPhaseEnemyKills { get { return m_current_phase_enemy_kills; } set { m_current_phase_enemy_kills = value; } }

            /// <summary> Returns the maximum number of active enemies for this phase. </summary>
            
            public int CurrentPhaseMaxActiveEnemies 
            {
                get
                {
                    if      ( m_current_phase <= 0 ){ return m_phase1_max_active_enemies; }
                    else if ( m_current_phase == 1 ){ return m_phase2_max_active_enemies; }
                    else if ( m_current_phase == 2 ){ return m_phase3_max_active_enemies; }
                    else if ( m_current_phase == 3 ){ return m_phase4_max_active_enemies; }
                    else                            { return m_phase5_max_active_enemies; }
                }
            }

            /// <summary> Returns the total amount of enemies the player has to kill for this phase. </summary>
            
            public int CurrentPhaseEnemyCount
            {
                get
                {
                    if      ( m_current_phase <= 0 ){ return m_phase1_enemy_count; }
                    else if ( m_current_phase == 1 ){ return m_phase2_enemy_count; }
                    else if ( m_current_phase == 2 ){ return m_phase3_enemy_count; }
                    else if ( m_current_phase == 3 ){ return m_phase4_enemy_count; }
                    else                            { return m_phase5_enemy_count; }
                }
            }

            /// <summary> Minimum time in between enemies being spawned for this phase. </summary>
            
            public float CurrentPhaseSpawnInterval
            {
                get
                {
                    if      ( m_current_phase <= 0 ){ return m_phase1_spawn_interval; }
                    else if ( m_current_phase == 1 ){ return m_phase2_spawn_interval; }
                    else if ( m_current_phase == 2 ){ return m_phase3_spawn_interval; }
                    else if ( m_current_phase == 3 ){ return m_phase4_spawn_interval; }
                    else                            { return m_phase5_spawn_interval; }
                }
            }            

            /// <summary> Amount of time since the current phase was last changed. </summary>

            public float TimeSincePhaseChange { get { return m_time_since_phase_change; } } 

            /// <summary> Current phase of enemies the player is attacking, from 0-4. </summary>

            public int CurrentPhase { get { return m_current_phase; } } 

            /// <summary> Current state of the game. </summary>

            public GameState CurrentGameState { get { return m_game_state; } }

            /// <summary> If the player has won/lost the game, this is the amount of time since the level finished. </summary>

            public float TimeSinceLevelOver { get { return m_time_since_level_over; } }

        //=========================================================================================
        // Variables
        //=========================================================================================           

            /// <summary> Number of enemies the player has killed for the current phase. </summary>

            private int m_current_phase_enemy_kills = 0;

            /// <summary> Score for the player. </summary>

            private float m_player_score = 0;

            /// <summary> Number of enemies to spawn in the level. Phase 1. </summary>

            private int m_phase1_enemy_count = 60;

            /// <summary> Number of enemies to spawn in the level. Phase 2. </summary>

            private int m_phase2_enemy_count = 60;

            /// <summary> Number of enemies to spawn in the level. Phase 3. </summary>

            private int m_phase3_enemy_count = 60;

            /// <summary> Number of enemies to spawn in the level. Phase 4. </summary>

            private int m_phase4_enemy_count = 90;

            /// <summary> Number of enemies to spawn in the level. Phase 5. </summary>

            private int m_phase5_enemy_count = 120;

            /// <summary> Minimum time in between enemies being spawned. Phase 1. </summary>

            private float m_phase1_spawn_interval = 0;

            /// <summary> Minimum time in between enemies being spawned. Phase 2. </summary>

            private float m_phase2_spawn_interval = 0;

            /// <summary> Minimum time in between enemies being spawned. Phase 3. </summary>

            private float m_phase3_spawn_interval = 0;

            /// <summary> Minimum time in between enemies being spawned. Phase 4. </summary>

            private float m_phase4_spawn_interval = 0;

            /// <summary> Minimum time in between enemies being spawned. Phase 5. </summary>

            private float m_phase5_spawn_interval = 0;

            /// <summary> Maximum number of enemies to have active at any one time in the level. Phase 1 </summary>

            private int m_phase1_max_active_enemies = 10;

            /// <summary> Maximum number of enemies to have active at any one time in the level. Phase 2 </summary>

            private int m_phase2_max_active_enemies = 10;

            /// <summary> Maximum number of enemies to have active at any one time in the level. Phase 3 </summary>

            private int m_phase3_max_active_enemies = 10;

            /// <summary> Maximum number of enemies to have active at any one time in the level. Phase 4 </summary>

            private int m_phase4_max_active_enemies = 15;

            /// <summary> Maximum number of enemies to have active at any one time in the level. Phase 5 </summary>

            private int m_phase5_max_active_enemies = 20;

            /// <summary> Time since the last enemy spawn. </summary>

            private float m_time_since_spawn = 0;

            /// <summary> Current phase of enemies the player is attacking, from 0-4. </summary>

            private int m_current_phase = 0;

            /// <summary> Type for a character object. </summary>

            private Type m_character_type = null;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Combat multiplier for the player. Score increments are multiplied by this and 
            /// it is increased with each kill
            /// and decreased upon taking damage.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private float m_player_combat_multiplier = 1;

            /// <summary> Amount of time since the current phase was last changed. </summary>

            private float m_time_since_phase_change = 0;

            /// <summary> Current state of the game. </summary>

            private GameState m_game_state = GameState.IN_PLAY;

            /// <summary> If the player has won/lost the game, this is the amount of time since the level finished. </summary>

            private float m_time_since_level_over = 0;

        //=========================================================================================
        // Enums
        //=========================================================================================  

            /// <summary> Enum representing the current state that the game is in. </summary>

            public enum GameState
            {
                /// <summary> The game is still in play. </summary>

                IN_PLAY ,

                /// <summary> The player has won this level and defeated all enemies. </summary>

                WON ,

                /// <summary> The player has lost this level. </summary>

                LOST
            };

        //=========================================================================================
        // Constants
        //=========================================================================================  

            /// <summary> Number of seconds it takes to change from one phase to another. Enemies are not spawned within this time. </summary>

            public const float PHASE_CHANGE_TIME = 4.0f;

        //=========================================================================================
        /// <summary>
        /// Constructor, creates the object.
        /// </summary>
        //=========================================================================================

        public LevelRules() : base(false,false,true,false)
        {
            // Get and save this type:

            m_character_type = Type.GetType("NinjaGame.Character");
        }

        //=========================================================================================
        /// <summary> 
        /// Resets the player's combat multiplier to 1.
        /// </summary>
        //=========================================================================================

        public void ResetCombatMultplier(){ m_player_combat_multiplier = 1; }

        //=========================================================================================
        /// <summary>
        /// Adds the given amount to the player's score. Note that every time this is done, the 
        /// player's combat multiplier goes up in score.
        /// </summary>
        /// <param name="amount"> Amount to modify the player's score by. </param>
        /// <param name="caller"> Thing that is giving the score to the player. </param>
        //=========================================================================================

        public void GivePlayerScore( float amount , GameObject caller )
        {   
            // If amount is negative then abort

            if ( amount <= 0 ) return;

            // Increase our score:

            m_player_score += amount * m_player_combat_multiplier; 

            // Don't allow score to be negative:

            if ( m_player_score < 0 ) m_player_score = 0;

            // Try and find the floating scores object:

            FloatingScores scores = (FloatingScores) Core.Level.Search.FindByType("FloatingScores");

            // If found then make a new floating score at the position of the caller:

            if ( scores != null && caller != null )
            {
                // Make new score:

                 scores.AddScore( amount * m_player_combat_multiplier , caller.Position );
            }

            // Increase combat multiplier:

            m_player_combat_multiplier++;
        }

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should read its own data from
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// read from here.
        /// </param>
        //=========================================================================================

        public override void ReadXml( XmlObjectData data )
        {
            // Call base function

            base.ReadXml(data);

            // Read all data:
     
            data.ReadInt    ( "Phase1EnemyCount"        , ref m_phase1_enemy_count          , 10    );
            data.ReadInt    ( "Phase2EnemyCount"        , ref m_phase2_enemy_count          , 25    );
            data.ReadInt    ( "Phase3EnemyCount"        , ref m_phase3_enemy_count          , 50    );
            data.ReadInt    ( "Phase4EnemyCount"        , ref m_phase4_enemy_count          , 75    );
            data.ReadInt    ( "Phase5EnemyCount"        , ref m_phase5_enemy_count          , 100   );
            data.ReadFloat  ( "Phase1SpawnInterval"     , ref m_phase1_spawn_interval       , 0.5f  );
            data.ReadFloat  ( "Phase2SpawnInterval"     , ref m_phase2_spawn_interval       , 0.5f  );
            data.ReadFloat  ( "Phase3SpawnInterval"     , ref m_phase3_spawn_interval       , 0.5f  );
            data.ReadFloat  ( "Phase4SpawnInterval"     , ref m_phase4_spawn_interval       , 0.5f  );
            data.ReadFloat  ( "Phase5SpawnInterval"     , ref m_phase5_spawn_interval       , 0.5f  );
            data.ReadInt    ( "Phase1MaxActiveEnemies"  , ref m_phase1_max_active_enemies   , 5     );
            data.ReadInt    ( "Phase2MaxActiveEnemies"  , ref m_phase2_max_active_enemies   , 8     );
            data.ReadInt    ( "Phase3MaxActiveEnemies"  , ref m_phase3_max_active_enemies   , 10    );
            data.ReadInt    ( "Phase4MaxActiveEnemies"  , ref m_phase4_max_active_enemies   , 12    );
            data.ReadInt    ( "Phase5MaxActiveEnemies"  , ref m_phase5_max_active_enemies   , 15    );
            data.ReadInt    ( "CurrentPhaseEnemyKills"  , ref m_current_phase_enemy_kills   , 0     );

            // Clamp values:

            if ( m_phase1_enemy_count < 1 ) m_phase1_enemy_count = 1;
            if ( m_phase2_enemy_count < 1 ) m_phase2_enemy_count = 1;
            if ( m_phase3_enemy_count < 1 ) m_phase3_enemy_count = 1;
            if ( m_phase4_enemy_count < 1 ) m_phase4_enemy_count = 1;
            if ( m_phase5_enemy_count < 1 ) m_phase5_enemy_count = 1;

            if ( m_current_phase_enemy_kills < 0 ) m_current_phase_enemy_kills = 0;
        }

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should write its own data to
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// written to here.
        /// </param>
        //=========================================================================================

        public override void WriteXml( XmlObjectData data )
        {
            // Call base function

            base.WriteXml(data);

            // Write all data:
     
            data.Write( "Phase1EnemyCount"          , m_phase1_enemy_count          );
            data.Write( "Phase2EnemyCount"          , m_phase2_enemy_count          );
            data.Write( "Phase3EnemyCount"          , m_phase3_enemy_count          );
            data.Write( "Phase4EnemyCount"          , m_phase4_enemy_count          );
            data.Write( "Phase5EnemyCount"          , m_phase5_enemy_count          );
            data.Write( "Phase1SpawnInterval"       , m_phase1_spawn_interval       );
            data.Write( "Phase2SpawnInterval"       , m_phase2_spawn_interval       );
            data.Write( "Phase3SpawnInterval"       , m_phase3_spawn_interval       );
            data.Write( "Phase4SpawnInterval"       , m_phase4_spawn_interval       );
            data.Write( "Phase5SpawnInterval"       , m_phase5_spawn_interval       );
            data.Write( "Phase1MaxActiveEnemies"    , m_phase1_max_active_enemies   );
            data.Write( "Phase2MaxActiveEnemies"    , m_phase2_max_active_enemies   );
            data.Write( "Phase3MaxActiveEnemies"    , m_phase3_max_active_enemies   );
            data.Write( "Phase4MaxActiveEnemies"    , m_phase4_max_active_enemies   );
            data.Write( "Phase5MaxActiveEnemies"    , m_phase5_max_active_enemies   );
            data.Write( "CurrentPhaseEnemyKills"    , m_current_phase_enemy_kills   );
        }

        //=========================================================================================
        /// <summary>
        /// Update function. Updates the level rules.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Call base function

            base.OnUpdate();

            // If the player has won or lost the game, increment the time since the level finished:

            if ( m_game_state == GameState.WON || m_game_state == GameState.LOST )
            {
                // Increment the amount of time the game has been over for:

                m_time_since_level_over += Core.Timing.ElapsedTime;
            }

            // Get the player ninja:

            PlayerNinja player = (PlayerNinja) Core.Level.Search.FindByType("PlayerNinja");

            // See if there

            if ( player != null )
            {
                // Check if the player got enough kills:

                if ( m_current_phase_enemy_kills >= CurrentPhaseEnemyCount )
                {
                    // Player got enough kills: move onto the next phase if not the last one

                    if ( m_current_phase != 4 )
                    {
                        // Move onto the next phase

                        m_current_phase++; m_time_since_phase_change = 0;

                        // Heal up the player:

                        player.Heal( player.MaximumHealth );

                        // Reset the number of kills the player has for this phase:

                        m_current_phase_enemy_kills = 0;
                    }
                    else
                    {
                        // Won the last phase: therefore the player has won this level

                        if ( m_game_state == GameState.IN_PLAY )
                        {
                            // But only if the in play state is set

                            m_game_state = GameState.WON;
                        }
                    }
                }
                else
                {
                    // Player hasn't enough kills - do spawning if we are not in a phase change

                    if ( m_time_since_phase_change > PHASE_CHANGE_TIME ) DoSpawning();
                }

                // Increase the time since the last phase change

                m_time_since_phase_change += Core.Timing.ElapsedTime;
            }
            else
            {
                // Player is not there: therefore the player has lost the game

                if ( m_game_state == GameState.IN_PLAY )
                {
                    // But only if the in play state is set

                    m_game_state = GameState.LOST;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Does enemy spawning and updates the time since the last spawn
        /// </summary>
        //=========================================================================================

        private void DoSpawning()
        {
            // See if it is time to spawn:

            if ( m_time_since_spawn >= CurrentPhaseSpawnInterval )
            {
                // Reset time since last spawn: time to spawn:

                m_time_since_spawn = 0;

                // Get the number of enemies active in the level:

                int active_enemies = ActiveEnemyCount;

                // See if there are enough enemies active as it is:

                if ( active_enemies < CurrentPhaseMaxActiveEnemies && active_enemies < CurrentPhaseEnemyCount - m_current_phase_enemy_kills )
                {
                    // Ok: we can spawn an enemy: choose a spawn point

                    EnemySpawnMarker marker = ChooseSpawnPoint();

                    // If we have a marker then spawn the enemy:

                    if ( marker != null ) SpawnEnemy(marker);
                }

            }
            else
            {
                // Not time to spawn: increase time since last spawn

                m_time_since_spawn += Core.Timing.ElapsedTime;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Chooses a suitable spawn point to spawn from.
        /// </summary>
        /// <returns> A spawn marker to spawn from, or null if no suitable point was found. </returns>
        //=========================================================================================

        private EnemySpawnMarker ChooseSpawnPoint()
        {
            // Get all the spawn points

            Dictionary<int,GameObject> spawn_points = Core.Level.Search.FindObjectsByType("EnemySpawnMarker");

            // Make a list of all the visible ones:

            LinkedList<EnemySpawnMarker> visible_spawn_markers = new LinkedList<EnemySpawnMarker>();

            // Run through the list of spawn points:

            if ( spawn_points != null )
            {
                // Get enumerator:

                Dictionary<int,GameObject>.Enumerator e = spawn_points.GetEnumerator();

                // Run through list:

                while ( e.MoveNext() )
                {
                    // Get this spawn marker:

                    EnemySpawnMarker marker = (EnemySpawnMarker) e.Current.Value;

                    // If this marker is not active for the current phase then ignore it:

                    if ( marker.MinimumPhase > m_current_phase ) continue;

                    // See if visible:

                    if ( marker.IsVisible( Core.Level.Renderer.Camera ) )
                    {
                        // Ok, it is visible: make sure no other character is overlapping it:
                        
                        Core.Level.Collision.Overlap( marker.Position , marker.BoxDimensions , marker );

                        // See if there is an overlapping character:

                        bool overlapping_character = false;

                        for ( int i = 0 ; i < Core.Level.Collision.OverlapResultCount ; i++ )
                        {
                            // See if this is a character:

                            if ( Core.Level.Collision.OverlapResults[i].QueryObject.GetType().IsSubclassOf(m_character_type) )
                            {
                                // Overlapping a character:

                                overlapping_character = true; break;
                            }
                        }

                        // See if a character is not overlaping the marker:

                        if ( overlapping_character == false ) visible_spawn_markers.AddLast(marker);
                    }
                }

                // See if there are any visible markers:

                if ( visible_spawn_markers.Count > 0 )
                {
                    // Good: pick one to spawn from:

                    int chosen_marker_index = Core.Random.Next( 0 , visible_spawn_markers.Count - 1 );

                    // Now get it:

                    LinkedList<EnemySpawnMarker>.Enumerator f = visible_spawn_markers.GetEnumerator();

                        // Move along the enumerator

                        do
                        {
                            // Move: 

                            f.MoveNext();

                            // Decrease index:

                            chosen_marker_index--;

                        } while ( chosen_marker_index > 0 );

                    // Return the marker chosen:

                    return f.Current;
                }
                else
                {
                    // No suitable markers to spawn from: return null
                    
                    return null;
                }
            }
            else
            {
                // No spawn points: return null:

                return null;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Spawns an enemy from the given spawn marker.
        /// </summary>
        /// <param name="marker"> Marker to spawn from </param>
        //=========================================================================================

        private void SpawnEnemy( EnemySpawnMarker marker )
        {
            // Abort if the marker is null:

            if ( marker == null ) return;

            // Attempt to spawn the given enemy type on the marker:

            XmlObject spawned_object = XmlFactory.CreateObject(marker.SpawnObjectType);

            // See if that worked:

            if ( spawned_object != null )
            {
                // Try and cast it to a game object:

                try
                {
                    // Do the cast:

                    GameObject g = (GameObject) spawned_object;
                
                    // Set the object's position to the marker position:

                    g.Position = marker.Position;

                    // Add it into the level:

                    Core.Level.Data.Add(g);
                }

                // On windows debug show what went wrong

                #if WINDOWS_DEBUG

                    catch ( Exception e ){ DebugConsole.PrintException(e); }

                #else

                    catch ( Exception ){}

                #endif
            }
        }

    }   // end of class

}   // end of namespace
