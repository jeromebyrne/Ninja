using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Represents a ninja character that is controllable by the player. Accepts both keyboard 
    /// and gamepad input.
    /// </summary>
    //#############################################################################################
    
    public class PlayerNinja : Ninja
    {
        //=========================================================================================
        // Properties
        //=========================================================================================
            
            /// <summary> Tells if the player can currently deflect projeciles. </summary>

            public bool CanDeflect 
            { 
                get 
                { 
                    // Can only deflect if blocking:

                    if ( m_blocking )
                    {
                        // Can only deflect if blocking for not too long

                        if ( m_time_blocking < DEFLECTION_MAX_BLOCK_TIME ) return true; else return false;
                    }
                    else
                    {
                        // Not blocking so can't deflect

                        return false;
                    }
                } 
            }

            /// <summary> Tells if the character is currently blocking </summary>
            
            public bool Blocking { get { return m_blocking; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> For gamepad input. This is a smoothed value for the right trigger. </summary>

            private float m_gamepad_right_trigger = 0;

            /// <summary> For gamepad input. This is a smoothed value for left stick movement. </summary>

            private float m_gamepad_left_stick_x = 0;

            /// <summary> For gamepad input. This is a smoothed value for right stick movement. </summary>

            private Vector2 m_gamepad_right_stick = Vector2.Zero;

            /// <summary> For simulated game pad movement using the keyboard. This variable holds what would be the left stick X vector value on the game pad </summary>

            private float m_keyboard_left_stick_x = 0;

            /// <summary> For simulated game pad movement using the keyboard. This variable holds what would be the right trigger on the game pad. </summary>

            private float m_keyboard_right_trigger = 0;

            /// <summary> For simulated game pad movement using the keyboard. This is a smoothed value for right stick movement. </summary>

            private Vector2 m_keyboard_right_stick = Vector2.Zero;

            /// <summary> The last known state of the keyboard. </summary>

            private KeyboardState m_last_kb_state;

            /// <summary> The last known state of the gamepad. </summary>

            private GamePadState m_last_gp_state;

            /// <summary> Type for a character. Stored here for easy access. </summary>

            private Type m_character_type = null;

            /// <summary> Time since the last footstep. Time is not neccesarily real time, and may be altered up depending on how fast the character is moving. </summary>

            private float m_time_since_last_footstep = 0;

            /// <summary> System.Type for a pickup object. Stored here for later use. </summary>

            private Type m_pickup_type = null;
            
            /// <summary> A list of active projectiles that the ninja has fired. Needed for aftertouch. </summary>

            private LinkedList<Projectile> m_projectiles = new LinkedList<Projectile>();

            /// <summary> The last total velocity of the character (movement and normal velocity combined). Used for velocity change rumbles. </summary>

            private Vector2 m_last_total_velocity = Vector2.Zero;

            /// <summary> The last total amount of rumble due to velocity change </summary>

            private float m_current_impact_rumble = 0;

            /// <summary> Last surface the player was in contact with. Use for doing impact rumbles. </summary>

            private CollisionQueryResult m_last_contact_surface = CollisionQueryResult.NoResult;

            /// <summary> Last amount of health the player had. Used for pain rumbles. </summary>

            private float m_last_health = 0;

            /// <summary> The last total amount of rumble due to health change or pain </summary>

            private float m_current_pain_rumble = 0;

            /// <summary> Tells if the character is currently blocking </summary>
            
            private bool m_blocking = false;

            /// <summary> How long has the player been blocking </summary>

            private float m_time_blocking = 0;

            /// <summary> Time since the player last emit glow particles for when blocking attacks. </summary>

            private float m_time_since_blocking_glow_particles = 0;

            /// <summary> Texture to use for the particle glow. </summary>

            private Texture2D m_glow_texture = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> The amount to smooth simulated left analog stick movement from the keyboard </summary>

            private const float KEYBOARD_LS_SMOOTH = 0.90f;

            /// <summary> The amount to smooth simulated right trigger movement from the keyboard </summary>

            private const float KEYBOARD_RT_SMOOTH = 0.15f;

            /// <summary> The amount to smooth simulated right stick movement from the keyboard </summary>

            private const float KEYBOARD_RS_SMOOTH = 0.75f;

            /// <summary> The amount to smooth left analog stick movement from the gamepad </summary>

            private const float GAMEPAD_LS_SMOOTH = 0.70f;

            /// <summary> The amount to smooth right analog stick movement from the gamepad </summary>

            private const float GAMEPAD_RS_SMOOTH = 0.85f;

            /// <summary> The amount to smooth right trigger movement from the gamepad </summary>

            private const float GAMEPAD_RT_SMOOTH = 0.80f;

            /// <summary> The point at which simulated analog gamepad input from the keyboard is considered insignificant </summary>

            private const float KEYBOARD_DEAD_ZONE = 0.1f;

            /// <summary> The amount that the camera smooths towards the player </summary>

            private const float CAMERA_FOLLOW_SMOOTH = 0.88f;

            /// <summary> The rate that footsteps play at, in steps per second. </summary>

            private const float FOOTSTEP_FRAME_RATE = 2.5f;

            /// <summary> Maximum footstep frame rate, regardless of speed </summary>

            private const float FOOTSTEP_FRAME_RATE_MAX = 5.0f;

            /// <summary> Amount that speed affects how fast footstep sounds play. </summary>

            private const float FOOTSTEP_SPEED_SCALE = 0.25f;

            /// <summary> Minimum value required from smoothed thumbsticks value to fire a shuriken. </summary>

            private const float THUMBSTICK_SHURIKEN_FIRE_THRESHOLD = 0.2f;

            /// <summary> This affects how much velocity change (from an impact with a surface or from jumping) affects vibration  </summary>

            private const float IMPACT_RUMBLE_SCALE = 0.1f;

            /// <summary> Affects how smoothly rumbles from velocity change (from an impact with a surface or from jumping) are slowed down. </summary>

            private const float IMPACT_RUMBLE_SLOWDOWN = 0.85f;

            /// <summary> Amount at which impact rumbles are zeroed to nothing </summary>

            private const float IMPACT_RUMBLE_DEAD_ZONE = 0.25f;

            /// <summary> This affects how much health change affects vibration due to pain </summary>

            private const float PAIN_RUMBLE_SCALE = 2.0f;

            /// <summary> Affects how smoothly rumbles from health change are slowed down. </summary>

            private const float PAIN_RUMBLE_SLOWDOWN = 0.90f;

            /// <summary> Amount at which pain rumbles are zeroed to nothing </summary>

            private const float PAIN_RUMBLE_DEAD_ZONE = 0.125f;

            /// <summary> Interval in between glow particles being shown around the player when blocking / deflecting. </summary>

            private const float BLOCKING_GLOW_PARTICLE_INTERVAL = 0.05f;

            /// <summary> If the player has been blocking for less than this time then he will deflect the shuriken </summary>

            private const float DEFLECTION_MAX_BLOCK_TIME = 0.5f;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates this object.
        /// </summary>
        //=========================================================================================

        public PlayerNinja() : base()
        {
            // Get this type

            m_pickup_type =  Type.GetType("NinjaGame.Pickup");

            // Save this:

            m_character_type = Type.GetType("NinjaGame.Character");

            // Load glow texture:

            m_glow_texture = Core.Graphics.LoadTexture("Graphics\\Particles\\Glow");

            // Reset all varaibles

            m_last_kb_state = Keyboard.GetState();
            m_last_gp_state = GamePad.GetState( PlayerIndex.One );

            // Settings for this type of character

            MaximumHealth   = 500;
            m_last_health   = MaximumHealth;
            TopSpeed        = 12;
            JumpForce       = 14;
            Acceleration    = 0.9f;
            Deacceleration  = 0.95f;
            AttackDamage    = 300.0f;

            // Heal up fully

            Heal(MaximumHealth);
        }

        //=========================================================================================
        /// <summary>
        /// Each character should implement this function and load it's own animation in this function.
        /// </summary>
        //=========================================================================================

        public override void LoadAnimation()
        {
            // Load the animation for the player ninja:

            Animation = new Animation( "Content\\Animations\\Ninja.xml" );
        }

        //=========================================================================================
        /// <summary>
        /// Update function for a player ninja. Allows the ninja to be controlled by the player.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Update base class

            base.OnUpdate();

            // Update footstep sounds:

            DoFootstepSounds();

            // Check for pickups:

            CheckForPickups();

            // Update blocking glow particles

            UpdateBlockingGlowParticles();

            // Process player input
            
            ProcessInput();

            // Now make our camera follow us: smooth it gradually

            Core.Level.Renderer.Camera.Position = Vector2.Lerp
            ( 
                Core.Level.Renderer.Camera.Position , 
                Position                            , 
                1.0f - CAMERA_FOLLOW_SMOOTH 
            );

            // Update impact rumbles

            UpdateRumbles();
        }

        //=========================================================================================
        /// <summary>
        /// Does rumbles for when the player jumps and hits a surface, and also for when the player 
        /// is hit.
        /// </summary>
        //=========================================================================================

        private void UpdateRumbles()
        {
            // Make sure the gampad is connected:

            if ( GamePad.GetState(PlayerIndex.One).IsConnected == false ) return;

            // If the player is dead then set impact rumbles to off:

            if ( Health <= 0 )
            { 
                GamePad.SetVibration( PlayerIndex.One , 0 , 0 ); return; 
            }

            // Figure out the new amount of rumble due to velocity change:

                // Get our current total velocity:

                Vector2 total_velocity = Velocity + MoveVelocity;

                // See if we have left or just hit a surface:

                if ( FlattestContactSurface.ValidResult != m_last_contact_surface.ValidResult )
                {
                    // Jumped or left a surface: get the difference in velocity between the last frame

                    Vector2 velocity_difference = total_velocity - m_last_total_velocity;

                    // Add to the current impact rumbling by this amount:

                    m_current_impact_rumble += velocity_difference.Length() * IMPACT_RUMBLE_SCALE;
                }

                // Slow down impact rumbling

                m_current_impact_rumble *= IMPACT_RUMBLE_SLOWDOWN;

                // Do impact rumble dead zoning:

                if ( m_current_impact_rumble < IMPACT_RUMBLE_DEAD_ZONE ) m_current_impact_rumble = 0;

                // Save the current contact surface as the last:

                m_last_contact_surface = FlattestContactSurface;

                // Save the current total velocity as the last:

                m_last_total_velocity = total_velocity;

            // Figure out the amount of rumble due to pain:

                // Get the difference in health from the last frame:

                float health_difference = Health - m_last_health;

                // Negate and if negative then zero since that means we got health rather than lose it

                health_difference *= -1; 
            
                if ( health_difference < 0 ){ health_difference = 0; }

                // Increase pain rumbling due to this change:

                m_current_pain_rumble += health_difference * PAIN_RUMBLE_SCALE;

                // Slow down pain rumbling

                m_current_pain_rumble *= PAIN_RUMBLE_SLOWDOWN;

                // Do pain rumble dead zoning:

                if ( m_current_pain_rumble < PAIN_RUMBLE_DEAD_ZONE ) m_current_pain_rumble = 0;

                // Save current health as the last:

                m_last_health = Health;

            // Set the rumble on the gamepad:

            GamePad.SetVibration(PlayerIndex.One,m_current_pain_rumble,m_current_impact_rumble);
        }

        //=========================================================================================
        /// <summary>
        /// On level loaded function for the character.
        /// </summary>
        //=========================================================================================

        public override void OnLevelLoaded()
        {
            // Call base function

            base.OnLevelLoaded();

            // Set spawn time left to zero:

            SpawnTimeLeft = 0;

            // Set the camera position to our position:

            Core.Level.Renderer.Camera.Position = Position;

            // Save the last health:

            m_last_health = Health;
        }

        //=========================================================================================
        /// <summary>
        /// Processes all user input for controlling the ninja and makes the ninja 
        /// perform relevant actions.
        /// </summary>
        //=========================================================================================

        public void ProcessInput()
        {
            // Get the state of our controller and keyboard

            GamePadState  gps = GamePad.GetState( PlayerIndex.One );
            KeyboardState kps = Keyboard.GetState();

            // Update simulated game pad input from the keyboard

            UpdateSimulatedGamepadInput();

            // Update input from the gamepad:

            UpdateGamepadInput();

            // Apply aftertouch to all our shurikens:

            ApplyAftertouch();

            // Now move according to the pressure applied to the left stick and fake keyboard analog input. If its not being pressed then the player wont move.

            Accelerate( MathHelper.Clamp ( m_gamepad_left_stick_x + m_keyboard_left_stick_x , -1 , 1 ) * Acceleration );

            // Try firing if we can:

            // We cant do any firing or attacking while we are blocking

            if ( m_blocking == false )
            {
                if
                (
                    MathHelper.Clamp(Math.Abs(m_gamepad_right_stick.X + m_keyboard_right_stick.X), -1, 1)
                    >=
                    THUMBSTICK_SHURIKEN_FIRE_THRESHOLD
                    ||
                    MathHelper.Clamp(Math.Abs(m_gamepad_right_stick.Y + m_keyboard_right_stick.Y), -1, 1)
                    >=
                    THUMBSTICK_SHURIKEN_FIRE_THRESHOLD
                )
                {
                    // Add and clamp both values:

                    Vector2 sticks = m_gamepad_right_stick + m_keyboard_right_stick;

                    sticks.X = MathHelper.Clamp(sticks.X, -1, 1);
                    sticks.Y = MathHelper.Clamp(sticks.Y, -1, 1);

                    // Thumbsticks moved: fire a shuriken

                    Projectile shuriken = FireShuriken(sticks);

                    // See if one was actually fired:

                    if (shuriken != null)
                    {
                        // Add it into the list of shurikens we've fired

                        m_projectiles.AddLast(shuriken);
                    }
                }

                // Try attacking if we can: but only if attacl has just been pressed

                if 
                ( 
                    ( kps.IsKeyDown(Keys.Space) && m_last_kb_state.IsKeyUp(Keys.Space) )
                    || 
                    ( gps.Buttons.A == ButtonState.Pressed && m_last_gp_state.Buttons.A == ButtonState.Released )
                )
                {
                    // Attack !!

                    Attack();
                }
            }

            // See if jump was being held the last ime:

            if ( m_last_gp_state.Triggers.Right > 0 || m_last_kb_state.IsKeyDown(Keys.NumPad5) )
            {
                // Good: now see if jump isn't held now

                if 
                ( 
                    ( gps.Triggers.Right < m_last_gp_state.Triggers.Right || m_last_gp_state.Triggers.Right <= 0 ) 
                    && 
                    kps.IsKeyUp(Keys.NumPad5)
                )
                {
                    // Do the jump:

                    Jump( MathHelper.Clamp( m_keyboard_right_trigger + m_gamepad_right_trigger , 0, 1 ) * JumpForce );
                }
            }

            // Save last keyboard and gamepad states

            m_last_kb_state = kps;
            m_last_gp_state = gps;
        }

        //=========================================================================================
        /// <summary>
        /// Applys aftertouch to our own fired shurikens.
        /// </summary>
        //=========================================================================================

        private void ApplyAftertouch()
        {
            // Make a new list of shuirkens that we will apply aftertouch to for the next frame:

            LinkedList<Projectile> new_shuriken_list = new LinkedList<Projectile>();

            // Ok: run through the current list of projectiles we've fired:

            LinkedList<Projectile>.Enumerator e = m_projectiles.GetEnumerator();

            while ( e.MoveNext() )
            {
                // Get this projectile:

                Projectile p = e.Current;

                // Right: now see if it is dead or not

                if ( p.Dead == false )
                {
                    // Get the input from the real and simulated sticks

                    Vector2 sticks = m_keyboard_right_stick + m_gamepad_right_stick;

                    sticks.X = MathHelper.Clamp( sticks.X , -1 , 1 );
                    sticks.Y = MathHelper.Clamp( sticks.Y , -1 , 1 );

                    // Good: apply aftertouch to it

                    p.ApplyAftertouch( sticks );

                    // Now add to the list of shurikens to apply aftertouch to for the next frame

                    new_shuriken_list.AddLast(p);
                }
            }

            // Save the new list of shurikens:

            m_projectiles = new_shuriken_list;
        }

        //=========================================================================================
        /// <summary>
        /// Checks for pickups for the player and picks up any items that it is in contact with.
        /// </summary>
        //=========================================================================================

        private void CheckForPickups()
        {
            // Shorten code:

            LevelCollisionQuery c = Core.Level.Collision;

            // Do an overlap query with the level:

            c.Overlap(Position,BoxDimensions,this);

            // Run through all the results:

            for ( int i = 0 ; i < c.OverlapResultCount ; i++ )
            {
                // Make sure this is a pickup type object:

                if ( c.OverlapResults[i].QueryObject.GetType().IsSubclassOf(m_pickup_type) )
                {
                    // Good cast to a pickup and call the pickup function on it:

                    ((Pickup)(c.OverlapResults[i].QueryObject)).OnPickup(this);

                    // Now remove it from the level:

                    Core.Level.Data.Remove(c.OverlapResults[i].QueryObject);
                }
            }

        }

        //#########################################################################################
        /// <summary>
        /// Updates the glow particles that are shown around the player when blocking and deflecting 
        /// and makes new ones if needed.
        /// </summary>
        //#########################################################################################

        private void UpdateBlockingGlowParticles()
        {
            // See if currently blocking:

            if ( m_blocking )
            {
                // Blocking: good.. Now see if we can make a glow particle

                if ( m_time_since_blocking_glow_particles >= BLOCKING_GLOW_PARTICLE_INTERVAL )
                {
                    // Cool: reset the time since the last glow particle

                    m_time_since_blocking_glow_particles = 0;

                    // Pick a glow color:

                    Vector4 glow_color = Vector4.One;

                    glow_color.X = 0.5f;
                    glow_color.Y = 0.5f;
                    glow_color.Z = 1;
                    glow_color.W = 0.125f;

                    // Make the glow particle:

                    ParentLevel.Emitter.CreateBurst
                    (
                        5               ,
                        m_glow_texture  ,
                        Position        ,
                        0.45f           ,
                        0.75f           ,
                        48.0f           ,
                        64.0f           ,
                        glow_color      ,
                        glow_color      ,
                        10              ,
                        700             ,
                        0               ,
                        false
                    );
                }
            }

            // Increase time since last glow particle:
        
            m_time_since_blocking_glow_particles += Core.Timing.ElapsedTime;
        }

        //=========================================================================================
        /// <summary>
        /// Updates all input from the gamepad.
        /// </summary>
        //=========================================================================================

        private void UpdateGamepadInput()
        {
            // Get the state of the gamepad:

            GamePadState gps = GamePad.GetState(PlayerIndex.One);

            // if we are using the block button then we are blocking, digital input.

            if ( gps.Triggers.Left > 0 )
            {
                // Blocking: increase blocking time

                m_blocking = true; m_time_blocking += Core.Timing.ElapsedTime;
            }
            else
            {
                // Not blocking: decrease blocking time

                m_blocking = false; m_time_blocking = 0;
            }

            // Update the value of the right trigger:

            m_gamepad_right_trigger *= GAMEPAD_RT_SMOOTH;
            m_gamepad_right_trigger += ( 1.0f - GAMEPAD_RT_SMOOTH ) * gps.Triggers.Right;

            // Update the value of the right analog stick:

            m_gamepad_right_stick *= GAMEPAD_RS_SMOOTH;
            m_gamepad_right_stick += ( 1.0f - GAMEPAD_RS_SMOOTH ) * gps.ThumbSticks.Right;

            // Update the value of the left analog stick:

            m_gamepad_left_stick_x *= GAMEPAD_LS_SMOOTH;
            m_gamepad_left_stick_x += ( 1.0f - GAMEPAD_LS_SMOOTH ) * gps.ThumbSticks.Left.X;
        }

        //=========================================================================================
        /// <summary>
        /// Updates all input from the keyboard which is smoothed over time to try and fake 
        /// analog input as it would be on the gamepad. This is the best that can be done 
        /// unfortunately in the absense of an XBOX 360 gamepad.
        /// </summary>
        //=========================================================================================

        private void UpdateSimulatedGamepadInput()
        {
            // Get the state of the keyboard 

            KeyboardState kb_state = Keyboard.GetState();

            // Slow down simulated keyboard gamepad input if keys are down

                if ( kb_state.IsKeyDown(Keys.A) == false && kb_state.IsKeyDown(Keys.D) == false )
                {
                    // Ignore for one frame if the last time it was pressed down:

                    if ( m_last_kb_state.IsKeyDown(Keys.A) == false && m_last_kb_state.IsKeyDown(Keys.D) == false )
                    {
                        m_keyboard_left_stick_x *= KEYBOARD_LS_SMOOTH;
                    }
                }

                if ( kb_state.IsKeyDown(Keys.NumPad5) == false ) 
                {
                    // Ignore for one frame if the last time it was pressed down:

                    if ( m_last_kb_state.IsKeyDown(Keys.NumPad5) == false ) 
                    {
                        m_keyboard_right_trigger *= KEYBOARD_RT_SMOOTH;
                    }
                }

                if ( kb_state.IsKeyDown(Keys.NumPad4) == false && kb_state.IsKeyDown(Keys.NumPad6) == false )
                {
                    // Ignore for one frame if the last time it was pressed down:

                    if ( m_last_kb_state.IsKeyDown(Keys.NumPad4) == false && m_last_kb_state.IsKeyDown(Keys.NumPad6) == false ) 
                    {
                        m_keyboard_right_stick.X *= KEYBOARD_RT_SMOOTH;
                    }
                }

                if ( kb_state.IsKeyDown(Keys.NumPad8) == false && kb_state.IsKeyDown(Keys.NumPad2) == false )
                {
                    // Ignore for one frame if the last time it was pressed down:

                    if ( m_last_kb_state.IsKeyDown(Keys.NumPad8) == false && m_last_kb_state.IsKeyDown(Keys.NumPad2) == false ) 
                    {
                        m_keyboard_right_stick.Y *= KEYBOARD_RT_SMOOTH;
                    }
                }

            // Left stick x movement - left

            if ( kb_state.IsKeyDown(Keys.A) ) 
            {
                m_keyboard_left_stick_x *= KEYBOARD_LS_SMOOTH; 
                m_keyboard_left_stick_x -= ( 1.0f - KEYBOARD_LS_SMOOTH );
            }

            // Left stick x movement - right

            if ( kb_state.IsKeyDown(Keys.D) ) 
            {
                m_keyboard_left_stick_x *= KEYBOARD_LS_SMOOTH; 
                m_keyboard_left_stick_x += ( 1.0f - KEYBOARD_LS_SMOOTH );
            }

            // Right stick x movement - left 

            if ( kb_state.IsKeyDown(Keys.NumPad4) ) 
            {
                m_keyboard_right_stick.X *= KEYBOARD_LS_SMOOTH; 
                m_keyboard_right_stick.X -= ( 1.0f - KEYBOARD_LS_SMOOTH );
            }

            // Right stick x movement - right 

            if ( kb_state.IsKeyDown(Keys.NumPad6) ) 
            {
                m_keyboard_right_stick.X *= KEYBOARD_LS_SMOOTH; 
                m_keyboard_right_stick.X += ( 1.0f - KEYBOARD_LS_SMOOTH );
            }

            // Right stick y movement - up

            if ( kb_state.IsKeyDown(Keys.NumPad8) ) 
            {
                m_keyboard_right_stick.Y *= KEYBOARD_LS_SMOOTH; 
                m_keyboard_right_stick.Y += ( 1.0f - KEYBOARD_LS_SMOOTH );
            }

            // Right stick y movement - down

            if ( kb_state.IsKeyDown(Keys.NumPad2) ) 
            {
                m_keyboard_right_stick.Y *= KEYBOARD_LS_SMOOTH; 
                m_keyboard_right_stick.Y -= ( 1.0f - KEYBOARD_LS_SMOOTH );
            }

            // Right trigger movement:

            if ( kb_state.IsKeyDown(Keys.NumPad5) )
            {
                m_keyboard_right_trigger *= KEYBOARD_LS_SMOOTH; 
                m_keyboard_right_trigger += ( 1.0f - KEYBOARD_LS_SMOOTH );
            }

            // Do dead zoning:

            if ( Math.Abs(m_keyboard_left_stick_x)  <= KEYBOARD_DEAD_ZONE ) m_keyboard_left_stick_x     = 0;
            if ( Math.Abs(m_keyboard_right_stick.X) <= KEYBOARD_DEAD_ZONE ) m_keyboard_right_stick.X    = 0;
            if ( Math.Abs(m_keyboard_right_stick.Y) <= KEYBOARD_DEAD_ZONE ) m_keyboard_right_stick.Y    = 0;
            if ( Math.Abs(m_keyboard_right_trigger) <= KEYBOARD_DEAD_ZONE ) m_keyboard_right_trigger    = 0;
        }

        //=========================================================================================
        /// <summary>
        /// Updates footstep sounds for the player character. Playing them when necessary.
        /// </summary>
        //=========================================================================================

        private void DoFootstepSounds()
        {
            // Don't do if we are not on the ground:

            if ( JumpSurface.ValidResult == false ) return;

            // Get the direction the player is facing in: rotated by surface orientation

            Vector2 direction = Vector2.Transform( Vector2.UnitX , Matrix.CreateRotationZ(SurfaceRotation) );

            // Get the speed that the player is moving at relative to the direction they are facing:

            float speed = Math.Abs( Vector2.Dot( direction , Velocity + MoveVelocity ) );

            // Ok: calculate the frame rate, or how many footstep sounds we will play per second:

            float steps_per_second = FOOTSTEP_FRAME_RATE * speed * FOOTSTEP_SPEED_SCALE;

            // Make sure it isn't over the max:

            if ( steps_per_second > FOOTSTEP_FRAME_RATE_MAX ) steps_per_second = FOOTSTEP_FRAME_RATE_MAX;

            // Good: calculate the time in between steps:

            float step_time = 1.0f / steps_per_second;

            // See if it is time for a step:

            if ( m_time_since_last_footstep >= step_time )
            {
                // Rest step time:

                m_time_since_last_footstep = 0;

                // Play the foostep:

                Core.Audio.Play("Footstep");
            }

            // Increment time:

            m_time_since_last_footstep += Core.Timing.ElapsedTime;
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

                    // Increase it's size by bounding box distance:

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

                    // See if it is a character:

                    if ( game_object.GetType().IsSubclassOf( m_character_type ) )
                    {
                        // Ok: make sure it is not us

                        if ( game_object != this )
                        {
                            // Good: cast to character

                            Character character = (Character) game_object;

                            // Figure out how long the entire attack animation is:

                            float time = ( 1.0f / (float) (p_arm.CurrentSequence.FrameRate) ) * (float) p_arm.CurrentSequence.Frames.Length;

                            // Figure out how much sword damage to do per frame from this:

                            float damage = ( Core.Timing.ElapsedTime / time ) * AttackDamage;

                            // Damage it:

                            character.Damage(damage,this);
                        }
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
            // Do base class function

            base.Damage(amount,damager);

            // Reset our combat multiplier: find the game rules object first though

            LevelRules rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

            // See if there:

            if ( rules != null ) rules.ResetCombatMultplier();
        }

        //=========================================================================================
        /// <summary>
        /// Does debug drawing for the character. 
        /// </summary>
        //=========================================================================================

        #if DEBUG

            public override void OnDebugDraw()
            {
                // Call base function

                base.OnDebugDraw();

                // Draw the rectangular area that the characters attack current extends to:
                
                {
                    // Get a right vector for the player rotated by current surface rotation:

                    Vector2 right = Vector2.UnitX;

                        // If the character is going left then flip it:

                        if ( Flipped == false ) right.X *= -1;

                        // Rotate it
                    
                        right = Vector2.Transform( right , Matrix.CreateRotationZ(SurfaceRotation) );

                        // Get the true axis aligned bounding box size from the animation: the current bb for the character is distorted with rotation

                        Vector2 box_dimensions = BoxDimensions;

                        if ( Animation != null )
                        {
                            box_dimensions = Animation.BoxDimensions;
                        }

                        // Increase it's size by x coordinate of box dimensions:

                        right *= box_dimensions;

                    // Make up a bounding box centered on this vector + players offset with the start and end points determining size of box

                    Vector2 box_p = right * 0.5f + Position;
                    Vector2 box_d = right * 0.5f;

                    if ( box_d.X < 0 ) box_d.X *= -1;
                    if ( box_d.Y < 0 ) box_d.Y *= -1;

                    // Draw this box:

                    DebugDrawing.DrawWorldRectangle(box_p,box_d,Color.Gold);
                }
            }

        #endif

    }   // end of class

}   // end of namespace
