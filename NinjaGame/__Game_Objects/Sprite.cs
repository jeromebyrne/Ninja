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

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that represents a simple sprite in the game. Just draws a sprite texture to 
    /// the screen. The size of the sprite is determined by it's bounding box dimensions.
    /// </summary>
    //#############################################################################################

    public class Sprite : GameObject
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Shader the sprite uses to draw itself </summary>

            public Effect Effect { get { return m_effect; } set { m_effect = value; }}

            /// <summary> Texture used by the sprite </summary>

            public Texture2D Texture { get { return m_texture;} set { m_texture = value; } }

            /// <summary> Normal map used by the sprite </summary>

            protected Texture2D NormalMap { get { return m_normal_map; } set { m_normal_map = value; } }

            /// <summary> 4 Vertices used to draw the sprite. These must be arranged in tri-strip order. </summary>

            protected VertexPositionColorTexture[] Vertices { get { return m_vertices; } }

            /// <summary> Amount of rotation to apply to the sprite. </summary>

            public float Rotation { get { return m_rotation; } set { m_rotation = value; } }

            /// <summary> Whether to flip the sprite horizontally. </summary>

            public bool HorizontalFlip { get { return m_horizontal_flip; } set { m_horizontal_flip = value; } }

            /// <summary> Whether to flip the sprite vertically. </summary>

            public bool VerticalFlip { get { return m_vertical_flip; } set { m_vertical_flip = value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Shader the sprite uses to draw itself </summary>

            private Effect m_effect = null;

            /// <summary> Name of the shader used by the sprite </summary>

            private string m_effect_name = "";

            /// <summary> Texture used by the sprite </summary>

            protected Texture2D m_texture = null;

            /// <summary> Name of the texture used by the sprite </summary>

            private string m_texture_name = "";

            /// <summary> Normal map used by the sprite </summary>

            protected Texture2D m_normal_map = null;

            /// <summary> Name of the normal map used by the sprite </summary>

            private string m_normal_map_name = "";

            /// <summary> Vertices for the sprite </summary>

            private VertexPositionColorTexture[] m_vertices = null;

            /// <summary> Amount of rotation to apply to the sprite. </summary>

            private float m_rotation = 0;

            /// <summary> Whether to flip the sprite horizontally. </summary>

            private bool m_horizontal_flip = false;

            /// <summary> Whether to flip the sprite vertically. </summary>

            private bool m_vertical_flip = false;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a default sprite with no texture or shader. 
        /// </summary>
        //=========================================================================================

        public Sprite() : base(true, false, false, false){ Setup(); }

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates a sprite with the specified properties set. This is useful for 
        /// derived classes. 
        /// </summary>
        /// <param name="collideable"> If the sprite will be considered for collision detection </param>
        /// <param name="updateable"> If the sprite will have it's update functions called </param>
        /// <param name="overlapable"> If the sprite can be involved in an overlap test </param>
        //=========================================================================================

        public Sprite( bool collideable , bool updateable , bool overlapable ) : base( true, collideable, updateable, overlapable )
        {
            Setup();
        }

        //=========================================================================================
        /// <summary> 
        /// Does setup for the sprite class
        /// </summary>
        //=========================================================================================
        
        private void Setup()
        {
            // Create the sprite's vertices:

            m_vertices = new VertexPositionColorTexture[4];

            // Set the texture coordinates for each vertex:

            m_vertices[0].TextureCoordinate = Vector2.Zero;
            m_vertices[1].TextureCoordinate = Vector2.UnitY;
            m_vertices[2].TextureCoordinate = Vector2.UnitX;
            m_vertices[3].TextureCoordinate = Vector2.One;

            // Set the colors for each vertex:

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;

            // Set the z component of the vertices:

            m_vertices[0].Position.Z = 0;
            m_vertices[1].Position.Z = 0;
            m_vertices[2].Position.Z = 0;
            m_vertices[3].Position.Z = 0;

            // Set default depth:

            Depth = 10000;
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

        public override void ReadXml(XmlObjectData data)
        {
            // Call base class function

            base.ReadXml(data);

            // Read all data:

            data.ReadTexture( "Texture"     , ref m_texture_name    , ref m_texture     , "Graphics\\default"  );
            data.ReadTexture( "NormalMap"   , ref m_normal_map_name , ref m_normal_map  , "Graphics\\default"  );
            data.ReadEffect ( "Effect"      , ref m_effect_name     , ref m_effect      , "Effects\\textured"  );

            data.ReadFloat( "Rotation"          , ref m_rotation            , 0     );
            data.ReadBool ( "HorizontalFlip"    , ref m_horizontal_flip     , false );
            data.ReadBool ( "VerticalFlip"      , ref m_vertical_flip       , false );
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

            data.Write( "Texture"       , m_texture_name    );
            data.Write( "NormalMap"     , m_normal_map_name );
            data.Write( "Effect"        , m_effect_name     );

            data.Write( "Rotation"          , m_rotation        );
            data.Write( "HorizontalFlip"    , m_horizontal_flip );
            data.Write( "VerticalFlip"      , m_vertical_flip   );
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
            
            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Take into account size of sprite

            m_vertices[0].Position.X = - BoxDimensions.X;
            m_vertices[1].Position.X = - BoxDimensions.X;
            m_vertices[2].Position.X = + BoxDimensions.X;
            m_vertices[3].Position.X = + BoxDimensions.X;

            m_vertices[0].Position.Y = + BoxDimensions.Y;
            m_vertices[1].Position.Y = - BoxDimensions.Y;
            m_vertices[2].Position.Y = + BoxDimensions.Y;
            m_vertices[3].Position.Y = - BoxDimensions.Y;

            // Do sprite rotation:

            Matrix rot = Matrix.CreateRotationZ( m_rotation );

                m_vertices[0].Position = Vector3.Transform( m_vertices[0].Position , rot );
                m_vertices[1].Position = Vector3.Transform( m_vertices[1].Position , rot );
                m_vertices[2].Position = Vector3.Transform( m_vertices[2].Position , rot );
                m_vertices[3].Position = Vector3.Transform( m_vertices[3].Position , rot );

            // Move sprite into it's world position:

            m_vertices[0].Position.X += Position.X;
            m_vertices[1].Position.X += Position.X;
            m_vertices[2].Position.X += Position.X;
            m_vertices[3].Position.X += Position.X;

            m_vertices[0].Position.Y += Position.Y;
            m_vertices[1].Position.Y += Position.Y;
            m_vertices[2].Position.Y += Position.Y;
            m_vertices[3].Position.Y += Position.Y;

            // Decide on the texture coordinates for the sprite:

            if ( m_horizontal_flip == false )
            {
                m_vertices[0].TextureCoordinate.X = 0;
                m_vertices[1].TextureCoordinate.X = 0;
                m_vertices[2].TextureCoordinate.X = 1;
                m_vertices[3].TextureCoordinate.X = 1;
            }
            else
            {
                m_vertices[0].TextureCoordinate.X = 1;
                m_vertices[1].TextureCoordinate.X = 1;
                m_vertices[2].TextureCoordinate.X = 0;
                m_vertices[3].TextureCoordinate.X = 0;
            }

            if ( m_vertical_flip == false )
            {
                m_vertices[0].TextureCoordinate.Y = 0;
                m_vertices[1].TextureCoordinate.Y = 1;
                m_vertices[2].TextureCoordinate.Y = 0;
                m_vertices[3].TextureCoordinate.Y = 1;
            }
            else
            {
                m_vertices[0].TextureCoordinate.Y = 1;
                m_vertices[1].TextureCoordinate.Y = 0;
                m_vertices[2].TextureCoordinate.Y = 1;
                m_vertices[3].TextureCoordinate.Y = 0;
            }

            // Set shader params:

            EffectParameter param_tex = m_effect.Parameters[ "Texture"               ];
            EffectParameter param_nor = m_effect.Parameters[ "NormalMap"             ];
            EffectParameter param_cam = m_effect.Parameters[ "CameraPosition"        ];
            EffectParameter param_wvp = m_effect.Parameters[ "WorldViewProjection"   ];

            if ( param_tex != null ) param_tex.SetValue(m_texture);
            if ( param_nor != null ) param_nor.SetValue(m_normal_map);
            if ( param_cam != null ) param_cam.SetValue(Core.Level.Renderer.Camera.Position);
            if ( param_wvp != null ) param_wvp.SetValue(view_projection);

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
        }

    }

}
