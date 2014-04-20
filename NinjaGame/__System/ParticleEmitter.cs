using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// This class allows bursts of particles to be generated. Handles the creation and drawing 
    /// of the particles.
    /// </summary>
    //#############################################################################################

    public class ParticleEmitter
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Returns the number of active particle bursts. </summary>

            public int ActiveBurstCount { get { return m_particle_bursts.Count; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Vertex buffer containing all of the vertices for the particles. </summary>

            private VertexBuffer m_vertex_buffer = null;

            /// <summary> A list of currently active particle bursts. </summary>

            private LinkedList<ParticleEmitterBurst> m_particle_bursts = new LinkedList<ParticleEmitterBurst>();
            
            /// <summary> The effect used by the particle emitter. </summary>

            private Effect m_effect = null;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Maximum number of particles that can be drawn in one go. </summary>

            private const int PARTICLE_BUFFER_SIZE = 8192;

        //=========================================================================================
        /// <summary>
        /// Creates the particle emitter.
        /// </summary>
        //=========================================================================================

        public ParticleEmitter()
        {
            // Load our effect:

            m_effect = Core.Graphics.LoadEffect("Effects\\particle");

            // Create the vertex buffer for holding all the particles:

            m_vertex_buffer = new VertexBuffer
            ( 
                Core.Graphics.Device                                                , 
                VertexPositionColorTexture.SizeInBytes * 6 * PARTICLE_BUFFER_SIZE   ,
                BufferUsage.WriteOnly
            );

            // Now we need to populate it:

            VertexPositionColorTexture[] vertex_data = new VertexPositionColorTexture[ 6 * PARTICLE_BUFFER_SIZE ];

            // Fill in each vertex:

            for ( int i = 0 ; i < PARTICLE_BUFFER_SIZE ; i++ )
            {                
                // Normalised size and position:

                    vertex_data[ i * 6 + 0 ].Position.X = -1;
                    vertex_data[ i * 6 + 0 ].Position.Y = +1;
                    vertex_data[ i * 6 + 1 ].Position.X = +1;
                    vertex_data[ i * 6 + 1 ].Position.Y = +1;
                    vertex_data[ i * 6 + 2 ].Position.X = +1;
                    vertex_data[ i * 6 + 2 ].Position.Y = -1;
                    vertex_data[ i * 6 + 3 ].Position.X = +1;
                    vertex_data[ i * 6 + 3 ].Position.Y = -1;
                    vertex_data[ i * 6 + 4 ].Position.X = -1;
                    vertex_data[ i * 6 + 4 ].Position.Y = -1;
                    vertex_data[ i * 6 + 5 ].Position.X = -1;
                    vertex_data[ i * 6 + 5 ].Position.Y = +1;

                // Texture coordinates: flip and rotate differently with each particle

                switch ( i % 6 )
                {
                    case 0:

                        vertex_data[ i * 6 + 0 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 0 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.Y = 0;

                    break;

                    case 1:

                        vertex_data[ i * 6 + 0 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 0 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.Y = 0;

                    break;

                    case 2:

                        vertex_data[ i * 6 + 0 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 0 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.Y = 1;

                    break;

                    case 3:

                        vertex_data[ i * 6 + 0 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 0 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.Y = 1;

                    break;

                    case 4:

                        vertex_data[ i * 6 + 0 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 0 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.Y = 1;

                    break;

                    default:

                        vertex_data[ i * 6 + 0 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 0 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 1 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 2 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 3 ].TextureCoordinate.Y = 1;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.X = 0;
                        vertex_data[ i * 6 + 4 ].TextureCoordinate.Y = 0;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.X = 1;
                        vertex_data[ i * 6 + 5 ].TextureCoordinate.Y = 0;

                    break;
                }

                // Colors: these are used for 4 random numbers in the vertex shader

                    Color c = new Color
                    (
                        new Vector4
                        (
                            (float)Core.Random.NextDouble(),
                            (float)Core.Random.NextDouble(),
                            (float)Core.Random.NextDouble(),
                            (float)Core.Random.NextDouble()
                        )
                    );

                    vertex_data[ i * 6 + 0 ].Color = c;
                    vertex_data[ i * 6 + 1 ].Color = c;
                    vertex_data[ i * 6 + 2 ].Color = c;
                    vertex_data[ i * 6 + 3 ].Color = c;
                    vertex_data[ i * 6 + 4 ].Color = c;
                    vertex_data[ i * 6 + 5 ].Color = c;

                // Encode an extra random number in the z position:

                {
                    float r = (float) Core.Random.NextDouble();
                        
                    vertex_data[ i * 6 + 0 ].Position.Z = r;
                    vertex_data[ i * 6 + 1 ].Position.Z = r;
                    vertex_data[ i * 6 + 2 ].Position.Z = r;
                    vertex_data[ i * 6 + 3 ].Position.Z = r;
                    vertex_data[ i * 6 + 4 ].Position.Z = r;
                    vertex_data[ i * 6 + 5 ].Position.Z = r;
                }

            }   // end for all buffer particles

            // Now upload all the data onto the vertex buffer:

            m_vertex_buffer.SetData<VertexPositionColorTexture>( vertex_data );
        }

        //=========================================================================================
        /// <summary>
        /// Updates all the particle bursts and advances their timing.
        /// </summary>
        //=========================================================================================

        public void Update()
        {
            // If there are no particles left then abort:

            if ( m_particle_bursts.Count <= 0 ) return;

            // Get the first node in the list:

            LinkedListNode<ParticleEmitterBurst> node = m_particle_bursts.First;

            // Now keep going through the list:

            while ( node != null )
            {
                // Get this burst:

                ParticleEmitterBurst b = node.Value;

                // Update the time this burst has been alive for:

                b.m_time_alive += Core.Timing.ElapsedTime;

                // Get the max live time for this particle burst:

                float live_time_max = MathHelper.Max( b.m_live_time1 , b.m_live_time2 );

                // If the burst should be killed then remove it from the list:

                if ( b.m_time_alive > live_time_max )
                {
                    // Make a temporary copy of this node:

                    LinkedListNode<ParticleEmitterBurst> t = node;

                    // Move onto the next node:

                    node = node.Next;

                    // Remove this node from the list:

                    m_particle_bursts.Remove(t);
                }
                else
                {
                    // Don't kill it: move onto the next node

                    node = node.Next;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Creates a directed burst of particles from the emitter.
        /// </summary>
        /// <param name="num_particles">    Number of particles to make.                    </param>
        /// <param name="texture">          Texture to use for the particles.               </param>
        /// <param name="position">         Position of the burst                           </param>
        /// <param name="direction">        Direction of the burst.                         </param>
        /// <param name="spread">           Variation from the direction of the burst.      </param>
        /// <param name="live_time1">       One of two random live times for the particles. </param>
        /// <param name="live_time2">       One of two random live times for the particles. </param>
        /// <param name="size1">            One of two random sizes for the particles.      </param>
        /// <param name="size2">            One of two random sizes for the particles.      </param>
        /// <param name="color1">           One of two random colors for the particles.     </param>
        /// <param name="color2">           One of two random colors for the particles.     </param>
        /// <param name="speed1">           One of two random speeds for the particles.     </param>
        /// <param name="speed2">           One of two random speeds for the particles.     </param>
        /// <param name="gravity">          Amount of gravity to apply to the particles.    </param>
        /// <param name="alpha_blend">      Whether to apply alpha or additve blending.     </param>
        //=========================================================================================

        public void CreateDirectedBurst
        (
            int         num_particles   ,
            Texture2D   texture         ,
            Vector2     position        ,
            Vector2     direction       ,
            float       spread          ,
            float       live_time1      ,
            float       live_time2      ,
            float       size1           ,
            float       size2           ,
            Vector4     color1          ,
            Vector4     color2          ,
            float       speed1          ,
            float       speed2          ,
            float       gravity         ,
            bool        alpha_blend 
        )
        {
            // If no texture given then abort:

            if ( texture == null ) return;

            // If no particles given then abort:

            if ( num_particles <= 0 ) return;

            // Make a new particle burst:

            ParticleEmitterBurst p = new ParticleEmitterBurst();

            // Add it into the list of emitter bursts:

            m_particle_bursts.AddLast(p);

            // Make sure live times are ok:

            if ( live_time1 < 0.00001f ) live_time1 = 0.00001f;
            if ( live_time2 < 0.00001f ) live_time2 = 0.00001f;

            // Get angle we are shooting in:

            float angle = DirectionToAngle(direction);

            // Set particle parameters:

            p.m_num_particles   = num_particles;
            p.m_texture         = texture;
            p.m_position        = position;
            p.m_time_alive      = 0;
            p.m_live_time1      = live_time1;
            p.m_live_time2      = live_time2;
            p.m_size1           = size1;
            p.m_size2           = size2;
            p.m_direction1      = angle - spread;
            p.m_direction2      = angle + spread;
            p.m_speed1          = speed1;
            p.m_speed2          = speed2;
            p.m_gravity         = gravity;
            p.m_alpha_blend     = alpha_blend;
            p.m_color1          = color1;
            p.m_color2          = color2; 
            p.m_random_seed     = (float) Core.Random.NextDouble();          
        }

        //=========================================================================================
        /// <summary>
        /// Creates a burst of particles from the emitter in all directions.
        /// </summary>
        /// <param name="num_particles">    Number of particles to make.                    </param>
        /// <param name="texture">          Texture to use for the particles.               </param>
        /// <param name="position">         Position of the burst                           </param>
        /// <param name="live_time1">       One of two random live times for the particles. </param>
        /// <param name="live_time2">       One of two random live times for the particles. </param>
        /// <param name="size1">            One of two random sizes for the particles.      </param>
        /// <param name="size2">            One of two random sizes for the particles.      </param>
        /// <param name="color1">           One of two random colors for the particles.     </param>
        /// <param name="color2">           One of two random colors for the particles.     </param>
        /// <param name="speed1">           One of two random speeds for the particles.     </param>
        /// <param name="speed2">           One of two random speeds for the particles.     </param>
        /// <param name="gravity">          Amount of gravity to apply to the particles.    </param>
        /// <param name="alpha_blend">      Whether to apply alpha or additve blending.     </param>
        //=========================================================================================

        public void CreateBurst
        (
            int         num_particles   ,
            Texture2D   texture         ,
            Vector2     position        ,
            float       live_time1      ,
            float       live_time2      ,
            float       size1           ,
            float       size2           ,
            Vector4     color1          ,
            Vector4     color2          ,
            float       speed1          ,
            float       speed2          ,
            float       gravity         ,
            bool        alpha_blend 
        )
        {
            // If no texture given then abort:

            if ( texture == null ) return;

            // If no particles given then abort:

            if ( num_particles <= 0 ) return;

            // Make a new particle burst:

            ParticleEmitterBurst p = new ParticleEmitterBurst();

            // Add it into the list of emitter bursts:

            m_particle_bursts.AddLast(p);

            // Make sure live times are ok:

            if ( live_time1 < 0.00001f ) live_time1 = 0.00001f;
            if ( live_time2 < 0.00001f ) live_time2 = 0.00001f;

            // Set particle parameters:

            p.m_num_particles   = num_particles;
            p.m_texture         = texture;
            p.m_position        = position;
            p.m_time_alive      = 0;
            p.m_live_time1      = live_time1;
            p.m_live_time2      = live_time2;
            p.m_size1           = size1;
            p.m_size2           = size2;
            p.m_direction1      = 0;
            p.m_direction2      = MathHelper.TwoPi;
            p.m_speed1          = speed1;
            p.m_speed2          = speed2;
            p.m_gravity         = gravity; 
            p.m_alpha_blend     = alpha_blend;
            p.m_color1          = color1;
            p.m_color2          = color2; 
            p.m_random_seed     = (float) Core.Random.NextDouble();          
        }

        //=========================================================================================
        /// <summary>
        /// Draws all of the particles in the emitter
        /// </summary>
        //=========================================================================================

        public void Draw()
        {
            // If we have no effect then abort:

            if ( m_effect == null ) return;

            // If there are no particles then abort:

            if ( m_particle_bursts.Count <= 0 ) return;

            // Get the first particle burst in the list:

            LinkedListNode<ParticleEmitterBurst> node = m_particle_bursts.First;

            // Figure out the view projection matrix for the camera:

            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            //-------------------------------------------------------------------------------------
            // Run through the list:
            //-------------------------------------------------------------------------------------

            while ( node != null )
            {
                // Get this emitter burst:

                ParticleEmitterBurst p = node.Value;

                // This is the number of particles we have left to draw for this burst:

                int particles_left = p.m_num_particles;

                // This is the number of times the rendering loop has been entered:

                int particle_batches_rendered = 0;

                //---------------------------------------------------------------------------------
                // If this particle burst uses additive blending then set it up
                //---------------------------------------------------------------------------------

                if ( p.m_alpha_blend == false )
                {
                    // Setup alpha blending:

                    Core.Graphics.Device.RenderState.DestinationBlend = Blend.One;
                }

                //---------------------------------------------------------------------------------
                // Set the params on the shader:
                //---------------------------------------------------------------------------------

                    // Texture    

                    {
                        // Get param:

                        EffectParameter param = m_effect.Parameters["Texture"];

                        // Set if there:

                        if ( param != null && param.ParameterType == EffectParameterType.Texture ) 
                        {
                            param.SetValue(p.m_texture);
                        }
                    }

                    // Random seed and time alive for the burst    

                    {
                        // Get param:

                        EffectParameter param = m_effect.Parameters["ParticleRandomSeedAndTimeAlive"];

                        // Set if there:

                        if ( param != null && param.ParameterType == EffectParameterType.Single ) 
                        {
                            // Save both values here:

                            Vector2 v = Vector2.Zero;

                            v.X = p.m_random_seed;
                            v.Y = p.m_time_alive;

                            // Set the seed and time alive:

                            param.SetValue(v);
                        }
                    }

                    // Transform:

                    {
                        // Get param:

                        EffectParameter param = m_effect.Parameters["WorldViewProjection"];

                        // Set if there:

                        if ( param != null && param.ParameterType == EffectParameterType.Single ) 
                        {
                            // Figure out world transform due to position and gravity:

                            Matrix world = Matrix.CreateTranslation
                            (
                                p.m_position.X                                                          ,
                                p.m_position.Y - 0.5f * p.m_gravity * p.m_time_alive * p.m_time_alive   ,
                                0
                            );

                            // Combine with the view and projection transform and set as shader transform:

                            param.SetValue( world * view_projection );
                        }
                    }

                    // Color1:

                    {
                        // Get param:

                        EffectParameter param = m_effect.Parameters["ParticleColor1"];

                        // Set if there:

                        if ( param != null && param.ParameterType == EffectParameterType.Single ) param.SetValue(p.m_color1);
                    }

                    // Color2:

                    {
                        // Get param:

                        EffectParameter param = m_effect.Parameters["ParticleColor2"];

                        // Set if there:

                        if ( param != null && param.ParameterType == EffectParameterType.Single ) param.SetValue(p.m_color2);
                    }

                    // Angles and movement speeds:

                    {
                        // Get param:

                        EffectParameter param = m_effect.Parameters["ParticleAnglesAndSpeeds"];

                        // Set if there:

                        if ( param != null && param.ParameterType == EffectParameterType.Single ) 
                        {
                            // Makeup vector

                            Vector4 v = Vector4.Zero;

                            v.X = p.m_direction1;
                            v.Y = p.m_direction2;
                            v.Z = p.m_speed1;
                            v.W = p.m_speed2;

                            // Set the value

                            param.SetValue(v);
                        }
                    }

                    // Sizes and live times:

                    {
                        // Get param:

                        EffectParameter param = m_effect.Parameters["ParticleSizesAndLiveTimes"];

                        // Set if there:

                        if ( param != null && param.ParameterType == EffectParameterType.Single ) 
                        {
                            // Makeup vector

                            Vector4 v = Vector4.Zero;

                            v.X = p.m_size1;
                            v.Y = p.m_size2;
                            v.Z = p.m_live_time1;
                            v.W = p.m_live_time2;

                            // Set the value

                            param.SetValue(v);
                        }
                    }

                //---------------------------------------------------------------------------------
                // Set the vertex buffer on the card
                //---------------------------------------------------------------------------------

                Core.Graphics.Device.Vertices[0].SetSource
                ( 
                    m_vertex_buffer                         , 
                    0                                       , 
                    VertexPositionColorTexture.SizeInBytes 
                );

                //---------------------------------------------------------------------------------
                // Continue drawing until all particles have been drawn:
                //---------------------------------------------------------------------------------

                // Begin drawing with our shader:

                m_effect.Begin(); m_effect.CurrentTechnique.Passes[0].Begin();

                    while ( particles_left > 0 )
                    {
                        // See if we can draw the remainder of the particles in one go:

                        if ( particles_left <= PARTICLE_BUFFER_SIZE )
                        {
                            // Cool: we can.. draw the rest of the particles

                            Core.Graphics.Device.DrawPrimitives(PrimitiveType.TriangleList,0,particles_left*2);

                            // Set the number of particles left to zero

                            particles_left = 0;
                        }
                        else
                        {
                            // Cant draw all of them in one go: draw as many as we can

                            Core.Graphics.Device.DrawPrimitives(PrimitiveType.TriangleList,0,PARTICLE_BUFFER_SIZE*2);

                            // Decrease the number of particles we have left to draw

                            particles_left -= PARTICLE_BUFFER_SIZE;
                        }

                        // Increment the number of particle batches rendered:

                        particle_batches_rendered++;

                    }   // end while particles left to draw

                // End drawing with our shader:

                m_effect.End(); m_effect.CurrentTechnique.Passes[0].End();

                //---------------------------------------------------------------------------------
                // Move onto the next particle burst:
                //---------------------------------------------------------------------------------

                node = node.Next;

                //---------------------------------------------------------------------------------
                // If this particle burst uses additive blending then restore previous setting
                //---------------------------------------------------------------------------------

                if ( p.m_alpha_blend == false )
                {
                    // Setup alpha blending:

                    Core.Graphics.Device.RenderState.DestinationBlend = GraphicsSystem.DefaultRenderState.DestinationBlend;
                }

            }   // end for all bursts
        }

        //=========================================================================================
        /// <summary>
        /// Converts a direction given as a vector into an angle.
        /// </summary>
        /// <param name="dir"> Direction to convert </param>
        /// <returns> The direction expressed as an angle </returns>
        //=========================================================================================

        private float DirectionToAngle( Vector2 dir )
        {
            // Get the length of the direction:

            float l = dir.Length();

            // If zero length then return zero:

            if ( l == 0 ) return 0;

            // Normalise the direction:

            dir.X /= l;
            dir.Y /= l;

            // Clamp x and y to 0-1 range:

            if ( dir.X > +1 ) dir.X = +1; if ( dir.X < -1 ) dir.X = -1;
            if ( dir.Y > +1 ) dir.Y = +1; if ( dir.Y < -1 ) dir.Y = -1;

            // Get the inverse cosine of the x direction:

            float acos = (float) Math.Acos(dir.X);

            // If the y direction then transform accordingly:

            if ( dir.Y < 0 ) acos = MathHelper.TwoPi - acos;

            // Return the angle

            return acos;
        }

    }   // end of class

}   // end of namespace
