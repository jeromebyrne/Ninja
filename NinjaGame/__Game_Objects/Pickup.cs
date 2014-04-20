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
    /// Abstract class representing a pickup in the game. Pickups include health.
    /// </summary>
    // 
    //#############################################################################################

    public abstract class Pickup : Sprite
    {   
        //=========================================================================================
        /// <summary>
        /// Creates the pickup.
        /// </summary>
        //=========================================================================================

        public Pickup() : base(false,false,true)
        {
            // Set default depth:

            Depth = 8000;        
        }

        //=========================================================================================
        /// <summary>
        /// This function is called by the player when the player overlaps the pickup. This 
        /// allows the pickup to perform it's action. After this function is called the pickup will 
        /// be removed from the scene.
        /// </summary>
        /// <param name="taker"> This is the object that took the pickup. </param>
        //=========================================================================================

        public virtual void OnPickup( PlayerNinja taker ){}

    }   // end of class

}   // end of namespace
