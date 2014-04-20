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
    /// <summary>
    /// This is a base class for all character types in the game, be they a ninja or a samurai.
    /// Most of the hard work relating to character movement and collision detection is done here.
    /// Derived classes implement speciality behaviour such as jumping or AI (for enemy characters)
    /// </summary>
    //#############################################################################################

    public abstract class Character : GameObject
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Gets / sets the animation used by the character. This is null initially. It is up to each character type to initialise this. </summary>

            public Animation Animation { get { return m_animation; } set { m_animation = value;} }

            /// <summary> Gets the amount of health the character has left. </summary>

            public float Health { get { return m_health; } }

            /// <summary> This the the maximum amount of health the character can have. </summary>

            public float MaximumHealth { get { return m_max_health; } set { m_max_health = value; } } 
            
            /// <summary> Gets the shader effect used to draw the character. </summary>
            
            public Effect Effect { get { return m_effect; } }

            /// <summary> The flatest surface the character is in contact with, if any. </summary>

            public CollisionQueryResult FlattestContactSurface { get { return m_flattest_contact_surface; } }

            /// <summary> The steepest surface the character is in contact with, if any. </summary>

            public CollisionQueryResult SteepestContactSurface { get { return m_steepest_contact_surface; } }

            /// <summary> The character should jump when this surface is valid. This surface can be slightly further awawy than the contact surfaces. </summary>
            
            public CollisionQueryResult JumpSurface { get { return m_jump_surface; } }

            /// <summary> The amount the charater is rotated (in radians) by according to the orientation of the surface they are on </summary>

            public float SurfaceRotation { get { return m_surface_rotation; } }

            /// <summary> Maximum speed the character can move at. this is only applicable to movement velocity. </summary>

            public float TopSpeed { get { return m_top_speed; } set { m_top_speed = MathHelper.Clamp(value,0,1000000); } }

            /// <summary> The amount of friction for the character against surfaces in the world. </summary>

            public float Friction { get { return m_friction; } set { m_friction = MathHelper.Clamp(value,0,1000000); } }

            /// <summary> How fast the character accelerates towards it's top speed. </summary>

            public float Acceleration { get { return m_acceleration; } set { m_acceleration = MathHelper.Clamp(value,0,1000000); } }

            /// <summary> How fast the character deaccelerates movement once input is stopped. 1 means no decacceleration</summary>

            public float Deacceleration { get { return m_deacceleration; } set { m_deacceleration = MathHelper.Clamp(value,0,1000000); } }

            /// <summary> Tells if the character is just spawning / fading in. </summary>

            public bool Spawning { get { return m_spawn_time_left > 0; } }

            /// <summary> Gets / Sets the amount of time left the character has to spawn. </summary>

            protected float SpawnTimeLeft { get { return m_spawn_time_left; } set { m_spawn_time_left = value; } }

            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Speed the object is moving at in units per second. This velocity concerns gravitational 
            /// and other non player movement forces. Gravity is kept separate to movement forces.
            /// </summary>
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            
            public Vector2 Velocity { get { return m_velocity; } set { m_velocity = value; } }

            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// X Speed the object is moving at in units per second. This velocity concerns gravitational 
            /// and other non player movement forces. Gravity is kept separate to movement forces.
            /// </summary>
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float VelocityX { get { return m_velocity.X; } set { m_velocity.X = value; } }

            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Y Speed the object is moving at in units per second. This velocity concerns gravitational 
            /// and other non player movement forces. Gravity is kept separate to movement forces.
            /// </summary>
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float VelocityY { get { return m_velocity.Y; } set { m_velocity.Y = value; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> The velocity at which the character will move at. This is factored in to overal 
            /// velocity but separate from gravitational velocity etc.. This velocity is also not affected 
            /// by surface resistance.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 MoveVelocity 
            { 
                get { return m_move_velocity; } 

                set
                {
                    // Set the value

                    m_move_velocity = value;

                    // If above the maximum speed then clamp:

                    float speed = m_move_velocity.Length(); 

                    if ( speed > m_top_speed )
                    {
                        // Going too fast: slow down

                        m_move_velocity /= speed; m_move_velocity *= m_top_speed;
                    }
                }
            
            }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> The X velocity at which the character will move at. This is factored in to overal 
            /// velocity but separate from gravitational velocity etc.. This velocity is also not affected 
            /// by surface resistance.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float MoveVelocityX
            { 
                get { return m_move_velocity.X; } 

                set
                {
                    // Set the value

                    m_move_velocity.X = value;

                    // If above the maximum speed then clamp:

                    float speed = m_move_velocity.Length(); 

                    if ( speed > m_top_speed )
                    {
                        // Going too fast: slow down

                        m_move_velocity /= speed; m_move_velocity *= m_top_speed;
                    }
                }
            
            }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> The Y velocity at which the character will move at. This is factored in to overal 
            /// velocity but separate from gravitational velocity etc.. This velocity is also not affected 
            /// by surface resistance.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public float MoveVelocityY
            { 
                get { return m_move_velocity.Y; } 

                set
                {
                    // Set the value

                    m_move_velocity.Y = value;

                    // If above the maximum speed then clamp:

                    float speed = m_move_velocity.Length(); 

                    if ( speed > m_top_speed )
                    {
                        // Going too fast: slow down

                        m_move_velocity /= speed; m_move_velocity *= m_top_speed;
                    }
                }
            
            }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the character's bounding elipse in +/- x and y directions from object 
            /// position or center of the object. Used in collision detection as the volume of the 
            /// object when it moves. This is read only and is determined from the objects animation
            /// xml file.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            
            public Vector2 EllipseDimensions { get { return m_ellipse_dimensions; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the character's bounding elipse in +/- x directions from object 
            /// position or center of the object. Used in collision detection as the volume of the 
            /// object when it moves. This is read only and is determined from the objects animation
            /// xml file.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            
            public float EllipseDimensionsX { get { return m_ellipse_dimensions.X; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the character's bounding elipse in +/- x directions from object 
            /// position or center of the object. Used in collision detection as the volume of the 
            /// object when it moves. This is read only and is determined from the objects animation
            /// xml file.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            
            public float EllipseDimensionsY { get { return m_ellipse_dimensions.Y; } }

            /// <summary> Set to true if the character sprite is flipped horizontally and in the oposite direction of motion. </summary>

            public bool Flipped { get { return m_flipped; } }
            
        //=========================================================================================
        // Variables
        //=========================================================================================
            
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Speed the object is moving at in units per second. This velocity concerns gravitational 
            /// and other non player movement forces. Gravity is kept separate to movement forces.
            /// </summary>
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Vector2 m_velocity = Vector2.Zero;

            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> The velocity at which the character will move at. This is factored in to overal 
            /// velocity but separate from gravitational velocity etc.. This velocity is also not affected 
            /// by surface resistance.
            /// </summary>
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Vector2 m_move_velocity = Vector2.Zero;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Dimensions of the object's bounding elipse in +/- x and y directions from object 
            /// position or center of the object. Used in collision detection as the volume of the 
            /// object when it moves. This is read only and is determined from the objects animation
            /// xml file.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            
            private Vector2 m_ellipse_dimensions = new Vector2(16,32);

            /// <summary> Amount of health the character has left </summary>

            private float m_health = 100;

            /// <summary> This the the maximum amount of health the character can have. </summary>

            public float m_max_health = 100;

            /// <summary> The amount of friction for the character against surfaces in the world. </summary>

            private float m_friction = 0.25f;

            /// <summary> Maximum speed the character can move at. this is only applicable to movement velocity. </summary>

            private float m_top_speed = 8;

            /// <summary> How fast the character accelerates towards it's top speed. </summary>

            private float m_acceleration = 1.0f;

            /// <summary> How fast the character deaccelerates movement once input is stopped. 1 means no decacceleration. </summary>

            private float m_deacceleration = 0.90f;

            /// <summary> The amount the charater is rotated (in radians) by according to the orientation of the surface they are on </summary>

            private float m_surface_rotation = 0;

            /// <summary> Holds all the parts, sequences and frames of animation for the character. </summary>

            private Animation m_animation = null;
            
            /// <summary> Shader effect used to draw the character. </summary>

            private Effect m_effect = null;

            /// <summary> An array of vertices used to draw the character. </summary>

            private VertexPositionColorTexture[] m_vertices = null;

            /// <summary> The flatest surface the character is in contact with, if any. </summary>

            private CollisionQueryResult m_flattest_contact_surface = CollisionQueryResult.NoResult;

            /// <summary> The steepest surface the character is in contact with, if any. </summary>

            private CollisionQueryResult m_steepest_contact_surface = CollisionQueryResult.NoResult;

            /// <summary> The character should jump when this surface is valid. This surface can be slightly further away than the contact surfaces. </summary>

            private CollisionQueryResult m_jump_surface = CollisionQueryResult.NoResult;

            /// <summary> Set to true if the character sprite is flipped horizontally and in the oposite direction of motion. </summary>

            private bool m_flipped = false;

            /// <summary> This is used to determine when to flip the character. Once it reaches a certain threshold on either positive or negative x then the character is flipped in that direction. </summary>

            private float m_smoothed_x_motion_direction = 0;

            /// <summary> Amount of time till the character is fully spawned. </summary>

            private float m_spawn_time_left = SPAWN_TIME;

            /// <summary> Lightning texture used for lightning spawn effect by the character. 1. Static chached version.</summary>

            private static Texture2D s_cached_spawn_lightning_texture_1 = null;

            /// <summary> Lightning texture used for lightning spawn effect by the character. 2. Static chached version.</summary>

            private static Texture2D s_cached_spawn_lightning_texture_2 = null;

            /// <summary> Lightning texture used for lightning spawn effect by the character. 1. </summary>

            private Texture2D m_spawn_lightning_texture_1 = null;

            /// <summary> Lightning texture used for lightning spawn effect by the character. 2. </summary>

            private Texture2D m_spawn_lightning_texture_2 = null;

            /// <summary> Time since the last bit of lightning was spawned. </summary>

            private float m_time_since_lightning = LIGHTNING_INTERVAL;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Maximum number of iterations that the character can use to try and resolve collisions. </summary>

            private const int COLLISION_ITERATIONS = 8;

            /// <summary> This is the amount in radians to straigten the characters surface rotation angle when not in contact with a surface </summary>

            private const float AIR_ORIENT_STRAIGHTEN = 0.08f;

            /// <summary> Controls how fast the character orients to underlying surfaces. 1 means no orientation, 0 means instant orientation. </summary>

            private const float SURFACE_ORIENT_SPEED = 0.80f; 

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// How quickly to transfer the x component of regular velocity into movment velocity when 
            /// the player is in the air. When the player jumps the velocity is added to regular velocity 
            /// but this can act to block x movement when in mid air. This hack transfers the x component 
            /// of normal velocity into movement velocity gradually as the player is air borne. 
            /// 1 means no transference, 0 means instant transference.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private const float NORMAL_VELOCITY_TRANSFERENCE = 0.97f;

            /// <summary> Velocity at which surfaces will repulse the character. </summary>

            private const float SURFACE_REPULSION_VELOCITY = 0.05f;
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// How much to expand the character's bounding elipse when looking for surfaces to orient to. 
            /// Controls how far away the character will orient. This also determines the distance the 
            /// character can touch a jump surface as well.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private const float SURFACE_ORIENT_DISTANCE = 10.0f;

            /// <summary> How much to expand the bounding elipse of the character when searching for nearby surfaces that it is in contact with. </summary>

            private const float CONTACT_SURFACE_DISTANCE = 2.0f; 
            
            /// <summary> Minimum y value of a surface normal if it is to be jumped off or oriented to. </summary>

            private const float MIN_ORIENT_SURFACE_NORMAL_Y = - 0.5f;

            /// <summary> Amount of gravity applied to characters. </summary>

            public const float GRAVITY = 0.35f;

            /// <summary> How much to smooth the value of the smoothed x motion variable. That variable determines when to flip the character. </summary>

            private const float SMOOTHED_X_MOTION_SMOOTHING = 0.75f;

            /// <summary> If the smoothed x motion falls above or below this threshold (positive or negative) then the character sprite is flipped. </summary>

            private const float SMOOTHED_X_MOTION_THRESHOLD = 0.80f;

            /// <summary> If movement velocity falls below this amount it is removed completely. </summary>

            private const float MOVE_VELOCITY_DEAD_ZONE = 0.0f;

            /// <summary> How much gravity (in percent) upwards to the give the character as it is moving up through a platform </summary>

            private const float PLATFORM_JUMP_UP_GRAVITY_BOOST = 2.0f;

            /// <summary> Time it takes for a character to spawn. </summary>

            private const float SPAWN_TIME = 0.25f;

            /// <summary> Percentage of normal acceleration that is allowed in air. </summary>

            private const float AIR_MOVEMENT_ACCELERATION = 0.85f;

            /// <summary> Time in between lightning bolts when spawning. </summary>

            private const float LIGHTNING_INTERVAL = 0.05f;

        //=========================================================================================
        ///<summary>
        /// Constructor. Creates the object.
        ///</summary>
        //=========================================================================================

        public Character() : base(true,true,true,true)
        {
            // Do character setup

            Setup();            

            // Set default depth:

            Depth = 5000;

            // Load our effect:

            m_effect = Core.Graphics.LoadEffect("Effects\\Black_edged_sprite");

            // Load the character's animation:

            LoadAnimation();

            // Load lightning textures for spawn if not cached globally:

                // Tex 1:

                if ( s_cached_spawn_lightning_texture_1 == null ) 
                {
                    m_spawn_lightning_texture_1 = Core.Graphics.LoadTexture("Graphics\\other\\Lightning1");
                }
                else
                {
                    m_spawn_lightning_texture_1 = s_cached_spawn_lightning_texture_1;
                }

                // Tex 2:

                if ( s_cached_spawn_lightning_texture_2 == null ) 
                {
                    m_spawn_lightning_texture_2 = Core.Graphics.LoadTexture("Graphics\\other\\Lightning2");
                }
                else
                {
                    m_spawn_lightning_texture_2 = s_cached_spawn_lightning_texture_2;
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
            // Load the lightning textures:

            s_cached_spawn_lightning_texture_1 = Core.Graphics.LoadTexture("Graphics\\other\\Lightning1");
            s_cached_spawn_lightning_texture_2 = Core.Graphics.LoadTexture("Graphics\\other\\Lightning2");
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
            // Clear the cache:

            s_cached_spawn_lightning_texture_1 = null;
            s_cached_spawn_lightning_texture_2 = null;
        }

        //=========================================================================================
        /// <summary>
        /// Each character should implement this function and load it's own animation in this function.
        /// </summary>
        //=========================================================================================

        public virtual void LoadAnimation(){}

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

            data.ReadFloat  ( "VelocityX"       , ref m_velocity.X          , 0     );
            data.ReadFloat  ( "VelocityY"       , ref m_velocity.Y          , 0     );
            data.ReadFloat  ( "MoveVelocityX"   , ref m_move_velocity.X     , 0     );
            data.ReadFloat  ( "MoveVelocityY"   , ref m_move_velocity.Y     , 0     );
            data.ReadFloat  ( "Health"          , ref m_health              , 100   );
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

            data.Write( "VelocityX"         , m_velocity.X          );
            data.Write( "VelocityY"         , m_velocity.Y          );
            data.Write( "MoveVelocityX"     , m_move_velocity.X     );
            data.Write( "MoveVelocityY"     , m_move_velocity.Y     );
            data.Write( "Health"            , m_health              );
        }

        //=========================================================================================
        /// <summary>
        /// Increases the character's speed in the left or right directions.
        /// </summary>
        /// <param name="acceleration"> Amount to accelerate character by </param>
        //=========================================================================================

        public void Accelerate( float acceleration )
        {
            // If we are in the air then slow down this amount:

            if ( JumpSurface.ValidResult == false ) acceleration *= AIR_MOVEMENT_ACCELERATION;

            // Accelerate the character according to its orientation to a surface:

            Matrix rot = Matrix.CreateRotationZ( m_surface_rotation );

            // If we are not on a contact surface then ignore rotation:

            if ( m_flattest_contact_surface.ValidResult == false ) rot = Matrix.Identity;           

            // Calculate amount of acceleration and add to the movement velocity:

            m_move_velocity += acceleration * Vector2.Transform( Vector2.UnitX , rot );

            // Make sire the movement velocity is past the maximum:

            float speed = m_move_velocity.Length(); 

            if ( speed > m_top_speed )
            {
                // Going too fast: slow down

                m_move_velocity /= speed; m_move_velocity *= m_top_speed;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Updates what animations are playing for the character. Each implementation of 
        /// 'Character' should override this function.
        /// </summary>
        //=========================================================================================

        protected virtual void UpdateAnimations(){}

        //=========================================================================================
        /// <summary>
        /// Performs update logic for the character. Does collision detection, surface 
        /// orientation and character physics.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Call base class function

            base.OnUpdate();

            UpdateBoundingVolumes();    // Update the characters bounding volumes
            ApplyGravity();             // Apply gravity to the character:
            FindContactSurfaces();      // Find what surfaces the ninja is on
            ApplySurfaceResistance();   // Apply surface resistance: must be done after FindContactSurfaces()
            ApplyFriction();            // Apply friction to our character
            ApplyPlatformJumpBoost();   // If we are moving up through a platform then give the character a little boost

            // Move the character: according to both regular and movement velocity

            Position += m_velocity;
            Position += m_move_velocity;
            
            OrientToSurface();          // Orient character to the surface it's on and figure out the jump surface           
            DoCollisionDetection();     // Do collision detection:

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // If the character is air-borne then transfer normal velocity into movement velcity 
            // gradually: this allows the character to regain control when in mid air, against 
            // forces created from jumps.
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            if ( m_flattest_contact_surface.ValidResult == false )
            {
                // Calculate amount to transfer:

                float transfer = VelocityX * ( 1.0f - NORMAL_VELOCITY_TRANSFERENCE );

                // Do the transfer:

                VelocityX -= transfer; m_move_velocity.X += transfer;
            }

            // Deaccelerate movement velocity:

            m_move_velocity *= m_deacceleration;

            // If move velocity is small then get rid of it completely:

            if ( m_move_velocity.Length() <= MOVE_VELOCITY_DEAD_ZONE ) 
            {
                m_move_velocity = Vector2.Zero;
            }
                       
            UpdateAnimations();                 // Update character animations      
            UpdateCharacterFlipDirection();     // Update which way the character is facing:

            // If we are dead.. then we are dead!

            if ( m_health <= 0 )
            {
                // Uh oh.. we are dead. Remove from the level

                Core.Level.Data.Remove(this);
            }

            // If spawning then reduce the spawn time left and do lightning:

            if ( m_spawn_time_left > 0 ) 
            {
                // Reduce the spawn time left

                m_spawn_time_left -= Core.Timing.ElapsedTime;

                // Do spawn lightning:

                DoSpawnLightning();
            }
        }

        //=========================================================================================
        /// <summary>
        /// On level loaded function. Called after level has finished loading.
        /// </summary>
        //=========================================================================================

        public override void OnLevelLoaded()
        {
            // Call the base class

            base.OnLevelLoaded();

            // Update bounding box:

            UpdateBoundingVolumes();
        }

        //=========================================================================================
        /// <summary>
        /// Updates the size of the characters bounding volumes according to the animation file 
        /// settings and the orientation of the character. The box is increased in size to accomodate
        /// for rotation but is still kept axis aligned. That will do for rough overlap tests when 
        /// slashing with swords etc.
        /// </summary>
        //=========================================================================================

        private void UpdateBoundingVolumes()
        {
            // See if we have an animation file:

            if ( m_animation != null )
            {
                // Good grab the bb and ellipsed dimensions:

                BoxDimensionsX = m_animation.BoxDimensions.X;
                BoxDimensionsY = m_animation.BoxDimensions.Y;

                m_ellipse_dimensions.X = m_animation.EllipseDimensions.X;
                m_ellipse_dimensions.Y = m_animation.EllipseDimensions.Y;

                // Now make the four points of the players bb and rotate them by surface orientation:

                Vector2 p1 = Vector2.Zero; p1.X -= BoxDimensionsX; p1.Y += BoxDimensionsY;
                Vector2 p2 = Vector2.Zero; p2.X += BoxDimensionsX; p2.Y += BoxDimensionsY;
                Vector2 p3 = Vector2.Zero; p3.X += BoxDimensionsX; p3.Y -= BoxDimensionsY;
                Vector2 p4 = Vector2.Zero; p4.X -= BoxDimensionsX; p4.Y -= BoxDimensionsY;

                Matrix rot = Matrix.CreateRotationZ( m_surface_rotation );

                p1 = Vector2.Transform( p1 , rot );
                p2 = Vector2.Transform( p2 , rot );
                p3 = Vector2.Transform( p3 , rot );
                p4 = Vector2.Transform( p4 , rot );
                
                // Now find the largest dimensions in x and y:

                float largest_x = Math.Abs( p1.X );
                float largest_y = Math.Abs( p1.Y );

                if ( Math.Abs(p2.X) > largest_x ) largest_x = Math.Abs(p2.X);
                if ( Math.Abs(p3.X) > largest_x ) largest_x = Math.Abs(p3.X);
                if ( Math.Abs(p4.X) > largest_x ) largest_x = Math.Abs(p4.X);

                if ( Math.Abs(p2.Y) > largest_y ) largest_y = Math.Abs(p2.Y);
                if ( Math.Abs(p3.Y) > largest_y ) largest_y = Math.Abs(p3.Y);
                if ( Math.Abs(p4.Y) > largest_y ) largest_y = Math.Abs(p4.Y);

                // Save the new bb dimensions:

                BoxDimensionsX = largest_x;
                BoxDimensionsY = largest_y;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Checks to see if the character is on the ground or touching a surface such as a wall. 
        /// This fucntion saves both the flatest and steepest surfaces.
        /// </summary>
        //=========================================================================================

        private void FindContactSurfaces()
        {
            // Makeup a new bounding elipse for the character and expand it slightly:

            Vector2 expanded_elipse_dimensions = EllipseDimensions;

            expanded_elipse_dimensions.X += CONTACT_SURFACE_DISTANCE;
            expanded_elipse_dimensions.Y += CONTACT_SURFACE_DISTANCE;

            // Shorten things:

            LevelCollisionQuery c = Core.Level.Collision;

            // Do collision detection with the level:

            c.Collide( Position , expanded_elipse_dimensions , m_surface_rotation , this );

            // See if there is any results:

            if ( c.CollisionResultCount > 0 )
            {
                // Got results: save the result with the flatest surface. Use the normal to determine this.

                int flattest_s_index = -1;

                for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
                {
                    // See if this surface is flatter

                    if ( flattest_s_index == -1 || c.CollisionResults[i].Normal.Y > c.CollisionResults[flattest_s_index].Normal.Y )
                    {
                        // Only do if we are travelling towards the surface:

                        if ( Vector2.Dot( m_velocity + m_move_velocity , c.CollisionResults[i].Normal ) <= 0.001f )
                        {
                            // Flatter: save it's index

                            flattest_s_index = i;
                        }
                    }
                }

                // Save the flattest contact surface found:

                if ( flattest_s_index >= 0 )
                {
                    m_flattest_contact_surface = Core.Level.Collision.CollisionResults[flattest_s_index];
                }
                else
                {
                    m_flattest_contact_surface = CollisionQueryResult.NoResult;
                }

                // Now find the steepest surface: again use the normal

                int steepest_s_index = -1;

                for ( int i = 0 ; i < Core.Level.Collision.CollisionResultCount ; i++ )
                {
                    // See if this surface is steeper

                    if ( steepest_s_index == -1 || c.CollisionResults[i].Normal.Y < c.CollisionResults[steepest_s_index].Normal.Y )
                    {
                        // Only do if we are travelling towards the surface:
                        
                        if ( Vector2.Dot( m_velocity + m_move_velocity , c.CollisionResults[i].Normal ) <= 0.001f )
                        {
                            // Steeper: save it's index

                            steepest_s_index = i;
                        }
                    }
                }

                // Save the steepest surface found:

                if ( steepest_s_index >= 0 )
                {
                    m_steepest_contact_surface = Core.Level.Collision.CollisionResults[steepest_s_index];
                }
                else
                {
                    m_steepest_contact_surface = CollisionQueryResult.NoResult;
                }
            }
            else
            {
                // No collision results: not in contact with anything

                m_flattest_contact_surface  = CollisionQueryResult.NoResult;
                m_steepest_contact_surface  = CollisionQueryResult.NoResult;
            }

        }

        //=========================================================================================
        /// <summary>
        /// Performs collision for the character. Takes a number of iterations to try and resolve 
        /// all collisions.
        /// </summary>
        //=========================================================================================

        private void DoCollisionDetection()
        {
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Keep doing collision detection until we have reached the maximum number of iterations, or 
            // if there is no collision.. DO ONLY PLANAR COLLISIONS FOR THESE ITERATION.
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            // Shorten code:

            LevelCollisionQuery c = Core.Level.Collision;

            //-------------------------------------------------------------------------------------
            // Do planar collisions first:
            //-------------------------------------------------------------------------------------

            for ( int i = 0 ; i < COLLISION_ITERATIONS ; i++ )
            {
                // Do collision detection:

                c.Collide(Position, EllipseDimensions, m_surface_rotation , this);

                // If there is no collision then bail out:

                if ( c.CollisionResultCount <= 0 ) break;

                // Solve the first planar collision found:

                for ( int j = 0 ; j < c.CollisionResultCount ; j++ )
                {
                    // Only do collision if we are travelling towards the surface:

                    if ( Vector2.Dot( m_velocity + m_move_velocity , c.CollisionResults[j].Normal ) <= 0.01f )
                    {
                        // See if this is a planar collision:

                        if ( c.CollisionResults[j].IsPointCollision == false )
                        {
                            // Resolve the collision

                            Position += c.CollisionResults[j].Penetration * c.CollisionResults[j].ResolveDirection;

                            // Resolve no more:

                            break;
                        }
                    }
                }

            }   // end of planar collisions

            //-------------------------------------------------------------------------------------
            // Do point collisions last:
            //-------------------------------------------------------------------------------------

            for ( int i = 0 ; i < COLLISION_ITERATIONS ; i++ )
            {
                // Do collision detection:

                c.Collide(Position, EllipseDimensions, m_surface_rotation , this);

                // If there is no collision then bail out:

                if ( c.CollisionResultCount <= 0 ) break;

                // Solve the first point collision found:

                for ( int j = 0 ; j < c.CollisionResultCount ; j++ )
                {
                    // See if this is a point collision:

                    if ( c.CollisionResults[j].IsPointCollision == true )
                    {
                        // Only do collision if we are travelling towards the surface:

                        if ( Vector2.Dot( m_velocity + m_move_velocity , c.CollisionResults[j].Normal ) <= 0.01f )
                        {
                            // Resolve the collision

                            Position += c.CollisionResults[j].Penetration * c.CollisionResults[j].ResolveDirection;

                            // Resolve no more:

                            break;
                        }
                    }
                }

            }   // end of point collisions

        }

        //=========================================================================================
        /// <summary>
        /// Updates whether the character is flipped left or right.
        /// </summary>
        //=========================================================================================

        private void UpdateCharacterFlipDirection()
        {
            // Get the players right direction with surface rotation taken into account:

            Vector2 right = Vector2.Transform( Vector2.UnitX , Matrix.CreateRotationZ(m_surface_rotation) );

            // Update the smoothed x motion direction

            if ( Vector2.Dot( right , m_velocity + m_move_velocity ) < - 0.01f )
            {
                // Smooth the smoothed x motion direction to -1 

                m_smoothed_x_motion_direction *= SMOOTHED_X_MOTION_SMOOTHING; 
                m_smoothed_x_motion_direction += ( 1.0f - SMOOTHED_X_MOTION_SMOOTHING ) * -1;
            }
            else if ( Vector2.Dot( right , m_velocity + m_move_velocity ) > 0.01f )
            {
                // Smooth the smoothed x motion direction to 1 

                m_smoothed_x_motion_direction *= SMOOTHED_X_MOTION_SMOOTHING; 
                m_smoothed_x_motion_direction += ( 1.0f - SMOOTHED_X_MOTION_SMOOTHING ) * 1;
            }
            else
            {
                // Smooth the smoothed x motion direction to 0

                m_smoothed_x_motion_direction *= SMOOTHED_X_MOTION_SMOOTHING; 
            }

            // Now update if the character is flipped depending on x motion direction:

            if ( m_smoothed_x_motion_direction < - SMOOTHED_X_MOTION_THRESHOLD )
            {
                // Moving left: no flip

                m_flipped = false;
            }
            else if ( m_smoothed_x_motion_direction > SMOOTHED_X_MOTION_THRESHOLD ) 
            {
                // Moving right: flip

                m_flipped = true;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Orients the character according to the surfaces they are in contact with. 
        /// This will cause the characters to run slightly up walls and orient to underlying terrain.
        /// Also determines a surface to jump off while its at it.
        /// </summary>
        //=========================================================================================

        private void OrientToSurface()
        {
            // Clear the jump surface:

            m_jump_surface.ValidResult = false;

            // Makeup a new bounding elipse for the character and expand it slightly:

            Vector2 expanded_elipse_dimensions = EllipseDimensions;

            expanded_elipse_dimensions.X += SURFACE_ORIENT_DISTANCE;
            expanded_elipse_dimensions.Y += SURFACE_ORIENT_DISTANCE;

            // Shorter:

            LevelCollisionQuery c = Core.Level.Collision;

            // Do collision detection with the level:

            c.Collide( Position , expanded_elipse_dimensions , m_surface_rotation , this );

            // If the character is not in contact with any surfaces then straighten it's orientation

            if ( c.CollisionResultCount <= 0 )
            {
                // See which way the surface angle of the character is:

                if ( m_surface_rotation < 0 )
                {
                    // Straigten up:

                    m_surface_rotation += AIR_ORIENT_STRAIGHTEN; 
                    
                    // Don't over do it:
                    
                    if ( m_surface_rotation > 0 ) m_surface_rotation = 0;
                }
                else
                {
                    // Straigten up:

                    m_surface_rotation -= AIR_ORIENT_STRAIGHTEN; 
                    
                    // Don't over do it:
                    
                    if ( m_surface_rotation < 0 ) m_surface_rotation = 0;
                }

                // Bail out after this: surface orientation is done

                return;
            }

            /******* OLD WAY OF DOING THINGS ***********************************************************************************************

            // Make a weighted average resolve direction for all the things collided with:

            Vector2 average_resolve_dir = Vector2.Zero;

            for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
            {
                // Ignore this surface if it is almost or is a ceiling:

                if ( c.CollisionResults[i].Normal.Y < MIN_ORIENT_SURFACE_NORMAL_Y ) continue;
                
                // - old way -
                //
                // Get the speed of the character in relation to the surface normal:
                //
                // float dot = Math.Abs( Vector2.Dot( - m_velocity - m_move_velocity , c.CollisionResults[i].ResolveDirection ) );
                //

                // Get the distance to the collision point:

                float d = Vector2.Distance( Position , c.CollisionResults[i].Point );

                // Save this surface also as the jump surface if there is none or if it is flatter than the current:

                if ( m_jump_surface.ValidResult == false || m_jump_surface.Normal.Y < c.CollisionResults[i].Normal.Y )
                {
                    // New jump surface: save

                    m_jump_surface = c.CollisionResults[i];
                }

                // Add to the results:

                // - old way -
                //
                // average_resolve_dir += c.CollisionResults[i].ResolveDirection * dot;
                //

                average_resolve_dir += c.CollisionResults[i].ResolveDirection;
            }

            // If the resolve dir is still zero then bail:

            if ( average_resolve_dir.Length() == 0 ) return;

            // Otherwise normalise:

            average_resolve_dir.Normalize();

            // Turn it into a surface direction:

            Vector2 surf_dir = new Vector2
            (
                + ( average_resolve_dir.Y ) ,
                - ( average_resolve_dir.X )
            );

            // Get an up vector for the character rotated by the current surface rotation:

            Vector2 char_up_vec = Vector2.UnitY; 

            // Make a rotation matrix:

            Matrix rot = Matrix.CreateRotationZ(m_surface_rotation); 

            // Do the rotation:

            char_up_vec = Vector2.Transform( char_up_vec , rot );            

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Get the cosine of the angle between the average resolve direction and rotated character 
            // up vector: should be zero when character is perfectly oriented with the surface
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
             
            float cos = Vector2.Dot( surf_dir , char_up_vec );

            // Get this cosine as an angle: but use sine function instead to get angular diference from the two vectors being perpendicular

            float angle = (float) Math.Asin(cos);

            // Now rotate the player towards being perpendicular with the underlying suface

            {
                // Save this variable temporarily:

                float t = m_surface_rotation;

                // Orient towards new orientation:

                m_surface_rotation *= SURFACE_ORIENT_SPEED; m_surface_rotation += ( t + angle ) * ( 1.0f - SURFACE_ORIENT_SPEED );
            }
             
            *****************************************************************************************************************************/

            // Store the average angle difference between the player and each surface here:

            float average_angle_difference = 0;

            // Find the average angle difference:

            for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
            {
                // Ignore this surface if the resolve dir is not right

                if ( c.CollisionResults[i].ResolveDirection.Y < MIN_ORIENT_SURFACE_NORMAL_Y ) continue;

                // Save this surface also as the jump surface if there is none or if it is flatter than the current:

                if ( m_jump_surface.ValidResult == false || m_jump_surface.Normal.Y < c.CollisionResults[i].Normal.Y )
                {
                    // New jump surface: save

                    m_jump_surface = c.CollisionResults[i];
                }

                // Get the angle of this surfaces normal:

                float angle = (float) Math.Acos( c.CollisionResults[i].ResolveDirection.X );

                // Make so that up is angle zero:
                
                angle -= MathHelper.PiOver2;

                // Add to the average angle difference:

                average_angle_difference += angle - m_surface_rotation;
            }

            // Find the average angle difference:

            average_angle_difference /= c.CollisionResultCount;

            // Need this temporary:

            float t = m_surface_rotation;

            // Interpolate towards our new orientation:

            m_surface_rotation *= SURFACE_ORIENT_SPEED; m_surface_rotation += ( t + average_angle_difference ) * ( 1.0f - SURFACE_ORIENT_SPEED );
        }

        //=========================================================================================
        /// <summary>
        /// Applys surface resistance to the velocitity of the character.
        /// NOTE !! : This function assumes that FindContactSurfaces() is called beforehand and 
        /// that the collision results are still contained in the LevelCollisionQuery object.
        /// It also relies on the JumpSurface being updated in OrientToSurface()
        /// </summary>
        //=========================================================================================

        private void ApplySurfaceResistance()
        { 
            // Shorten things:

            LevelCollisionQuery c = Core.Level.Collision;

            // Run through all the contact surfaces found:

            for ( int i = 0 ; i < c.CollisionResultCount ; i++ )
            {
                // Get the amount of velocity the character has in relation to the surface normal direction:
                
                float vel = Vector2.Dot( m_velocity , c.CollisionResults[i].ResolveDirection );

                // Only do this if the answer is negative: this means the character is travelling towards the surface

                if ( vel < 0 )
                {
                    // Subtract this amount of velocity from the player in direction of the resolve dir:

                    m_velocity -= c.CollisionResults[i].ResolveDirection * vel;

                    // Do surface repulsion:

                    m_velocity += c.CollisionResults[i].ResolveDirection * SURFACE_REPULSION_VELOCITY;
                }

                // Do the same for movement velocity:

                vel = Vector2.Dot( m_move_velocity , c.CollisionResults[i].ResolveDirection );

                if ( vel < 0 )
                {
                    // Subtract this amount of velocity from the player in direction of the resolve dir:

                    m_move_velocity -= c.CollisionResults[i].ResolveDirection * vel;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Applys the force of gravity to the character.
        /// </summary>
        //=========================================================================================

        private void ApplyGravity()
        { 
            m_velocity.Y -= GRAVITY; 
        }

        //=========================================================================================
        /// <summary>
        /// Applys surface friction to the characer, slowing it down.
        /// </summary>
        //=========================================================================================

        private void ApplyFriction()
        {
            //-------------------------------------------------------------------------------------
            // See if there is a contact surface: note if the character is teethering over an edge 
            // (which would be a point collision) then do not apply friction and allow the character
            // to slip over it gradually- unless they move against it.
            //-------------------------------------------------------------------------------------

            if ( m_flattest_contact_surface.ValidResult && m_flattest_contact_surface.IsPointCollision == false )
            {
                // Get the player's velocity:

                float vel = m_velocity.Length();

                // Don't do friction with walls or ceilings:

                if ( m_flattest_contact_surface.Normal.Y <= 0 ) return;

                // Get the direction of velocity:

                Vector2 vel_dir = m_velocity; vel_dir /= vel;

                // Reduce the friction amount with how steep the surface is:

                float friction = m_friction * m_steepest_contact_surface.ResolveDirection.Y * m_steepest_contact_surface.ResolveDirection.Y;

                // See if the velocity is less than the friction strength- if so then do a dead stop

                if ( vel < friction )
                {
                    m_velocity = Vector2.Zero;
                }
                else
                {
                    m_velocity -= vel_dir * friction;
                }

                // If the velocity is very small then zero it and abort:

                if ( vel < 0.1f ){ m_velocity = Vector2.Zero; }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Gives a little boost to the character as it is going up through platforms. Most of the 
        /// time when the character jumps up through platforms it will go right through seamlessly.
        /// However if the Y velocity of the character begins to negate whilst it is still going 
        /// through then collision detection will again be switched on and the character will 
        /// jump up suddenly. This hack aims to stop this occurance from happenning. 
        /// NOTE!: This function relys on FindContactSurfaces having been called beforehand.
        /// </summary>
        //=========================================================================================

        private void ApplyPlatformJumpBoost()
        {
            // Store the flattest surface we are in contact with here

            CollisionQueryResult flattest_surface = CollisionQueryResult.NoResult;
            
            // Use this to shorten code:

            LevelCollisionQuery c = Core.Level.Collision;

            //-------------------------------------------------------------------------------------
            // Run through the results from FindContactSurface() and find the flattest surface we are 
            // in contact with, that we are moving away from. FindContactSurface() already finds the 
            // flattest contact surface that we are in contact with but it's the surface we are moving 
            // TOWARDS, not away from. We need the opposite in this case.
            //-------------------------------------------------------------------------------------

            for ( int i = 0 ; i < c.CollisionResultCount; i++ )
            {
                // See if this surface is flatter

                if ( flattest_surface.ValidResult == false || c.CollisionResults[i].Normal.Y > flattest_surface.Normal.Y )
                {
                    // Only do if we are travelling away from the surface:

                    if ( Vector2.Dot( m_velocity + m_move_velocity , c.CollisionResults[i].Normal ) > 0.05f + SURFACE_REPULSION_VELOCITY )
                    {
                        // Flatter: save..

                        flattest_surface = c.CollisionResults[i];
                    }
                    else
                    {
                        // If we are very very close to the surface then give a little boost also:

                        if ( c.CollisionResults[i].Penetration > m_ellipse_dimensions.Y * 0.75f )
                        {
                            // Flatter: save..

                            flattest_surface = c.CollisionResults[i];
                        }
                    }
                }
            }

            // Take the flattest surface for this: if it doesn't exist then bail

            if ( flattest_surface.ValidResult == false ) return;

            // We are moving away from this surface: give a boost

            m_velocity += flattest_surface.ResolveDirection * GRAVITY * PLATFORM_JUMP_UP_GRAVITY_BOOST;
        }

        //=========================================================================================
        /// <summary>
        /// Given a line representing what a sword can hit, this function checks to see if it is inside
        /// or overlapping the rotated bounding box of the character (rotated by surface orientation) 
        /// and returns true if there is an overlap or if it is inside the box. This function is used 
        /// for hit detection with melee combat.
        /// </summary>
        /// <param name="sword_start_pt"> Start point of the line representing the sword swipe  </param>
        /// <param name="sword_end_pt">   End point of the line representing the sword swipe    </param>
        //=========================================================================================

        public bool CheckSwordHit( Vector2 sword_start_pt , Vector2 sword_end_pt )
        {
            // Get the points relative to our bounding box:

            sword_start_pt  -= Position;
            sword_end_pt    -= Position;

            //-------------------------------------------------------------------------------------
            // Get the real dimensions of the bounding box. BoxDimensions is gotten from taking the 
            // bounding box size stored in the animation file and rotating it, taking the maximum 
            // size as the current size of the bounding box. It is thus distorted when rotation happens.
            // This is ok for rough testing, but we don't want it here.
            //-------------------------------------------------------------------------------------

            Vector2 box_dimensions = BoxDimensions;

            if ( m_animation != null )
            {
                box_dimensions = m_animation.BoxDimensions;
            }
            
            // Now make the four points of the players actual bb and rotate them by surface orientation:

            Vector2 p1 = Vector2.Zero; p1.X -= BoxDimensionsX; p1.Y += BoxDimensionsY;
            Vector2 p2 = Vector2.Zero; p2.X += BoxDimensionsX; p2.Y += BoxDimensionsY;
            Vector2 p3 = Vector2.Zero; p3.X += BoxDimensionsX; p3.Y -= BoxDimensionsY;
            Vector2 p4 = Vector2.Zero; p4.X -= BoxDimensionsX; p4.Y -= BoxDimensionsY;

            Matrix rot = Matrix.CreateRotationZ( m_surface_rotation );

            p1 = Vector2.Transform( p1 , rot );
            p2 = Vector2.Transform( p2 , rot );
            p3 = Vector2.Transform( p3 , rot );
            p4 = Vector2.Transform( p4 , rot );

            // Now get the normals for this box:

            Vector2 n1 = new Vector2( -p1.Y , p1.X );
            Vector2 n2 = new Vector2( -p2.Y , p2.X );
            Vector2 n3 = new Vector2( -p3.Y , p3.X );
            Vector2 n4 = new Vector2( -p4.Y , p4.X );

            // Ok: now see if one of the points is inside the box: return true if it is 

            if 
            ( 
                    Vector2.Dot( sword_start_pt - p1 , n1 ) < 0 &&
                    Vector2.Dot( sword_start_pt - p2 , n2 ) < 0 &&
                    Vector2.Dot( sword_start_pt - p3 , n3 ) < 0 &&
                    Vector2.Dot( sword_start_pt - p4 , n4 ) < 0
            )
            {
                // Point in the box: intersection, return true

                return true;
            }

            if 
            ( 
                    Vector2.Dot( sword_end_pt - p1 , n1 ) < 0 &&
                    Vector2.Dot( sword_end_pt - p2 , n2 ) < 0 &&
                    Vector2.Dot( sword_end_pt - p3 , n3 ) < 0 &&
                    Vector2.Dot( sword_end_pt - p4 , n4 ) < 0
            )
            {
                // Point in the box: intersection, return true

                return true;
            }

            // Ok: that failed. Make four lines and see if the the sword line interects it

            Line l1 = new Line( p1 , p2 );
            Line l2 = new Line( p2 , p3 );
            Line l3 = new Line( p3 , p4 );
            Line l4 = new Line( p4 , p1 );

                // Make the sword line too

                Line sword_line = new Line( sword_start_pt , sword_end_pt );

                // Save intersection point here

                Vector2 intersection_point = Vector2.Zero;

                // Check to see if sword line intersects bound box lines

                if ( l1.Intersect(sword_line,ref intersection_point) ) return true;
                if ( l2.Intersect(sword_line,ref intersection_point) ) return true;
                if ( l3.Intersect(sword_line,ref intersection_point) ) return true;
                if ( l4.Intersect(sword_line,ref intersection_point) ) return true;

            // If we have got to here there is no intersection: return false

            return false;
        }

        //=========================================================================================
        /// <summary>
        /// Applys damage to the character. Virtual so that derived classes can implement special 
        /// events for this action.
        /// </summary>
        /// <param name="amount">   Amount of damage to apply to the character.     </param>
        /// <param name="damager">  Thing that damaged the character. May be null.  </param>
        //=========================================================================================

        public virtual void Damage( float amount , GameObject damager )
        {
            // If the amount is nothing then do not bother:

            if ( amount <= 0 ) return;

            // Damage the character:

            m_health -= amount;

            // If health is less than zero: make zero

            if ( m_health < 0 ) m_health = 0;
        }

        //=========================================================================================
        /// <summary>
        /// Applys healing to the character. Virtual so that derived classes can implement special 
        /// events for this action.
        /// </summary>
        /// <param name="amount"> Amount of healing to apply to the character. </param>
        //=========================================================================================

        public virtual void Heal( float amount )
        {
            // If the amount is nothing then do not bother:

            if ( amount <= 0 ) return;

            // Heal the character:

            m_health += amount;

            // Clamp to the maximum health:

            if ( m_health > m_max_health ) m_health = m_max_health;
        }

        //=========================================================================================
        /// <summary>
        /// Draws a given part of the character.
        /// </summary>
        /// <param name="part_name"> Name of the part to draw </param>
        //=========================================================================================

        protected void DrawCharacterPart( string part_name )
        {
            // Do nothing if we have no effect:

            if ( m_effect == null ) return;

            // Try and get the given body part:

            AnimationPart part = m_animation.GetPart(part_name);

            // If it doesn't exist then abort:

            if ( part == null ) return;

            // Set graphics device vertex declaration:

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

            // Get view camera transforms:

            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Set the texture and transform on the shader:

            EffectParameter param_tex = m_effect.Parameters["Texture"];
            EffectParameter param_wvp = m_effect.Parameters["WorldViewProjection"];

            if ( param_tex != null && part.CurrentFrame != null ) 
            {
                param_tex.SetValue( part.CurrentFrame );
            }

            if ( param_wvp != null ) 
            {
                param_wvp.SetValue(view_projection);
            }

            // Calculate the alpha for the character based on the spawn time left:

            float alpha = 1.0f - ( MathHelper.Clamp( m_spawn_time_left / SPAWN_TIME , 0 , 1 ) );

            // Set the alpha for all the character's vertices:

            m_vertices[0].Color = new Color( new Vector4(1,1,1,alpha) );
            m_vertices[1].Color = new Color( new Vector4(1,1,1,alpha) );
            m_vertices[2].Color = new Color( new Vector4(1,1,1,alpha) );
            m_vertices[3].Color = new Color( new Vector4(1,1,1,alpha) );

            // Set the posiiton of all the sprites vertices with just offset and size taken into account:

            m_vertices[0].Position.X = - part.Size.X;
            m_vertices[1].Position.X = - part.Size.X;
            m_vertices[2].Position.X = + part.Size.X;
            m_vertices[3].Position.X = + part.Size.X;

            m_vertices[0].Position.Y = + part.Size.Y;
            m_vertices[1].Position.Y = - part.Size.Y;
            m_vertices[2].Position.Y = + part.Size.Y;
            m_vertices[3].Position.Y = - part.Size.Y;

            // If the sprite is flipped then flip the x offset:

            if ( m_flipped )
            {
                m_vertices[0].Position += new Vector3( part.Offset , 0 );
                m_vertices[1].Position += new Vector3( part.Offset , 0 );
                m_vertices[2].Position += new Vector3( part.Offset , 0 );
                m_vertices[3].Position += new Vector3( part.Offset , 0 );
            }
            else
            {
                m_vertices[0].Position += new Vector3( -part.Offset.X , part.Offset.Y , 0 );
                m_vertices[1].Position += new Vector3( -part.Offset.X , part.Offset.Y , 0 );
                m_vertices[2].Position += new Vector3( -part.Offset.X , part.Offset.Y , 0 );
                m_vertices[3].Position += new Vector3( -part.Offset.X , part.Offset.Y , 0 );
            }

            // Rotate all the vertices by the surface orientation of the character:

            {
                // Make a rotation matrix to do this:

                Matrix rotation = Matrix.CreateRotationZ( m_surface_rotation );

                // Do the rotations:

                m_vertices[0].Position = Vector3.Transform( m_vertices[0].Position , rotation );
                m_vertices[1].Position = Vector3.Transform( m_vertices[1].Position , rotation );
                m_vertices[2].Position = Vector3.Transform( m_vertices[2].Position , rotation );
                m_vertices[3].Position = Vector3.Transform( m_vertices[3].Position , rotation );
            }

            // Now move the vertices according to the character position:

            m_vertices[0].Position += new Vector3( Position , 0 );
            m_vertices[1].Position += new Vector3( Position , 0 );
            m_vertices[2].Position += new Vector3( Position , 0 );
            m_vertices[3].Position += new Vector3( Position , 0 );

            // Set the z component of the vertices to zero:

            m_vertices[0].Position.Z = 0;
            m_vertices[1].Position.Z = 0;
            m_vertices[2].Position.Z = 0;
            m_vertices[3].Position.Z = 0;

            // Get the speed of the character in relation to it's orientation

            float dot = 0;

            {
                // Get the right vector for the character

                Vector2 right = Vector2.UnitX;

                // Will be rotated according to surface orientation:

                Matrix rot = Matrix.CreateRotationZ( m_surface_rotation );

                // Do the rotation:

                right = Vector2.Transform( right , rot );

                // Get the speed in relation to orientation:

                dot =  Vector2.Dot( right , Velocity + m_move_velocity );
            }

            // Now depending on the result of what way the character is moving, flip the character back to normal or to the right 

            if ( m_flipped == false )
            {
                m_vertices[0].TextureCoordinate = Vector2.Zero;
                m_vertices[1].TextureCoordinate = Vector2.UnitY;
                m_vertices[2].TextureCoordinate = Vector2.UnitX;
                m_vertices[3].TextureCoordinate = Vector2.One;
            }
            else
            {
                m_vertices[0].TextureCoordinate = Vector2.UnitX;
                m_vertices[1].TextureCoordinate = Vector2.One;
                m_vertices[2].TextureCoordinate = Vector2.Zero;
                m_vertices[3].TextureCoordinate = Vector2.UnitY;
            }

            // Begin drawing with the shader:

            m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                // Draw the body part

                Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                (
                    PrimitiveType.TriangleStrip ,
                    m_vertices                  ,
                    0                           ,
                    2
                );

            // End drawing with the shader:

            m_effect.CurrentTechnique.Passes[0].End(); m_effect.End();           
        }

        //=========================================================================================
        /// <summary>
        /// Makes a debris object from the given part (given by name) at the character's current 
        /// position and that. 
        /// </summary>
        /// <param name="part_name"> Name of the part to make debris from. </param>
        //=========================================================================================

        protected void CreateDebrisFromPart( string part_name )
        {
            // Try and get the given body part:

            AnimationPart part = m_animation.GetPart(part_name);

            // If it doesn't exist then abort:

            if ( part == null ) return;

            // Get the position of the part and rotate according to how we are rotated:

            Vector2 part_pos = Vector2.Transform( part.Offset , Matrix.CreateRotationZ(m_surface_rotation) );

            // Now add the characters position to the part position:

            part_pos += Position;

            // Cool: now make a new debris object

            Debris d = new Debris
            (
                part_pos            ,
                part.Size           ,
                m_flipped           ,
                m_surface_rotation  ,
                0.50f               ,
                4.00f               ,
                0.01f               ,
                0.05f               ,
                0.5f                ,
                1.5f                ,
                20                  ,
                GRAVITY * 0.25f     ,
                part.CurrentFrame   ,
                m_effect
            );

            // Add the debris into the level:

            Core.Level.Data.Add(d);
        }

        //=========================================================================================
        /// <summary>
        /// Creates lightning out of the character if spawning.
        /// </summary>
        //=========================================================================================

        private void DoSpawnLightning()
        {
            // See if we are spawning:

            if ( m_spawn_time_left > 0 )
            {
                // Only do if time to spawn:

                if ( m_time_since_lightning >= LIGHTNING_INTERVAL )
                {
                    // Color for the lightning:

                    Vector4 lc = Vector4.One;

                    lc.X = 1.0f;
                    lc.Y = 0.2f;
                    lc.Z = 0.0f;
                    lc.W = 1.0f;

                    // Create particles with 1st emitter:

                    Core.Level.Emitter.CreateBurst
                    (
                        1                           ,
                        m_spawn_lightning_texture_1 ,
                        Position                    ,
                        0.05f                       ,
                        0.20f                       ,
                        12.0f                       ,
                        24.0f                       ,
                        lc                          ,
                        lc                          ,
                        100                         ,
                        200                        ,
                        0                           ,
                        false
                    );

                    // Create particles with 2nd emitter:

                    Core.Level.Emitter.CreateBurst
                    (
                        1                           ,
                        m_spawn_lightning_texture_2 ,
                        Position                    ,
                        0.05f                       ,
                        0.20f                       ,
                        12.0f                       ,
                        24.0f                       ,
                        lc                          ,
                        lc                          ,
                        100                         ,
                        200                         ,
                        0                           ,
                        false
                    );

                    // Reset time since lightning:

                    m_time_since_lightning = 0;
                }

                // Increment time since lightning:

                m_time_since_lightning += Core.Timing.ElapsedTime;
            }
        }        

        //=========================================================================================
        /// <summary> 
        /// Does setup for the characters sprite
        /// </summary>
        //=========================================================================================

        private void Setup()
        {
            // Create the sprite's vertices:

            m_vertices = new VertexPositionColorTexture[4];

            // Set the texture coordinates for each vertex:

            m_vertices[0].TextureCoordinate = Vector2.Zero;
            m_vertices[1].TextureCoordinate = Vector2.UnitY;
            m_vertices[2].TextureCoordinate = Vector2.UnitX;
            m_vertices[3].TextureCoordinate = Vector2.One;

            // Set the colors for each vertex:

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;

            // Set default depth:

            Depth = 1000;
        }

    }   // end of class

}   // end of namespace
