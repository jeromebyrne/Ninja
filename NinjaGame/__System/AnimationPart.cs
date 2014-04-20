using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

//################################################################################################
//################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that represents a body part in an animation. Contains a list of sequences for that 
    /// part, and each sequence contains a list of frames. Each part can be animated in whatever 
    /// way the application chooses.
    /// </summary>
    //#############################################################################################

    public class AnimationPart
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Tells if the animation is finished. Returns true if the animation is on the last frame
            /// and it is time to change over to another frame.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool Finished { get { return IsFinished(); } }

            /// <summary> The current animation frame in the current animation sequence that is being shown </summary>

            public Texture2D CurrentFrame
            {
                get 
                { 
                    // If we have no sequence return null:

                    if ( m_current_sequence == null ) return null;

                    // If there are no frames also return null:

                    if ( m_current_sequence.Frames.Length <= 0 ) return null;

                    // If we are past the end then return the last frame:

                    if ( m_current_frame_num >= m_current_sequence.Frames.Length )
                    {
                        return m_current_sequence.Frames[m_current_sequence.Frames.Length-1];
                    }
                    
                    // Otherwise just return the frame normally

                    return m_current_sequence.Frames[m_current_frame_num]; 
                }
            }

            /// <summary> Returns the number of frames in the current animation sequence. </summary>

            public int FrameCount
            {
                get 
                { 
                    // Make sure the sequence isn't null - if it is return zero.

                    if ( m_current_sequence != null ) 
                    {
                        return m_current_sequence.Frames.Length;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            /// <summary> Returns the current frame number the animation is on. If the animation is finished then the last one will be returned. </summary>

            public int FrameNumber 
            { 
                get 
                { 
                    // If there is no sequence return 0:

                    if ( m_current_sequence == null ) return 0;

                    // If there are no frames in the animation return 0

                    if ( m_current_sequence.Frames.Length <= 0 ) return 0;

                    // If we are past the last frame (finished animation) in the sequence return the last one:

                    if ( m_current_frame_num >= m_current_sequence.Frames.Length ) return m_current_sequence.Frames.Length - 1;

                    // In normal cases just return the frame number:

                    return m_current_frame_num; 
                } 
            } 

            /// <summary> Gets how much the animation part should be offset from the center of the object rendering it. </summary>

            public Vector2 Offset { get { return m_offset; } }

            /// <summary> Gets the size of the part </summary>
            
            public Vector2 Size { get { return m_size; } }

            /// <summary> The current animation sequence that the animation part is playing </summary>

            public AnimationSequence CurrentSequence { get { return m_current_sequence; } }

            /// <summary> Name of the animation part, such as legs etc.. </summary>

            public string Name { get { return m_name; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> A map containing all of the sequences for this animation part, indexed by name. </summary>

            private Dictionary<string,AnimationSequence> m_sequences = new Dictionary<string,AnimationSequence>();
            
            /// <summary> The current animation sequence that the animation part is playing </summary>

            private AnimationSequence m_current_sequence = null; 

            /// <summary> The current animation frame number in the current animation sequence that is being shown </summary>

            private int m_current_frame_num = 0;

            /// <summary> The time in seconds since the last frame change. Used to determine when we need to change frames. </summary>
            
            private float m_frame_time_elapsed = 0;

            /// <summary> Offset of the part from the center of the object being animated </summary>

            private Vector2 m_offset = Vector2.Zero;

            /// <summary> Size of the part </summary>
            
            private Vector2 m_size = Vector2.One * 128;

            /// <summary> Name of the animation part, such as legs etc.. </summary>

            private string m_name = "";

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the animation part from the data in the given xml node.
        /// This node should contain data about the part such as sequences and underlying frames, 
        /// as well as the size and offset of the part.
        /// </summary>
        /// <param name="xml_node"> 
        /// XML Node containing all the sequences for the animation part.
        /// </param>
        //=========================================================================================

        public AnimationPart( XmlNode xml_node )
        {
            // On windows debug throw an exception if the node given is invalid:

            #if WINDOWS_DEBUG 

                if ( xml_node == null ) throw new ArgumentNullException("Xml Node for animation part cannot be null");

            #endif

            // Initialise size to it's default:

            m_size = new Vector2( 128 , 128 );

            // Read the animation part data from the xml node

            if ( xml_node != null ) ReadXML( xml_node );

            // Run through the nodes attributes:

            foreach ( XmlAttribute attrib in xml_node.Attributes )
            {
                // See if it matches one of the attributes for an animation part:

                if ( attrib.Name.Equals( "OffsetX" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    try { m_offset.X = Convert.ToSingle( attrib.Value, Locale.DevelopmentCulture.NumberFormat ); } catch {}
                }
                else if ( attrib.Name.Equals( "OffsetY" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    try { m_offset.Y = Convert.ToSingle( attrib.Value, Locale.DevelopmentCulture.NumberFormat ); } catch {}
                }
                else if ( attrib.Name.Equals( "SizeX" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    try { m_size.X = Convert.ToSingle( attrib.Value ); } catch {}
                }
                else if ( attrib.Name.Equals( "SizeY" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    try { m_size.Y = Convert.ToSingle( attrib.Value, Locale.DevelopmentCulture.NumberFormat ); } catch {}
                }
            }

            // Get the first animation sequence and set it as the current sequence:

            {                
                // Get enumerator into dictionary:

                Dictionary<string, AnimationSequence>.Enumerator e = m_sequences.GetEnumerator();

                // Get first part if it exists and make it the current sequence:

                if ( e.MoveNext() ) m_current_sequence = m_sequences[e.Current.Key];
            }
        }

        //=========================================================================================
        /// <summary>
        /// Copy constructor. Creates this animation part from another one.
        /// </summary>
        /// <param name="part"> 
        /// Other animation part to make this one from.
        /// </param>
        //=========================================================================================

        public AnimationPart( AnimationPart part )
        {
            // Make sure it isn't null:

            if ( part != null )
            {
                // Copy all non-sequence data

                m_current_sequence      = part.CurrentSequence;
                m_current_frame_num     = part.m_current_frame_num;
                m_frame_time_elapsed    = part.m_frame_time_elapsed;
                m_offset                = part.m_offset;
                m_size                  = part.m_size;
                m_name                  = part.m_name;

                // See if the part has sequences:

                if ( part.m_sequences != null )
                {
                    // Run through all the sequences in the other part and copy:

                    Dictionary<string,AnimationSequence>.Enumerator e = part.m_sequences.GetEnumerator();

                    // Run through all sequences:

                    while ( e.MoveNext() )
                    {
                        // Copy this sequence: we only need a reference so that's ok.. 
                        
                        m_sequences[e.Current.Key] = e.Current.Value;
                    }
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Sets the current animation sequence in use by this part.
        /// </summary>
        /// <param name="name"> Name of the sequence to use. </param>
        //=========================================================================================
        
        public void SetSequence( string name )
        {
            // Do nothing if no name given:

            if ( name == null || name.Length == 0 ) return;

            // Start from the first frame

            Restart();

            // Make a lowercase copy of the sequence name for case insensitive searchng:

            name = name.ToLower();

            // Only switch if the part exists:

            if ( m_sequences.ContainsKey(name) )
            {
                AnimationSequence new_sequence = m_sequences[name];

                if ( new_sequence != null )
                {
                    m_current_sequence = m_sequences[name];
                }
            }
        }

        ///========================================================================================
        /// <summary>
        /// Advances the animation of the current sequence. If the animation has reached 
        /// the end then it is returned back to the start.
        /// </summary>
        ///========================================================================================

        public void AnimateLooped()
        {
            // Animate at the frame rate for the current sequence:

            if ( m_current_sequence != null ) AnimateLooped( m_current_sequence.FrameRate );
        }

        ///========================================================================================
        /// <summary>
        /// Advances the animation of the current sequence. If the animation has reached 
        /// the end then it is returned back to the start.
        /// </summary>
        /// <param name="frame_rate"> Frame rate to animate at </param>
        ///========================================================================================

        public void AnimateLooped( float frame_rate )
        {
            // Abort if there is no current sequence:

            if ( m_current_sequence == null ) return;

            // If there are no frames then abort:

            if ( m_current_sequence.Frames.Length <= 0 ) return;

            // If the frame rate is negative or zero then abort:

            if ( frame_rate <= 0 ) return;

            // Increment the frame time elapsed

            m_frame_time_elapsed += Core.Timing.ElapsedTime;

            // Get the amount of time each frame takes:

            float time_per_frame = 1.0f / frame_rate;

            // See if it is time to change frame;

            if ( m_frame_time_elapsed >= time_per_frame )
            {
                // Done with this frame. Decrease the frame time elapsed.

                m_frame_time_elapsed -= time_per_frame;

                // Increment to the next frame:

                m_current_frame_num++;

                // If past the end then put back to the first frame:

                if ( m_current_frame_num >= CurrentSequence.Frames.Length ) m_current_frame_num = 0;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Advances the animation of the current sequence. If the animation has reached the end 
        /// it holds it there and Finished will be true
        /// </summary>
        //=========================================================================================

        public void Animate()
        {
            // Animate at the frame rate for the current sequence:

            if ( m_current_sequence != null ) Animate( m_current_sequence.FrameRate );
        }

        //=========================================================================================
        /// <summary>
        /// Advances the animation of the current sequence. If the animation has reached the end 
        /// it holds it there and Finished will be true.
        /// </summary>
        /// <param name="frame_rate"> Frame rate to run the animation at. </param>
        //=========================================================================================

        public void Animate( float frame_rate )
        {
            // Abort if there is no current sequence:

            if ( m_current_sequence == null ) return;

            // If there are no frames then abort:

            if ( m_current_sequence.Frames.Length <= 0 ) return;

            // If the frame rate is zero or less then do nothing:

            if ( frame_rate <= 0 ) return;

            // If not finished then move on the animation

            if ( IsFinished() == false ) 
            {
                // Increment the frame time elapsed

                m_frame_time_elapsed += Core.Timing.ElapsedTime;                

                // Get the amount of time each frame takes:

                float time_per_frame = 1.0f / frame_rate;

                // See if it is time to change frame;

                if ( m_frame_time_elapsed >= time_per_frame )
                {
                    // Done with this frame. Decrease the frame time elapsed.

                    m_frame_time_elapsed -= time_per_frame;

                    // Increment to the next frame:

                    m_current_frame_num++;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Restarts the current animation sequence back to the first frame.
        /// </summary>
        //=========================================================================================
        
        public void Restart()
        {
            m_current_frame_num = 0; m_frame_time_elapsed = 0;
        }

        //=========================================================================================
        /// <summary>
        /// Tells if the current animation sequence is finished. 
        /// </summary>
        /// <returns> 
        /// Returns true if the the current animation sequence is past the last frame.
        /// </returns>
        //=========================================================================================

        private bool IsFinished()
        {
            // See if we have an animation to begin with:

            if ( m_current_sequence != null )
            {
                // See if we are past the last frame: if so then return true

                if ( m_current_frame_num >= m_current_sequence.Frames.Length  ) return true;
            }
            else
            {
                // No sequence: finished

                return true;
            }

            // Not finished: return false

            return false;
        }

        //=========================================================================================
        /// <summary>
        /// Reads all the animation sequences for this part contained within the given xml node.
        /// </summary>
        /// <param name="xmlNode"> The node to read the sequences from. </param>
        //=========================================================================================

        private void ReadXML( XmlNode xml_node )
        {
            // Save the name of the animation part

            m_name = xml_node.Name;

            // Try and read each animation sequence for the part 

            try
            {
                foreach ( XmlNode node in xml_node.ChildNodes )
                {
                    // Make sure this is an element type:

                    if ( node.NodeType == XmlNodeType.Element )
                    {
                        // Make a lowercase copy of the sequence name for later case insensitive comparison

                        m_sequences.Add( node.Name.ToLower() , new AnimationSequence(node) );
                    }
                }
            }
            
            // On windows debug print what went wrong if something went wrong
            
            #if WINDOWS_DEBUG 

                catch ( Exception e ) { DebugConsole.PrintException(e);}
            
            #else
                
                catch( Exception ){}

            #endif
        }

    }   // end of class

}   // end of namespace
