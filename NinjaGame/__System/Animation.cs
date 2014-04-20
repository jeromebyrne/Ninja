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
using System.Xml;

//################################################################################################
//################################################################################################

namespace NinjaGame
{
    public class Animation
    {
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This is the suggested bounding box size the object using the animation should use. 
            /// This is also stored in the xml file with the rest of the animation data.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 BoxDimensions { get { return m_animation_bounding_box; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This is the suggested bounding elipse size the object using the animation should use. 
            /// This is also stored in the xml file with the rest of the animation data.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Vector2 EllipseDimensions { get { return m_animation_bounding_ellipse; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> stores all of our animation parts in a dictionary which can be retrieved by name. </summary>
            
            private Dictionary<string, AnimationPart> m_parts = new Dictionary<string,AnimationPart>();
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This is the suggested bounding box size the object using the animation should use. 
            /// This is also stored in the xml file with the rest of the animation data.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Vector2 m_animation_bounding_box = Vector2.Zero;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This is the suggested bounding elipse size the object using the animation should use. 
            /// This is also stored in the xml file with the rest of the animation data.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Vector2 m_animation_bounding_ellipse = Vector2.Zero;

        //=========================================================================================
        /// <summary>
        /// Our constructor, takes a file name as a parameter, this file is the xml document which 
        /// holds all of our animation info.
        /// </summary>
        /// <param name="file"> Name of the xml file containing the animations for the object </param>
        //=========================================================================================
        
        public Animation( string file )
        {
            // On windows debug throw an exception if the file given is invalid:

            #if WINDOWS_DEBUG 

                if ( file == null || file.Length <= 0 ) throw new ArgumentNullException("Animation file name must be given");

            #endif

            // Read the file given

            if ( file != null ){ ReadXML(file); }
        }

        //=========================================================================================
        /// <summary>
        /// Makes a copy of the given animation as the basis for this animation. Use to avoid 
        /// re-loading animation data from the disk.
        /// </summary>
        /// <param name="file"> Name of the xml file containing the animations for the object </param>
        //=========================================================================================

        public Animation( Animation animation )
        {
            // Only do if the other animation is not null:

            if ( animation != null )
            {
                // Copy bounding box and ellipse dimensions:

                m_animation_bounding_box        = animation.m_animation_bounding_box;
                m_animation_bounding_ellipse    = animation.m_animation_bounding_ellipse;

                // See if it has any parts:

                if ( animation.m_parts != null )
                {
                    // Run through all the parts in the other animation and copy:

                    Dictionary<string,AnimationPart>.Enumerator e = animation.m_parts.GetEnumerator();

                    // Run through all parts:

                    while ( e.MoveNext() )
                    {
                        // Copy this part:
                        
                        m_parts[e.Current.Key] = new AnimationPart( e.Current.Value );
                    }
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// returns a specific part from our part dictionary based on the name passed in.
        /// </summary>
        /// <param name="part_name"> Name of the part to retrieve </param>
        /// <returns> The part or null if not found. </returns>
        //=========================================================================================

        public AnimationPart GetPart(string part_name )
        {
            if ( part_name != null )
            {
                // Make lowercase copy of the part name:

                string part_name_lower = part_name.ToLower();

                // See if it exists in the dictionary:

                if ( m_parts.ContainsKey(part_name_lower) )
                {
                    // Return the part:

                    return m_parts[part_name_lower];
                }
                else
                {
                    // Couldn't find the part: return null

                    return null;
                }

            }
            else
            {
                // Invalid part name given. return null.

                return null;
            }            
        }

        //=========================================================================================
        /// <summary>
        /// Sets the animation sequence used on a particular part of the animation.
        /// </summary>
        /// <param name="part_name"> Name of the part to set sequence for. </param>
        /// <param name="sequence_name"> Name of the animation sequence to use. </param>
        //=========================================================================================

        public void SetPartSequence( string part_name , string sequence_name )
        {
            // Try and get the part:

            AnimationPart part = GetPart(part_name);

            // If it exists then set the sequence on the part:

            if ( part != null ) part.SetSequence(sequence_name);
        }

        //=========================================================================================
        /// <summary>
        /// Takes a sring as a parameter which should be the path of the xml file which holds all 
        /// of our animation information, all of our animations parts (and sequences, and frames) 
        /// are loaded and stored in the 
        /// parts dictionary
        /// </summary>
        /// <param name="file"> Name of the xml file containing the animation data </param>
        //=========================================================================================

        private void ReadXML( string file )
        {
            // Clear the animation parts dictionary:

            m_parts.Clear();

            // Attempt to read the given file:

            try
            {
                // create a new xml document to load up our xml file

                XmlDocument xmlDoc = new XmlDocument();

                // load it

                xmlDoc.Load( Locale.GetLocFile(file) );

                // Store the root node

                XmlNode root = xmlDoc.FirstChild;

                // see if there is a root node:

                if ( root != null )
                {
                    // Run through all the attributes in the node and read recognised ones:

                    foreach ( XmlAttribute a in root.Attributes )
                    {
                        // This might fail:

                        try
                        {
                            // See what attribute this is and save it if recognised:

                            if ( a.Name.Equals( "BoxDimensionsX" , StringComparison.CurrentCultureIgnoreCase ) )
                            {
                                m_animation_bounding_box.X = Convert.ToSingle( a.Value, Locale.DevelopmentCulture.NumberFormat );
                            }
                            else if ( a.Name.Equals( "BoxDimensionsY" , StringComparison.CurrentCultureIgnoreCase ) )
                            {
                                m_animation_bounding_box.Y = Convert.ToSingle( a.Value , Locale.DevelopmentCulture.NumberFormat );
                            }
                            else if ( a.Name.Equals( "EllipseDimensionsX" , StringComparison.CurrentCultureIgnoreCase ) )
                            {
                                m_animation_bounding_ellipse.X = Convert.ToSingle( a.Value , Locale.DevelopmentCulture.NumberFormat );
                            }
                            else if ( a.Name.Equals( "EllipseDimensionsY" , StringComparison.CurrentCultureIgnoreCase ) )
                            {
                                m_animation_bounding_ellipse.Y = Convert.ToSingle( a.Value , Locale.DevelopmentCulture.NumberFormat );
                            }
                        }

                        #if WINDOWS_DEBUG
                                
                            // Show what happened on windows debug

                            catch ( Exception e ){ DebugConsole.PrintException(e); }

                        #else

                            catch ( Exception ){}

                        #endif
                    }

                    // load the data for each node:

                    foreach ( XmlNode node in root.ChildNodes )
                    {
                        // Make sure this is an element type:

                        if ( node.NodeType == XmlNodeType.Element ) m_parts.Add( node.Name.ToLower() , new AnimationPart(node) );
                    }
                }

            }   // end of try block

            // On windows debug print whatever exception happens

            #if WINDOWS_DEBUG

                catch (Exception e){ DebugConsole.PrintException(e); }

            #else

                catch( Exception ){}

            #endif             
        }

    }   // end of class

}   // end of namespace
