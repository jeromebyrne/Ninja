using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Specialised projectile for enemies that only hits the player. 
    /// </summary>
    //#############################################################################################

    public class EnemyProjectile : Projectile
    {
        //=========================================================================================
        // Variables
        //=========================================================================================
        private bool m_isDeflected;

        private int DEFLECTION_SCORE = 100;

        //=========================================================================================
        /// <summary>
        /// Default constructor to create a projectile. Projectile is not intended to be added to 
        /// levels by the user so there are no xml read and write functions for this object. The 
        /// default constructor is just to keep the xml system happy.
        /// </summary>
        //=========================================================================================

        public EnemyProjectile(){}

        //=========================================================================================
        /// <summary>
        /// Creates the projectile with the specified settings.
        /// </summary>
        /// <param name="owner">            Object that created the projectile.             </param>
        /// <param name="texture">          Texture to use for the projectile.              </param>
        /// <param name="spark_texture">    Spark texture to use for a projectile spark.    </param>
        /// <param name="effect">           Effect to render projectile with.               </param>
        /// <param name="position">         Intiial position of the projectile.             </param>
        /// <param name="live_time">        Amount of time the projectile has to live.      </param>
        /// <param name="velocity">         Initial velocity of the projectile.             </param>
        /// <param name="gravity">          Amount of gravity to apply to the projectile.   </param>
        /// <param name="size">             Size of the projectile.                         </param>
        /// <param name="damage">           Amount of damage this projectile does           </param>
        //=========================================================================================

        public EnemyProjectile
        ( 
            GameObject  owner           , 
            Texture2D   texture         ,
            Texture2D   spark_texture   ,
            Effect      effect          ,
            Vector2     position        , 
            Vector2     velocity        ,
            float       live_time       ,
            float       gravity         ,
            float       size            ,
            float       damage      
        )
        : base( owner, texture, spark_texture , effect, position, velocity, live_time, gravity, size, damage )
        { 
            // This projectile has not been deflected

            m_isDeflected = false;
        }

        //=========================================================================================
        /// <summary>
        /// Checks for hits the projectile may have made with characters. If the function finds  
        /// a character that is in contact with the projectile, it damages it and returns true.
        /// </summary>
        /// <returns> True if something solid was hit by the projectile. </returns>
        //=========================================================================================

        protected override bool CheckHits()
        {
            // See if the projectile has been deflected or not:

            if ( ! this.m_isDeflected )
            {
	            // Find the player object: 

	            PlayerNinja ninja = (PlayerNinja) Core.Level.Search.FindByType("PlayerNinja");

	            // See if there:

                if ( ninja != null )
                {
                    // Good: see if our bb overlaps with it:

                    OverlapQueryResult result = ninja.OnOverlapQuery(Position, BoxDimensions, this);

                    // See what the result was:

                    if ( result.ValidResult && result.QueryObject != Owner )
                    {
                        // See if the player is blocking

                        if ( ninja.Blocking == false )
                        {
                            // Hit the player: damage

                            ninja.Damage(Damage,this);

                            // Return true for hit

                            return true;
                        }
                        else
                        {
                            // The player blocked the projectile: show a spark

                            CreateSpark();

                            // play deflection sound

                            Core.Audio.Play("Deflect");

                            // We can deflect it and this projectile hasnt already been deflected

                            if ( ninja.CanDeflect && this.m_isDeflected == false )
                            {
                                // Send it back in the opposite direction and increase speed
                                
                                this.Velocity *= -1.5f;

                                // This projectile has now been deflected
                                
                                this.m_isDeflected = true;

                                // Increase the damage of the projectile (rewards player for being skillfull)
                                
                                this.m_damage *= 4;

                                LevelRules rules = (LevelRules)Core.Level.Search.FindByType("LevelRules");

                                // Reward the player with some points for deflecting

                                rules.GivePlayerScore( DEFLECTION_SCORE , this );

                                // We want to keep the projectile alive so return false

                                return false;
                            }
                            else
                            {
                                // we didnt deflect so return true as a succesful hit

                                return true;
                            }
                        }

                    }
                }
            }
            // if this projectile HAS been deflected, if so then we want to check collisions 
            //with any characters, even the damages its creator
            else
            {
                // Check for bounding box overlaps:

                Core.Level.Collision.Overlap(Position, BoxDimensions, this);

                // Run through all the results:

                for ( int i = 0; i < Core.Level.Collision.OverlapResultCount; i++ )
                {
                    // See what the thing was that we hit:

                    GameObject hit_object = Core.Level.Collision.OverlapResults[i].QueryObject;

                    // See if it is a character type:

                    if ( hit_object.GetType().IsSubclassOf(Type.GetType("NinjaGame.Character")) &&
                        !hit_object.GetType().IsSubclassOf(Type.GetType("NinjaGame.PlayerNinja")))
                    {
                        // Is a character:

                        Character character = (Character)hit_object;

                        // hurt 'em

                        character.Damage(Damage,this);

                        // We hit something:

                        return true;                        
                    }
                }
            }

            // Didn't hit something:

            return false;
        }

    }   // end of namespace

}   // end of class
