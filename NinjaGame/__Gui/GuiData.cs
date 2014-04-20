using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    //
    /// <summary> 
    /// 
    /// This class handles the storage of all the widgets in a GUI. It is pretty much the same as 
    /// the LevelData object for the level except that it stores GUI objects. Slight duplication, 
    /// but still.. The same system worked decently enough for the level so why not reuse it for 
    /// the GUI ? 
    /// 
    /// </summary>
    //
    //#############################################################################################

    public class GuiData
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the widgets in the gui, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary<int,GuiWidget> Objects { get { return m_objects; } }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the widget names in the gui: this hash contains sub 
            /// hashes for mapping ids to widgets, because names may not be unique. All names are 
            /// kept in lowercase for case insensitivity.
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary< String , Dictionary<int,GuiWidget> > ObjectNames { get { return m_names; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the widget types in the gui: again like the names, sub hashes 
            /// are used to identify unique object instances of each type. All names are 
            /// case sensitive, since the programming language is case sensitive. 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary< String , Dictionary<int,GuiWidget> > ObjectTypes { get { return m_types; } }            

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// If this is true then modification to the gui object lists is temporarily suspened.
            /// In this case all deletion and add operations will be temporarily held in lists and 
            /// applied when the gui data is unlocked. The gui data is automatically locked during 
            /// the update and rendering of all gui objects. It is also locked when OnGuiLoaded() 
            /// is called. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool Locked { get { return m_lock_count != 0; } } 

            /// <summary> Parent gui of the gui data object. This level holds the gui data. </summary>
            
            public Gui ParentGui { get { return m_gui; } } 

        //=========================================================================================
        // Variables
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the widgets in the gui, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary<int,GuiWidget> m_objects = new Dictionary<int,GuiWidget>();
                  
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the widget names in the gui: this hash contains sub 
            /// hashes for mapping ids to widgets, because names may not be unique. All names are 
            /// kept in lowercase for case insensitivity.
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary< String , Dictionary<int,GuiWidget> > m_names = new Dictionary< String , Dictionary<int,GuiWidget> >();

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the widget types in the gui: again like the names, sub hashes 
            /// are used to identify unique object instances of each type. All names are 
            /// case sensitive, since the programming language is case sensitive. 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary< String , Dictionary<int,GuiWidget> > m_types = new Dictionary< String , Dictionary<int,GuiWidget> >();          

            /// <summary> A list of free ids that were released when objects were removed from the gui. </summary>

            private LinkedList<int> m_free_ids = new LinkedList<int>();

            /// <summary> The next available ID for an object added to the scene. This is incremented every time an widget is added to the gui if there were no removals. </summary> 
        
            private int m_next_id = 1;

            /// <summary> Parent gui of the gui data object. This level holds the gui data. </summary>
            
            private Gui m_gui = null;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// This variable counts the number of times Lock() has been called. If this is greater
            /// than zero then all deletions / additions and name registration changes will be 
            /// temporarily suspended and held until Unlock() has been called a sufficient number 
            /// of times to release the 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private int m_lock_count = 0;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// Contains a list of pending changes to be applied to the gui data . Changes are put 
            /// here when the gui data has been locked using Lock() and applied later when Unlock() 
            /// is called. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private LinkedList<PendingGuiChange> m_pending_changes = new LinkedList<PendingGuiChange>();

        //=========================================================================================
        // Enums
        //=========================================================================================

            /// <summary> Enum representing a type of pending change to be applied to the gui data </summary>

            private enum PendingGuiChangeType
            {
                /// <summary> An object should be removed from the gui data </summary>

                REMOVE_OBJECT ,

                /// <summary> An object should be added to the gui data </summary>

                ADD_OBJECT ,

                /// <summary> An object should have a name registered with the gui data </summary>

                REGISTER_NAME ,

                /// <summary> An object should unregister it's name with the gui data </summary>

                UNREGISTER_NAME
            };

        //=========================================================================================
        // Structs
        //=========================================================================================            

            /// <summary> Enum representing a stored pending change to be applied to the gui data </summary>

            private struct PendingGuiChange
            {
                /// <summary> The widget that is involved in the pending change </summary>

                public GuiWidget obj;

                /// <summary> What type of change is to be applied </summary>

                public PendingGuiChangeType changeType;

                /// <summary> Name of the object when it applied the change. This field is only used for name reg/unreg changes. </summary>

                public string objectName;

                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                /// <summary>
                /// Constructor for the pending gui change structure.
                /// </summary>
                /// <param name="o"> Object in question. </param>
                /// <param name="t"> Type of level change. </param>
                /// <param name="n"> Name of the object at type of change. For name changes only!</param>
                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                public PendingGuiChange( GuiWidget o , PendingGuiChangeType t , string n )
                {
                    obj = o; changeType = t; objectName = n;
                }
            };

        //=========================================================================================
        /// <summary> 
        /// Constructor. Creates the gui data block.
        /// </summary>
        /// 
        /// <param name="parent"> Parent gui the gui data block belongs to. Cannot be null. </param>
        //=========================================================================================

        public GuiData( Gui parent )
        {
            // If the parent gui is null then complain in windows debug:

            #if WINDOWS_DEBUG

                if ( parent == null ) throw new Exception("GuiData must have a valid parent gui object");

            #endif

            // Save parent gui:

            m_gui = parent;
        }

        //=========================================================================================
        /// <summary> 
        /// Adds a given widget into the gui data block.
        /// </summary>
        /// 
        /// <param name="obj"> Widget to add </param>
        //=========================================================================================

        public void Add( GuiWidget obj )
        {
            // Make sure object not in another scene:

            if ( obj.Id != 0 || obj.ParentGui != null || obj.ParentContainer != null ) return;

            // If the gui data is locked then just add to the list of pending gui data changes:

            if ( m_lock_count != 0 )
            {
                // Add this change to the list:

                m_pending_changes.AddLast
                (
                    new PendingGuiChange
                    (
                        obj                                 ,
                        PendingGuiChangeType.ADD_OBJECT     ,
                        obj.Name
                    )
                );

                // Abort: do not actually add the object into the list - we will do this later on Unlock()
                
                return;
            }

            // Choose an id for the object:

            int id = m_next_id;

                // If there are any free ids from deletion then use them: otherwise use the next free id and increment it

                if ( m_free_ids.Count > 0 )
                {
                    // Use the next free id gotten from deletion:

                    id = m_free_ids.First.Value; m_free_ids.RemoveFirst();
                }
                else
                {
                    // No id's ready from deletion: use the next free id and increment it

                    m_next_id++;
                }

            // Set ID, parent gui and container:

            obj.Id              = id;
            obj.ParentGui       = this.m_gui;
            obj.ParentContainer = this;

            // Add to the dictionary for all widgets:

            m_objects.Add( obj.Id , obj );

            // Register the widgets name and type for later quick lookup

            RegisterName(obj,obj.Name); RegisterType(obj);
        }

        //=========================================================================================
        /// <summary> 
        /// Removes the given widget from the gui.
        /// </summary>
        /// 
        /// <param name="obj"> Widget to remove</param>
        //=========================================================================================

        public void Remove( GuiWidget obj )
        {
            // Do nothing if objet is null:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the gui:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentGui != this.m_gui ) return;

            // See if object exists in gui:

            if ( m_objects.ContainsKey( obj.Id ) )
            {
                // If the gui data is locked then just add to the list of pending gui data changes:

                if ( m_lock_count != 0 )
                {
                    // Add this change to the list:

                    m_pending_changes.AddLast
                    (
                        new PendingGuiChange
                        (
                            obj                                     ,
                            PendingGuiChangeType.REMOVE_OBJECT    ,
                            obj.Name
                        )
                    );

                    // Abort: do not actually add the object into the list - we will do this later on Unlock()
                    
                    return;
                }

                // Cleanup after the object:

                obj.OnDelete();

                // Unregister object name and type:

                UnregisterName(obj,obj.Name); UnregisterType(obj);

                // Remove widget from the master dictionary:

                m_objects.Remove( obj.Id );

                // Add this id to the list of free ids

                m_free_ids.AddLast(obj.Id);

                // Clear all the object's scene related variables

                obj.Id              = 0;
                obj.ParentGui       = null;
                obj.ParentContainer = null;
            }

        }

        //=========================================================================================
        /// <summary> 
        /// Removes the Widget with the given ID from the gui.
        /// </summary>
        /// 
        /// <param name="objectId"> ID of the Widget to remove</param>
        //=========================================================================================

        public void Remove( int objectId )
        {
            // See if there is an object with this ID: if so then remove it

            if ( m_objects.ContainsKey(objectId) ) Remove( m_objects[objectId] );
        }

        //=========================================================================================
        /// <summary> 
        /// Registers an objects name with the gui so the object can later be found by using this 
        /// name. The name does not have to be unique and multiple objects could be retrieved under 
        /// the same name. 
        /// </summary>
        /// 
        /// <param name="obj"> Object to register name for </param>
        /// <param name="name"> Name to register the object under </param>
        //=========================================================================================

        public void RegisterName( GuiWidget obj , string name )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the gui:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentGui != this.m_gui ) return;

            // Do nothing if no name was given:

            if ( name == null || name.Length <= 0 ) return;

            // If the gui data is locked then just add to the list of pending gui data changes:

            if ( m_lock_count != 0 )
            {
                // Add this change to the list:

                m_pending_changes.AddLast
                (
                    new PendingGuiChange
                    (
                        obj                                     ,
                        PendingGuiChangeType.REGISTER_NAME    ,
                        name
                    )
                );

                // Abort: do not actually add the object into the list - we will do this later on Unlock()
                
                return;
            }

            // Make the name lowercase:

            string nameLower = name.ToLower();

            // Create a hash for objects with this name if not already in existance

            if ( m_names.ContainsKey( nameLower ) == false ) 
            {
                m_names.Add( nameLower , new Dictionary<int,GuiWidget>() );
            }

            // Add the object into the hash:

            m_names[nameLower][obj.Id] = obj;
        }

        //=========================================================================================
        /// <summary> 
        /// Unregisters an object's name with the gui. After this has finished the object will 
        /// no longer be found when searching for objects with this name.
        /// </summary>
        /// 
        /// <param name="obj"> Object to unregister name for </param>
        /// <param name="name"> Name that the object is registered under </param>
        //=========================================================================================

        public void UnregisterName( GuiWidget obj , string name )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the scene:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentGui != this.m_gui ) return;

            // Do nothing if no name was given:

            if ( name == null || name.Length <= 0 ) return;

            // If the level data is locked then just add to the list of pending gui data changes:

            if ( m_lock_count != 0 )
            {
                // Add this change to the list:

                m_pending_changes.AddLast
                (
                    new PendingGuiChange
                    (
                        obj                                     ,
                        PendingGuiChangeType.UNREGISTER_NAME  ,
                        name
                    )
                );

                // Abort: do not actually add the object into the list - we will do this later on Unlock()
                
                return;
            }

            // Make the name lowercase:

            string nameLower = name.ToLower();

            // Make sure the hash contains the name

            if ( m_names.ContainsKey( nameLower ) )
            {
                // Get the hash for this name:

                Dictionary<int,GuiWidget> hash = m_names[nameLower];

                // See if this widget exists in the hash

                if ( hash.ContainsKey( obj.Id ) )
                {
                    // Bingo: remove the widget from the hash
                   
                    hash.Remove( obj.Id );

                    // If the hash is now empty then remove it from the parent hash

                    if ( hash.Count == 0 ){ m_names.Remove( nameLower ); }
                }
            }

        }

        //=========================================================================================
        /// <summary> 
        /// Registers a widgets type name with the gui data so the object can later be found by 
        /// using this type name. Multiple objects can be registered with the same type.
        /// </summary>
        /// 
        /// <param name="obj">Object to register type name for</param>
        //=========================================================================================

        private void RegisterType( GuiWidget obj )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the gui:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentGui != this.m_gui ) return;

            // Get the object's type name:

            string typeName = obj.GetType().Name;

            // Create a hash for widgets with this type if not already in existance

            if ( m_types.ContainsKey( typeName ) == false ) 
            {
                m_types.Add( typeName , new Dictionary<int,GuiWidget>() );
            }

            // Add the object into the hash:

            m_types[typeName][obj.Id] = obj;
        }

        //=========================================================================================
        /// <summary> 
        /// Unregisters a widgets's type name with the scene. After this has finished the object will 
        /// no longer be found when searching for objects with this type name.
        /// </summary>
        /// 
        /// <param name="obj"> Object to unregister name for </param>
        //=========================================================================================

        private void UnregisterType( GuiWidget obj )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the gui:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentGui != this.m_gui ) return;

            // Get the object's type name:

            string typeName = obj.GetType().Name;

            // Make sure the hash contains the type name

            if ( m_types.ContainsKey( typeName ) )
            {
                // Get the hash for this type name:

                Dictionary<int,GuiWidget> hash = m_types[typeName];

                // See if this object exists in the hash

                if ( hash.ContainsKey( obj.Id ) )
                {
                    // Bingo: remove the object from the hash
                   
                    hash.Remove( obj.Id );

                    // If the hash is now empty then remove it from the parent hash

                    if ( hash.Count == 0 ){ m_types.Remove( typeName ); }
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Locks the gui data so that any additions, deletions or name changes will not be 
        /// applied until the data has been unlocked again. This should be called before enumerating 
        /// over a collection of objects in the gui. A count of the number of times the data has 
        /// been locked is maintained and this count must reach zero before the gui's data 
        /// structures can be modified again. The main purpose of lock is to protect against exceptions
        /// which occur if a collection is modified whilst enumerating over it.
        /// </summary>
        //=========================================================================================

        public void Lock()
        { 
            // Increase the lock count:

            m_lock_count++;
        }

        //=========================================================================================
        /// <summary>
        /// Unlocks gui data and applies any pending changes that were held back whilst the 
        /// gui data was locked. These changes include object removals or deltions and name 
        /// registration changes. 
        /// </summary>
        //=========================================================================================

        public void Unlock()
        {
            // If the lock count is already zero then abort:

            if ( m_lock_count == 0 ) return;

            // Decrement the lockcount:

            m_lock_count--;

            // See if the lock count is now zero: if so then apply all pending changes

            if ( m_lock_count == 0 ) 
            {
                //---------------------------------------------------------------------------------
                // Lock count now zero: apply all pending changes that were saved
                //---------------------------------------------------------------------------------

                // Continue until the pending changes list is empty:

                while ( m_pending_changes.Count > 0 )
                {
                    // Get the next pending change:

                    PendingGuiChange change = m_pending_changes.First.Value; 
                    
                    // Remove the next change from the list:

                    m_pending_changes.RemoveFirst();

                    // See what the change is:

                    switch ( change.changeType )
                    {
                        case PendingGuiChangeType.ADD_OBJECT:
                        {
                            // Add this object into the gui data:

                            Add(change.obj);

                        }   break;

                        case PendingGuiChangeType.REMOVE_OBJECT:
                        {
                            // Remove this object from the gui data:

                            Remove(change.obj);

                        }   break;

                        case PendingGuiChangeType.REGISTER_NAME:
                        {
                            // Register this object's name with the gui data:

                            RegisterName(change.obj,change.objectName);

                        }   break;

                        case PendingGuiChangeType.UNREGISTER_NAME:
                        {
                            // Unregister this object's name with the gui data:

                            UnregisterName(change.obj,change.objectName);

                        }   break;

                    }   // end switch change type

                }   // end for all remaining pending changes

            }   // end if lockCount == 0 

        }

    }   // end of class

}   // end of namespace
