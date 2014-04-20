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
    /// This object marks a place where an enemy can be spawned. The level rules module is then 
    /// responsible for doing the actual spawning.
    /// </summary>
    //#############################################################################################
    
    public class EnemySpawnMarker : GameObject
    {
        //=========================================================================================
        // Variables
        //=========================================================================================
            
            /// <summary> The type of enemy object to spawn at this marker. </summary>

            public string SpawnObjectType { get { return m_spawn_object_type; } }

            /// <summary> Minimum phase the player must be on for the spawn point to be active. 0-2 </summary>

            public int MinimumPhase { get { return m_mininum_phase; } }

        //=========================================================================================
        // Variables
        //=========================================================================================
            
            /// <summary> The type of enemy object to spawn at this marker. </summary>

            private string m_spawn_object_type = "";

            /// <summary> Minimum phase the player must be on for the spawn point to be active. 0-2 </summary>

            private int m_mininum_phase = 0;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the object.
        /// </summary>
        //=========================================================================================

        public EnemySpawnMarker() : base(false,false,false,false){}

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

            // Read all data:
     
            data.ReadString ( "SpawnObjectType"  , ref m_spawn_object_type   , "EnemyNinja" );
            data.ReadInt    ( "MinimumPhase"     , ref m_mininum_phase       , 0            );

            // Clamp the minimum phase from 0-2:

            if ( m_mininum_phase < 0 ) m_mininum_phase = 0;
            if ( m_mininum_phase > 2 ) m_mininum_phase = 2;
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

            // Write all data:
     
            data.Write( "SpawnObjectType"   , m_spawn_object_type   );
            data.Write( "MinimumPhase"      , m_mininum_phase       );
        }

    }   // end of class

}   // end of namespace
