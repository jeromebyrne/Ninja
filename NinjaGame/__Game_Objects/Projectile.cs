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
    /// Represents a projectile object which can be used to damage characters in the level. 
    /// This particular projectile damages everything other than the character that fired the 
    /// projectile.
    /// </summary>
    //#############################################################################################

    public class Projectile : Sprite
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Tells if the projectile is dead. </summary>

            public bool Dead { get { return m_dead; } }

            /// <summary> Game object which owns this projectile. </summary>

            public GameObject Owner { get { return m_owner; } }

            /// <summary> Amount of time the projectile has been alive for. </summary>

            public float TimeAlive { get { return m_time_alive; } }

            /// <summary> Maximum amount of time the projectile can be alive for

            public float LiveTime { get { return m_live_time; } }

            /// <summary> Amount of damage this projectile does. </summary>

            public float Damage { get { return m_damage; } } 

            /// <summary> Speed the object is moving at in units per second. </summary>
            
            public Vector2 Velocity { get { return m_velocity; } set { m_velocity = value; } }

            /// <summary> X Speed the object is moving at in units per second. </summary>

            public float VelocityX { get { return m_velocity.X; } set { m_velocity.X = value; } }

            /// <summary> Y Speed the object is moving at in units per second. </summary>

            public float VelocityY { get { return m_velocity.Y; } set { m_velocity.Y = value; } }

            /// <summary> Amount to multiply the total allowed aftertouch by for each frame. Lower values mean less aftertouch. from 0-1.</summary>

            public float AftertouchMultiplier
            {
                get
                {
                    return m_aftertouch_multiplier;
                }

                set
                {
                    m_aftertouch_multiplier = MathHelper.Clamp( value , 0 , 1 );
                }
            }

            /// <summary> Currently allowed amount of aftertouch in percent. </summary>

            public float CurrentAftertouch { get { return m_current_aftertouch; } }

            /// <summary> Gets / sets the speed that the projectile rotates at per second. </summary>

            public float RotateSpeed { get { return m_rotate_speed; } set { m_rotate_speed = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Amount of time the projectile has been alive for. </summary>

            private float m_time_alive = 0;

            /// <summary> Maximum amount of time the projectile can be alive for

            private float m_live_time = 0;

            /// <summary> Amount of damage this projectile does. </summary>

            protected float m_damage = 0;

            /// <summary> Game object which owns this projectile. </summary>

            private GameObject m_owner = null;

            /// <summary> Amount of gravity applied to the projectile. </summary>

            private float m_gravity = 0;

            /// <summary> Type for a character. Saved here for future reference. </summary>

            private Type m_character_type = null;

            /// <summary> Speed the object is moving at in units per second. </summary>
        
            private Vector2 m_velocity = Vector2.Zero;

            /// <summary> Set to true if the projectile is dead. </summary>

            private bool m_dead = false;

            /// <summary> Amount to multiply the total allowed aftertouch by for each frame. Lower values mean less aftertouch. from 0-1.</summary>

            private float m_aftertouch_multiplier = 0.85f;

            /// <summary> Currently allowed amount of aftertouch in percent. </summary>

            private float m_current_aftertouch = 1.0f; 

            /// <summary> Speed that the projectile rotates at per second. </summary>

            private float m_rotate_speed = 18.0f;

            /// <summary> Texture used for projectile spark. </summary>

            private Texture2D m_spark_texture = null;

        //=========================================================================================
        /// <summary>
        /// Default constructor to create a projectile. Projectile is not intended to be added to 
        /// levels by the user so there are no xml read and write functions for this object. The 
        /// default constructor is just to keep the xml system happy.
        /// </summary>
        //=========================================================================================

        public Projectile()
        {
            // Get the character type:

            m_character_type = Type.GetType("NinjaGame.Character");

            // Throw a wobbler in debug if not there:

            #if DEBUG
            
                if ( m_character_type == null ) throw new Exception("Code missing: no Character type got for projectile");

            #endif

            // Set default depth:

            Depth = 4500;
        }

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

        public Projectile
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
        : base(false,true,false)
        {
            // Get the character type:

            m_character_type = Type.GetType("NinjaGame.Character");

            // Throw a wobbler in debug if not there:

            #if DEBUG
            
                if ( m_character_type == null ) throw new Exception("Code missing: no Character type got for projectile");

            #endif

            // Save all the stuff

            m_owner         = owner;
            Texture         = texture;
            m_spark_texture = spark_texture;
            Effect          = effect;
            Position        = position;
            m_velocity      = velocity;
            m_gravity       = gravity;
            m_live_time     = live_time;
            m_damage        = damage;

            // Set the size of the projectile:

            BoxDimensionsX = size;
            BoxDimensionsY = size;

            // Set default depth:

            Depth = 4500;
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

            // Read all attributes

            data.ReadFloat( "VelocityX" , ref m_velocity.X , 0 );
            data.ReadFloat( "VelocityY" , ref m_velocity.Y , 0 );
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

            // Write all attributes

            data.Write( "VelocityX" , m_velocity.X );
            data.Write( "VelocityY" , m_velocity.Y );
        }

        //=========================================================================================
        /// <summary>
        /// Updates the projectile. Moves it and checks for hit's with the level or 
        /// with other characters. If the projectile exceeds it's maximum live time 
        /// then it is also deadened.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Do base class function

            base.OnUpdate();

            // Increment the amount of time the thing has been alive for:

            m_time_alive += Core.Timing.ElapsedTime;

            // See if time to live is past the max:L if so then kill this projectile:

            if ( m_time_alive > m_live_time )
            {
                // Be alive no longer: kill this projectile

                Core.Level.Data.Remove(this); return;
            }

            // Move: if we hit anything then kill the particle:

            bool hit_solid = DoMovement();

            // See if hit something kill this projectile and abort

            if ( hit_solid ){ Kill(); return; }

            // Check if we hit anything: if so then kill

            if ( CheckHits() ) Kill();

            // If we are offscreen then kill the projectile:

            if ( CheckOffscreen() ) Kill();

            // Reduce the amount of aftertouch allowed:

            m_current_aftertouch *= m_aftertouch_multiplier;

            // Rotate the projectile:

            Rotation += Core.Timing.ElapsedTime * m_rotate_speed;

            // Try and keep in range:

            if ( Rotation < - MathHelper.TwoPi ) Rotation += MathHelper.TwoPi;
            if ( Rotation >   MathHelper.TwoPi ) Rotation -= MathHelper.TwoPi;
        }

        //=========================================================================================
        /// <summary>
        /// Does the movment for the projectile. Returns true if anything was hit and the projectile
        /// should be destroyed.
        /// </summary>
        /// <returns> True if something solid was hit by the projectile. </returns>
        //=========================================================================================

        protected bool DoMovement()
        {
            // Bounding box dimensions must be at least this size:

            if ( BoxDimensionsX < 1 ) BoxDimensionsX = 1;
            if ( BoxDimensionsY < 1 ) BoxDimensionsY = 1;

            // Shorten code:

            LevelCollisionQuery c = Core.Level.Collision;

            // Apply gravity to the velocity:

            m_velocity.Y -= m_gravity;

            // Get the smallest bounding box dimension for the projectile:

            float smallest_d = BoxDimensionsX;

            if ( BoxDimensionsY < smallest_d ) 
            {
                smallest_d = BoxDimensionsY;
            }

            // Get the total amount we must move for this frame:

            Vector2 move_amount = m_velocity * Core.Timing.ElapsedTime;

            // Allow only this many iterations:

            int iterations = 4;

            // Continue moving until we have moved the full amount:

            while ( iterations > 0 )
            {
                // Decrement this:

                iterations--;

                // Get the total amount to move left:

                float left = move_amount.Length();

                // If nothing left return false:

                if ( left <= 0.001f || left == float.NaN ) return false;

                // We will move a maximum of 80% of our bounding elipse size: see if we can move the remaining amount left entirely

                if ( left < smallest_d * 0.8f )
                {
                    // Good: we can move the remainder

                    Position += move_amount;

                    // Do collision detection with the level: use the bounding box for the object as a bounding elipse

                    c.Collide( Position, BoxDimensions, 0 , this );

                    // No run through the results:

                    for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
                    {
                        // See if we are moving towards this surface: if not then there will be no collision

                        if ( Vector2.Dot( Velocity , c.CollisionResults[i].Normal ) < 0 ) 
                        {
                            // Good: we are in collision - make a spark

                            CreateSpark();

                            // Return true as we hit something:                          

                            return true;
                        }
                    }

                    // Done moving:

                    break;
                }
                else
                {
                    // We can't move the whole way: get direction of movement

                    Vector2 move_dir = move_amount; move_dir.Normalize();

                    // Get amount to move in that direction:

                    Vector2 move_vec = move_dir * smallest_d * 0.8f;

                    // Move:

                    Position += move_vec;

                    // Do collision detection with the level: use the bounding box for the object as a bounding elipse

                    c.Collide( Position, BoxDimensions, 0 , this );

                    // No run through the results:

                    for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
                    {
                        // See if we are moving towards this surface: if not then there will be no collision

                        if ( Vector2.Dot( Velocity , c.CollisionResults[i].Normal ) < 0 )
                        {
                            // Good: we are in collision - make a spark

                            CreateSpark();

                            // Return true as we hit something:                          

                            return true;
                        }
                    }

                    // Reduce the remaining movement left to do

                    move_amount -= move_vec;
                }
            }

            // Return false: we didn't hit anything

            return false;
        }

        //=========================================================================================
        /// <summary>
        /// Checks for hits the projectile may have made with characters. If the function finds  
        /// a character that is in contact with the projectile, it damages it and returns true.
        /// </summary>
        /// <returns> True if something solid was hit by the projectile. </returns>
        //=========================================================================================

        protected virtual bool CheckHits()
        {
            // Check for bounding box overlaps:

            Core.Level.Collision.Overlap( Position , BoxDimensions , this );

            // Run through all the results:

            for ( int i = 0 ; i < Core.Level.Collision.OverlapResultCount ; i++ )
            {
                // See what the thing was that we hit:

                GameObject hit_object = Core.Level.Collision.OverlapResults[i].QueryObject;

                // See if it is a character type:

                if ( hit_object.GetType().IsSubclassOf(m_character_type) )
                {
                    // Is a character:

                    Character character = (Character) hit_object;

                    // Hurt if not the character that created us:

                    if ( character != m_owner )
                    {
                        // get the velocity of the projectile
                        Vector2 direction = this.Velocity;

                        // normalise to find the direction
                        direction.Normalize();

                        // splatter blood all over the gaff
                        //character.BloodEmitter.StartExplosion(this.Position, direction, 0, 10, 20);

                        // hurt 'em
                        character.Damage(m_damage,this);

                        // We hit something:

                        return true;
                    }
                }
            }

            // Didn't hit something:

            return false;
        }

        //=========================================================================================
        /// <summary>
        /// Checks if the projectile is offscreen and returns true if it is.
        /// </summary>
        /// <returns> True if the projectile is offscreen. </returns>
        //=========================================================================================

        protected bool CheckOffscreen()
        {
            // Get the camera:

            Camera c = Core.Level.Renderer.Camera;

            // Get area of the world shown by the camera

            Vector2 world_area = c.ViewArea;

            // Half world area: since it is not given from the center of the camera

            world_area.X *= 0.5f;
            world_area.Y *= 0.5f;

            // Expand slightly by our bb dimensions and a little extra for good measure:

            world_area      += BoxDimensions;
            world_area.X    += 4;
            world_area.Y    += 4;

            // Now get the projectile pos relative to the camera:

            Vector2 r = Position - c.Position;

            // See if we are offscreen:

            if ( r.X < - world_area.X ) return true;
            if ( r.X > + world_area.X ) return true;
            if ( r.Y < - world_area.Y ) return true;
            if ( r.Y > + world_area.Y ) return true;

            // If we got to here then we are not offscreen: return false

            return false;
        }

        //=========================================================================================
        /// <summary>
        /// Applys aftertouch to the projectile.
        /// </summary>
        /// <param name="direction"> 
        /// Direction we want the projectile to move in with the aftertouch. 
        /// </param>
        //=========================================================================================

        public void ApplyAftertouch( Vector2 direction )
        {
            // If no direction is given then abort:

            if ( direction.X == 0 && direction.Y == 0 ) return;

            // Normalise the direction:

            direction.Normalize();

            // Get the current length of our velocity:

            float speed = Velocity.Length();

            // Get the direction of the aftertouch: if it is in the opposite to the way we are moving then ignore:

            if ( Vector2.Dot( direction , Velocity ) < 0 )
            {
                // Can't do aftertouch this way: abort

                return;
            }

            // Reduce the current velocity according to the amount of aftertouch:

            VelocityX /= speed;
            VelocityY /= speed;

            VelocityX *= speed * ( 1.0f - m_current_aftertouch );
            VelocityY *= speed * ( 1.0f - m_current_aftertouch );

            // Now apply aftertouch in the given direction:

            VelocityX += direction.X * m_current_aftertouch * speed;
            VelocityY += direction.Y * m_current_aftertouch * speed;
        }

        //=========================================================================================
        /// <summary>
        /// Kills this projectile and removes it from the level.
        /// </summary>
        //=========================================================================================

        public void Kill()
        {
            // Remove ourselves from the level

            Core.Level.Data.Remove(this);

            // We are now dead

            m_dead = true;

            // Play the shuriken hit sound:

            Core.Audio.Play("Shuriken_Hit");
        }

        //=========================================================================================
        /// <summary>
        /// Makes a spark at the projectiles current position:
        /// </summary>
        //=========================================================================================

        public void CreateSpark()
        {
            // Get the direction of the projectile velocity:

            Vector2 dir = this.m_velocity;

            // Normalise that direction:

            dir.Normalize();

            // Start a spark:

            Core.Level.Emitter.CreateDirectedBurst
            (
                40              ,
                m_spark_texture ,
                Position        ,
                -dir            ,
                0.25f           ,
                0.25f           ,
                0.50f           ,
                1.0f            ,
                2.0f            ,
                Vector4.One     ,
                Vector4.Zero    ,
                300             ,
                400             ,
                300             ,
                false
            );
        }

    }   // end of namespace

}   // end of class
