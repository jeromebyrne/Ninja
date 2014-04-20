using System;
using System.Xml;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NinjaGameFontTool
{
    static class Program
    {
        public static Form s_form = null;
        
        public static TextBox s_file_text_box = null;
        public static Button s_file_button = null;
        public static Label s_file_label = null;

        public static TextBox s_xna_file_text_box = null;
        public static Label s_xna_file_label = null;

        public static TextBox s_size_text_box = null;
        public static Label s_size_label = null;
        public static TextBox s_r_text_box = null;
        public static Label s_r_label = null;
        public static TextBox s_g_text_box = null;
        public static Label s_g_label = null;
        public static TextBox s_b_text_box = null;
        public static Label s_b_label = null;
        public static TextBox s_a_text_box = null;
        public static Label s_a_label = null;
        public static TextBox s_char_spacing_text_box = null;
        public static Label s_char_spacing_label = null;
        public static TextBox s_line_spacing_text_box = null;
        public static Label s_line_spacing_label = null;

        public static Button s_run = null;

            /// <summary> A structure representing the size and position of a character in the font. </summary>

            public struct CharacterMetrics
            {
                /// <summary> Position of the character in the texture (X). 0-1 texture coordinate. </summary>

                public float tc_x;

                /// <summary> Position of the character in the texture (Y). 0-1 texture coordinate. </summary>

                public float tc_y;

                /// <summary> Width of the character in the texture. 0-1 texture coordinate. </summary>

                public float tc_w;

                /// <summary> Height of the character in the texture. 0-1 texture coordinate. </summary>

                public float tc_h;
            }

         static Game game = new Game();

        static GraphicsDeviceManager gdm = new GraphicsDeviceManager(game);

        public static GraphicsDevice gd = null;

        [STAThread] static void Main(string[] args)
        {
            game.Window.Title = "NinjaGame font tool: Please close this window to proceed";
            game.Run();

            s_form = new Form(); 

            s_form.SetDesktopBounds(0,0,640,480);
            s_form.Show();
            s_form.Text = "NinjaGame XML font data Generator";

            s_file_label = new Label();
            s_file_label.SetBounds(20,20,100,20);
            s_file_label.Text = "Font texture file:";
            s_file_label.Parent = s_form;

            s_file_text_box = new TextBox();
            s_file_text_box.SetBounds(140,20,400,20);
            s_file_text_box.Parent = s_form;

            s_file_button = new Button();
            s_file_button.SetBounds(540,20,40,20);
            s_file_button.Text = "...";
            s_file_button.Parent = s_form;

            s_xna_file_label = new Label();
            s_xna_file_label.SetBounds(20,40,100,20);
            s_xna_file_label.Text = "XNA tex path:";
            s_xna_file_label.Parent = s_form;

            s_xna_file_text_box = new TextBox();
            s_xna_file_text_box.SetBounds(140,40,400,20);
            s_xna_file_text_box.Parent = s_form;

            s_file_button.Click += new EventHandler(s_file_button_Click);

            s_size_label = new Label();
            s_size_label.SetBounds(20,60,100,20);
            s_size_label.Text = "Font size:";
            s_size_label.Parent = s_form;

            s_size_text_box = new TextBox();
            s_size_text_box.SetBounds(140,60,400,20);
            s_size_text_box.Parent = s_form;
            s_size_text_box.Text = "16";

            s_r_label = new Label();
            s_r_label.SetBounds(20,80,100,20);
            s_r_label.Text = "Font color r:";
            s_r_label.Parent = s_form;

            s_r_text_box = new TextBox();
            s_r_text_box.SetBounds(140,80,400,20);
            s_r_text_box.Parent = s_form;
            s_r_text_box.Text = "1";

            s_g_label = new Label();
            s_g_label.SetBounds(20,100,100,20);
            s_g_label.Text = "Font color g:";
            s_g_label.Parent = s_form;

            s_g_text_box = new TextBox();
            s_g_text_box.SetBounds(140,100,400,20);
            s_g_text_box.Parent = s_form;
            s_g_text_box.Text = "1";

            s_b_label = new Label();
            s_b_label.SetBounds(20,120,100,20);
            s_b_label.Text = "Font color b:";
            s_b_label.Parent = s_form;

            s_b_text_box = new TextBox();
            s_b_text_box.SetBounds(140,120,400,20);
            s_b_text_box.Parent = s_form;
            s_b_text_box.Text = "1";

            s_a_label = new Label();
            s_a_label.SetBounds(20,140,100,20);
            s_a_label.Text = "Font color a:";
            s_a_label.Parent = s_form;

            s_a_text_box = new TextBox();
            s_a_text_box.SetBounds(140,140,400,20);
            s_a_text_box.Parent = s_form;
            s_a_text_box.Text = "1";

            s_char_spacing_label = new Label();
            s_char_spacing_label.SetBounds(20,160,100,20);
            s_char_spacing_label.Text = "Font char spacing:";
            s_char_spacing_label.Parent = s_form;

            s_char_spacing_text_box = new TextBox();
            s_char_spacing_text_box.SetBounds(140,160,400,20);
            s_char_spacing_text_box.Parent = s_form;
            s_char_spacing_text_box.Text = "2";

            s_line_spacing_label = new Label();
            s_line_spacing_label.SetBounds(20,180,100,20);
            s_line_spacing_label.Text = "Font line spacing:";
            s_line_spacing_label.Parent = s_form;

            s_line_spacing_text_box = new TextBox();
            s_line_spacing_text_box.SetBounds(140,180,400,20);
            s_line_spacing_text_box.Parent = s_form;
            s_line_spacing_text_box.Text = "-4";

            s_run = new Button();
            s_run.SetBounds(20,250,400,40);
            s_run.Text = "Generate font xml descriptor";
            s_run.Parent = s_form;
            s_run.Click += new EventHandler( RunTool );

            while ( s_form.IsDisposed == false )
            {            
                System.Windows.Forms.Application.DoEvents();
            }
        }

        static void s_file_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.Multiselect = false;
            fd.RestoreDirectory = true;
            fd.SupportMultiDottedExtensions = true;
            fd.Filter = "TGA textures (*.tga)|*.tga|Gif textures (*.gif)|*.gif|DDS Textures|*.dds";

            fd.FileName = "";
            fd.Title = "Select texture file for font";

            fd.ShowDialog();

            s_file_text_box.Text = fd.FileName;
            s_xna_file_text_box.Text = fd.FileName;

            int c_index = s_file_text_box.Text.LastIndexOf("Content\\",StringComparison.OrdinalIgnoreCase);

            if ( c_index >= 0 )
            {
                s_xna_file_text_box.Text = s_xna_file_text_box.Text.Substring( c_index + "Content\\".Length );

                int e_index = s_xna_file_text_box.Text.LastIndexOf(".",StringComparison.OrdinalIgnoreCase);

                if ( e_index >= 0 )
                {
                    s_xna_file_text_box.Text = s_xna_file_text_box.Text.Remove(e_index);
                }
            }
        }

        static void RunTool(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();

            sd.RestoreDirectory = true;
            sd.SupportMultiDottedExtensions = true;
            sd.Filter = "XML Font descriptors (*.xml)|*.xml";
            sd.Title = "Select xml to save font description and metrics to";

            sd.ShowDialog();

            try
            {
                XmlDocument doc = new XmlDocument();

                doc.AppendChild( doc.CreateElement("data") );

                XmlNode r = doc.FirstChild;

                {
                    XmlElement elem = doc.CreateElement("Texture");
                    elem.InnerText = s_xna_file_text_box.Text;
                    r.AppendChild(elem);
                }

                {
                    XmlElement elem = doc.CreateElement("FontSize");
                    elem.InnerText = s_size_text_box.Text;
                    r.AppendChild(elem);
                }

                {
                    XmlElement elem = doc.CreateElement("FontCharSpacing");
                    elem.InnerText = s_char_spacing_text_box.Text;
                    r.AppendChild(elem);
                }

                {
                    XmlElement elem = doc.CreateElement("FontLineSpacing");
                    elem.InnerText = s_line_spacing_text_box.Text;
                    r.AppendChild(elem);
                }

                {
                    XmlElement elem = doc.CreateElement("FontColorR");
                    elem.InnerText = s_r_text_box.Text;
                    r.AppendChild(elem);
                }

                {
                    XmlElement elem = doc.CreateElement("FontColorG");
                    elem.InnerText = s_g_text_box.Text;
                    r.AppendChild(elem);
                }

                {
                    XmlElement elem = doc.CreateElement("FontColorB");
                    elem.InnerText = s_b_text_box.Text;
                    r.AppendChild(elem);
                }

                {
                    XmlElement elem = doc.CreateElement("FontColorA");
                    elem.InnerText = s_a_text_box.Text;
                    r.AppendChild(elem);
                }

                Texture2D texture = Texture2D.FromFile(game.GraphicsDevice ,s_file_text_box.Text);

                CharacterMetrics[] metrics = GenerateFontMetrics(texture);

                for ( int i = 0 ; i < metrics.Length ; i++ )
                {
                    XmlElement elem = doc.CreateElement("CharMetric");
                    elem.SetAttribute("Code",i.ToString());

                    elem.SetAttribute("UP",metrics[i].tc_x.ToString());
                    elem.SetAttribute("VP",metrics[i].tc_y.ToString());
                    elem.SetAttribute("US",metrics[i].tc_w.ToString());
                    elem.SetAttribute("VS",metrics[i].tc_h.ToString());

                    r.AppendChild(elem);
                }

                doc.Save(sd.FileName);

            }
            catch ( Exception ex ){ MessageBox.Show("Exception raised!:" + ex.Message); }
        }

        public static CharacterMetrics[] GenerateFontMetrics( Texture2D texture )
        {
            // If we have no texture then abort:

            if ( texture == null ) return null;

            CharacterMetrics[] metrics = new CharacterMetrics[256];

            for ( int i = 0 ; i < 256 ; i++ )
            {
                int row = ( i >> 4 ) & 0x0F;
                int col = ( i      ) & 0x0F;

                metrics[i].tc_x = (float)(col) / 16.0f;
                metrics[i].tc_y = (float)(row) / 16.0f;
                
                metrics[i].tc_w = 1.0f / 16.0f;
                metrics[i].tc_h = 1.0f / 16.0f;
            }

            // Get the array of pixels from the texture:

            Color[] pixels = new Color[ texture.Width * texture.Height ]; 
                
                // Get the data
            
                texture.GetData<Color>( pixels );

            // Do each character in the char set:

            for ( int i = 0 ; i < 256 ; i++ )
            {
                // Calculate the row and column of this char in the texture

                int row = ( i >> 4 ) & 0x0F;
                int col = ( i      ) & 0x0F;

                // Calculate the width of each character in the texture:

                int w = texture.Width  >> 4;
                int h = texture.Height >> 4;
                
                // Calculate the x and y position of the top left pixel in the texture:

                int x = col * w;
                int y = row * h;

                // Calculate the right and bottom pixel boundaries for this character in the texture:

                int r = x + w;
                int b = y + h;

                // Now calculate the largest and smallest x and y values for the character 

                int s_x = x + w;
                int l_x = x;
                int s_y = y + h;
                int l_y = y;

                bool found_opaque_pixel = false;

                    // Run through all the pixels for this character:
                    
                    for ( int pos_x = x ; pos_x < r ; pos_x ++ )
                    for ( int pos_y = y ; pos_y < b ; pos_y ++ )
                    {
                        // See if this is a transparent pixel:

                        if ( ( pixels[ ( pos_y * texture.Width ) + pos_x ].A ) > 0 )
                        {
                            found_opaque_pixel = true;

                            // Not transparent: see if this x and y value is smaller or bigger than current:

                            if ( pos_x < s_x ) s_x = pos_x;
                            if ( pos_x > l_x ) l_x = pos_x;
                            if ( pos_y < s_y ) s_y = pos_y;
                            if ( pos_y > l_y ) l_y = pos_y;
                        }
                    }

                if ( found_opaque_pixel == false )
                {
                    s_x = ( x + r ) / 2;
                    l_x = ( x + r ) / 2;
                    s_y = ( y + b ) / 2;
                    l_y = ( y + b ) / 2;
                }

                
                // Save the metrics of this character: add one extra pixel of spacing to be safe

                s_x-=1;
                l_x+=1;
                s_y-=1;
                l_y+=1;

                metrics[i].tc_x = (float)( s_x        ) / (float)( texture.Width     );
                metrics[i].tc_y = (float)( s_y        ) / (float)( texture.Height    );
                metrics[i].tc_w = (float)( l_x - s_x  ) / (float)( texture.Width     );
                metrics[i].tc_h = (float)( l_y - s_y  ) / (float)( texture.Height    );
            }

            return metrics;
        }

    }
}

