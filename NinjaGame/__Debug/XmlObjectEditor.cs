#if WINDOWS_DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;

//################################################################################################
//################################################################################################

namespace NinjaGame
{
    //############################################################################################
    //
    /// <summary>
    /// Class that brings up a GUI which shows all of the XML Properties that an object has and 
    /// which allows those properties to be modified. This class is only defined on windows debug.
    /// </summary>
    //
    //############################################################################################

    public class XmlObjectEditor
    {
        //========================================================================================
        // Attributes
        //========================================================================================

            /// <summary> Tells if the object editor is open. </summary>

            public static bool Open { get { return s_form != null; } }

        //========================================================================================
        // Variables
        //========================================================================================

            /// <summary> Form shown by the editor </summary>

            private static Form s_form = null;

            /// <summary> A table containing all the fields and their values </summary>

            private static TableLayoutPanel s_table = null;

            /// <summary> An array of text boxes for all the field names in the table </summary>

            private static TextBox[] s_fields = null;

            /// <summary> An array of text boxes containing values for all the fields in the table </summary>

            private static TextBox[] s_values = null;

        //========================================================================================
        /// <summary>
        /// Begins editing of an object and opens up a GUI showing all of it's attributes. 
        /// </summary>
        /// <param name="obj"> Object to be edited. </param>
        /// <param name="comments"> comments to add for editing </param>
        //========================================================================================

        public static void EditObject( XmlObject obj , string comments )
        {
            // Abort if no object:

            if ( obj == null ) return;

            // Make a new form:

            s_form = new Form();

            // Set it's attribtes:

            s_form.Text             = "XML Object editor: close window to save.";
            s_form.Width            = 640;
            s_form.Height           = 700;
            s_form.AllowDrop        = false;
            s_form.AutoScroll       = true;
            s_form.FormBorderStyle  = FormBorderStyle.FixedSingle;
            s_form.MaximizeBox      = false;
            s_form.AutoSize         = false;

            // Create a comments label:

            Label comments_label = new Label();

            comments_label.Top = 20;
            comments_label.Width = 600;
            comments_label.Height = 40;
            comments_label.Text = comments;
            comments_label.Parent = s_form;

            // Create the table for the form:

            CreateEditorTable(obj);

            // Show the form:

            s_form.ShowDialog();

            // Save settings to the object

            try
            {
                WriteXml(obj);
            }
            catch ( Exception e){ DebugConsole.PrintException(e); }

            // Destroy the form and table:

            s_form      = null;
            s_table     = null;
            s_fields    = null;
            s_values    = null;
        }

        //========================================================================================
        /// <summary>
        /// Createa and populates the table in the xml editor.
        /// </summary>
        /// <param name="obj"> Object the editor is editing </param>
        //========================================================================================

        private static void CreateEditorTable( XmlObject obj )
        {
            // Read the xml properties of the object:

            XmlObjectData xml_data = ReadXml(obj);

            // Get the xml fields:

            LinkedList<string> xml_fields = xml_data.GetFields();

            // Make the table 

            s_table = new TableLayoutPanel(); s_table.Parent = s_form;

            s_table.Top = 60;

            // Create the arrays of fields and values:

            s_fields = new TextBox[xml_fields.Count];
            s_values = new TextBox[xml_fields.Count];            

            // Set number of rows and columns in table:

            s_table.ColumnCount     = 2;
            s_table.RowCount        = xml_fields.Count;
            s_table.Width           = 600;
            s_table.Height          = 5000;

            // Refresh

            s_table.Refresh();

            // Populate the rows and columns:

            int row_num = 0;

            foreach ( string field in xml_fields )
            {
                // Make a text box control:

                TextBox text_box1 = new TextBox(); text_box1.Text = field;

                text_box1.Width     = 200;
                text_box1.Enabled   = false;

                // Add this field into the table and array:

                s_table.Controls.Add( text_box1 , 0 , row_num );
                s_fields[row_num] = text_box1;

                // Create a text box control:

                TextBox text_box2 = new TextBox(); 

                text_box2.Width = 400;

                // Set the text box's value:

                string field_value = ""; xml_data.ReadString( field, ref field_value );
                
                text_box2.Text = field_value;

                // Add the text box into the table and array:

                s_table.Controls.Add( text_box2 , 1 , row_num );
                s_values[row_num] = text_box2;

                // Increment the row number:

                row_num++;
            }
        }

        //========================================================================================
        /// <summary>
        /// Reads the xml data for a given object.
        /// </summary>
        /// <param name="obj"> Object to read the data for </param>
        /// <returns> Data read </returns>
        //========================================================================================

        private static XmlObjectData ReadXml( XmlObject obj )
        {
            // Make an xml document:

            XmlDocument xml_doc = new XmlDocument();

            // Make a new node to hold the data:

            XmlNode xml_node = xml_doc.CreateElement("data");

            // Append to document:

            xml_doc.AppendChild(xml_node);

            // Initialise xml data object:

            XmlObjectData xml_data = new XmlObjectData(xml_node);

            // Read all the object's xml data:

            try
            {
                obj.WriteXml(xml_data);
            }
            catch ( Exception e ){ DebugConsole.PrintException(e); }

            // Return the data read:

            return xml_data;
        }

        //========================================================================================
        /// <summary>
        /// Writes the xml data in the editor table to the given object.
        /// </summary>
        /// <param name="obj"> Object to write the data to </param>
        //========================================================================================

        private static void WriteXml( XmlObject obj )
        {
            // Make an xml document:

            XmlDocument xml_doc = new XmlDocument();

            // Make a new node to hold the data:

            XmlNode xml_node = xml_doc.CreateElement("data");

            // Append to document:

            xml_doc.AppendChild(xml_node);

            // Initialise xml data object:

            XmlObjectData xml_data = new XmlObjectData(xml_node);

            // Run through the table:

            for ( int i = 0 ; i < s_fields.Length ; i++ )
            {
                // Set this field in the xml data:
                
                xml_data.Write( s_fields[i].Text , s_values[i].Text );
            }

            // Get the object to read this data:

            obj.ReadXml(xml_data);
        }

    }
}

#endif  // If WINDOWS_DEBUG