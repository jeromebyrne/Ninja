using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

//################################################################################################
//################################################################################################

namespace NinjaGame
{
    //############################################################################################
    //
    ///
    /// <summary> 
    /// 
    /// Class responsible for managing all the objects in a scene. Contains objects which also
    /// allow collision and search queries to be performed. Also handles rendering and updates. 
    /// 
    /// </summary>
    // 
    //############################################################################################

    public class Level
    {
        //=========================================================================================
        // Properties
        //=========================================================================================  

            /// <summary> Data for the level. Contains all of the objects in the level. </summary>

            public LevelData Data { get { return m_data; } } 

            /// <summary> Search query object used to search the level data for objects, according to different criteria. </summary>
            
            public LevelSearchQuery Search { get { return m_search; } } 

            /// <summary> Collision query object which can be used to do collision detection tests with level objects. </summary>

            public LevelCollisionQuery Collision { get { return m_collision; } }

            /// <summary> Level renderer which draws the level. </summary>

            public LevelRenderer Renderer { get { return m_renderer; } }

            /// <summary> Particle emitter used by the level. </summary>

            public ParticleEmitter Emitter { get { return m_emitter; } } 

            /// <summary> Name of the level file currently loaded. </summary>

            public string FileName { get { return m_file_name; } }

        //=========================================================================================
        // Variables 
        //=========================================================================================        

            /// <summary> Data for the level. Contains all of the objects in the level. </summary>

            private LevelData m_data = null;

            /// <summary> Search query object used to search the level data for objects, according to different criteria. </summary>

            private LevelSearchQuery m_search = null;

            /// <summary> Collision query object which can be used to do collision detection tests with level objects. </summary>

            private LevelCollisionQuery m_collision = null;

            /// <summary> Level renderer which draws the level. </summary>

            private LevelRenderer m_renderer = null;

            /// <summary> Particle emitter used by the level. </summary>

            private ParticleEmitter m_emitter = null;

            /// <summary> Name of the level file currently loaded. </summary>

            private string m_file_name = "";

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A list of build cache functions for each game object class. 
            /// Functions are sorted by inheritance with base functions being called first.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private static CacheFunction[] s_build_cache_functions = null;

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A list of clear cache functions for each game object class. 
            /// Functions are sorted by inheritance with derived functions being called first.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private static CacheFunction[] s_clear_cache_functions = null;

        //=========================================================================================
        // Delegates
        //=========================================================================================    

            /// <summary> This is the form that a function for precaching / clearing gameobject class cache should take. </summary>

            private delegate void CacheFunction();

        //=========================================================================================
        /// <summary> 
        /// Static constructor for the level. Builds a list of cache and clear cache functions for 
        /// each game object.
        /// </summary>
        //=========================================================================================

        static Level(){ FindGameObjectCacheFunctions(); }

        //=========================================================================================
        /// <summary> 
        /// Constructor for the level. Initialises the basic level data structures and objects.
        /// </summary>
        //=========================================================================================

        public Level()
        {
            // Create all required objects:

            m_data      = new LevelData(this);
            m_search    = new LevelSearchQuery(this);
            m_renderer  = new LevelRenderer(this);
            m_collision = new LevelCollisionQuery(this);
            m_emitter   = new ParticleEmitter();

            // If debug then initialise debug level stuff:

            #if DEBUG

                DebugLevel.Initialize();

            #endif
        }

        //=========================================================================================
        /// <summary> 
        /// Clears the level. This should be used as opposed to manually clearing level data 
        /// when a level-wipe is required.
        /// </summary>
        //=========================================================================================

        public void Clear()
        {
            // Recreate all objects and override the old ones:

            m_data      = new LevelData(this);
            m_search    = new LevelSearchQuery(this);
            m_renderer  = new LevelRenderer(this);
            m_collision = new LevelCollisionQuery(this);
            m_emitter   = new ParticleEmitter();

            // Set level file name to blank:

            m_file_name = "";
        }
      
        //=========================================================================================
        /// <summary> 
        /// Loads a level and all objects in the level from the given XML file. If this process 
        /// fails then the current level will remain active. 
        /// </summary>
        /// 
        /// <param name="file"> Name of the file to load </param>
        //=========================================================================================

        public void Load( String file )
        {
            // Do nothing if no file is given:

            if ( file == null || file.Length <= 0 ) return;

            // Try and read the given level:

            LinkedList<XmlObject> objects = XmlFactory.ReadXml( Locale.GetLocFile(file) );

            // See if that succeeded:

            if ( objects != null )
            {
                // Clear the currrent cache for each game object class:

                ClearClassCaches();

                // Success: clear the level

                Clear();

                // Set new level file name:

                m_file_name = file;

                {
                    // Run through the list of objects read:

                    LinkedList<XmlObject>.Enumerator i = objects.GetEnumerator();

                    while ( i.MoveNext() )
                    {
                        // Get this object:

                        XmlObject obj = i.Current;

                        // Attempt to cast to a game object:

                        try
                        {
                            // Do the cast:

                            GameObject game_obj = (GameObject)(obj);

                            // Save the object to the level:

                            m_data.Add(game_obj);
                        }

                        // If something went wrong then show what on debug windows:

                        #if WINDOWS_DEBUG

                            catch ( Exception e )
                            {
                                // Print what happened: 

                                DebugConsole.Print("Level.Load(): invalid object type encountered. " ); DebugConsole.PrintException(e);
                            }

                        #else

                            catch ( Exception ){;}

                        #endif
                        
                    }

                }

                // Lock the level data:

                m_data.Lock();

                // Call the OnLevelLoaded() event on all objects:

                {
                    // Get all the objects:

                    Dictionary<int,GameObject>.Enumerator i = m_data.Objects.GetEnumerator();

                    // Run through this list:

                    while ( i.MoveNext() )
                    {
                        // Attempt to call OnLevelLoaded:

                        try
                        {
                            // Do the call:

                            i.Current.Value.OnLevelLoaded();
                        }

                        // If something went wrong then show what on debug windows:

                        #if WINDOWS_DEBUG

                            catch ( Exception e )
                            {
                                // Print what happened: 

                                DebugConsole.Print("Level.Load(): OnLevelLoad() failed for object. " ); DebugConsole.PrintException(e);
                            }

                        #else

                            catch ( Exception ){;}

                        #endif
                    }

                }

                // Unlock the level data:

                m_data.Unlock();

                // Build a new list of caches for each game object 

                BuildClassCaches();
            }

        }

        //=========================================================================================
        /// <summary> 
        /// Saves the level and it's objects to the given XML file.
        /// </summary>
        /// 
        /// <param name="file"> Name of the file to save to </param>
        //=========================================================================================

        public void Save( String file )
        {
            // Make a list of objects to save:

            LinkedList<XmlObject> objects = new LinkedList<XmlObject>();

            // Lock the level data:

            m_data.Lock();

                // Put all the objects in the level into the list:

                Dictionary<int,GameObject>.Enumerator e = m_data.Objects.GetEnumerator();

                while ( e.MoveNext() )
                {
                    objects.AddLast(e.Current.Value);
                }

            // Unlock the level data:

            m_data.Unlock();

            // Let the XmlFactory do the rest:

            XmlFactory.WriteXml(file,objects);
        }     

        //=========================================================================================
        /// <summary> 
        /// Updates the level, calling the Update() function for all game objects.
        /// </summary>
        //========================================================================================= 

        public void Update()
        {
            // Prevent modification of the level data collections:

            m_data.Lock();

            // In debug mode only do this if we are not step framing or if a number of frames is left to step through:

            #if DEBUG

                if ( DebugLevel.FrameStep == false || DebugLevel.FrameStepCount > 0 )
                {

            #endif

                    // Run through the list of updateable objects and update them all:

                    Dictionary<int,GameObject>.Enumerator e = m_data.UpdateableObjects.GetEnumerator();

                    while ( e.MoveNext() )
                    {
                        // Update this object:

                        e.Current.Value.OnUpdate();
                    }

                    // Update the particle emitter:

                    m_emitter.Update();

            #if DEBUG

                    // If in step frame mode: reduce number of frames 

                    if ( DebugLevel.FrameStep ) DebugLevel.FrameStepCount--;
                }

            #endif

            // If in debug build then update debug level stuff

            #if DEBUG

                DebugLevel.Update();

            #endif

            // Ok to modify level data again: apply all with-held changes:

            m_data.Unlock();
        }

        //=========================================================================================
        /// <summary> 
        /// Renders the level using the level renderer
        /// </summary>
        //========================================================================================= 

        public void Draw()
        {
            // Let the level renderer draw the level:

            m_renderer.Draw();

            // Do any debug drawing the level debug module wants to do in debug mode:

            #if DEBUG

                DebugLevel.Draw();

            #endif
        }

        //=========================================================================================
        /// <summary> 
        /// Builds a list of BuildClassCache() and ClearClassCache() functions for each gameobject
        /// class for later calling. Functions are sorted according to derivation, with base 
        /// BuildClassCache() functions being called first and derived ClearClassCache() functions 
        /// being called first- like the constructor/destructor conventions.
        /// </summary>
        //========================================================================================= 

        private static void FindGameObjectCacheFunctions()
        {
            // Get the type for a game object:
            
            Type game_object_type = Type.GetType("NinjaGame.GameObject");

            // Get the type for a level:

            Type level_type = Type.GetType("NinjaGame.Level");

            // This is the type of nested function we will look for next:

            BindingFlags cache_function_binding_flags = BindingFlags.Public | BindingFlags.NonPublic;

            // Get the type for a cache function:

            Type cache_function_type = level_type.GetNestedType("CacheFunction",cache_function_binding_flags);

            // Get all the class types in the assembly:

            Module[] modules = Assembly.GetCallingAssembly().GetModules();

            // Store the build and clear cache functions here:

            List<CacheFunction> build_cache_functions = new List<CacheFunction>();
            List<CacheFunction> clear_cache_functions = new List<CacheFunction>();

            // Store the types of each class for which we have build and clear cache functions here:

            List<Type> cached_types = new List<Type>();

            // Run through all the types:

            foreach ( Module module in modules )
            {
                // Get the module types:

                Type[] types = module.GetTypes();

                // Run through all the module types:

                foreach ( Type type in types )
                {
                    // Try and see if the type derives is a game object or derived class: if not then ignore

                    if ( ( type.Equals(game_object_type) || type.IsSubclassOf(game_object_type) ) == false ) continue;

                    // This code might fail:

                    try
                    {
                        // Look for a build cache function:

                        CacheFunction build_cache_function = null;

                        // Have a peek:

                        {
                            // This is the type of method we are looking for:

                            BindingFlags b = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

                            // Try and get the build cache function:

                            MethodInfo build_cache_method_info = type.GetMethod("BuildClassCache",b);

                            // If not there then onto the next type:

                            if ( build_cache_method_info == null ) continue;

                            // Make sure it has no parameters:

                            ParameterInfo[] parameters = build_cache_method_info.GetParameters();
                            
                            // See if it has any:

                            if ( parameters != null && parameters.Length > 0 )
                            {
                                // It does, complain: 

                                throw new Exception("The class has a BuildClassCache() function which does not match the specification in the GameObject class");
                            }

                            // Make sure there are no returns:

                            if ( build_cache_method_info.ReturnType.Name.Equals("Void") == false )
                            {
                                // A return type: can't allow this.. 

                                throw new Exception("The class has a BuildClassCache() function which does not match the specification in the GameObject class");
                            }                                                       

                            // Ok, that's great - convert it to a delegate:
                            
                            build_cache_function = (CacheFunction) Delegate.CreateDelegate( cache_function_type , null , build_cache_method_info );
                        }

                        // Now look for a clear cache function:

                        CacheFunction clear_cache_function = null;

                        // Have a peek:

                        {
                            // This is the type of method we are looking for:

                            BindingFlags b = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

                            // Try and get the build cache function:

                            MethodInfo clear_cache_method_info = type.GetMethod("ClearClassCache",b);

                            // If not there then complain since there can be no build cache function without a corresponding clear cache function:

                            if ( clear_cache_method_info == null )
                            {
                                throw new Exception("The class must have a ClearClassCache() function if implementing BuildClassCache()");
                            }

                            // Make sure it has no parameters:

                            ParameterInfo[] parameters = clear_cache_method_info.GetParameters();
                            
                            // See if it has any:

                            if ( parameters != null && parameters.Length > 0 )
                            {
                                // It does, complain: 

                                throw new Exception("The class has a ClearClassCache() function which does not match the specification in the GameObject class");
                            }

                            // Make sure there are no returns:

                            if ( clear_cache_method_info.ReturnType.Name.Equals("Void") == false )
                            {
                                // A return type: can't allow this.. 

                                throw new Exception("The class has a ClearClassCache() function which does not match the specification in the GameObject class");
                            }

                            // Ok, that's great - convert it to a delegate:
                            
                            clear_cache_function = (CacheFunction) Delegate.CreateDelegate( cache_function_type , null , clear_cache_method_info );
                        }

                        // Cool: we're ok.. Save both methods in the list

                        clear_cache_functions.Add( clear_cache_function );
                        build_cache_functions.Add( build_cache_function );

                        // Add this type to the list of types that support caching:

                        cached_types.Add(type);
                    }

                    #if WINDOWS_DEBUG

                        catch ( Exception e )
                        {                        
                            // Print that the precaching failed:

                            DebugConsole.Print("\nUnable to build precache function list for GameObject type '" + type.Name + "'." );

                            // Print why the registration failed:

                            DebugConsole.Print("\nReason: " + e.Message );
                        }

                    #else
                        
                        catch ( Exception ){}

                    #endif

                }   // end for each type in the module

            }   // end foreach module in the assembly

            // Make arrays for the build and clear cache functions:

            s_build_cache_functions = build_cache_functions.ToArray();
            s_clear_cache_functions = clear_cache_functions.ToArray();

            // Make arrays for the types involved in the build and clear cache functions:

            Type[] build_cache_types = cached_types.ToArray();
            Type[] clear_cache_types = cached_types.ToArray();

            // Now that we have a list of build and clear cache functions, sort them according to inheritance:

                // Sort build cache functions: base functions should be called first

                for ( int i = 0     ; i < s_build_cache_functions.Length - 1    ; i++ )
                for ( int j = i + 1 ; j < s_build_cache_functions.Length        ; j++ )
                {
                    // See if type 2 is a base of type 1:

                    if ( build_cache_types[j].Equals( game_object_type ) || build_cache_types[i].IsSubclassOf( build_cache_types[j] ) )
                    {
                        // Type 2 is more general than type 1: swap

                        Type            t1 = build_cache_types[j];
                        CacheFunction   t2 = s_build_cache_functions[j];

                        build_cache_types[j]        = build_cache_types[i];
                        s_build_cache_functions[j]  = s_build_cache_functions[i];
                        build_cache_types[i]        = t1;
                        s_build_cache_functions[i]  = t2;
                    }
                }

                // Sort clear cache functions: derived functions should be called first

                for ( int i = 0     ; i < s_clear_cache_functions.Length - 1    ; i++ )
                for ( int j = i + 1 ; j < s_clear_cache_functions.Length        ; j++ )
                {
                    // See if type 1 is a base of type 2:

                    if ( clear_cache_types[i].Equals( game_object_type ) || clear_cache_types[j].IsSubclassOf( clear_cache_types[i] ) )
                    {
                        // Type 1 is more general than type 2: swap

                        Type            t1 = clear_cache_types[j];
                        CacheFunction   t2 = s_clear_cache_functions[j];

                        clear_cache_types[j]        = clear_cache_types[i];
                        s_clear_cache_functions[j]  = s_clear_cache_functions[i];
                        clear_cache_types[i]        = t1;
                        s_clear_cache_functions[i]  = t2;
                    }
                }
        }

        //=========================================================================================
        /// <summary> 
        /// This function allows all game objects to pre-cache their important data that is common
        /// between instances (such as textures) after a level has loaded.
        /// </summary>
        //========================================================================================= 

        private void BuildClassCaches()
        {
            // Call all the build class cache functions:

            for ( int i = 0 ; i < s_build_cache_functions.Length ; i++ )
            {
                s_build_cache_functions[i]();
            }
        }

        //=========================================================================================
        /// <summary> 
        /// This clears data that was pre-cached by BuildPrecache() just before a new level is 
        /// loaded.
        /// </summary>
        //========================================================================================= 

        private void ClearClassCaches()
        {
            // Call all the clear class cache functions:

            for ( int i = 0 ; i < s_build_cache_functions.Length ; i++ )
            {
                s_clear_cache_functions[i]();
            }
        }

    }   // end of class

}   // end of namespace