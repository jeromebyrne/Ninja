using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Class which draws a tiled, parralax scrolled background image.
    /// </summary>
    //#############################################################################################

    public class ParallaxBackground : GameObject
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

            /// <summary> How big each repeat of the texture should be. </summary>

            private float m_repeat_width = 128;

            /// <summary> How much camera movement affects parallax scrolling. </summary>

            private float m_parallax_multiplier = 0;

            /// <summary> Amount to offset the x texture coordinate by </summary>

            private float m_texture_offset_x = 0;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the object.
        /// </summary>
        //=========================================================================================

        public ParallaxBackground() : base(true,false,false,false)
        {
            // Set default depth:

            Depth = 30000;        
        }

        //=========================================================================================
        /// <summary> 
        /// Does setup for the parallax background class
        /// </summary>
        //=========================================================================================
        
        private void Setup()
        {
            // Create the background's vertices:

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

            data.ReadFloat( "RepeatWidth"           , ref m_repeat_width        , 32  );
            data.ReadFloat( "ParallaxMultiplier"    , ref m_parallax_multiplier , 0   );
            data.ReadFloat( "TextureOffsetX"        , ref m_texture_offset_x    , 0   );

            // Setup vertices for rendering:

            Setup();
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
            data.Write( "NormalMap"             , m_normal_map_name     );
            data.Write( "Effect"                , m_effect_name         );
            data.Write( "RepeatWidth"           , m_repeat_width        );
            data.Write( "ParallaxMultiplier"    , m_parallax_multiplier );
            data.Write( "TextureOffsetX"        , m_texture_offset_x    );
        }

        //=========================================================================================
        /// <summary>
        /// Tells if the object is visible.
        /// </summary>
        /// <param name="c"> Camera the scene is being viewed from </param>
        /// <returns> True if visible </returns>
        //=========================================================================================

        public override bool IsVisible( Camera c ){ return true; }

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

            // Get half the view area but expand it slightly:

            Vector2 view_area = Core.Level.Renderer.Camera.ViewArea; view_area *= 0.525f;

            // Get the position of the camera:

            Vector2 cam_pos = Core.Level.Renderer.Camera.Position;

            // Decide on sprite vertex positions:

            m_vertices[0].Position.X = cam_pos.X - view_area.X;
            m_vertices[1].Position.X = cam_pos.X - view_area.X;
            m_vertices[2].Position.X = cam_pos.X + view_area.X;
            m_vertices[3].Position.X = cam_pos.X + view_area.X;

            m_vertices[0].Position.Y = Position.Y + BoxDimensionsY;
            m_vertices[1].Position.Y = Position.Y - BoxDimensionsY;
            m_vertices[2].Position.Y = Position.Y + BoxDimensionsY;
            m_vertices[3].Position.Y = Position.Y - BoxDimensionsY;

            // Figure out how many repeats on the x axis will fit into this view area:

            float repeats = ( view_area.X * 2.0f ) / m_repeat_width;

            // Figure out how much to shift texture coordinates by:

            float x_shift = cam_pos.X  * m_parallax_multiplier;

            // Figure out the x texture coordinates for each vertex:

            m_vertices[0].TextureCoordinate.X = x_shift + m_texture_offset_x;
            m_vertices[1].TextureCoordinate.X = x_shift + m_texture_offset_x;
            m_vertices[2].TextureCoordinate.X = x_shift + repeats + m_texture_offset_x;
            m_vertices[3].TextureCoordinate.X = x_shift + repeats + m_texture_offset_x;

            // Set shader params:

            EffectParameter param_tex = m_effect.Parameters[ "Texture"               ];
            EffectParameter param_nor = m_effect.Parameters[ "NormalMap"             ];
            EffectParameter param_cam = m_effect.Parameters[ "CameraPosition"        ];
            EffectParameter param_wvp = m_effect.Parameters[ "WorldViewProjection"   ];

            if ( param_tex != null ) param_tex.SetValue(m_texture);
            if ( param_nor != null ) param_nor.SetValue(m_normal_map);
            if ( param_cam != null ) param_cam.SetValue(Core.Level.Renderer.Camera.Position);
            if ( param_wvp != null ) param_wvp.SetValue(view_projection);

            // Disable clamping in the x direction:

            SamplerState sampler_state1 = Core.Graphics.Device.SamplerStates[0];
            SamplerState sampler_state2 = Core.Graphics.Device.SamplerStates[1];

            sampler_state1.AddressU = TextureAddressMode.Wrap;
            sampler_state2.AddressU = TextureAddressMode.Wrap;

            // This fix is required to stop issues on the xbox: otherwise the normal map will jump around

            #if XBOX360

                sampler_state2.AddressV = GraphicsSystem.DefaultSamplerState.AddressV;
                sampler_state2.AddressW = GraphicsSystem.DefaultSamplerState.AddressW;

                sampler_state2.MinFilter = GraphicsSystem.DefaultSamplerState.MinFilter;
                sampler_state2.MagFilter = GraphicsSystem.DefaultSamplerState.MagFilter;
                sampler_state2.MipFilter = GraphicsSystem.DefaultSamplerState.MipFilter;

            #endif       

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

            sampler_state1.AddressU = GraphicsSystem.DefaultSamplerState.AddressU;
            sampler_state2.AddressU = GraphicsSystem.DefaultSamplerState.AddressU;
        }

    }   // end of class

}   // end of namespace 
