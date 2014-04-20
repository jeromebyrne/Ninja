using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// This class manages a host of currently playing sounds and allows access to those 
    /// sounds to modify or change their parameters if required.
    /// </summary>
    //#############################################################################################

    public class AudioSystem
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> True if the audio system successfully initialized </summary>

            public bool Valid { get { return m_valid; } } 

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> True if the audio system successfully initialized </summary>

            private bool m_valid = false;

            /// <summary> XACT audio engine for playing sounds with </summary>

            private AudioEngine m_engine = null; 

            /// <summary> XACT wave bank containing our sound data </summary>

            private WaveBank m_waves = null;

            /// <summary> XACT sound bank containing cues to play that use the wave bank </summary>

            private SoundBank m_sounds = null;

            /// <summary> ID that the next sound played will be assigned. Incremented each  time a sound is played. </summary>
            
            private int m_next_id = 1;

            /// <summary> Hash table mapping from sound id's to active and playing cues </summary>

            private Dictionary<int,Cue> m_cues = new Dictionary<int,Cue>();

            /// <summary> List containing sound ids that were freed through deletion. Once this fills to a certain level, ids are reused. </summary>

            private LinkedList<int> m_free_ids = new LinkedList<int>();

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Maximum size of the free IDs list before we begin to recycle IDs. </summary>

            private const int FREE_IDS_LIST_MAX_SIZE = 1024;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the audio system object. If this operation fails 
        /// the audio systme will be flagged as invalid and it will not do anything.
        /// </summary>
        /// <param name="file_gs"> Name of the XACT global settings file to use.        </param>
        /// <param name="file_wb"> Name of the XACT wave bank to use.                   </param>
        /// <param name="file_sb"> Name of the XACT sound bank to use.                  </param>
        /// <param name="stream">  Whether this audio system will play streamed sounds. </param>
        //=========================================================================================

        public AudioSystem( string file_gs , string file_wb , string file_sb , bool stream )
        {
            // This might fail:

            try
            {
                // Load global settins file and make audio engine:

                m_engine = new AudioEngine( Locale.GetLocFile(file_gs) );

                // Load wave bank:

                if ( stream )
                {
                    // Load streamed wave bank:

                    m_waves = new WaveBank( m_engine, Locale.GetLocFile(file_wb) , 0 , 4 );                    
                }
                else
                {
                    // Load non streamed wave bank:

                    m_waves = new WaveBank( m_engine, Locale.GetLocFile(file_wb) );
                }
                
                // Load sound bank:

                m_sounds = new SoundBank( m_engine, Locale.GetLocFile(file_sb) );

                // Now ok:

                m_valid = true;
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); m_valid = false; }      

            #else
                
                catch ( Exception ){ m_valid = false; }

            #endif

        }

        //=========================================================================================
        /// <summary>
        /// Stops all currently playing cues in the activeCues table
        /// and also removes them.
        /// </summary>
        //=========================================================================================

        public void Restart()
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return;

            // This might fail:

            try
            {
                // traverse our dictionary of currently playing cues

                Dictionary<int,Cue>.Enumerator e = m_cues.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Get this cue:

                    Cue cue = e.Current.Value;

                    // Stop if playing:

                    if ( cue.IsPlaying ) cue.Stop(AudioStopOptions.Immediate);
                }

                // clear all cues from the table

                m_cues.Clear();

                // Reset next id

                m_next_id = 1;
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }      

            #else
                
                catch ( Exception ){}

            #endif            
        }

        //=========================================================================================
        /// <summary>
        /// Update checks the hash table containing the currently playing cues 
        /// and makes sure to delete and remove them from the hash table when 
        /// they are finished playing
        /// </summary>
        //=========================================================================================

        public void Update()
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return;

            // This might fail:

            try
            {
                // Put the id of all the sounds that have to be removed from the dicitonary here:

                LinkedList<int> finishedSounds = new LinkedList<int>();

                // traverse our dictionary of currently playing cues

                {
                    Dictionary<int,Cue>.Enumerator e = m_cues.GetEnumerator();

                    while ( e.MoveNext() )
                    {
                        // Get cue:

                        Cue cue = e.Current.Value;

                        // See if playing:

                        if ( cue.IsPlaying == false ) finishedSounds.AddLast(e.Current.Key);
                    }

                }

                // Remove all cues that aren't playing from the dictionary:

                {
                    LinkedList<int>.Enumerator e = finishedSounds.GetEnumerator();

                    while ( e.MoveNext() )
                    { 
                        // Remove this cue:

                        m_cues.Remove(e.Current); 

                        // Add it's id to the list of free id's:

                        m_free_ids.AddLast(e.Current);
                    }
                }
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }      

            #else
                
                catch ( Exception ){}

            #endif

        }

        //=========================================================================================
        /// <summary>
        /// Pauses all currently playing sounds in the audio system.
        /// </summary>
        /// <returns> True on success. False on failure. </returns>
        //=========================================================================================

        public bool PauseAll()
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return false;

            // This might fail:

            try
            {
                // traverse our dictionary of currently active cues

                Dictionary<int,Cue>.Enumerator e = m_cues.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Get this cue:

                    Cue cue = e.Current.Value;

                    // See if playing: if so then stop

                    if ( cue.IsPlaying ) cue.Pause();
                }

                // Return true for success:

                return true;
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Pauses a single sound.
        /// </summary>
        /// <param name="id"> Id of the sound to pause. </param>
        /// <returns> True on success. False on failure. </returns>
        //=========================================================================================

        public bool Pause( int id )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return false;

            // This might fail:

            try
            {
                // See if this cue exists:

                if ( m_cues.ContainsKey(id) )
                {
                    // Good- get the cue:

                    Cue cue = m_cues[id];

                    // Only do if the cue is not disposed:

                    if ( cue.IsDisposed == false )
                    {
                        // Now try to pause it:

                        if ( cue.IsPlaying ) cue.Pause();

                        // Return true for success:

                        return true;
                    }
                    else
                    {
                        // Cue disposed: so it is paused already in a way

                        return true;
                    }
                }
                else
                {
                    // Invalid cue: return false for failure

                    return false;
                }
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Resumes playing a paused sound.
        /// </summary>
        /// <param name="id"> Id of the sound to pause. </param>
        /// <returns> True on success. False on failure. </returns>
        //=========================================================================================

        public bool Resume( int id )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return false;

            // This might fail:

            try
            {
                // See if this cue exists:

                if ( m_cues.ContainsKey(id) )
                {
                    // Good- get the cue:

                    Cue cue = m_cues[id];

                    // If the cue is disposed then do nothing:

                    if ( cue.IsDisposed == false )
                    {
                        // Now try to pause it:

                        if ( cue.IsPaused ) cue.Resume();

                        // Return true for success:

                        return true;
                    }
                    else
                    {
                        // Cue disposed: return false for failure

                        return false;
                    }
                }
                else
                {
                    // Invalid cue: return false for failure

                    return false;
                }
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Stops the sound with the given id.
        /// </summary>
        /// <param name="id"> Id of the sound to pause. </param>
        /// <returns> True on success. False on failure. </returns>
        //=========================================================================================

        public bool Stop( int id )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return false;

            // This might fail:

            try
            {
                // See if this cue exists:

                if ( m_cues.ContainsKey(id) )
                {
                    // Good- get the cue:

                    Cue cue = m_cues[id];

                    // If the cue is disposed then do nothing:

                    if ( cue.IsDisposed == false )
                    {
                        // Now try to pause it:

                        if ( cue.IsPaused || cue.IsPlaying ) cue.Stop( AudioStopOptions.AsAuthored );

                        // Return true for success:

                        return true;
                    }
                    else
                    {
                        // Cue disposed: return true as its stopped anyway

                        return true;
                    }
                }
                else
                {
                    // Invalid cue: return false for failure

                    return false;
                }
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Stops all currently playing sounds.
        /// </summary>
        /// <returns> True on success. False on failure. </returns>
        //=========================================================================================

        public bool StopAll()
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return true;

            // This might fail:

            try
            {
                // Make a list of all the cue id's:

                LinkedList<int> ids = new LinkedList<int>();

                // Populate the id list:

                {
                    // Get enumerator into cue list:

                    Dictionary<int,Cue>.Enumerator e = m_cues.GetEnumerator();

                    // Run through the list and add all the sounds in:

                    while ( e.MoveNext() ) ids.AddLast( e.Current.Key );
                }
                
                // Store here if we could stop all the sounds:

                bool stopped_all = true;

                // Now stop all the sounds:

                while ( ids.Count > 0 )
                {
                    // Stop this sound:

                    bool success = Stop(ids.First.Value);

                    // See if successful:

                    if ( success == false ) stopped_all = false;

                    // Remove this id from the list:

                    ids.RemoveFirst();
                }

                // Return if all the sounds were stopped:

                return stopped_all;
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Resumes all currently paused sounds.
        /// </summary>
        /// <returns> True on success. False on failure. </returns>
        //=========================================================================================

        public bool ResumeAll()
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return false; 

            // This might fail:

            try
            {
                // traverse our dictionary of currently active cues

                Dictionary<int,Cue>.Enumerator e = m_cues.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Get this cue:

                    Cue cue = e.Current.Value;

                    // If paused then play:

                    if ( cue.IsPaused ) cue.Play();
                }

                // Return true for success:

                return true;
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Adds a cue to the activeCue list and plays it immediately.
        /// </summary>
        /// <param name="name"> Name of the cue to play </param>
        /// <returns> 
        /// ID of the sound being played or zero on failure. Note that the ID may be reused at some 
        /// point in the future, so all code referencing it should periodically check if the sound 
        /// is still in existance, and wipe the reference if it is not.
        /// </returns>
        //=========================================================================================

        public int Play( string name )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return 0;

            // This might fail:

            try
            {
                // Abort if no name given:

                if ( name == null || name.Length <= 0 ) return 0;

                // Get the cue from the soundbank               

                Cue cue = m_sounds.GetCue(name);

                // Make sure everything is ok with the cue:

                if ( cue != null )
                {
                    // Pick an id for the sound:

                    int id = 0;

                    // See if the free id's list is big enough: if so then reuse the first id:

                    if ( m_free_ids.Count > FREE_IDS_LIST_MAX_SIZE )
                    {
                        // Good: reuse this id:

                        id = m_free_ids.First.Value; m_free_ids.RemoveFirst();
                    }
                    else
                    {
                        // Use the next available id:

                        id = m_next_id;

                        // increment to the next ID that can be used by a cue

                        m_next_id++;
                    }

                    // add the cue to the active cue table along with its ID
                    
                    m_cues.Add( id, cue);

                    // play the cue
                    
                    cue.Play();

                    // Return the id of the sound:

                    return id;
                }
                else
                {
                    // Not a valid cue: return 0

                    return 0;
                }
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e )
                { 
                    // Print what happened:

                    DebugConsole.PrintException(e); DebugConsole.Print("\nFailed to play cue: " + name);

                    // Return 0 for failure:

                    return 0;
                }            

            #else
                
                catch ( Exception ){ return 0; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Tells if a sound is still present in the audio system. After a sound has stopped playing 
        /// it will be automatically removed and in this case the function will return false. That 
        /// is, if the id hasn't been reused before then- which is a remote possibility at bost.
        /// </summary>
        //=========================================================================================

        public bool Exists( int id )
        {
            // If we didn't initialize then it definitely does not exist:

            if ( m_valid == false ) return false;

            // Otherwise check if it exists in the dictionary:

            return m_cues.ContainsKey(id);
        }

        //=========================================================================================
        /// <summary>
        /// Tells if the sound with the given id is playing in the audio system.
        /// </summary>
        /// <returns> True if the sound is playing, false otherwise or if it doesn't exist </returns>
        //=========================================================================================

        public bool IsPlaying( int id )
        {
            // If we didn't initialize then it definitely is not playing:

            if ( m_valid == false ) return false;

            // See if it exists in the dictionary:

            if ( m_cues.ContainsKey(id) )
            {
                // Exists in the dictionary: see if it is playing:

                Cue cue = m_cues[id];

                // This might fail:

                try
                {
                    // Return if the sound is playing:

                    return cue.IsPlaying;
                }

                // On windows debug show what happened if something went wrong
                
                #if WINDOWS_DEBUG

                    catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

                #else

                    catch ( Exception ){ return false; }

                #endif
            }
            else
            {
                // Doesn't exist in the dictionary - return false

                return false;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Tells if the sound with the given id is paused in the audio system.
        /// </summary>
        /// <returns> True if the sound is paused or if it doesn't exist, false otherwise </returns>
        //=========================================================================================

        public bool IsPaused( int id )
        {
            // If we didn't initialize properly then sound is permanently paused so to speak:

            if ( m_valid == false ) return true;

            // See if it exists in the dictionary:

            if ( m_cues.ContainsKey(id) )
            {
                // Exists in the dictionary: see if it is paused:

                Cue cue = m_cues[id];

                // This might fail:

                try
                {
                    // Return if the sound is paused:

                    return cue.IsPaused;
                }

                // On windows debug show what happened if something went wrong
                
                #if WINDOWS_DEBUG

                    catch ( Exception e ){ DebugConsole.PrintException(e); return true; }

                #else

                    catch ( Exception ){ return true; }

                #endif
            }
            else
            {
                // Doesn't exist in the dictionary - return true

                return true;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Tells if the sound with the given id is stopped in the audio system.
        /// </summary>
        /// <returns> True if the sound is stopped or if it doesn't exist, false otherwise </returns>
        //=========================================================================================

        public bool IsStopped( int id )
        {
            // If we didn't initialize properly then sound is permanently paused so to speak:

            if ( m_valid == false ) return true;

            // See if it exists in the dictionary:

            if ( m_cues.ContainsKey(id) )
            {
                // Exists in the dictionary: see if it is paused:

                Cue cue = m_cues[id];

                // This might fail:

                try
                {
                    // Return if the sound is stopped:

                    return cue.IsStopped;
                }

                // On windows debug show what happened if something went wrong
                
                #if WINDOWS_DEBUG

                    catch ( Exception e ){ DebugConsole.PrintException(e); return true; }

                #else

                    catch ( Exception ){ return true; }

                #endif
            }
            else
            {
                // Doesn't exist in the dictionary - return true

                return true;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Sets a local variable relating to a cue.
        /// </summary>
        /// <param name="id"> id of the sound to play </param>
        /// <param name="name"> Name of the variable to set </param>
        /// <param name="value"> Value to set the variable to </param>
        /// <returns> True if the operation succeeded, false otherwise. </returns>
        //=========================================================================================

        public bool SetLocal( int id , string name , float value )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return false;

            // This might fail:

            try
            {
                // Make sure a valid cue:

                if ( m_cues.ContainsKey(id) )
                {
                    // Ok: get the cue

                    Cue cue = m_cues[id];

                    // Only do if cue is not disposed:

                    if ( cue.IsDisposed == false )
                    {
                        // Set the variable

                        cue.SetVariable(name,value);

                        // Return true for success:

                        return true;
                    }
                    else
                    {
                        // Cue disposed: do nothing

                        return false;
                    }
                }
                
                #if WINDOWS_DEBUG

                    else
                    {
                        // On debug windows warn if an invalid id is given

                        DebugConsole.Print("\nAudioSystem.SetVariable(): Unable to find cue with id " + id );

                        // Return false for failure:

                        return false;
                    }

                #else
                    
                    else
                    {
                        // Return false for failure:

                        return false;
                    }

                #endif
                
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Gets a local variable relating to a cue.
        /// </summary>
        /// <param name="id"> Id of the sound to get the variable for </param>
        /// <param name="name"> Name of the variable to get </param>
        /// <returns> Value of the variable, or float.NaN on failure. </returns>
        //=========================================================================================

        public float GetLocal( int id, string name )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return float.NaN;

            // This might fail:

            try
            {
                // Make sure the cue exists:

                if ( m_cues.ContainsKey(id) )
                {
                    // Get the cue:

                    Cue cue = m_cues[id]; 

                    // Only do if not dispose:

                    if ( cue.IsDisposed == false )
                    {
                        // Return the variable:

                        return cue.GetVariable(name);
                    }
                    else
                    {
                        // Disposed:

                        return float.NaN;
                    }
                }

                #if WINDOWS_DEBUG

                    else
                    {
                        // On debug windows warn if an invalid id is given

                        DebugConsole.Print("\nAudioSystem.GetVariable(): Unable to find cue with id " + id );

                        // Return NaN

                        return float.NaN;
                    }

                #else

                    else { return float.NaN; }

                #endif

            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return float.NaN; }      

            #else
                
                catch ( Exception ){ return float.NaN; }

            #endif
        }
    
        //=========================================================================================
        /// <summary>
        /// Sets a global variable for the audio engine.
        /// </summary>
        /// <param name="name"> Name of the variable to set </param>
        /// <param name="value"> Value to set the variable to </param>
        /// <returns> True if the operation succeeded, false otherwise. </returns>
        //=========================================================================================

        public bool SetGlobal( string name , float value )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return false;

            // This might fail:

            try
            {
                // Set it:

                m_engine.SetGlobalVariable( name , value );
                
                // Return true for success:

                return true;
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }      

            #else
                
                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Gets a global variable for the audio engine.
        /// </summary>
        /// <param name="name"> Name of the variable to get </param>
        /// <returns> Value of the variable, or float.NaN on failure. </returns>
        //=========================================================================================

        public float GetGlobal( string name )
        {
            // If we have not initialised successfully then abort

            if ( m_valid == false ) return float.NaN;

            // This might fail:

            try
            {
                // Return the variable

                return m_engine.GetGlobalVariable( name );
            }

            // On windows if something went wrong then print what happened:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); return float.NaN; }      

            #else
                
                catch ( Exception ){ return float.NaN; }

            #endif
        }

    }   // end of class

}   // end of namespace
