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
    /// This helper object provides functions to search for objects in a gui based
    /// on their type or name.
    /// </summary>
    //
    //#############################################################################################

    public class GuiSearchQuery
    {

        //=========================================================================================
        // Variables 
        //=========================================================================================

            /// <summary> Gui the search query belongs to. </summary>

            Gui m_gui = null;
        
        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a gui search query. 
        /// </summary>
        /// 
        /// <param name="parent"> 
        /// Gui the search query belongs to. This gui's data is what will be searched. 
        /// This cannot be null !
        /// </param>
        //=========================================================================================

        public GuiSearchQuery( Gui parent )
        {
            // Ensure that this is not null in debug windows:

            #if WINDOWS_DEBUG

                if ( parent == null ) throw new Exception("GuiSearchQuery must have a valid parent gui object");

            #endif

            // Save the gui:

            m_gui = parent;
        }

        //=========================================================================================
        /// <summary>
        /// Searchs for a widget according to it's id. Id's are unique so only 
        /// one widget can be returned.
        /// </summary>
        /// <param name="widget_id"> Id of widget to find </param>
        /// <returns> The widget in question or null on failure. </returns>
        //=========================================================================================

        public GuiWidget FindById( int widget_id )
        {
            // See if the object exists:

            if ( m_gui.Data.Objects.ContainsKey(widget_id) )
            {
                // Found the object: return it:

                return m_gui.Data.Objects[widget_id];
            }
            else
            {
                // No object with this id exists: return null

                return null;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Searchs for widgets of the given type name and returns the first widget found. Type 
        /// names are case sensitive.
        /// </summary>
        /// <param name="type_name"> Type name of the widget to find. </param>
        /// <returns> First object found of given type, or null on failure. </returns>
        //=========================================================================================

        public GuiWidget FindByType( string type_name )
        {
            // See if this type exists:

            if ( m_gui.Data.ObjectTypes.ContainsKey(type_name) )
            {
                // Check on debug windows: the dictionary for this type name should never be empty

                #if WINDOWS_DEBUG

                    if ( m_gui.Data.ObjectTypes[type_name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectTypes dictionary found for type '" + type_name + "'.");
                    }

                #endif

                // Get enumerator into the dictionary:

                Dictionary<int,GuiWidget>.Enumerator e = m_gui.Data.ObjectTypes[type_name].GetEnumerator();

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
        /// Returns the number of object's of a given type in the gui.
        /// </summary>
        /// <param name="type_name"> Type name of the object to find. </param>
        /// <returns> Number of that type of object in the level. </returns>
        //=========================================================================================

        public int GetTypeCount( string type_name )
        {
            // See if this type exists:

            if ( m_gui.Data.ObjectTypes.ContainsKey(type_name) )
            {
                // Check on debug windows: the dictionary for this type name should never be empty

                #if WINDOWS_DEBUG

                    if ( m_gui.Data.ObjectTypes[type_name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty widget dictionary found for type '" + type_name + "'.");
                    }

                #endif

                // Return the first object found:

                return m_gui.Data.ObjectTypes[type_name].Count;
            }
            else
            {
                // Doesn't exist: return 0
                
                return 0;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Searchs for widgets with the given name and returns the first widget found. Widget 
        /// names are not case sensitive. 
        /// </summary>
        /// <param name="name"> Name of the widget to find. </param>
        /// <returns> First widget found of given type, or null on failure. </returns>
        //=========================================================================================

        public GuiWidget FindByName( string name )
        {
            // Make a lowercase copy of the name: all names in the dictionary will be lowercase

            name = name.ToLower();

            // See if this name exists:

            if ( m_gui.Data.ObjectNames.ContainsKey(name) )
            {
                // Check on debug windows: the dictionary for this name should never be empty:

                #if WINDOWS_DEBUG

                    if ( m_gui.Data.ObjectNames[name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty widget dictionary found for name '" + name + "'.");
                    }

                #endif

                // Get enumerator into the dictionary:

                Dictionary<int,GuiWidget>.Enumerator e = m_gui.Data.ObjectNames[name].GetEnumerator();

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
        /// Returns all the widgets of the given type, contained within a dictionary 
        /// mapping from widget ids to widgets themselves. 
        /// </summary>
        /// <param name="typeName"> Type name for the widgets to find </param>
        /// <returns> A dictionary containing all these widgets, or null on failure. </returns>
        //=========================================================================================

        public Dictionary<int,GuiWidget> FindObjectsByType( string typeName )
        {
            // See if this type name exists:

            if ( m_gui.Data.ObjectTypes.ContainsKey(typeName) )
            {
                // Check on debug windows: the dictionary for this type should never be empty:

                #if WINDOWS_DEBUG

                    if ( m_gui.Data.ObjectTypes[typeName].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectType dictionary found for type '" + typeName + "'.");
                    }

                #endif

                // Return all the widgets of this type:

                return m_gui.Data.ObjectTypes[typeName];
            }
            else
            {
                // Could not find any widgets of this type: return null

                return null;
            }

        }

        //=========================================================================================
        /// <summary>
        /// Returns all the widgets with the given name. These objects are contained in a 
        /// dictionary mapping from widget ids to widgets themselves.
        /// </summary>
        /// <param name="name"> Name for the widgets to find </param>
        /// <returns> A dictionary containing all these objects, or null on failure. </returns>
        //=========================================================================================

        public Dictionary<int,GuiWidget> FindObjectsByName( string name )
        {
            // Make a lowercase copy of the name:

            name = name.ToLower();

            // See if this type name exists:

            if ( m_gui.Data.ObjectNames.ContainsKey(name) )
            {
                // Check on debug windows: the dictionary for this type should never be empty:

                #if WINDOWS_DEBUG

                    if ( m_gui.Data.ObjectNames[name].Count <= 0 )
                    {
                        // No objects in the dictionary for this type: throw exception

                        throw new Exception("Empty objectName dictionary found for name '" + name + "'.");
                    }

                #endif

                // Return all the objects of this name:

                return m_gui.Data.ObjectNames[name];
            }
            else
            {
                // Could not find any objects of this type: return null

                return null;
            }

        }

    }   // end of class

}   // end of namespace 

