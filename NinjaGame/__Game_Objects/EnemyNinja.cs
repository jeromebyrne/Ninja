using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class representing an enemy controlled ninja. This ninja is controlled by AI. 
    /// </summary>
    //#############################################################################################

    public class EnemyNinja : Ninja
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> A behaviour set for this AI character. Contains all of the behaviours the character can have. </summary>

            private AIBehaviourSet m_behaviours = null;

            /// <summary> Type for a player ninja. Stored here for quick access. </summary>

            private Type m_player_ninja_type = null;

            /// <summary> Cached animations for all ninjas in the level. </summary>

            private static Animation[] s_cached_animations = null;

            /// <summary> Cached shuriken texture for the ninja. </summary>

            private static Texture2D s_cached_shuriken_texture = null;

        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Score given to the player for killing this enemy. </summary>

            private const float SCORE_VALUE = 1000.0f;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the Object.
        /// </summary>
        //=========================================================================================

        public EnemyNinja()
        {
            // Save this type:

            m_player_ninja_type = Type.GetType("NinjaGame.PlayerNinja");

            // Throw a wobbler if it is not there in debug:

            #if DEBUG

                if ( m_player_ninja_type == null ) throw new Exception("Class missing. EnemyNinja requires PlayerNinja class !!!");

            #endif

            // Create the behaviour set:

            m_behaviours = new AIBehaviourSet(this);

            // Select the behaviours and their importance:

            m_behaviours.AddBehaviour( "AI_Rest"                          , 1.5f        );
            m_behaviours.AddBehaviour( "AI_Patrol"                        , 30          );
            m_behaviours.AddBehaviour( "AI_Sight"                         , 60          );
            m_behaviours.AddBehaviour( "AI_Pursue"                        , 40          );
            m_behaviours.AddBehaviour( "AI_Pursue_Jump_Gap"               , 800.0f      );
            m_behaviours.AddBehaviour( "AI_Pursue_Wall_Run"               , 0.00008f    );
            m_behaviours.AddBehaviour( "AI_Pursue_Platform_Jump"          , 10.0f       );
            m_behaviours.AddBehaviour( "AI_Pursue_Move_Around_Platform"   , 0.03f       );
            m_behaviours.AddBehaviour( "AI_Decluster"                     , 0.02f       );
            m_behaviours.AddBehaviour( "AI_Turn_Around"                   , 20.1f       );
            m_behaviours.AddBehaviour( "AI_Pursue_Wall_Jump"              , 30.0f       );
            m_behaviours.AddBehaviour( "AI_Pursue_Wall_Clear"             , 200.0f      );
            m_behaviours.AddBehaviour( "AI_Attack"                        , 0.65f       );
            m_behaviours.AddBehaviour( "AI_Throw_Shuriken"                , 0.5f        );

            // Set the smoothness of behaviour change:

            m_behaviours.BehaviourSmooth      = 0.75f;
            m_behaviours.BehaviourHoldTime    = 0.20f;

            // Settings for this type of character

            JumpForce       = 12;
            TopSpeed        = 9;
            ShurikenSpeed   = 600.0f;
            ShurikenDamage  = 25.0f;
            ShurikenGravity = 0;
            AttackDamage    = 50.0f;

            // Set default depth:

            Depth = 6000;

            // Set shuriken texture: see if cached first though

            if ( s_cached_shuriken_texture != null )
            {
                // Have cache: use

                ShurikenTexture = s_cached_shuriken_texture;
            }
            else
            {
                // Ain't got cache: load

                ShurikenTexture = Core.Graphics.LoadTexture("Graphics\\Objects\\EnemyShuriken");
            }
        }

        //=====================================================================================
        /// <summary>
        /// This function is called once after a level has loaded and after the OnLevelLoaded()
        /// function. It allows the class to cahche commonly loaded data for future created 
        /// instances. Example uses would be to pre-cache enemy ninja sprites once on startup 
        /// rather than loading them every time a ninja is spawned (slow!!). All a class needs to 
        /// do for this to happen is to declare this function with this exact name and format 
        /// and it will be called. 
        /// 
        /// N.B Base class versions of this function are called first; as per constructor convention.
        /// This will be called for abstract classes also.
        /// </summary>
        //=====================================================================================

        private static void BuildClassCache()
        {
            // Load our shuriken texture

            s_cached_shuriken_texture = Core.Graphics.LoadTexture("Graphics\\Objects\\EnemyShuriken");

            // Load all animations:

            s_cached_animations = new Animation[]
            {
                new Animation( "Content\\Animations\\EnemyNinjaColor1.xml" )    ,
                new Animation( "Content\\Animations\\EnemyNinjaColor2.xml" )    ,
                new Animation( "Content\\Animations\\EnemyNinjaColor3.xml" )    ,
                new Animation( "Content\\Animations\\EnemyNinjaColor4.xml" )
            };
        }

        //=====================================================================================
        /// <summary>
        /// This function is called once after a level has been cleared. The data that was 
        /// allocated in BuildCache() should be destroyed or references should be cleared to 
        /// it here. All a class needs to do for this to happen is to declare this function
        /// with this exact name and format and it will be called. 
        /// 
        /// N.B Derived class versions of this function are called first; as per destructor convention.
        /// This will be called for abstract classes also. If BuildCache() is not present however 
        /// this function will not be called. If BuildCache() is there however, this function 
        /// MUST be implemented or an error will be thrown and the class will be ignored for 
        /// caching.
        /// </summary>
        //=====================================================================================

        private static void ClearClassCache()
        {
            // Clear animations:

            s_cached_animations = null;

            // Clear shuriken texture

            s_cached_shuriken_texture = null;
        }

        //=========================================================================================
        /// <summary>
        /// Each character should implement this function and load it's own animation in this function.
        /// </summary>
        //=========================================================================================

        public override void LoadAnimation()
        {
            // See if we pre-cahced our animations or not:

            if ( s_cached_animations == null )
            {
                // No animation cache: load one of the four enemy ninja animations:

                switch ( Core.Random.Next() & 3 )
                {
                    case 0:     Animation = new Animation( "Content\\Animations\\EnemyNinjaColor1.xml" );   break;
                    case 1:     Animation = new Animation( "Content\\Animations\\EnemyNinjaColor2.xml" );   break;
                    case 2:     Animation = new Animation( "Content\\Animations\\EnemyNinjaColor3.xml" );   break;
                    default:    Animation = new Animation( "Content\\Animations\\EnemyNinjaColor4.xml" );   break;
                }
            }
            else
            {
                // Got a cache: make our animation from one of the cached items

                switch ( Core.Random.Next() & 3 )
                {
                    case 0:     Animation = new Animation( s_cached_animations[0] );   break;
                    case 1:     Animation = new Animation( s_cached_animations[1] );   break;
                    case 2:     Animation = new Animation( s_cached_animations[2] );   break;
                    default:    Animation = new Animation( s_cached_animations[3] );   break;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// On delete event for this object. Gives the player some score for killing the enemy ninja 
        /// and calls base function.
        /// </summary>
        //=========================================================================================

        public override void OnDelete()
        {
            // Call base function

            base.OnDelete();

            // Increase the players score:

            LevelRules rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

            // Increase the player's score if we were killed normally:

            if ( Health <= 0 ) 
            {
                // Increase player's score

                rules.GivePlayerScore( SCORE_VALUE , this );

                // Increase the number of enemies killed:

                rules.CurrentPhaseEnemyKills++;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Update function. Does AI logic for the enemy ninja.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Do AI: but only if not spawning or dead:

            if ( Spawning == false && Health > 0 )
            {
                // Set to true when we are out of sight and and not chasing the player:

                bool sleep = false;

                // Grab the AI sight module:

                AI_Sight b_sight = (AI_Sight) m_behaviours.GetBehaviour("AI_Sight");

                // See if found:

                if ( b_sight != null )
                {
                    // Ok: see if we are currently pursuing the player:

                    if ( b_sight.PlayerInSight == false)
                    {
                        // Right we are not in sight: if we are out of camera range then turn off AI

                        if ( IsVisible( Core.Level.Renderer.Camera ) == false )
                        {
                            // Out of sight: disable ai

                            sleep = true;
                        }
                    }
                }

                // Run behaviours and ai: but only if we are not sleeping. If we are sleeping then kill the enemy

                if ( sleep == false ) 
                {
                    // Call base function:

                    base.OnUpdate();

                    // Run behaviours:

                    m_behaviours.RunBehaviours();
                }
                else
                {
                    // Delete:

                    Core.Level.Data.Remove(this);
                }
            }
            else
            {
                // If we are spawning and in sight then run physics: otherwise delete us

                if ( IsVisible(Core.Level.Renderer.Camera) )
                {
                    // Call base function:

                    base.OnUpdate();
                }
                else
                {
                    // Delete:

                    Core.Level.Data.Remove(this);
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Deals out attack damage when the ninja is attacking. Attack damage for swords is dealt
        /// out over the frames so that if the ninja starts a sword slash before it is in contact 
        /// with another character, some of the damage comes through if the ninja crosses the 
        /// character whilst swiping and running. This function is virtual for the ninja type and 
        /// should be implemented by both enemy and player ninjas.
        /// </summary>
        //=========================================================================================

        public override void DealAttackDamage()
        {
            // Get the arm part of the body:

            AnimationPart p_arm = Animation.GetPart("Arm");

            // If not there or no sequence then abort:

            if ( p_arm == null || p_arm.CurrentSequence == null ) return;

            // See what sequence we are using: only allow attack damage if attack sequence is playing

            if ( p_arm.CurrentSequence.Name.Equals("Attacking",StringComparison.CurrentCultureIgnoreCase) )
            {
                // Get a right vector for the player rotated by current surface rotation:

                Vector2 right = Vector2.UnitX;

                    // If the character is going left then flip it:

                    if ( Flipped == false ) right.X *= -1;

                    // Rotate it:

                    right = Vector2.Transform( right , Matrix.CreateRotationZ(SurfaceRotation) );

                    // Get the true axis aligned bounding box size from the animation: the current bb for the character is distorted with rotation

                    Vector2 box_dimensions = BoxDimensions;

                    if ( Animation != null )
                    {
                        box_dimensions = Animation.BoxDimensions;
                    }

                    // Increase it's size by x coordinate of box dimensions:

                    right *= box_dimensions.X;

                // Make up a bounding box centered on this vector + players offset with the start and end points determining size of box

                Vector2 box_p = right * 0.5f + Position;
                Vector2 box_d = right * 0.5f;

                if ( box_d.X < 0 ) box_d.X *= -1;
                if ( box_d.Y < 0 ) box_d.Y *= -1;

                // Now do a bounding box overlap for the level:

                Core.Level.Collision.Overlap( box_p , box_d , this );

                // Run through the results:

                for ( int i = 0 ; i < Core.Level.Collision.OverlapResultCount; i++ )
                {
                    // Get this object:

                    GameObject game_object = Core.Level.Collision.OverlapResults[i].QueryObject;

                    // See if it is a player ninja:

                    if ( game_object.GetType().Equals( m_player_ninja_type ) )
                    {
                        // Good: cast to a player ninja

                        PlayerNinja player = (PlayerNinja) game_object;

                        // Figure out how long the entire attack animation is:

                        float time = ( 1.0f / (float) (p_arm.CurrentSequence.FrameRate) ) * (float) p_arm.CurrentSequence.Frames.Length;

                        // Figure out how much sword damage to do per frame from this:

                        float damage = ( Core.Timing.ElapsedTime / time ) * AttackDamage;

                        // Damage the player:

                        player.Damage(damage,this);
                    }
                }
            }            
        }

        //=========================================================================================
        /// <summary>
        /// Applys damage to the character. Virtual so that derived classes can implement special 
        /// events for this action.
        /// </summary>
        /// <param name="amount">   Amount of damage to apply to the character.     </param>
        /// <param name="damager">  Thing that damaged the character. May be null.  </param>
        //=========================================================================================

        public override void Damage( float amount , GameObject damager )
        {
            // Call base function:

            base.Damage(amount,damager);

            // Good: now get the AI sight module

            AI_Sight b_sight = (AI_Sight) m_behaviours.GetBehaviour("AI_Sight");

            // If we have the sight module then suddenly jolt the player into visibilty: the ai should wake up after being hurt !!!

            if ( b_sight != null ) b_sight.AlertAI();
        }

        //=========================================================================================
        /// <summary>
        /// This function makes a shuriken that the character can fire. It is virtual so different 
        /// types of ninja's can make different types of shurikens.
        /// </summary>
        /// <param name="velocity"> Velocity the shuriken should have. </param>
        /// <returns> The shuriken that was made. </returns>
        //=========================================================================================

        protected override Projectile MakeShuriken( Vector2 velocity )
        {
            // Make an enemy projectile for the enemy ninja type:

            return new EnemyProjectile
            (
                this                    ,
                ShurikenTexture         ,
                ShurikenSparkTexture    ,
                Effect                  ,
                Position                ,
                velocity                ,
                ShurikenLiveTime        ,
                ShurikenGravity         ,
                ShurikenSize            ,
                ShurikenDamage
            );
        }

        //=========================================================================================
        /// <summary>
        /// Does debug drawing for the enemy ninja.
        /// </summary>
        //=========================================================================================

        #if DEBUG

            public override void OnDebugDraw()
            {
                // Do base class debug drawing:

                base.OnDebugDraw();

                // Do debug drawing for behaviours:

                m_behaviours.OnDebugDraw();
            }
    
        #endif

    }   // end of namespace

}   // end of class
