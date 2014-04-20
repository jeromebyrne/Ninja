using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//####################################################################################################
//####################################################################################################

namespace NinjaGame
{
    //#################################################################################################
    /// <summary>
    /// Class representing all ninja characters in the game. Both player controlled and enemy ninjas
    /// are derived from this.
    /// </summary>
    //#################################################################################################

    abstract public class Ninja : Character
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================
            
            /// <summary> Tells if the ninja can fire a shuriken at this moment in time. </summary>

            public bool CanFireShuriken { get { return m_time_since_shuriken_fire >= m_shuriken_fire_time; } }

            /// <summary> Tells if the ninja can attack using a sword at this moment in time. </summary>

            public bool CanAttack 
            { 
                get 
                { 
                    // Get the body:

                    AnimationPart p_body = Animation.GetPart("Body");

                    // If not there then we cannot attack:

                    if ( p_body != null && p_body.CurrentSequence != null )
                    {
                        // See if we are already attacking:

                        if ( p_body.CurrentSequence.Name.Equals("Attacking",StringComparison.CurrentCultureIgnoreCase) )
                        {
                            // Already attacking: cannot attack

                            return false;
                        }
                        else
                        {
                            // Not attacking: go ahead

                            return true;
                        }
                    }
                    else
                    {
                        // Not there: we cannot attack

                        return false;
                    }
                } 
            }

            /// <summary> Get/sets maximum force that the ninja can jump at. </summary>

            public float JumpForce { get { return m_jump_force; } set { m_jump_force = MathHelper.Clamp( value , 0 , 1000000 ); } }

            /// <summary> Percentage. Minimum percent of jump force that must be applied by the character in a jump. </summary>

            private float MinimumJumpForcePercent { get { return m_min_jump_force_percent; } set { m_min_jump_force_percent = MathHelper.Clamp(value,0,1); } }

            /// <summary> Get/sets minimum amount of time in between shuriken firing for this character. </summary>

            public float SurikenFireTime { get { return m_shuriken_fire_time; } set { m_shuriken_fire_time = MathHelper.Clamp( value , 0 , 1000000 ); } }

            /// <summary> Get/sets speed that this character's shurikens fire at. </summary>

            public float ShurikenSpeed { get { return m_shuriken_speed; } set { m_shuriken_speed = MathHelper.Clamp( value , 0 , 1000000 ); } }

            /// <summary> Get/sets amount of damage that this character's shurikens do.  </summary>

            public float ShurikenDamage { get { return m_shuriken_damage; } set { m_shuriken_damage = MathHelper.Clamp( value , 0 , 1000000 ); }  }

            /// <summary> Get/sets time the ninja's shurikens have to live. </summary>

            public float ShurikenLiveTime { get { return m_shuriken_live_time; } set { m_shuriken_live_time = MathHelper.Clamp( value , 0 , 1000000 ); } }

            /// <summary> Get/sets gravity applied to the ninjas shurikens.  </summary>

            public float ShurikenGravity { get { return m_shuriken_gravity; } set { m_shuriken_gravity = MathHelper.Clamp( value , 0 , 1000000 ); } }

            /// <summary> Get/sets size of the ninja's shurikens. </summary>

            public float ShurikenSize { get { return m_shuriken_size; } set { m_shuriken_size = value; } }

            /// <summary> Get/sets texture used for the ninja's shurikens. </summary>

            public Texture2D ShurikenTexture { get { return m_shuriken_texture; } set { m_shuriken_texture = value; } }

            /// <summary> Texture used for the ninja's shuriken sparks. </summary>

            public Texture2D ShurikenSparkTexture { get { return m_shuriken_spark_texture; } }

            /// <summary> Get/sets amount of damage an attack by the ninja does. </summary>

            public float AttackDamage { get { return m_atttack_damage; } set { m_atttack_damage = MathHelper.Clamp( value , 0 , 1000000 ); } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Maximum force that the ninja can jump at. </summary>
            
            private float m_jump_force = 16;

            /// <summary> Percentage. Minimum amount of jump force that must be applied by the character. </summary>

            private float m_min_jump_force_percent = 0.25f;

            /// <summary> Number of seconds the ninja has been standing still for. Used to determine when to use the still frames. </summary>

            private float m_time_standing_still = 0;

            /// <summary> Minimum amount of time in between shuriken firing for this character. </summary>

            private float m_shuriken_fire_time = 0.35f;

            /// <summary> Speed that this character's shurikens fire at. </summary>

            private float m_shuriken_speed = 750.0f;

            /// <summary> Amount of damage that this character's shurikens do. </summary>

            private float m_shuriken_damage = 75.0f;

            /// <summary> Time the ninja's shurikens have to live. </summary>

            private float m_shuriken_live_time = 10.0f;

            /// <summary> Gravity applied to the ninjas shurikens. </summary>

            private float m_shuriken_gravity = 16.0f;

            /// <summary> Size of the ninja's shurikens. </summary>

            private float m_shuriken_size = 8.0f;

            /// <summary> Amount of damage an attack by the ninja does. </summary>

            private float m_atttack_damage = 100.0f;

            /// <summary> Texture used for the ninja's shurikens. Default is null. </summary>

            private Texture2D m_shuriken_texture = null;

            /// <summary> Texture the character uses for it's blood. </summary>

            protected Texture2D m_blood_texture = null;

            /// <summary> Texture used for the shuriken spark. </summary>

            private Texture2D m_shuriken_spark_texture = null;

            /// <summary> Amount of time since the last shuriken was fired. </summary>

            private float m_time_since_shuriken_fire = float.PositiveInfinity;

            /// <summary> Time left until the next bit of blood will spawn </summary>

            private float m_time_to_blood = 0;

            /// <summary> Time left until the next pain sound will play. </summary>

            private float m_time_to_pain_sound = 0;

            /// <summary> Time left until the next blood sound will play. </summary>

            private float m_time_to_blood_sound = 0;

            /// <summary> Health the ninja had for the last frame. </summary>

            private float m_last_health = 0;

        //=========================================================================================
        // Constants
        //==========================================================================================

            /// <summary> Minimum time in between pain sounds - 1 </summary>

            private const float PAIN_SOUND_INTERVAL_1 = 0.5f;

            /// <summary> Minimum time in between pain sounds - 2 </summary>

            private const float PAIN_SOUND_INTERVAL_2 = 1.0f;

            /// <summary> Minimum time in between blood sounds - 1 </summary>

            private const float BLOOD_SOUND_INTERVAL_1 = 0.25f;

            /// <summary> Minimum time in between blood sounds - 2 </summary>

            private const float BLOOD_SOUND_INTERVAL_2 = 0.50f;

            /// <summary> Minimum time in between blood - 1 </summary>

            private const float BLOOD_INTERVAL_1 = 0.125f;

            /// <summary> Minimum time in between blood - 2 </summary>

            private const float BLOOD_INTERVAL_2 = 0.250f;

            /// <summary> Number of seconds a ninja must be standing still for in order for the still frames to be activated. </summary>

            private const float STILL_FRAME_CHANGE_TIME = 0.25f;

            /// <summary> Speed (or under) at which the ninja is regarded as being standing still. </summary>

            private const float STILL_SPEED_THRESHOLD = 0.001f;

            /// <summary> If the ninja has this much of maximum jump speed (in percent) in the direction of a jump then disallow jumping. </summary>

            private const float DOUBLE_JUMP_THRESHOLD = 0.20f;

            /// <summary> The amount that ninja speed affects the animation playback rate </summary>

            protected const float ANIMATION_SPEED_SCALE = 0.125f;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the object.
        /// </summary>
        //=========================================================================================

        public Ninja() : base()
        {
            // Load the shuriken texture:

            m_shuriken_texture = Core.Graphics.LoadTexture("Graphics\\Objects\\Shuriken");

            // Load blood texture:

            m_blood_texture = Core.Graphics.LoadTexture("Graphics\\Particles\\BloodSplatter");

            // Load spark texture for shuriken:

            m_shuriken_spark_texture = Core.Graphics.LoadTexture("Graphics\\Particles\\Spark");

            // Save last health:

            m_last_health = Health;
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

            // Set current health to last health:

            m_last_health = Health;
        }

        //=========================================================================================
        /// <summary>
        /// Updates for a ninja character. Updates when we can throw shurikens and jump etc. 
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Do base class function first:

            base.OnUpdate();

            // Update the amount of time since the last fire if we cannot currently fire:

            if ( m_time_since_shuriken_fire < m_shuriken_fire_time )
            {
                m_time_since_shuriken_fire += Core.Timing.ElapsedTime;
            }

            // Update amount of time till blood can be made again:

            m_time_to_blood -= Core.Timing.ElapsedTime;

            // Deal any attack damage that has to be dealt from sword swipes:

            DealAttackDamage();

            // Do pain sounds:

            UpdatePainAndBloodSounds();

            // Save our current health as the last:

            m_last_health = Health;
        }

        //=========================================================================================
        /// <summary>
        /// Called when the character has to draw itself. Renders the character.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // Draw all the character parts

            DrawCharacterPart("Arm");
            DrawCharacterPart("Body");
        }

        //=========================================================================================
        /// <summary>
        /// On delete function. Splits the ninja up into debris and adds the debris to the level.
        /// </summary>
        //=========================================================================================

        public override void OnDelete()
        {
            // Only do this if we were killed through combat:

            if ( Health <= 0 )
            {
                // Make debris:

	            CreateDebrisFromPart("Arm");
	            CreateDebrisFromPart("Body");

                // First blood color to use:

                Vector4 blood_color1 = Vector4.One;

                blood_color1.X = 1.0f;
                blood_color1.Y = 1.0f;
                blood_color1.Z = 1.0f;
                blood_color1.W = 0.35f;

                // Second blood color to use:

                Vector4 blood_color2 = Vector4.One;

                blood_color2.X = 0.25f;
                blood_color2.Y = 0.25f;
                blood_color2.Z = 0.25f;
                blood_color2.W = 0.55f;

                // Spawn blood:

                Core.Level.Emitter.CreateBurst
                (
                    100                 ,
                    m_blood_texture     ,
                    Position            ,
                    0.50f               ,
                    1.25f               ,
                    16.0f               ,
                    20.0f               ,
                    blood_color1        ,
                    blood_color2        ,
                    25                  ,
                    200                 ,
                    500                 ,
                    true
                );
            }
        }

        //=========================================================================================
        /// <summary>
        /// Makes the ninja fire a shuriken in the given direction.
        /// </summary>
        /// <param name="direction"> Direction to fire the shuriken in. </param>
        /// <returns> The created shuriken, or null if we could not fire </returns>
        //=========================================================================================

        public Projectile FireShuriken( Vector2 direction )
        {
            // Make sure we can do this:

            if ( m_time_since_shuriken_fire >= m_shuriken_fire_time )
            {
                // Abort if no direction is given:

                if ( direction.X == 0 && direction.Y == 0 ) return null;

                // Normalise the direction:

                direction.Normalize();

                // Good: reset the time since the last shuriken fire

                m_time_since_shuriken_fire = 0;

                // Get the arm part of the animation:

                AnimationPart p_arm = Animation.GetPart("arm");

                // Set the arm to throwing if not throwing already:

                if ( p_arm != null && p_arm.CurrentSequence != null )
                {
                    // See if throwing:

                    if ( ! p_arm.CurrentSequence.Name.Equals("Throwing",StringComparison.CurrentCultureIgnoreCase) )
                    {
                        // Not throwing: start the throwing animation

                        p_arm.SetSequence("Throwing");
                    }
                }

                // Good: now make the shuriken

                Projectile shuriken = MakeShuriken( direction * m_shuriken_speed );

                // Add the shuriken into the level:

                Core.Level.Data.Add(shuriken);

                // Play the fire shuriken sound:

                Core.Audio.Play("Shuriken");

                // Return the shuriken fired:

                return shuriken;
            }
            else
            {
                // Can't fire: return null

                return null;
            }

        }

        //=========================================================================================
        /// <summary>
        /// This function makes a shuriken that the character can fire. It is virtual so different 
        /// types of ninja's can make different types of shurikens.
        /// </summary>
        /// <param name="velocity"> Velocity the shuriken should have. </param>
        /// <returns> The shuriken that was made. </returns>
        //=========================================================================================

        protected virtual Projectile MakeShuriken( Vector2 velocity )
        {
            // Make the shuriken

            return new Projectile
            (
                this                    ,
                m_shuriken_texture      ,
                m_shuriken_spark_texture,
                Effect                  ,
                Position                ,
                velocity                ,
                m_shuriken_live_time    ,
                m_shuriken_gravity      ,
                m_shuriken_size         ,
                m_shuriken_damage
            );
        }

        //=========================================================================================
        /// <summary>
        /// Makes the ninja jump with the given force.
        /// </summary>
        /// <param name="force"> 
        /// Force to jump with. Must be within the limits of the character's maximum jump force.
        /// </param>
        //=========================================================================================

        public void Jump( float force )
        {
            // See if we are in contact with any surface and we are allowed to jump:

            if ( JumpSurface.ValidResult )
            {
                // Good: now make sure the jump force is within bounds

                force = MathHelper.Clamp( force , m_jump_force * m_min_jump_force_percent , m_jump_force );

                // Get direction to jump in:

                Vector2 jump_dir = JumpSurface.ResolveDirection;

                // Get our current velocity in that direction:

                float vel_in_jump_dir = Vector2.Dot( MoveVelocity + Velocity , jump_dir );

                // If the velocity exceeds the amount allowed for double jumps then cancel the jump:

                if ( vel_in_jump_dir > JumpForce * DOUBLE_JUMP_THRESHOLD ) return;

                // If the resolve direction points downwards then disregard the y component: can't jump down

                if ( jump_dir.Y < 0 ) jump_dir.Y = 0;

                // Jump according to the jump surface:

                Velocity += force * jump_dir;

                // If we somehow got more upward velocity than our jump force allows then stop it:

                if ( Velocity.Y > m_jump_force ) VelocityY = m_jump_force;

                // Set the animation sequence to jumping:

                if ( Animation != null )
                {
                    // Get the part:

                    AnimationPart p_body = Animation.GetPart("Body");

                    // Set the sequence:

                    if ( p_body != null ) p_body.SetSequence("Jumping");
                }

                // Play the jump sound:

                Core.Audio.Play("Jump");
            }
        }

        //=========================================================================================
        /// <summary>
        /// Makes the ninja do a sword-swipe. Damages characters nearby. This base function simply
        /// changes animation. Nothing more. Damage is dealt out over the frames.
        /// </summary>
        //=========================================================================================

        public void Attack()
        {
            // Get the main body part:

            AnimationPart p_arm = Animation.GetPart("Arm");

            // If null then abort:

            if ( p_arm == null || p_arm.CurrentSequence == null ) return;
            
            // If the current sequence is already attack then abort:

            if ( p_arm.CurrentSequence.Name.Equals("Attacking", StringComparison.CurrentCultureIgnoreCase) ) return;

            // Play the attacking sound:

            Core.Audio.Play("Sword");

            // Put into the attack sequence:

            p_arm.SetSequence("Attacking");
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

            // If we can do blood then do it:

            if ( m_time_to_blood <= 0 )
            {
                // Pick random time to next blood:

                float r = (float) Core.Random.NextDouble();

                // Pick time till next blood:

                m_time_to_blood = BLOOD_INTERVAL_1 * r + ( 1.0f - r ) * BLOOD_INTERVAL_2;

                // See if there was an object given:

                if ( damager != null )
                {
                    // Get direction to the object:

                    Vector2 dir = damager.Position - Position;

                    // Randomnly rotate the direction:

                    Matrix rot = Matrix.CreateRotationZ( 0.25f * (float) Core.Random.NextDouble() - 0.125f );

                    // Rotate direction randomnly:

                    dir = Vector2.Transform( dir , rot );

                    // First blood color to use:

                    Vector4 blood_color1 = Vector4.One;

                    blood_color1.X = 1.0f;
                    blood_color1.Y = 1.0f;
                    blood_color1.Z = 1.0f;
                    blood_color1.W = 0.35f;

                    // Second blood color to use:

                    Vector4 blood_color2 = Vector4.One;

                    blood_color2.X = 0.25f;
                    blood_color2.Y = 0.25f;
                    blood_color2.Z = 0.25f;
                    blood_color2.W = 0.65f;

                    // Spawn blood:

                    Core.Level.Emitter.CreateDirectedBurst
                    (
                        100                 ,
                        m_blood_texture     ,
                        Position            ,
                        dir                 ,
                        0.25f               ,
                        0.25f               ,
                        0.75f               ,
                        12.0f               ,
                        18.0f               ,
                        blood_color1        ,
                        blood_color2        ,
                        50                  ,
                        300                 ,
                        1000                ,
                        true    
                    );

                }

            }
        }

        //=========================================================================================
        /// <summary>
        /// Updates the animations for this character type.
        /// </summary>
        //=========================================================================================

        protected override void UpdateAnimations()
        {
            // If we don't have an animation the abort:

            if ( Animation == null ) return;

            // Get the direction the player is facing in: rotated by surface orientation

            Vector2 direction = Vector2.Transform( Vector2.UnitX , Matrix.CreateRotationZ(SurfaceRotation) );

            // Get the speed that the player is moving at relative to the direction they are facing:

            float speed = Math.Abs( Vector2.Dot( direction , Velocity + MoveVelocity ) );

            // Get the animation parts we need:

            AnimationPart p_arm     = Animation.GetPart("Arm");
            AnimationPart p_body    = Animation.GetPart("Body");

            // See if we are standing still:

            if ( speed <= STILL_SPEED_THRESHOLD )
            {
                // Standing still: increase the time we've been standing still for

                m_time_standing_still += Core.Timing.ElapsedTime;
            }
            else
            {
                // Not standing still:

                m_time_standing_still = 0;
            }

            //-------------------------------------------------------------------------------------
            // If we have been standing still for long enough then change to the still frames:
            //-------------------------------------------------------------------------------------

            if ( m_time_standing_still >= STILL_FRAME_CHANGE_TIME )
            {
                //-------------------------------------------------------------------------------------
                // Don't set the body if jumping and not finished:
                //-------------------------------------------------------------------------------------

                if ( p_body != null )
                {
                    // See if we are jumping:

                    if ( p_body.CurrentSequence.Name.Equals("Jumping",StringComparison.CurrentCultureIgnoreCase) )
                    {
                        // Jumping: make sure it is finished

                        if ( p_body.Finished ) 
                        {
                            // Finished: change

                            p_body.SetSequence("Still");
                        }
                        else
                        {
                            // Not finished: see if we are running: 

                            if ( p_body.CurrentSequence.Name.Equals("Running",StringComparison.CurrentCultureIgnoreCase) )
                            {
                                // Running: calculate the frame rate we want to animate the legs at:

                                float frame_rate = p_body.CurrentSequence.FrameRate * speed * ANIMATION_SPEED_SCALE;

                                // Clamp to the min/max suggested framerate for this sequence:

                                frame_rate = MathHelper.Clamp
                                ( 
                                    frame_rate                              , 
                                    p_body.CurrentSequence.MinFrameRate     ,
                                    p_body.CurrentSequence.MaxFrameRate
                                );

                                // Do the animation:

                                p_body.AnimateLooped( frame_rate );
                            }
                            else
                            {
                                // Not running: animation normally

                                p_body.Animate();
                            }
                        }
                    }
                    else
                    {
                        // Not jumping: just set the sequence

                        p_body.SetSequence("Still");
                    }
                }

                //-------------------------------------------------------------------------------------
                // Animate the arm:
                //-------------------------------------------------------------------------------------

                if ( p_arm != null && p_arm.CurrentSequence != null )
                {
                    // See if the throwing/attacking animation is over: if so then switch to the running sequence

                    if ( p_arm.Finished ) 
                    {
                        // Go back to still frame:

                        p_arm.SetSequence("Still");
                    }
                    else
                    {
                        // Not finished: continue the throw/attack

                        p_arm.Animate();
                    }
                }

                //-------------------------------------------------------------------------------------
                // Abort: we are done now
                //-------------------------------------------------------------------------------------

                return;
            }
            
            //-------------------------------------------------------------------------------------
            // Animate the arm:
            //-------------------------------------------------------------------------------------

            if ( p_arm != null && p_arm.CurrentSequence != null )
            {
                // See if the throwing/attacking animation is over: if so then switch to the running sequence

                if ( p_arm.Finished ) 
                {
                    // Go back to still frame:

                    p_arm.SetSequence("Still");
                }
                else
                {
                    // Not finished: continue the throw/attack

                    p_arm.Animate();
                }
            }

            //-------------------------------------------------------------------------------------
            // Animate the main body part
            //-------------------------------------------------------------------------------------

            if ( p_body != null && p_body.CurrentSequence != null )
            {
                // See if we are jumping:

                if ( p_body.CurrentSequence.Name.Equals( "Jumping" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    // See if it is finished: 

                    if ( p_body.Finished ) 
                    {
                        // Go back to running:

                        p_body.SetSequence("Running");

                        // Animate running:

                        if ( p_body.CurrentSequence != null )
                        {
                            // Calculate the frame rate we want to animate the legs at:

                            float frame_rate = p_body.CurrentSequence.FrameRate * speed * ANIMATION_SPEED_SCALE;

                            // Clamp to the min/max suggested framerate for this sequence:

                            frame_rate = MathHelper.Clamp
                            ( 
                                frame_rate                              , 
                                p_body.CurrentSequence.MinFrameRate     ,
                                p_body.CurrentSequence.MaxFrameRate
                            );

                            // Do the animation:

                            p_body.AnimateLooped( frame_rate );
                        }
                    }
                    else
                    {
                        // Not finished: continue the jump

                        p_body.Animate();
                    }
                }
                else
                {
                    // If we are not in the running sequence then make it so:

                    if ( ! p_body.CurrentSequence.Name.Equals( "Running" , StringComparison.CurrentCultureIgnoreCase ) )
                    {
                        // Go into the running sequence

                        p_body.SetSequence("Running");
                    }

                    // Animate running:

                    if ( p_body.CurrentSequence != null )
                    {
                        // Calculate the frame rate we want to animate the legs at:

                        float frame_rate = p_body.CurrentSequence.FrameRate * speed * ANIMATION_SPEED_SCALE;

                        // Clamp to the min/max suggested framerate for this sequence:

                        frame_rate = MathHelper.Clamp
                        ( 
                            frame_rate                              , 
                            p_body.CurrentSequence.MinFrameRate     ,
                            p_body.CurrentSequence.MaxFrameRate
                        );

                        // Do the animation:

                        p_body.AnimateLooped( frame_rate );
                    }
                }
            }

        }

        //=========================================================================================
        /// <summary>
        /// Monitors the state of the ninja's health and plays pain/blood sounds if the ninja has 
        /// taken damage.
        /// </summary>
        //=========================================================================================

        private void UpdatePainAndBloodSounds()
        {
            // Reduce the time till the next pain and blood sound:

            m_time_to_pain_sound    -= Core.Timing.ElapsedTime;
            m_time_to_blood_sound   -= Core.Timing.ElapsedTime;

            // See if we lost health:

            if ( m_last_health > Health )
            {
                // Lost health: see if we can play a pain sound:

                if ( m_time_to_pain_sound <= 0 )
                {
                    // Good: pick a random time till the next pain sound can be played

                    float t = (float) Core.Random.NextDouble();

                    m_time_to_pain_sound = t * PAIN_SOUND_INTERVAL_1 + ( 1.0f - t ) * PAIN_SOUND_INTERVAL_2;

                    // Now play the pain sound:

                    Core.Audio.Play("Ninja_Pain");
                }

                // See if we can play a blood sound:

                if ( m_time_to_blood_sound <= 0 )
                {
                    // Good: pick a random time till the next blood sound can be played

                    float t = (float) Core.Random.NextDouble();

                    m_time_to_blood_sound = t * BLOOD_SOUND_INTERVAL_1 + ( 1.0f - t ) * BLOOD_SOUND_INTERVAL_2;

                    // Now play the blood sound:

                    Core.Audio.Play("Blood");

                    // Play the hit sound:

                    Core.Audio.Play("Hit");
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

        public virtual void DealAttackDamage(){}
        
    }   // end of class

}   // end of namespace
