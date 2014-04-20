using System;
using System.Collections.Generic;
using System.Text;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary>
    /// This helper object provides functions to search for objects in a level based
    /// on their type or name.
    /// </summary>
    //
    //#############################################################################################

    public class LevelSearchQuery
    {

        //=========================================================================================
        // Variables 
        //=========================================================================================

            /// <summary> Level the search query belongs to. </summary>

            Level m_level = null;
        
        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a level search query. 
        /// </summary>
        /// 
        /// <param name="parent"> 
        /// Level the search query belongs to. This levels data is what will be searched. 
        /// This cannot be null !
        /// </param>
        //=========================================================================================

        public LevelSearchQuery( Level parent )
        {
            // Ensure that this is not null in debug windows:

            #if WINDOWS_DEBUG

                if ( parent == null ) throw new Exception("LevelSearchQuery must have a valid parent Level object");

            #endif

            // Save the level:

            m_level = parent;
        }

        //=========================================================================================
        /// <summary>
        /// Searchs for a game object according to it's id. Id's are unique so only 
        /// one game object can be returned.
        /// </summary>
        /// <param name="object_id"> Id of game object to find </param>
        /// <returns> The game object in question or null on failure. </returns>
        //=========================================================================================

        public GameObject FindById( int object_id )
        {
            // See if the object exists:

            if ( m_level.Data.Objects.ContainsKey(object_id) )
            {
                // Found the object: return it:

                return m_level.Data.Objects[object_id];
            }
            else
            {
                // No object with this id exists: return null

                return null;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Searchs for game objects of the given type name and returns the first object found. Type 
        /// names are case sensitive.
        /// </summary>
        /// <param name="type_name"> Type name of the object to find. </param>
        /// <returns> First object found of given type, or null on failure. </returns>
        //=========================================================================================

        public GameObject FindByType( string type_name )
        {
            // See if this type exists:

            if ( m_level.Data.ObjectTypes.ContainsKey(type_name) )
            {
                // Check on debug windows: the dictionary for this type name should never be empty

                #if WINDOWS_DEBUG

                    if ( m_level.Data.ObjectTypes[type_name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectTypes dictionary found for type '" + type_name + "'.");
                    }

                #endif

                // Get enumerator into the dictionary:

                Dictionary<int,GameObject>.Enumerator e = m_level.Data.ObjectTypes[type_name].GetEnumerator();

                // Move onto the first object:

                e.MoveNext();

                // Return the first object found:

                return e.Current.Value;
            }
            else
            {
                // Couldn't find the type name: return null
                
                return null;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Returns the number of object's of a given type in the level.
        /// </summary>
        /// <param name="type_name"> Type name of the object to find. </param>
        /// <returns> Number of that type of object in the level. </returns>
        //=========================================================================================

        public int GetTypeCount( string type_name )
        {
            // See if this type exists:

            if ( m_level.Data.ObjectTypes.ContainsKey(type_name) )
            {
                // Check on debug windows: the dictionary for this type name should never be empty

                #if WINDOWS_DEBUG

                    if ( m_level.Data.ObjectTypes[type_name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectTypes dictionary found for type '" + type_name + "'.");
                    }

                #endif

                // Return the first object found:

                return m_level.Data.ObjectTypes[type_name].Count;
            }
            else
            {
                // Doesn't exist: return 0
                
                return 0;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Searchs for game objects with the given name and returns the first object found. Object 
        /// names are not case sensitive. 
        /// </summary>
        /// <param name="name"> Name of the object to find. </param>
        /// <returns> First object found of given type, or null on failure. </returns>
        //=========================================================================================

        public GameObject FindByName( string name )
        {
            // Make a lowercase copy of the name: all names in the dictionary will be lowercase

            name = name.ToLower();

            // See if this name exists:

            if ( m_level.Data.ObjectNames.ContainsKey(name) )
            {
                // Check on debug windows: the dictionary for this name should never be empty:

                #if WINDOWS_DEBUG

                    if ( m_level.Data.ObjectNames[name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectName dictionary found for name '" + name + "'.");
                    }

                #endif

                // Get enumerator into the dictionary:

                Dictionary<int,GameObject>.Enumerator e = m_level.Data.ObjectNames[name].GetEnumerator();

                // Move onto the first object:

                e.MoveNext();

                // Return the first object found:

                return e.Current.Value;
            }
            else
            {
                // Couldn't find the name: return null
                
                return null;
            }
        }
        
        //=========================================================================================
        /// <summary>
        /// Returns all the game objects of the given type, contained within a dictionary 
        /// mapping from game object ids to objects themselves. 
        /// </summary>
        /// <param name="type_name"> Type name for the objects to find </param>
        /// <returns> A dictionary containing all these objects, or null on failure. </returns>
        //=========================================================================================

        public Dictionary<int,GameObject> FindObjectsByType( string type_name )
        {
            // See if this type name exists:

            if ( m_level.Data.ObjectTypes.ContainsKey(type_name) )
            {
                // Check on debug windows: the dictionary for this type should never be empty:

                #if WINDOWS_DEBUG

                    if ( m_level.Data.ObjectTypes[type_name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectType dictionary found for type '" + type_name + "'.");
                    }

                #endif

                // Return all the objects of this type:

                return m_level.Data.ObjectTypes[type_name];
            }
            else
            {
                // Could not find any objects of this type: return null

                return null;
            }

        }

        //=========================================================================================
        /// <summary>
        /// Returns all the game objects with the given name. These objects are contained in a 
        /// dictionary mapping from game object ids to objects themselves.
        /// </summary>
        /// <param name="name"> Name for the objects to find </param>
        /// <returns> A dictionary containing all these objects, or null on failure. </returns>
        //=========================================================================================

        public Dictionary<int,GameObject> FindObjectsByName( string name )
        {
            // Make a lowercase copy of the name:

            name = name.ToLower();

            // See if this type name exists:

            if ( m_level.Data.ObjectNames.ContainsKey(name) )
            {
                // Check on debug windows: the dictionary for this type should never be empty:

                #if WINDOWS_DEBUG

                    if ( m_level.Data.ObjectNames[name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectName dictionary found for name '" + name + "'.");
                    }

                #endif

                // Return all the objects of this name:

                return m_level.Data.ObjectNames[name];
            }
            else
            {
                // Could not find any objects of this type: return null

                return null;
            }

        }

    }   // end of class

}   // end of namespace 

