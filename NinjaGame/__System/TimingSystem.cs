using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary>
    /// Class which handles the management of time in the game. All game objects should move
    /// themselves according to the value of the ElapsedTime property. The reasoning for this is 
    /// that a new timing system fixed/variable can be easily plugged in later if needed or we can 
    /// do cool stuff like slow down / speed up time with the Timescale property.
    /// </summary>
    // 
    //#############################################################################################

    public class TimingSystem
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Amount of elasped time between current and last frame, with time scaling taken into account. </summary>

            public float ElapsedTime { get { return m_elapsed_time * m_time_scale; } }

            /// <summary> Amount to scale time by. If this is 2 then the game runs twice as fast. 0.5 is half speed etc.. </summary>

            public float TimeScale { get { return m_time_scale; } set { m_time_scale = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================
            
            /// <summary> Amount of elasped time between current and last frame. This does not include time scaling. </summary>

            private float m_elapsed_time = 0;

            /// <summary> Amount to scale time by. If this is 2 then the game runs twice as fast. 0.5 is half speed etc.. </summary>

            private float m_time_scale = 1;

        //=========================================================================================
        /// <summary>
        /// Restarts the timing system and discounts all time that has passed up until this point.
        /// This does not affect time scaling.
        /// </summary>
        //=========================================================================================

        public void Restart(){ m_elapsed_time = 0; }

        //=========================================================================================
        /// <summary>
        /// Restarts the timing system and discounts all time that has passed up until this point.
        /// This does not affect time scaling.
        /// </summary>
        //=========================================================================================

        public void Update( GameTime t )
        { 
            // Save the amount of elapsed time

            //m_elapsed_time = (float) t.ElapsedRealTime.Seconds;

            m_elapsed_time = 1.0f / 60.0f;
            
        }

    }   // end of class 

}   // end of namespace

