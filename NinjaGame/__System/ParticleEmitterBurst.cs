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
    /// Represents a burst of particles in a particle emitter. This class simply holds the 
    /// data for a burst and nothing more.
    /// </summary>
    //#############################################################################################

    public class ParticleEmitterBurst
    {       
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> The number of particles in the burst. </summary>

            public int m_num_particles = 0;

            /// <summary> Texture used for the particle burst. </summary>

            public Texture2D m_texture = null;

            /// <summary> A seed to make the particle distribution different from other bursts </summary>
            
            public float m_random_seed = 0;

            /// <summary> Position of the burst in the world. </summary>

            public Vector2 m_position = Vector2.Zero;

            /// <summary> How long this burst of particles has been alive for. </summary>

            public float m_time_alive = 0;

            /// <summary> Random live time for the particles - number 1 </summary>

            public float m_live_time1 = 1;

            /// <summary> Random live time for the particles - number 2 </summary>

            public float m_live_time2 = 1;

            /// <summary> Random color for the particles - number 1 </summary>

            public Vector4 m_color1 = Vector4.One;

            /// <summary> Random color for the particles - number 2 </summary>

            public Vector4 m_color2 = Vector4.One;

            /// <summary> Random size for the particles - number 1 </summary>

            public float m_size1 = 1;

            /// <summary> Random size for the particles - number 2 </summary>

            public float m_size2 = 1;

            /// <summary> Random direction for the particle flow - 1 </summary>

            public float m_direction1 = 0;

            /// <summary> Random direction for the particle flow - 2 </summary>

            public float m_direction2 = 0;

            /// <summary> Random speed for the particle flow - 1 </summary>

            public float m_speed1 = 0;

            /// <summary> Random speed for the particle flow - 1 </summary>

            public float m_speed2 = 0;

            /// <summary> Amount of gravity applied to the particles </summary>

            public float m_gravity = 0;

            /// <summary> Apply alpha blending to the burst ? If false additive blending will be used instead. </summary>

            public bool m_alpha_blend = true;

    }   // end of class

}   // end of namespace
