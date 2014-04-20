using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    ///
    /// <summary> 
    /// 
    /// Class that holds XML data for an object and which provides an easy mechanism for that 
    /// data to be read and written to. Underneath the hood it uses .NET XML functionality. 
    /// This object is designed to be used by XMLObject type objects.
    /// 
    /// </summary>
    /// 
    //#############################################################################################

    public class XmlObjectData
    {
        //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        /// <summary>
        /// Data node for the object data. Under this node are sub nodes which each contain a 
        /// variable for an XMLObject.
        /// </summary>
        //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        private XmlNode m_data_node = null;

        //=========================================================================================
        /// <summary>
        /// Construtor for the XML data object. Creates a blank data object.
        /// </summary>
        //=========================================================================================

        public XmlObjectData()
        {
            // Create an xml document:

            XmlDocument xml_doc = new XmlDocument();

            // Create a blank data node and save:

            m_data_node = xml_doc.CreateElement("Data");
        }

        //=========================================================================================
        /// <summary>
        /// Construtor for the XML data object. 
        /// </summary>
        /// 
        /// <param name="xml_file"> 
        /// Name of a file containing xml data. All the data is stored in xml elements under the 
        /// root node.
        /// </param>
        //=========================================================================================

        public XmlObjectData( string xml_file )
        {
            // Checks for debug:

            #if DEBUG

                // Make sure file ok:

                if ( xml_file == null ) throw new ArgumentNullException("Xml file name cannot be null");

                // Make sure name given:

                if ( xml_file.Length == 0 ) throw new ArgumentException("Xml file name must be given");

            #endif

            // Create an xml document:

            XmlDocument xml_doc = new XmlDocument();
            
            // Try and read it:

            try
            {
                // Load the document:

                xml_doc.Load( Locale.GetLocFile(xml_file) );

                // Good: see if there is a root node:

                if ( xml_doc.FirstChild != null )
                {
                    // Good: save it:

                    m_data_node = xml_doc.FirstChild;
                }
                else
                {
                    // None: make our own

                    m_data_node = xml_doc.CreateElement("Data");
                }
            }

            #if WINDOWS_DEBUG

                // On windows debug show anything that goes wrong

                catch ( Exception e )
                { 
                    // Show what happened

                    DebugConsole.PrintException(e); 

                    // Create a blank data node
                
                    m_data_node = xml_doc.CreateElement("Data"); 
                }

            #else

                catch ( Exception )
                { 
                    // Create a blank data node

                    m_data_node = xml_doc.CreateElement("Data"); 
                }

            #endif

        }

        //=========================================================================================
        /// <summary>
        /// Construtor for the XML data object. 
        /// </summary>
        /// 
        /// <param name="xml_file"> 
        /// Name of a file containing xml data. All the data is stored in xml elements under the 
        /// root node.
        /// </param>
        //=========================================================================================

        public XmlObjectData( Stream input_stream )
        {
            // Checks for debug:

            #if DEBUG

                // Make sure file ok:

                if ( input_stream == null ) throw new ArgumentNullException("Xml file stream cannot be null");

            #endif

            // Create an xml document:

            XmlDocument xml_doc = new XmlDocument();
            
            // Try and read it:

            try
            {
                // Load the document:

                xml_doc.Load( input_stream );

                // Good: see if there is a root node:

                if ( xml_doc.FirstChild != null )
                {
                    // Good: save it:

                    m_data_node = xml_doc.FirstChild;
                }
                else
                {
                    // None: make our own

                    m_data_node = xml_doc.CreateElement("Data");
                }
            }

            #if WINDOWS_DEBUG

                // On windows debug show anything that goes wrong

                catch ( Exception e )
                { 
                    // Show what happened

                    DebugConsole.PrintException(e); 

                    // Create a blank data node
                
                    m_data_node = xml_doc.CreateElement("Data"); 
                }

            #else

                catch ( Exception )
                { 
                    // Create a blank data node

                    m_data_node = xml_doc.CreateElement("Data"); 
                }

            #endif

        }

        //=========================================================================================
        /// <summary>
        /// Construtor for the XML data object. 
        /// </summary>
        /// 
        /// <param name="classNode"> 
        /// A data node containing all the data for a class. All data is stored in sub nodes.
        /// </param>
        //=========================================================================================

        public XmlObjectData( XmlNode object_data_node )
        {
            // Checks for debug:

            #if DEBUG

                // Throw an exception if the node given is not valid:

                if ( object_data_node == null )
                {
                    throw new Exception("XMLObjectData.XMLObjectData() - object data node cannot be null");
                }

                // Throw an exception if the node given is not writeable:

                if ( object_data_node.IsReadOnly == true )
                {
                    throw new Exception("XMLObjectData.XMLObjectData() - object data node cannot be read only");
                }

            #endif

            // Save the data node:

            m_data_node = object_data_node;

            // Remove anything that isn't an element type node:

            LinkedList<XmlNode> remove_nodes = new LinkedList<XmlNode>();

            foreach ( XmlNode node in m_data_node.ChildNodes )
            {
                // See if this is not an element type node:

                if ( node.NodeType != XmlNodeType.Element )
                {
                    // Add it to the list of nodes to remove: not an element node

                    remove_nodes.AddLast(node);
                }
            }

            // Do the removal

            foreach ( XmlNode node in remove_nodes ) m_data_node.RemoveChild(node);
        }

        //=========================================================================================
        /// <summary>
        /// Returns a list of data fields present in this block of xml data.
        /// </summary>
        /// <returns> A list containing the name of each field. </returns>
        //=========================================================================================

        public LinkedList<string> GetFields()
        {
            // Save the list here:

            LinkedList<string> field_names = new LinkedList<string>();

            // Add to the list:

            foreach ( XmlNode node in m_data_node.ChildNodes )
            {
                field_names.AddLast( node.Name );
            }

            // Return the list:

            return field_names;
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to save all the xml fields in this data block to the given file under a 
        /// root 'data' element.
        /// </summary>
        /// <param name="file_name"> Name of the file to save to </param>
        //=========================================================================================

        public void Save( string file_name )
        {
            // This might fail:

            try
            {
                // Open a new xml document:

                XmlDocument doc = new XmlDocument();

                // Make a new root node for the document:

                doc.AppendChild( doc.CreateElement("Data") );
                
                // Get the root node:

                XmlNode root = doc.FirstChild;

                // If we have no data node then make one:

                if ( m_data_node == null ) m_data_node = doc.CreateElement("Data");

                // Run through all the data node elements:

                foreach ( XmlNode node in m_data_node.ChildNodes )
                {
                    // See if this node is an element:

                    if ( node.NodeType == XmlNodeType.Element )
                    {
                        // Cast to element:

                        XmlElement element = (XmlElement) node;

                        // Make a new element for the save doc

                        XmlElement new_element = doc.CreateElement(element.Name);

                        // Set its inner text:

                        new_element.InnerText = element.InnerText;

                        // Append to document:

                        root.AppendChild(new_element);
                    }
                }

                // Cool: now save the document

                doc.Save(file_name);
            }

            // In windows debug show what happened if something went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }
            
            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to save all the xml fields in this data block to the given file under a 
        /// root 'data' element.
        /// </summary>
        /// <param name="output_stream"> Output stream to write to </param>
        //=========================================================================================

        public void Save( Stream output_stream )
        {
            // This might fail:

            try
            {
                // Open a new xml document:

                XmlDocument doc = new XmlDocument();

                // Make a new root node for the document:

                doc.AppendChild( doc.CreateElement("Data") );
                
                // Get the root node:

                XmlNode root = doc.FirstChild;

                // If we have no data node then make one:

                if ( m_data_node == null ) m_data_node = doc.CreateElement("Data");

                // Run through all the data node elements:

                foreach ( XmlNode node in m_data_node.ChildNodes )
                {
                    // See if this node is an element:

                    if ( node.NodeType == XmlNodeType.Element )
                    {
                        // Cast to element:

                        XmlElement element = (XmlElement) node;

                        // Make a new element for the save doc

                        XmlElement new_element = doc.CreateElement(element.Name);

                        // Set its inner text:

                        new_element.InnerText = element.InnerText;

                        // Append to document:

                        root.AppendChild(new_element);
                    }
                }

                // Cool: now save the document

                doc.Save(output_stream);
            }

            // In windows debug show what happened if something went wrong:

            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }
            
            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to write the given value to a variable with the given name.
        /// </summary>
        /// <param name="name"> name of the variable to write to </param>
        /// <param name="value"> value that the variable will take </param>
        //=========================================================================================

        public void Write( string name , string value )
        {
            // Abort if no variable name was given:

            if ( name == null || name.Length <= 0 ) return;

            // If the value given was null then replace it with white space:

            if ( value == null ) value = "";

            try
            {
                // See if the variable already exists:

                XmlElement write_element = m_data_node[name];

                if ( write_element != null )
                {
                    // Exists: set it's value

                    write_element.Value = value;
                }
                else
                {
                    // Doesn't exist: create it and set it's value

                        // Get the parent xml document of the node we are writing to:

                        XmlDocument doc = m_data_node.OwnerDocument;

                        // Create a node in the document:

                        XmlElement new_element = doc.CreateElement(name);

                        // Set it's value:

                        new_element.InnerText = value;   

                        // Put it under the data node for our XMLObject:

                        m_data_node.AppendChild(new_element);                     
                }

            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to write the given value to a variable with the given name.
        /// </summary>
        /// <param name="name"> name of the variable to write to </param>
        /// <param name="value"> value that the variable will take </param>
        //=========================================================================================

        public void Write( string name , float value ){ Write( name , value.ToString(Locale.DevelopmentCulture.NumberFormat) ); }

        //=========================================================================================
        /// <summary>
        /// Attempts to write the given value to a variable with the given name.
        /// </summary>
        /// <param name="name"> name of the variable to write to </param>
        /// <param name="value"> value that the variable will take </param>
        //=========================================================================================

        public void Write( string name , int value ){ Write( name , value.ToString(Locale.DevelopmentCulture.NumberFormat) ); }

        //=========================================================================================
        /// <summary>
        /// Attempts to write the given value to a variable with the given name.
        /// </summary>
        /// <param name="name"> name of the variable to write to </param>
        /// <param name="value"> value that the variable will take </param>
        //=========================================================================================

        public void Write( string name , bool value ){ Write( name , value.ToString(Locale.DevelopmentCulture.NumberFormat) ); }

        //=========================================================================================
        /// <summary>
        /// Tells if a given variable exists in the set of xml data
        /// </summary>
        /// <param name="name"> name of the variable to check for existance </param>
        //=========================================================================================

        public bool Exists( string name )
        {
            // Return false if no variable name was given:

            if ( name == null || name.Length <= 0 ) return false;

            // Have a look and see if the variable exists:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // Return if it exists:

                if ( element == null ) return false; else return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif

        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given floating point value into the given variable.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value"> value to save the result to </param>
        /// <returns> true on success, false on failure </returns>
        //=========================================================================================

        public bool ReadFloat( string name , ref float value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then return false

                if ( element == null ) return false;

                // Attempt to convert it to a float

                float elementValue = Convert.ToSingle(element.InnerText, Locale.DevelopmentCulture.NumberFormat);

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given floating point value into the given variable. Assigns a 
        /// default value to the variable if not found.
        /// </summary>
        /// <param name="name"> Name of the value to read </param>
        /// <param name="value"> Value to save the result to </param>
        /// <param name="default_value"> Default value to use if the value is not found. </param>
        /// <returns> 
        /// True on success, false on failure. note that false is still returned if the variable fails 
        /// to load and it is assigned a default value.
        /// </returns>
        //=========================================================================================

        public bool ReadFloat( string name , ref float value , float default_value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then assign it the default value

                if ( element == null )
                {
                    value = default_value; return false;
                }

                // Attempt to convert it to a float

                float elementValue = Convert.ToSingle(element.InnerText, Locale.DevelopmentCulture.NumberFormat);

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given integer value into the given variable.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value"> value to save the result to </param>
        /// <returns> true on success, false on failure </returns>
        //=========================================================================================

        public bool ReadInt( string name , ref int value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then return false

                if ( element == null ) return false;

                // Attempt to convert it to an integer

                int elementValue = Convert.ToInt32(element.InnerText,Locale.DevelopmentCulture.NumberFormat);

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given integer value into the given variable. Assigns a 
        /// default value to the variable if not found.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value"> value to save the result to </param>
        /// <param name="default_value"> Default value to use if the value is not found. </param>
        /// <returns> 
        /// True on success, false on failure. note that false is still returned if the variable fails 
        /// to load and it is assigned a default value.
        /// </returns>
        //=========================================================================================

        public bool ReadInt( string name , ref int value , int default_value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then assign it a default value

                if ( element == null )
                {
                    value = default_value; return false;
                }

                // Attempt to convert it to an integer

                int elementValue = Convert.ToInt32(element.InnerText,Locale.DevelopmentCulture.NumberFormat);

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given boolean value into the given variable.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value"> value to save the result to </param>
        /// <returns> true on success, false on failure </returns>
        //=========================================================================================

        public bool ReadBool( string name , ref bool value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then return false for failure

                if ( element == null ) return false;

                // Attempt to convert it to a boolean

                bool elementValue = Convert.ToBoolean(element.InnerText);

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given boolean value into the given variable. Assigns a 
        /// default value to the variable if not found.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value"> value to save the result to </param>
        /// <param name="default_value"> Default value to use if the value is not found. </param>
        /// <returns> 
        /// True on success, false on failure. note that false is still returned if the variable fails 
        /// to load and it is assigned a default value.
        /// </returns>
        //=========================================================================================

        public bool ReadBool( string name , ref bool value , bool default_value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then assign it a default value

                if ( element == null )
                {
                    value = default_value; return false;
                }

                // Attempt to convert it to a boolean

                bool elementValue = Convert.ToBoolean(element.InnerText);

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given string value into the given variable.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value"> value to save the result to </param>
        /// <returns> true on success, false on failure </returns>
        //=========================================================================================

        public bool ReadString( string name , ref string value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then return false

                if ( element == null ) return false;

                // Grab the value of the element:

                string elementValue = element.InnerText;

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given string value into the given variable. Assigns a 
        /// default value to the variable if not found.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value"> value to save the result to </param>
        /// <param name="default_value"> Default value to use if the value is not found. </param>
        /// <returns> 
        /// True on success, false on failure. note that false is still returned if the variable fails 
        /// to load and it is assigned a default value.
        /// </returns>
        //=========================================================================================

        public bool ReadString( string name , ref string value , string default_value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then assign it a default value

                if ( element == null )
                {
                    value = default_value; return false;
                }

                // Grab the value of the element:

                string elementValue = element.InnerText;

                // Save the value:

                value = elementValue;

                // Return true for success:

                return true;
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given texture value into the given variable.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value_name"> where to store the name of the texture </param>
        /// <param name="value"> where to store the actual texture itself </param>
        /// <returns> true on success, false on failure </returns>
        //=========================================================================================

        public bool ReadTexture( string name , ref string value_name , ref Texture2D value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then return false

                if ( element == null ) return false;

                // Attempt to convert it to a float

                string elementValue = element.InnerText;

                // Attempt to load the texture:

                Texture2D loaded_texture = Core.Graphics.LoadTexture(elementValue);

                // See if that succeeded:

                if ( loaded_texture != null )
                {
                    // Save the texture:

                    value_name = elementValue; value = loaded_texture;

                    // Return true for success

                    return true;
                }
                else
                {
                    // Couldn't load the texture: return false for failure

                    return false;
                }
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given texture value into the given variable. Tries to load a 
        /// default texture if the variable containing the texture name is not found in the xml.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value_name"> where to store the name of the texture </param>
        /// <param name="value"> where to store the actual texture itself </param>
        /// <param name="default_value"> Name of the default texture to load if load fails. </param>
        /// <returns> 
        /// True on success, false on failure. note that false is still returned if the variable fails 
        /// to load and it is assigned a default value.
        /// </returns>
        //=========================================================================================

        public bool ReadTexture( string name , ref string value_name , ref Texture2D value , string default_value_name ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then try and load the default texture:

                if ( element == null )
                {
                    // Attempt to load the texture:

                    Texture2D t = Core.Graphics.LoadTexture(default_value_name);

                    // See if that succeeded:

                    if ( t != null )
                    {
                        // Save the texture:

                        value_name = default_value_name; value = t;
                    }

                    // Return false for failure:

                    return false;
                }

                // Attempt to convert it to a float

                string elementValue = element.InnerText;

                // Attempt to load the texture:

                Texture2D loaded_texture = Core.Graphics.LoadTexture(elementValue);

                // See if that succeeded:

                if ( loaded_texture != null )
                {
                    // Save the texture:

                    value_name = elementValue; value = loaded_texture;

                    // Return true for success:

                    return true;
                }
                else
                {
                    // Couldn't load the texture: try to load the default one

                    return false;
                }
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given effect value into the given variable.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value_name"> where to store the name of the effect </param>
        /// <param name="value"> where to store the actual effect itself </param>
        /// <returns> true on success, false on failure </returns>
        //=========================================================================================

        public bool ReadEffect( string name , ref string value_name , ref Effect value ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then return false

                if ( element == null ) return false;

                // Attempt to convert it to a float

                string elementValue = element.InnerText;

                // Attempt to load the effect:

                Effect loaded_effect = Core.Graphics.LoadEffect(elementValue);

                // See if that succeeded:

                if ( loaded_effect != null )
                {
                    // Save the texture:

                    value_name = elementValue; value = loaded_effect;

                    // Return true for success:

                    return true;
                }
                else
                {
                    // Couldn't load the texture: return false for failure

                    return false;
                }
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Attempts to read a given effect value into the given variable. Tries to load a 
        /// default effect if name of the effect is not found in the xml.
        /// </summary>
        /// <param name="name"> name of the value to read </param>
        /// <param name="value_name"> where to store the name of the effect </param>
        /// <param name="value"> where to store the actual effect itself </param>
        /// <param name="default_value"> Name of the default effect to load if load fails. </param>
        /// <returns> 
        /// True on success, false on failure. note that false is still returned if the variable fails 
        /// to load and it is assigned a default value.
        /// </returns>
        //=========================================================================================

        public bool ReadEffect( string name , ref string value_name , ref Effect value , string default_value_name ) 
        {
            // If no name was given then return false:

            if ( name == null || name.Length <= 0 ) return false;

            // Attempt to read the variable:

            try
            {
                // Try and get the variable:

                XmlElement element = m_data_node[name];

                // If the variable doesn't exist then try to load a default shader:

                if ( element == null )
                {
                    // Attempt to load the effect:

                    Effect e = Core.Graphics.LoadEffect(default_value_name);

                    // See if that succeeded:

                    if ( e != null )
                    {
                        // Save the shader:

                        value_name = default_value_name; value = e;
                    }
                    
                    // Return false for failure

                    return false;
                }

                // Attempt to convert it to a float

                string elementValue = element.InnerText;

                // Attempt to load the effect:

                Effect loaded_effect = Core.Graphics.LoadEffect(elementValue);

                // See if that succeeded:

                if ( loaded_effect != null )
                {
                    // Save the texture:

                    value_name = elementValue; value = loaded_effect;

                    // Return true for success:

                    return true;
                }
                else
                {
                    // Couldn't load the texture: return false for failure

                    return false;
                }
            }

            #if WINDOWS_DEBUG

                // On windows debug print what happened if something went wrong:

                catch ( Exception e ){ DebugConsole.PrintException(e); return false; }

            #else

                catch ( Exception ){ return false; }

            #endif
        }

    }   // end of class

}   // end of namespace 
