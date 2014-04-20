using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    ///
    /// <summary> 
    /// 
    /// This class is central to the XML object creation and modification
    /// scheme. It maintains a list of all types of objects that can be 
    /// constructed through XML in the Assembly and handles all the details of
    /// reading and writing their data.
    /// 
    /// </summary>
    /// 
    //#############################################################################################

    public class XmlFactory
    {
        //=========================================================================================
        // Variables
        //=========================================================================================
        
        //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        /// <summary>
        /// A hash that maps from class names to XMLConstruction functions: used for constructing 
        /// XMLObject objects, given a string representing a type name.
        /// </summary>
        //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        private static Dictionary<string,ConstructorInfo> s_constructors = null;

        //=========================================================================================
        /// <summary> 
        /// Initializes the XML constructor. Should be done before anything else in the game.
        /// </summary>
        //=========================================================================================

        static public void Initialize()
        {
            // Create the construction hash that will map from type names to actual constructor functions

            s_constructors = new Dictionary<string,ConstructorInfo>();

            // Register all XML classes with the constructor

            RegisterClasses();
        }

        //=========================================================================================
        /// <summary> 
        /// This function searches the assembly or program for all classes that can be created and 
        /// modified via XML and saves pointers to their constructors for later use.
        /// </summary>
        //=========================================================================================

        static private void RegisterClasses()
        {
            // On debug windows print to the console whats happening:

            #if WINDOWS_DEBUG

                DebugConsole.PrintTitle("XML Class registration");

            #endif

            // Get the all the types needed for this registration process:
            
            Type XMLClassType = Type.GetType("NinjaGame.XmlObject");

            // Abort if any of the required types are missing in the assembly:

            if ( XMLClassType == null ) return;

            // Get all the class types in the assembly:

            Module[] modules = Assembly.GetCallingAssembly().GetModules();

            // Run through all the types:

            foreach ( Module module in modules )
            {
                // Get the module types:

                Type[] types = module.GetTypes();

                // Run through all the module types:

                foreach ( Type type in types )
                {
                    // Try and see if the type derives from XMLClass or is abstract: if so then abort

                    if ( type.IsSubclassOf(XMLClassType) == false || type.IsAbstract ) continue;

                    // This code might fail:

                    try
                    {
                        // Get the xmlRead and xmlWrite methods types if they exist:

                        MethodInfo readXml_methodInfo    = type.GetMethod("ReadXml");
                        MethodInfo writeXml_methodInfo   = type.GetMethod("WriteXml");

                        // Warn and abort registration if either do not exist:

                        if ( readXml_methodInfo  == null ) throw new Exception("ReadXml  function missing.");
                        if ( writeXml_methodInfo == null ) throw new Exception("WriteXml function missing.");   

                        // Get the method parameters:

                        ParameterInfo[] readXml_parameters   = readXml_methodInfo.GetParameters();
                        ParameterInfo[] writeXml_parameters  = writeXml_methodInfo.GetParameters();

                        // Abort if parameters are invalid:

                        if ( readXml_parameters.Length   != 1 ) throw new Exception("invalid number of ReadXml  Parameters. Should be 1, not " + readXml_parameters.Length  );
                        if ( writeXml_parameters.Length  != 1 ) throw new Exception("invalid number of WriteXml Parameters. Should be 1, not " + writeXml_parameters.Length );
                        
                        // Make sure parameters and return types are valid:
                        
                        if ( readXml_parameters[0].ParameterType.Name  != "XmlObjectData" ) throw new Exception("invalid ReadXml parameter.  Param 1 should be XmlObjectData , not " + readXml_parameters[0].ParameterType.Name   );
                        if ( writeXml_parameters[0].ParameterType.Name != "XmlObjectData" ) throw new Exception("invalid WriteXml parameter. Param 1 should be XmlObjectData , not " + writeXml_parameters[0].ParameterType.Name  );

                        if ( readXml_methodInfo.ReturnType.Name != "Void" ) throw new Exception("ReadXml  must return void");
                        if ( readXml_methodInfo.ReturnType.Name != "Void" ) throw new Exception("WriteXml must return void");

                        // Make sure the class has a default constructor:

                        Type[] EmptyTypes = new Type[0]; 
                        
                        ConstructorInfo constructor = type.GetConstructor( EmptyTypes );

                        if ( constructor == null ) throw new Exception("XmlObject types must have a default constructor");
                        
                        // Save the constructor for future use:
                        
                        s_constructors[type.Name] = constructor;

                        // Print that the class was registered:

                        #if WINDOWS_DEBUG

                            DebugConsole.Print( "\n" + type.Name );

                        #endif 

                    }

                    #if WINDOWS_DEBUG

                        catch ( Exception e )
                        {                        
                            // Print that registration failed:

                            DebugConsole.Print("\nUnable to register XmlObject type '" + type.Name + "'." );

                            // Print why the registration failed:

                            DebugConsole.Print("\nReason: " + e.Message );
                        }

                    #else
                        
                        catch ( Exception ){}

                    #endif

                }   // end for each type in the module

            }   // end foreach module in the assembly

        }

        //=========================================================================================
        /// <summary> 
        /// Reads the given XML document and saves all the objects created 
        /// from the XML data contained therein to the given linked list.
        /// </summary>
        /// 
        /// <param name="doc">Document to read XML objects from</param>
        /// <returns> A list of XMLObject type objects read, or null on failure </returns>
        //=========================================================================================

        static public LinkedList<XmlObject> ReadXml( XmlDocument doc )
        {
            // Abort if no xml construction hash:

            if ( s_constructors == null ) return null;

            // If the document is null then abort:

            if ( doc == null ) return null;

            // Make an output list to return:

            LinkedList<XmlObject> output = new LinkedList<XmlObject>();

            // Try to read the XML file:

            try
            {
                // Try to get the root data node:

                XmlNode root = doc.FirstChild;

                // If that doesn't exist then return an empty list:

                if ( root == null ) return output;

                // Run through all the nodes:

                foreach ( XmlNode node in root.ChildNodes )
                {
                    // Make sure this is an element type node, if not then ignore:

                    if ( node.NodeType != XmlNodeType.Element ) continue;

                    // Ok: See if the class name exists in the hash containing constructors for all xml objects:

                    if ( s_constructors.ContainsKey(node.Name) )
                    {
                        // Try and read the object in from the XML file:

                        try
                        {
                            // Get the constructor for this object:

                            ConstructorInfo constructor = s_constructors[node.Name]; 

                            // Construct the object and save it:

                            XmlObject obj = (XmlObject) constructor.Invoke( null );

                            // Make a new XMLObjectData collection so the object can read data easily:

                            XmlObjectData data = new XmlObjectData(node);

                            // Read the objects XML data:

                            obj.ReadXml(data);

                            // Put it into the given list of objects:

                            output.AddLast(obj);
                        }

                        // On debug windows if something went wrong then show what:

                        #if WINDOWS_DEBUG

                            catch ( Exception e )
                            {
                                DebugConsole.Print("\nXml file parse error, reason: "); DebugConsole.PrintException(e);
                            }

                        #else

                            catch ( Exception ){}

                        #endif

                    }   // end if this is a valid XMLObject derived type

                }   // end for all the xml nodes              

            }

            // Show what went wrong on debug windows if an exception happened:

            #if WINDOWS_DEBUG

                catch ( Exception e )
                {
                    // Print what happened:

                    DebugConsole.Print("\nXML File read failed! - Details: "); DebugConsole.PrintException(e);

                    // Return null for failure:

                    return null;
                }

            #else

                catch ( Exception ){ return null; }

            #endif

            // Otherwise return the data read

            return output;
        }

        //=========================================================================================
        /// <summary> 
        /// Reads the given XML file and saves all the objects created 
        /// from the XML data contained therein to the given linked list.
        /// </summary>
        /// 
        /// <param name="file"> XML Document to read XML objects from</param>
        /// <returns> A list of XMLObject type objects read, or null on failure </returns>
        //=========================================================================================

        static public LinkedList<XmlObject> ReadXml( String file )
        {
            // Abort if no xml construction hash:

            if ( s_constructors == null ) return null;

            // Try to read the XML file:

            try
            {
                // Create an xml document:

                XmlDocument xmlDoc = new XmlDocument(); 
                
                // Attmept to read from the given file:
                
                xmlDoc.Load( Locale.GetLocFile(file) );

                // Call the document version of this function:

                return ReadXml( xmlDoc );
            }

            // On windows debug print what happened if something went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e )
                {
                    // Print an error in the case of an exception:

                    DebugConsole.Print("\nXML File read failed! - "); DebugConsole.Print(file);

                    // Print what happened:

                    DebugConsole.PrintException(e);

                    // Return null on failure:

                    return null;
                }

            #else

                catch ( Exception ){ return null; }

            #endif
        }

        //=========================================================================================
        /// <summary> 
        /// Writes the given list of XML objects and their attributes to the 
        /// given XML file. 
        /// </summary>
        /// 
        /// <param name="file"> XML file to save to </param>
        /// <param name="input"> List of objects to save </param>
        //=========================================================================================

        static public void WriteXml( String file , LinkedList<XmlObject> input )
        {
            // Do nothing on null input:

            if ( input == null ) return;

            // Try to write to the XML file:

            try
            {
                // Create the XML file:

                XmlDocument doc = new XmlDocument();

                // Create a root xml data node:

                XmlNode root = doc.CreateElement("data");

                // Append the data node to the document:

                doc.AppendChild(root);

                // Run through the list of input objects:

                LinkedList<XmlObject>.Enumerator i = input.GetEnumerator();

                while ( i.MoveNext() )
                {
                    // Get the current object:

                    XmlObject save_object = i.Current;

                    // Get the class name of the object:

                    String type = save_object.GetType().Name;

                    // Create an element in the document for this object:

                    XmlNode element = doc.CreateElement(type);

                    // Create a xml object data object so that the object can save it's data:

                    XmlObjectData data = new XmlObjectData(element);

                    // Get the object to write all it's data:

                    try
                    {
                        // Save the data:

                        save_object.WriteXml(data);

                        // Append the data to the root data node:

                        root.AppendChild(element);
                    }
                    
                    #if WINDOWS_DEBUG
                    
                        // On debug windows print what happened:

                        catch ( Exception e )
                        {
                            DebugConsole.Print("An exception occured whilst writing XML data for an object: "); DebugConsole.PrintException(e);
                        }

                    #else

                        catch ( Exception ){}

                    #endif

                }

                // Finally write the xml file to the given file

                doc.Save(file);

            }

            // On debug windows print what happened if an exception occured:

            #if WINDOWS_DEBUG

                catch ( Exception e )
                {
                    DebugConsole.Print("\nXML File write failed! - "); DebugConsole.Print(file); DebugConsole.PrintException(e);
                }

            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary> 
        /// Convience function that creates a given XMLClass type object from
        /// a string representing its type name. 
        /// </summary>
        /// 
        /// <param name="type">Type name of the object e.g 'Camera'</param>
        /// 
        /// <remarks>
        /// This function does not initialse the object or set
        /// any variables, it only creates it.
        /// </remarks>
        //=========================================================================================

        static public XmlObject CreateObject( String type )
        {
            // Abort if no xml construction hash:

            if ( s_constructors == null ) return null;

            // This might fail

            try
            {
                // Store the class constructor here:

                ConstructorInfo c = null;

                // Try and get the class constructor:

                s_constructors.TryGetValue( type , out c );

                // See if we have a constructor:

                if ( c != null )
                {
                    // Invoke the constructor and save the object

                    XmlObject new_object = (XmlObject) c.Invoke(null);

                    // Now return the object:

                    return new_object;
                }

            }
            catch {;}

            // If we're here then we've failed: return null

            return null;
        }

    }   // end of class

}   // end of namespace
