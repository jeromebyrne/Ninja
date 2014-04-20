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

//#############################################################################################
//#############################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Gui widget that shows the players health. Nuff said.
    /// </summary>
    //#############################################################################################

    public class Gui_Health : GuiWidget
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Shader the sprite uses to draw itself </summary>

            public Effect Effect { get { return m_effect; } }

            /// <summary> Texture used by the sprite </summary>

            public Texture2D Texture { get { return m_texture;} }

            /// <summary> Size that the picture is shown at. </summary>

            public Vector2 Size { get { return m_size; } set { m_size = value; } }

            /// <summary> Size (X) that the picture is shown at. </summary>

            public float SizeX { get { return m_size.X; } set { m_size.X = value; } }

            /// <summary> Size (Y) that the picture is shown at. </summary>

            public float SizeY { get { return m_size.Y; } set { m_size.Y = value; } }

            /// <summary> Flip the image vertically ? </summary>

            public bool FlipHorizontal { get { return m_flip_horizontal; } set { m_flip_horizontal = value; } }

            /// <summary> Flip the image vertically ? </summary>

            public bool FlipVertical { get { return m_flip_vertical; } set { m_flip_vertical = value; } }

            /// <summary> Number of repeats for the image in the X direction. </summary>

            public float HorizontalRepeats { get { return m_horizontal_repeats; } set { m_horizontal_repeats = value; } }

            /// <summary> Number of repeats for the image in the Y direction. </summary>

            public float VerticalRepeats { get { return m_vertical_repeats; } set { m_vertical_repeats = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Shader the sprite uses to draw itself </summary>

            private Effect m_effect = null;

            /// <summary> Name of the shader used by the sprite </summary>

            private string m_effect_name = "";

            /// <summary> Texture used by the sprite </summary>

            protected Texture2D m_texture = null;

            /// <summary> Size that the picture is shown at. </summary>

            protected Vector2 m_size = Vector2.Zero;

            /// <summary> Name of the texture used by the sprite </summary>

            private string m_texture_name = "";

            /// <summary> Flip the image vertically ? </summary>

            private bool m_flip_horizontal = false;

            /// <summary> Flip the image horizontally ? </summary>

            private bool m_flip_vertical = false;

            /// <summary> Number of repeats for the image in the X direction. </summary>

            private float m_horizontal_repeats = 1;

            /// <summary> Number of repeats for the image in the Y direction. </summary>

            private float m_vertical_repeats = 1;

            /// <summary> Vertices for the sprite </summary>

            private VertexPositionColorTexture[] m_vertices = null;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a default sprite with no texture or shader. 
        /// </summary>
        //=========================================================================================

        public Gui_Health()
        {   
            // Create the sprite's vertices:

            m_vertices = new VertexPositionColorTexture[4];

            // Set the colors for each vertex:

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;           

            // Set default depth:

            Depth = 1000;    
        }

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should read its own data from
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// read from here.
        /// </param>
        //=========================================================================================

        public override void ReadXml( XmlObjectData data )
        {
            // Call base class function

            base.ReadXml(data);

            // Read all data:

            data.ReadTexture( "Texture"             , ref m_texture_name        , ref m_texture , "Graphics\\default"  );
            data.ReadEffect ( "Effect"              , ref m_effect_name         , ref m_effect  , "Effects\\textured"  );
            data.ReadFloat  ( "SizeX"               , ref m_size.X              , 256   );
            data.ReadFloat  ( "SizeY"               , ref m_size.Y              , 256   );
            data.ReadBool   ( "FlipHorizontal"      , ref m_flip_horizontal     , false );
            data.ReadBool   ( "FlipVertical"        , ref m_flip_vertical       , false );
            data.ReadFloat  ( "HorizontalRepeats"   , ref m_horizontal_repeats  , 1     );
            data.ReadFloat  ( "VerticalRepeats"     , ref m_vertical_repeats    , 1     );
        }

        //=========================================================================================
        /// <summary> 
        /// In this function each derived class should write its own data to
        /// the given XML node representing this object and its attributes. Base methods should 
        /// also be called as part of this process.
        /// </summary>
        /// 
        /// <param name="data"> 
        /// An object representing the xml data for this XMLObject. Data values should be 
        /// written to here.
        /// </param>
        //=========================================================================================

        public override void WriteXml( XmlObjectData data )
        {
            // Call base class function

            base.WriteXml(data);

            // Write all data:

            data.Write( "Texture"               , m_texture_name        );
            data.Write( "Effect"                , m_effect_name         );
            data.Write( "SizeX"                 , m_size.X              );
            data.Write( "SizeY"                 , m_size.Y              );
            data.Write( "FlipHorizontal"        , m_flip_horizontal     );
            data.Write( "FlipVertical"          , m_flip_vertical       );
            data.Write( "HorizontalRepeats"     , m_horizontal_repeats  );
            data.Write( "VerticalRepeats"       , m_vertical_repeats    );
        }

        //=========================================================================================
        /// <summary>
        /// Draws the sprite.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // If we are missing an effect or texture then abort:

            if ( m_texture == null ) return;
            if ( m_effect  == null ) return;

            // Get view camera transforms:
            
            Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

            // Get the player from the scene, if there, and get health etc. 

            float player_max_health = 1;
            float player_cur_health = 0;

            PlayerNinja player = (PlayerNinja) Core.Level.Search.FindByType("PlayerNinja");

            if ( player != null )
            {
                // Save player current and maximum health

                player_cur_health = player.Health;
                player_max_health = player.MaximumHealth;
            }

            // Set the posiiton of all the vertices:

            m_vertices[0].Position.X = Position.X;
            m_vertices[1].Position.X = Position.X;
            m_vertices[2].Position.X = Position.X;
            m_vertices[3].Position.X = Position.X;

            m_vertices[0].Position.Y = Position.Y;
            m_vertices[1].Position.Y = Position.Y;
            m_vertices[2].Position.Y = Position.Y;
            m_vertices[3].Position.Y = Position.Y;

            m_vertices[0].Position.X -= m_size.X;
            m_vertices[1].Position.X -= m_size.X;
            m_vertices[2].Position.X -= m_size.X;
            m_vertices[3].Position.X -= m_size.X;

            m_vertices[2].Position.X += ( m_size.X * 2 ) * ( player_cur_health / player_max_health );
            m_vertices[3].Position.X += ( m_size.X * 2 ) * ( player_cur_health / player_max_health );

            m_vertices[0].Position.Y += m_size.Y;
            m_vertices[1].Position.Y -= m_size.Y;
            m_vertices[2].Position.Y += m_size.Y;
            m_vertices[3].Position.Y -= m_size.Y;

            // Set texture coords:

            if ( m_flip_horizontal == false )
            {
                m_vertices[0].TextureCoordinate.X = 0;
                m_vertices[1].TextureCoordinate.X = 0;
                m_vertices[2].TextureCoordinate.X = m_horizontal_repeats * ( player_cur_health / player_max_health );
                m_vertices[3].TextureCoordinate.X = m_horizontal_repeats * ( player_cur_health / player_max_health );
            }
            else
            {
                m_vertices[0].TextureCoordinate.X = m_horizontal_repeats * ( player_cur_health / player_max_health );
                m_vertices[1].TextureCoordinate.X = m_horizontal_repeats * ( player_cur_health / player_max_health );
                m_vertices[2].TextureCoordinate.X = 0;
                m_vertices[3].TextureCoordinate.X = 0;
            }

            if ( m_flip_vertical == false )
            {
                m_vertices[0].TextureCoordinate.Y = 0;
                m_vertices[1].TextureCoordinate.Y = m_vertical_repeats;
                m_vertices[2].TextureCoordinate.Y = 0;
                m_vertices[3].TextureCoordinate.Y = m_vertical_repeats;
            }
            else
            {
                m_vertices[0].TextureCoordinate.Y = m_vertical_repeats;
                m_vertices[1].TextureCoordinate.Y = 0;
                m_vertices[2].TextureCoordinate.Y = m_vertical_repeats;
                m_vertices[3].TextureCoordinate.Y = 0;
            }

            // Set the z component of the vertices according to the depth of the sprite:

            m_vertices[0].Position.Z = -Depth;
            m_vertices[1].Position.Z = -Depth;
            m_vertices[2].Position.Z = -Depth;
            m_vertices[3].Position.Z = -Depth;

            // Set the texture and transform on the shader:

            EffectParameter param_tex = m_effect.Parameters[ "Texture"              ];
            EffectParameter param_wvp = m_effect.Parameters[ "WorldViewProjection"  ];

            if ( param_tex != null ) param_tex.SetValue( m_texture          );
            if ( param_wvp != null ) param_wvp.SetValue( view_projection    );

            // If the number of repeats in x or y is not one then disable texture clamping:

            if ( m_horizontal_repeats != 1 ) 
            {
                Core.Graphics.Device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            }

            if ( m_vertical_repeats != 1 ) 
            {
                Core.Graphics.Device.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            }

            // Begin drawing with the shader:

            Effect.Begin();

                // Set vertex declaration on graphics device:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

                // Begin pass:

                Effect.CurrentTechnique.Passes[0].Begin();

                    // Draw the sprite:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleStrip ,
                        m_vertices                  ,
                        0                           ,
                        2
                    );

                // End pass:

                Effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            Effect.End();

            // Restore clamping to what it was:

            if ( m_horizontal_repeats != 1 ) 
            {
                Core.Graphics.Device.SamplerStates[0].AddressU = GraphicsSystem.DefaultSamplerState.AddressU;
            }

            if ( m_vertical_repeats != 1 ) 
            {
                Core.Graphics.Device.SamplerStates[0].AddressV = GraphicsSystem.DefaultSamplerState.AddressV;
            }
        }

    }   // end of class

}   // end of namespace
