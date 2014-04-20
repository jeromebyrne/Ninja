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
    /// This class handles the storage of all the data in the level. It maintains hash lookup tables 
    /// so that objects can be retrieved by name and also by ID. Duplicate hash lookups are also 
    /// used so that objects can be quickly retrieved by type if required, or by their flags. 
    /// For example a hash of all renderable objects is maintained so that the game only has to 
    /// go through this hash to draw renderable objects- non renderable objects can be quickly 
    /// ignored. 
    /// 
    /// </summary>
    //
    //#############################################################################################

    public class LevelData
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the objects in the scene, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary<int,GameObject> Objects { get { return m_objects; } }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the object names in the scene: this hash contains sub 
            /// hashes for mapping ids to objects, because names may not be unique. All names are 
            /// kept in lowercase for case insensitivity.
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary< String , Dictionary<int,GameObject> > ObjectNames { get { return m_names; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the object types in the scene: again like the names, sub hashes 
            /// are used to identify unique object instances of each type. All names are 
            /// case sensitive, since the programming language is case sensitive. 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary< String , Dictionary<int,GameObject> > ObjectTypes { get { return m_types; } }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the renderable objects in the scene, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary<int,GameObject> RenderableObjects { get { return m_renderables; } }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the updateable objects in the scene, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary<int,GameObject> UpdateableObjects { get { return m_updateables; } }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all collision blocking objects in the scene, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary<int,GameObject> CollideableObjects { get { return m_collideables; } }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> A master hash of all objects in the scene that can be part of the bounding box overlap test, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public Dictionary<int,GameObject> OverlapableObjects { get { return m_overlapables; } }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// If this is true then modification to the level object lists is temporarily suspened.
            /// In this case all deletion and add operations will be temporarily held in lists and 
            /// applied when the level data is unlocked. The level data is automatically locked during 
            /// the update and rendering of all level objects. It is also locked when OnLevelLoaded() 
            /// is called. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool Locked { get { return m_lock_count != 0; } } 

            /// <summary> Parent level of the level data object. This level holds the level data. </summary>
            
            public Level ParentLevel { get { return m_level; } } 

        //=========================================================================================
        // Variables
        //=========================================================================================

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the objects in the scene, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary<int,GameObject> m_objects = new Dictionary<int,GameObject>();
                  
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the object names in the scene: this hash contains sub 
            /// hashes for mapping ids to objects, because names may not be unique. All names are 
            /// kept in lowercase for case insensitivity.
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary< String , Dictionary<int,GameObject> > m_names = new Dictionary< String , Dictionary<int,GameObject> >();

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the object types in the scene: again like the names, sub hashes 
            /// are used to identify unique object instances of each type. All names are 
            /// case sensitive, since the programming language is case sensitive. 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary< String , Dictionary<int,GameObject> > m_types = new Dictionary< String , Dictionary<int,GameObject> >();          

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A master hash of all the renderable objects in the scene, identified by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary<int,GameObject> m_renderables = new Dictionary<int,GameObject>();
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> A master hash of all the updateable objects in the scene, identified by 
            /// their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary<int,GameObject> m_updateables = new Dictionary<int,GameObject>();
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> A master hash of all collision blocking objects in the scene, identified 
            /// by their id 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary<int,GameObject> m_collideables = new Dictionary<int,GameObject>();
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> A master hash of all objects in the scene that can be part of the bounding 
            /// box overlap test, identified by their id. 
            /// 
            /// ** NB: PLEASE USE Lock() BEFORE ATTEMPTING TO ENUMERATE THIS COLLECTION. 
            /// CALL Unlock() WHEN FINISHED ENUMERATION. PLEASE DO NOT MODIFY THIS COLLECTION 
            /// DIRECTLY - IT IS NOT SAFE TO DO SO !!
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private Dictionary<int,GameObject> m_overlapables = new Dictionary<int,GameObject>();

            /// <summary> A list of free ids that were released when objects were removed from the scene. </summary>

            private LinkedList<int> m_free_ids = new LinkedList<int>();

            /// <summary> The next available ID for an object added to the scene. This is incremented every time an object is added to the scene if there were no removals. </summary> 
        
            private int m_next_id = 1;

            /// <summary> Parent level of the level data object. This level holds the level data. </summary>
            
            private Level m_level = null;

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
            /// Contains a list of pending changes to be applied to the level data . Changes are put 
            /// here when the level data has been locked using Lock() and applied later when Unlock() 
            /// is called. 
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private LinkedList<PendingLevelChange> m_pending_changes = new LinkedList<PendingLevelChange>();

        //=========================================================================================
        // Enums
        //=========================================================================================

            /// <summary> Enum representing a type of pending change to be applied to the level data </summary>

            private enum PendingLevelChangeType
            {
                /// <summary> An object should be removed from the level data </summary>

                REMOVE_OBJECT ,

                /// <summary> An object should be added to the level data </summary>

                ADD_OBJECT ,

                /// <summary> An object should have a name registered with the level data </summary>

                REGISTER_NAME ,

                /// <summary> An object should unregister it's name with the level data </summary>

                UNREGISTER_NAME
            };

        //=========================================================================================
        // Structs
        //=========================================================================================            

            /// <summary> Enum representing a stored pending change to be applied to the level data </summary>

            private struct PendingLevelChange
            {
                /// <summary> The object that is involved in the pending change </summary>

                public GameObject obj;

                /// <summary> What type of change is to be applied </summary>

                public PendingLevelChangeType changeType;

                /// <summary> Name of the object when it applied the change. This field is only used for name reg/unreg changes. </summary>

                public string objectName;

                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                /// <summary>
                /// Constructor for the pending level change structure.
                /// </summary>
                /// <param name="o"> Object in question. </param>
                /// <param name="t"> Type of level change. </param>
                /// <param name="n"> Name of the object at type of change. For name changes only!</param>
                //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                public PendingLevelChange( GameObject o , PendingLevelChangeType t , string n )
                {
                    obj = o; changeType = t; objectName = n;
                }
            };

        //=========================================================================================
        /// <summary> 
        /// Constructor. Creates the level data block.
        /// </summary>
        /// 
        /// <param name="parent"> Parent level the level data block belongs to. Cannot be null. </param>
        //=========================================================================================

        public LevelData( Level parent )
        {
            // If the parent level is null then complain in windows debug:

            #if WINDOWS_DEBUG

                if ( parent == null ) throw new Exception("LevelData must have a valid parent Level object");

            #endif

            // Save parent level:

            m_level = parent;
        }

        //=========================================================================================
        /// <summary> 
        /// Adds a given game object into the level data block.
        /// </summary>
        /// 
        /// <param name="obj"> GameObject to add </param>
        //=========================================================================================

        public void Add( GameObject obj )
        {
            // Make sure object not in another scene:

            if ( obj.Id != 0 || obj.ParentLevel != null || obj.ParentContainer != null ) return;

            // If the level data is locked then just add to the list of pending level data changes:

            if ( m_lock_count != 0 )
            {
                // Add this change to the list:

                m_pending_changes.AddLast
                (
                    new PendingLevelChange
                    (
                        obj                                 ,
                        PendingLevelChangeType.ADD_OBJECT   ,
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

            // Set ID, parent level and container:

            obj.Id              = id;
            obj.ParentLevel     = this.m_level;
            obj.ParentContainer = this;

            // Add to the appropriate dictionaries:

            m_objects.Add( obj.Id , obj );
            
            if ( obj.Renderable     ) m_renderables.Add     ( obj.Id , obj );
            if ( obj.Updateable     ) m_updateables.Add     ( obj.Id , obj );
            if ( obj.Collideable    ) m_collideables.Add    ( obj.Id , obj );
            if ( obj.Overlapable    ) m_overlapables.Add    ( obj.Id , obj );

            // Register the objects name and type for later quick lookup

            RegisterName(obj,obj.Name); RegisterType(obj);
        }

        //=========================================================================================
        /// <summary> 
        /// Removes the given GameObject from the level.
        /// </summary>
        /// 
        /// <param name="obj"> GameObject to remove</param>
        //=========================================================================================

        public void Remove( GameObject obj )
        {
            // Do nothing if objet is null:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the scene:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentLevel != this.m_level ) return;

            // See if object exists in scene:

            if ( m_objects.ContainsKey( obj.Id ) )
            {
                // If the level data is locked then just add to the list of pending level data changes:

                if ( m_lock_count != 0 )
                {
                    // Add this change to the list:

                    m_pending_changes.AddLast
                    (
                        new PendingLevelChange
                        (
                            obj                                     ,
                            PendingLevelChangeType.REMOVE_OBJECT    ,
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

                // Remove object from all containers:

                m_objects.Remove( obj.Id );

                if ( m_renderables.ContainsKey(obj.Id)  ) m_renderables.Remove(obj.Id);
                if ( m_updateables.ContainsKey(obj.Id)  ) m_updateables.Remove(obj.Id);
                if ( m_collideables.ContainsKey(obj.Id) ) m_collideables.Remove(obj.Id);
                if ( m_overlapables.ContainsKey(obj.Id) ) m_overlapables.Remove(obj.Id);

                // Add this id to the list of free ids

                m_free_ids.AddLast(obj.Id);

                // Clear all the object's scene related variables

                obj.Id              = 0;
                obj.ParentLevel     = null;
                obj.ParentContainer = null;
            }

        }

        //=========================================================================================
        /// <summary> 
        /// Removes the GameObject with the given ID from the level.
        /// </summary>
        /// 
        /// <param name="objectId"> ID of the GameObject to remove</param>
        //=========================================================================================

        public void Remove( int objectId )
        {
            // See if there is an object with this ID: if so then remove it

            if ( m_objects.ContainsKey(objectId) ) Remove( m_objects[objectId] );
        }

        //=========================================================================================
        /// <summary> 
        /// Registers an objects name with the scene so the object can later be found by using this 
        /// name. The name does not have to be unique and multiple objects could be retrieved under 
        /// the same name. 
        /// </summary>
        /// 
        /// <param name="obj"> Object to register name for </param>
        /// <param name="name"> Name to register the object under </param>
        //=========================================================================================

        public void RegisterName( GameObject obj , string name )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the scene:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentLevel != this.m_level ) return;

            // Do nothing if no name was given:

            if ( name == null || name.Length <= 0 ) return;

            // If the level data is locked then just add to the list of pending level data changes:

            if ( m_lock_count != 0 )
            {
                // Add this change to the list:

                m_pending_changes.AddLast
                (
                    new PendingLevelChange
                    (
                        obj                                     ,
                        PendingLevelChangeType.REGISTER_NAME    ,
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
                m_names.Add( nameLower , new Dictionary<int,GameObject>() );
            }

            // Add the object into the hash:

            m_names[nameLower][obj.Id] = obj;
        }

        //=========================================================================================
        /// <summary> 
        /// Unregisters an object's name with the scene. After this has finished the object will 
        /// no longer be found when searching for objects with this name.
        /// </summary>
        /// 
        /// <param name="obj"> Object to unregister name for </param>
        /// <param name="name"> Name that the object is registered under </param>
        //=========================================================================================

        public void UnregisterName( GameObject obj , string name )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the scene:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentLevel != this.m_level ) return;

            // Do nothing if no name was given:

            if ( name == null || name.Length <= 0 ) return;

            // If the level data is locked then just add to the list of pending level data changes:

            if ( m_lock_count != 0 )
            {
                // Add this change to the list:

                m_pending_changes.AddLast
                (
                    new PendingLevelChange
                    (
                        obj                                     ,
                        PendingLevelChangeType.UNREGISTER_NAME  ,
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

                Dictionary<int,GameObject> hash = m_names[nameLower];

                // See if this object exists in the hash

                if ( hash.ContainsKey( obj.Id ) )
                {
                    // Bingo: remove the object from the hash
                   
                    hash.Remove( obj.Id );

                    // If the hash is now empty then remove it from the parent hash

                    if ( hash.Count == 0 ){ m_names.Remove( nameLower ); }
                }
            }

        }

        //=========================================================================================
        /// <summary> 
        /// Registers an objects type name with the level data so the object can later be found by 
        /// using this type name. Multiple objects can be registered with the same type.
        /// </summary>
        /// 
        /// <param name="obj">Object to register type name for</param>
        //=========================================================================================

        private void RegisterType( GameObject obj )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the scene:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentLevel != this.m_level ) return;

            // Get the object's type name:

            string typeName = obj.GetType().Name;

            // Create a hash for objects with this type if not already in existance

            if ( m_types.ContainsKey( typeName ) == false ) 
            {
                m_types.Add( typeName , new Dictionary<int,GameObject>() );
            }

            // Add the object into the hash:

            m_types[typeName][obj.Id] = obj;
        }

        //=========================================================================================
        /// <summary> 
        /// Unregisters an object's type name with the scene. After this has finished the object will 
        /// no longer be found when searching for objects with this type name.
        /// </summary>
        /// 
        /// <param name="obj"> Object to unregister name for </param>
        //=========================================================================================

        private void UnregisterType( GameObject obj )
        {
            // Abort if null object:

            if ( obj == null ) return;

            // Do nothing if the object has no ID or is not in the scene:

            if ( obj.Id == 0 || obj.ParentContainer != this || obj.ParentLevel != this.m_level ) return;

            // Get the object's type name:

            string typeName = obj.GetType().Name;

            // Make sure the hash contains the type name

            if ( m_types.ContainsKey( typeName ) )
            {
                // Get the hash for this type name:

                Dictionary<int,GameObject> hash = m_types[typeName];

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
        /// Locks the level data so that any additions, deletions or name changes will not be 
        /// applied until the data has been unlocked again. This should be called before enumerating 
        /// over a collection of objects in the level. A count of the number of times the data has 
        /// been locked is maintained and this count must reach zero before the level's data 
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
        /// Unlocks level data and applies any pending changes that were held back whilst the 
        /// level data was locked. These changes include object removals or deltions and name 
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

                    PendingLevelChange change = m_pending_changes.First.Value; 
                    
                    // Remove the next change from the list:

                    m_pending_changes.RemoveFirst();

                    // See what the change is:

                    switch ( change.changeType )
                    {
                        case PendingLevelChangeType.ADD_OBJECT:
                        {
                            // Add this object into the level data:

                            Add(change.obj);

                        }   break;

                        case PendingLevelChangeType.REMOVE_OBJECT:
                        {
                            // Remove this object from the level data:

                            Remove(change.obj);

                        }   break;

                        case PendingLevelChangeType.REGISTER_NAME:
                        {
                            // Register this object's name with the level data:

                            RegisterName(change.obj,change.objectName);

                        }   break;

                        case PendingLevelChangeType.UNREGISTER_NAME:
                        {
                            // Unregister this object's name with the level data:

                            UnregisterName(change.obj,change.objectName);

                        }   break;

                    }   // end switch change type

                }   // end for all remaining pending changes

            }   // end if lockCount == 0 

        }

    }   // end of class

}   // end of namespace
