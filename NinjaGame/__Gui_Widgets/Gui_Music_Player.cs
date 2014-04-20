using System;
using System.Collections.Generic;
using System.Text;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// A very simple widget that plays the specified music cue when the gui is loaded.
    /// This also stops all other pieces of music that are currently playing.
    /// </summary>
    //#############################################################################################

    public class Gui_Music_Player : GuiWidget
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Name of the music cue to play. </summary>

            private string m_music_cue_name = "";

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
            // Call base class function

            base.ReadXml(data);

            // Read all data:

            data.ReadString( "MusicCueName" , ref m_music_cue_name , "" );
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
            // Call base class function

            base.WriteXml(data);

            // Write all data:

            data.Write( "MusicCueName" , m_music_cue_name );
        }

        //=========================================================================================
        /// <summary>
        /// On gui loaded event. Stops all playing music and plays the music specified in this widget.
        /// </summary>
        //=========================================================================================

        public override void OnGuiLoaded()
        {
            // Call base function

            base.OnGuiLoaded();

            // Stop all playing music:

            Core.Music.StopAll();

            // If we have music then play it:

            if ( m_music_cue_name != null && m_music_cue_name.Length > 0 )
            {
                Core.Music.Play(m_music_cue_name);
            }
        } 

    }   // end of class

}   // end of namespace
