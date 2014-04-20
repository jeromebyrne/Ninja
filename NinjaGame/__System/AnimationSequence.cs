using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that represents a sequence in an animation. Contains a list of frames in that 
    /// sequence as well as the frame rate that the sequence should be played at.
    /// </summary>
    //#############################################################################################

    public class AnimationSequence
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> An array of frames which make up the animation sequence. </summary>

            public Texture2D[] Frames { get { return m_frames; } }

            /// <summary> Frame rate which the sequence should normally be played at. </summary>

            public float FrameRate { get { return m_frame_rate; } }

            /// <summary> Maximum frame rate the sequence should be played at. This is a guide only. It does not have to be obeyed. </summary>

            public float MaxFrameRate { get { return m_max_frame_rate; } }

            /// <summary> Mininum frame rate the sequence can be played at. This is a guide only. It does not have to be obeyed. </summary>

            public float MinFrameRate { get { return m_min_frame_rate; } }

            /// <summary> Name of the animation sequence. Such as running etc.. </summary>

            public string Name { get { return m_name; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> An array of frames which make up the animation sequence. </summary>
            
            private Texture2D[] m_frames = null;

            /// <summary> Frame rate which the sequence should normally be played at. </summary>

            private float m_frame_rate = 5;

            /// <summary> Minimum frame rate the sequence can be played at. This is a guide only. It does not have to be obeyed. </summary>

            private float m_min_frame_rate = 5;

            /// <summary> Maximum frame rate the sequence can be played at. This is a guide only. It does not have to be obeyed. </summary>

            private float m_max_frame_rate = 5;

            /// <summary> Name of the animation sequence. Such as running etc.. </summary>

            private string m_name = "";

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the animation sequence from the data in the given xml node.
        /// </summary>
        /// <param name="xmlNode"> Node containing the frames and frame rate for the sequence. </param>
        //=========================================================================================

        public AnimationSequence( XmlNode xml_node )
        {
            // On windows debug throw an exception if the node given is invalid:

            #if WINDOWS_DEBUG 

                if ( xml_node == null ) throw new ArgumentNullException("XML node for animation sequence cannot be null !!");

            #endif

            // Read the xml data:
                
            ReadXML(xml_node);
        }

        ///========================================================================================
        /// <summary>
        /// Reads all the data for the sequence from the given xml node. The data 
        /// includes both the frame rate as an attribute and the frame textures themselves 
        /// which are included inside sub tags.
        /// </summary>
        /// <param name="xml_node"> node to read the data for the sequence from </param>
        ///========================================================================================
        
        private void ReadXML( XmlNode xml_node )
        {
            // Read the name of the sequence.

            m_name = xml_node.Name;

            // Run through the attributes for the sequence:

            foreach ( XmlAttribute attrib in xml_node.Attributes )
            {
                // See if this is the frame rate attribute:

                if ( attrib.Name.Equals( "FrameRate" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    // Try and save the frame rate:

                    try { m_frame_rate = Convert.ToSingle( attrib.Value.Trim(), Locale.DevelopmentCulture.NumberFormat ); } 
                    
                    // Show errors on windows debug

                    #if WINDOWS_DEBUG

                        catch ( Exception e ){ DebugConsole.PrintException(e); }
                    
                    #else 

                        catch {}

                    #endif
                }
                else if ( attrib.Name.Equals( "MaxFrameRate" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    // Maximum frame rate value: try and save it:

                    try { m_max_frame_rate = Convert.ToSingle( attrib.Value.Trim(), Locale.DevelopmentCulture.NumberFormat ); }
                                        
                    // Show errors on windows debug

                    #if WINDOWS_DEBUG

                        catch ( Exception e ){ DebugConsole.PrintException(e); }
                    
                    #else 

                        catch {}

                    #endif
                }
                else if ( attrib.Name.Equals( "MinFrameRate" , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    // Minimum frame rate value: try and save it:

                    try { m_min_frame_rate = Convert.ToSingle( attrib.Value.Trim(), Locale.DevelopmentCulture.NumberFormat ); }
                                        
                    // Show errors on windows debug

                    #if WINDOWS_DEBUG

                        catch ( Exception e ){ DebugConsole.PrintException(e); }
                    
                    #else 

                        catch {}

                    #endif
                }

            }

            // Try loading all the sequence frames:

                // Store the frames here:

                List<Texture2D> frames = new List<Texture2D>();

                // Attempt to load each frame

                foreach ( XmlNode node in xml_node.ChildNodes )
                {
                    // Attempt to load this frame:

                    try
                    {
                        // Make sure this is an element type node:

                        if ( node.NodeType == XmlNodeType.Element )
                        {
                            // Try and load this frame:

                            frames.Add( Core.Graphics.LoadTexture( node.InnerText.Trim() ) );
                        }
                    }

                    #if WINDOWS_DEBUG

                        // On windows debug print what happened if something went wrong

                        catch (Exception e) { DebugConsole.PrintException(e); }

                    #else
                        
                        catch( Exception ){}

                    #endif
                }

                // Save the frames to the array:

                if ( frames.Count > 0 ) m_frames = frames.ToArray();
        }

    }   // end of class

}   // end of namespace
