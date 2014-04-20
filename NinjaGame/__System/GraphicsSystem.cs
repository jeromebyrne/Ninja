using System;
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
    //
    /// <summary>
    /// Class that handles management of the graphics device and loading of textures etc. via a 
    /// content manager. Also defines a default render state which must be restored by every object
    /// after it has finished drawing. This default render state is an optimised default, which is 
    /// set to the most frequently used settings for the game. This minimizes the amount of state 
    /// switching that we have to do.
    /// </summary>
    // 
    //#############################################################################################

    public class GraphicsSystem
    {
        //=========================================================================================
        // Properties
        //=========================================================================================

            /// <summary> Graphics device used by the game. Can be used to render things. </summary>

            public GraphicsDevice Device { get { return m_device_manager.GraphicsDevice; } }

            /// <summary> Graphics device manager which manages all graphics devices. </summary>

            public GraphicsDeviceManager DeviceManager { get { return m_device_manager; } }

            /// <summary> Vertex declaration for the vertex position color texture type of vertex. </summary>

            public VertexDeclaration VertexPositionColorTextureDeclaration 
            { 
                get 
                { 
                    return m_vertex_position_color_texture_declaration;                 
                } 
            }

            /// <summary> Vertex declaration for the vertex position color type of vertex. </summary>

            public VertexDeclaration VertexPositionColorDeclaration 
            { 
                get 
                { 
                    return m_vertex_position_color_declaration;                 
                } 
            }

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary>
            /// Contains the default render state settings for the game. All game object should 
            /// restore this default state after they have finished drawing. This set of defaults 
            /// has been optimised to suit the game; so that a minimum of state switching can 
            /// be used.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public struct DefaultRenderState
            {
                #region Enabled features 
                    
                    /// <summary> Default is true. </summary>

                    public static readonly bool AlphaBlendEnable = true;
                    
                    /// <summary> Default is false. </summary>

                    public static readonly bool SeparateAlphaBlendEnabled = false;

                    /// <summary> Default is true. </summary>

                    public static readonly bool AlphaTestEnable = true;

                    /// <summary> Default is false. </summary>

                    public static readonly bool ScissorTestEnable = false;

                    /// <summary> Default is false. </summary>

                    public static readonly bool StencilEnable = false;

                    /// <summary> Default is false. </summary>

                    public static readonly bool DepthBufferEnable = false;

                    /// <summary> Default is false. </summary>
                    
                    public static readonly bool DepthBufferWriteEnable = false;
                    
                    /// <summary> Default is false. </summary>

                    public static readonly bool FogEnable = false;

                    /// <summary> Default is false. </summary>

                    public static readonly bool RangeFogEnable = false;

                    /// <summary> Default is false. </summary>

                    public static readonly bool PointSpriteEnable = false;

                #endregion

                #region Cull mode and fill mode

                    /// <summary> Default is CullMode.None </summary>

                    public static readonly CullMode CullMode = CullMode.None;

                    /// <summary> Default is FillMode.Solid </summary>

                    public static readonly FillMode FillMode = FillMode.Solid;

                #endregion
                
                #region Depth buffer and depth test

                    /// <summary> Default is CompareFunction.LessEqual </summary>

                    public static readonly CompareFunction DepthBufferFunction = CompareFunction.LessEqual;   
     
                    /// <summary> Default is 0 </summary>
                    
                    public static readonly float DepthBias = 0;
                    
                    /// <summary> Default is 0 </summary>

                    public static readonly float SlopeScaleDepthBias = 0;

                #endregion

                #region Alpha test

                    /// <summary> Default is 1 </summary>

                    public static readonly int ReferenceAlpha = 1;

                    /// <summary> Default is CompareFunction.Greater </summary>

                    public static readonly CompareFunction AlphaFunction = CompareFunction.Greater;

                #endregion

                #region Blending

                    /// <summary> Default is BlendFunction.Add </summary>

                    public static readonly BlendFunction BlendFunction = BlendFunction.Add;

                    /// <summary> Default is BlendFunction.Add </summary>

                    public static readonly BlendFunction AlphaBlendOperation = BlendFunction.Add;

                    /// <summary> Default is Blend.SourceAlpha </summary>

                    public static readonly Blend SourceBlend = Blend.SourceAlpha;

                    /// <summary> Default is Blend.InverseSourceAlpha </summary>

                    public static readonly Blend DestinationBlend = Blend.InverseSourceAlpha;

                    /// <summary> Default is Blend.SourceAlpha </summary>

                    public static readonly Blend AlphaSourceBlend = Blend.SourceAlpha;
                    
                    /// <summary> Default is Blend.InverseSourceAlpha </summary>

                    public static readonly Blend AlphaDestinationBlend = Blend.InverseSourceAlpha; 
    
                    /// <summary> Default is Color.White </summary>

                    public static readonly Color BlendFactor = Color.White;

                #endregion

                #region Multsampling - only on xbox360

                    #if XBOX360

                        /// <summary> Default is false </summary>

                        public static readonly bool MultiSampleAntiAlias = false;

                        /// <summary> Default is Int32.MaxValue </summary>

                        public static readonly int MultiSampleMask = Int32.MaxValue;

                    #endif

                #endregion

                #region Texture wrapping

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap0 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap1 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap2 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap3 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap4 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap5 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap6 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap7 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap8 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap9 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap10 = TextureWrapCoordinates.None;
                    
                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap11 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap12 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap13 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap14 = TextureWrapCoordinates.None;

                    /// <summary> Default is TextureWrapCoordinates.None </summary>

                    public static readonly TextureWrapCoordinates Wrap15 = TextureWrapCoordinates.None;

                #endregion

                #region Point Sprites 

                    /// <summary> Default is 1.0f </summary>

                    public static readonly float PointSize = 1.0f;

                    /// <summary> Default is 1.0f </summary>

                    public static readonly float PointSizeMin = 1.0f;

                    /// <summary> Default is 64.0f </summary>

                    public static readonly float PointSizeMax = 64.0f;

                #endregion

                #region Stencil buffer behaviour

                    /// <summary> Default is false </summary>

                    public static readonly bool TwoSidedStencilMode = false;

                    /// <summary> Default is 0 </summary>

                    public static readonly int ReferenceStencil = 0;

                    /// <summary> Default is Int32.MaxValue </summary>

                    public static readonly int StencilMask = Int32.MaxValue;

                    /// <summary> Default is Int32.MaxValue </summary>

                    public static readonly int StencilWriteMask = Int32.MaxValue;

                    /// <summary> Default is CompareFunction.Always </summary>

                    public static readonly CompareFunction StencilFunction = CompareFunction.Always;

                    /// <summary> Default is CompareFunction.Always </summary>

                    public static readonly CompareFunction CounterClockwiseStencilFunction = CompareFunction.Always;        

                    /// <summary> Default is StencilOperation.Keep </summary>

                    public static readonly StencilOperation StencilDepthBufferFail = StencilOperation.Keep;

                    /// <summary> Default is StencilOperation.Keep </summary>

                    public static readonly StencilOperation CounterClockwiseStencilDepthBufferFail = StencilOperation.Keep;

                    /// <summary> Default is StencilOperation.Keep </summary>

                    public static readonly StencilOperation StencilPass = StencilOperation.Keep;

                    /// <summary> Default is StencilOperation.Keep </summary>

                    public static readonly StencilOperation CounterClockwiseStencilPass = StencilOperation.Keep;

                    /// <summary> Default is StencilOperation.Keep </summary>

                    public static readonly StencilOperation StencilFail = StencilOperation.Keep;

                    /// <summary> Default is StencilOperation.Keep </summary>

                    public static readonly StencilOperation CounterClockwiseStencilFail = StencilOperation.Keep;

                #endregion

                #region Fog - only on windows

                    #if WINDOWS

                        /// <summary> Default is Color.TransparentBlack </summary>

                        public static readonly Color FogColor = Color.TransparentBlack;

                        /// <summary> Default is 1.0f </summary>

                        public static readonly float FogDensity = 1.0f;

                        /// <summary> Default is 1.0f </summary>

                        public static readonly float FogEnd = 1.0f;

                        /// <summary> Default is 0.0f </summary>

                        public static readonly float FogStart = 0.0f;

                        /// <summary> Default is FogMode.None </summary>

                        public static readonly FogMode FogTableMode = FogMode.None;

                        /// <summary> Default is FogMode.None </summary>

                        public static readonly FogMode FogVertexMode = FogMode.None;

                    #endif 

                #endregion

                #region Color write behaviour

                    /// <summary> Default is ColorWriteChannels.All </summary>

                    public static readonly ColorWriteChannels ColorWriteChannels = ColorWriteChannels.All;

                    /// <summary> Default is ColorWriteChannels.All </summary>

                    public static readonly ColorWriteChannels ColorWriteChannels1 = ColorWriteChannels.None;

                    /// <summary> Default is ColorWriteChannels.All </summary>

                    public static readonly ColorWriteChannels ColorWriteChannels2 = ColorWriteChannels.None;

                    /// <summary> Default is ColorWriteChannels.All </summary>

                    public static readonly ColorWriteChannels ColorWriteChannels3 = ColorWriteChannels.None;

                #endregion
            };
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary>
            /// This structure contains the default state for all texture samplers on the graphics card.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public struct DefaultSamplerState
            {
                /// <summary> Default is TextureAddressMode.Clamp </summary>

                public static readonly TextureAddressMode AddressU = TextureAddressMode.Clamp;

                /// <summary> Default is TextureAddressMode.Clamp </summary>

                public static readonly TextureAddressMode AddressV = TextureAddressMode.Clamp;

                /// <summary> Default is TextureAddressMode.Clamp </summary>

                public static readonly TextureAddressMode AddressW = TextureAddressMode.Clamp;

                /// <summary> Default is Color.TransparentBlack </summary>

                public static readonly Color BorderColor = Color.TransparentBlack;

                /// <summary> Default is TextureFilter.Linear </summary>

                public static readonly TextureFilter MagFilter = TextureFilter.Linear;

                /// <summary> Default is TextureFilter.Linear </summary>

                public static readonly TextureFilter MinFilter = TextureFilter.Linear;

                /// <summary> Default is TextureFilter.Linear </summary>
                
                public static readonly TextureFilter MipFilter = TextureFilter.Linear;

                /// <summary> Default is 4 </summary>

                public static readonly int MaxAnisotropy = 4;

                /// <summary> Default is 32 </summary>

                public static readonly int MaxMipLevel = 32;

            };

        //=========================================================================================
        // Variables
        //=========================================================================================
            
            /// <summary> Graphics device manager used by the game </summary>

            private GraphicsDeviceManager m_device_manager = null;

            /// <summary> Content manager used to load graphics content for the game </summary>

            private ContentManager m_content_manager = null;

            /// <summary> Vertex declaration for the vertex position color texture type of vertex. </summary>

            private VertexDeclaration m_vertex_position_color_texture_declaration = null;

            /// <summary> Vertex position color type vertex declaration. </summary>

            private VertexDeclaration m_vertex_position_color_declaration = null;

        //=========================================================================================
        /// <summary>
        /// Constructor. Initialises the default render state that will be used in the game.
        /// </summary>
        /// <param name="g"> Graphics device manager for the game   </param>
        /// <param name="c"> Content manager for the game           </param>
        //=========================================================================================

        public GraphicsSystem( GraphicsDeviceManager g , ContentManager c )
        {
            // If either are null then throw an exception:

            if ( g == null ) throw new Exception( "GraphicsSystem must have non null graphics device manager." );
            if ( c == null ) throw new Exception( "GraphicsSystem must have non null content manager." );

            // If the graphics device manager has no graphics device yet then generate an exception

            if ( g.GraphicsDevice == null ) throw new Exception("GraphicsSystem must have non null graphics device.");

            // Save graphics device and content manager:

            m_device_manager  = g;
            m_content_manager = c;

            // Add an event handler to catch a device reset event:
            
            m_device_manager.DeviceReset += new EventHandler(OnDeviceReset);

            // Create vertex declarations:

            m_vertex_position_color_texture_declaration = new VertexDeclaration
            ( 
                m_device_manager.GraphicsDevice , 
                VertexPositionColorTexture.VertexElements 
            );

            m_vertex_position_color_declaration = new VertexDeclaration
            (
                m_device_manager.GraphicsDevice , 
                VertexPositionColor.VertexElements 
            );

            // Disable multi-sampling and depth buffers:

            m_device_manager.GraphicsDevice.PresentationParameters.EnableAutoDepthStencil   = false;
            m_device_manager.PreferMultiSampling                                            = false;
            m_device_manager.GraphicsDevice.DepthStencilBuffer                              = null;
            m_device_manager.GraphicsDevice.PresentationParameters.AutoDepthStencilFormat   = DepthFormat.Unknown;
            m_device_manager.GraphicsDevice.PresentationParameters.MultiSampleType          = MultiSampleType.None;            
            
            // Apply settings:

            m_device_manager.ApplyChanges();

            // Set the default render and sampler state on the graphics device

            SetDefaultRenderState();
            SetDefaultSamplerState();
        }

        //=========================================================================================
        /// <summary>
        /// On device reset event handler. Restores the graphics device to it's normal state.
        /// </summary>
        /// <param name="obj"> Object that sent the event </param>
        /// <param name="args"> Event arguments </param>
        //=========================================================================================

        private void OnDeviceReset( Object obj , EventArgs args )
        {
            // Restore the graphics device to normal 

            if ( Core.Graphics != null )
            {
                // Set default states:

                Core.Graphics.SetDefaultRenderState();
                Core.Graphics.SetDefaultSamplerState();

                // Disable multi-sampling and depth buffers:

                m_device_manager.GraphicsDevice.PresentationParameters.EnableAutoDepthStencil   = false;
                m_device_manager.PreferMultiSampling                                            = false;
                m_device_manager.GraphicsDevice.DepthStencilBuffer                              = null;
                m_device_manager.GraphicsDevice.PresentationParameters.AutoDepthStencilFormat   = DepthFormat.Unknown;
                m_device_manager.GraphicsDevice.PresentationParameters.MultiSampleType          = MultiSampleType.None;

                // Try and reapply core preferences:

                CorePreferences.ApplyAllSettings();
            }
        }

        //=========================================================================================
        /// <summary>
        /// Sets the graphics device to be in the default render state. Be warned, this is a slow 
        /// operation and should be only done once per frame; better still, once per game startup.
        /// </summary>
        //=========================================================================================

        public void SetDefaultRenderState()
        {
            // Put the graphics device into the default render state:

            if ( m_device_manager.GraphicsDevice.IsDisposed == false )
            {
                // Get reference to graphics device to cut down on typing:

                GraphicsDevice g = m_device_manager.GraphicsDevice;

                // Do things the hard way: setting by setting - do all render sates

                    // Alpha blending

                    g.RenderState.AlphaBlendEnable              = DefaultRenderState.AlphaBlendEnable;
                    g.RenderState.SeparateAlphaBlendEnabled     = DefaultRenderState.SeparateAlphaBlendEnabled;
                    g.RenderState.BlendFunction                 = DefaultRenderState.BlendFunction;
                    g.RenderState.AlphaBlendOperation           = DefaultRenderState.AlphaBlendOperation;
                    g.RenderState.SourceBlend                   = DefaultRenderState.SourceBlend;
                    g.RenderState.DestinationBlend              = DefaultRenderState.DestinationBlend;
                    g.RenderState.AlphaSourceBlend              = DefaultRenderState.AlphaSourceBlend;
                    g.RenderState.AlphaDestinationBlend         = DefaultRenderState.AlphaDestinationBlend;
                    g.RenderState.BlendFactor                   = DefaultRenderState.BlendFactor;

                    // Alpha test
                
                    g.RenderState.AlphaTestEnable  = DefaultRenderState.AlphaTestEnable;
                    g.RenderState.ReferenceAlpha   = DefaultRenderState.ReferenceAlpha;
                    g.RenderState.AlphaFunction    = DefaultRenderState.AlphaFunction;
            
                    // Cull mode and fill mode

                    g.RenderState.CullMode = DefaultRenderState.CullMode;
                    g.RenderState.FillMode = DefaultRenderState.FillMode;
               
                    // Depth buffer settings

                    g.RenderState.DepthBufferEnable       = DefaultRenderState.DepthBufferEnable;
                    g.RenderState.DepthBufferWriteEnable  = DefaultRenderState.DepthBufferWriteEnable;
                    g.RenderState.DepthBufferFunction     = DefaultRenderState.DepthBufferFunction;        
                    g.RenderState.DepthBias               = DefaultRenderState.DepthBias;
                    g.RenderState.SlopeScaleDepthBias     = DefaultRenderState.SlopeScaleDepthBias;
                    
                    // Texture wrapping:

                    g.RenderState.Wrap0    = DefaultRenderState.Wrap0;
                    g.RenderState.Wrap1    = DefaultRenderState.Wrap1;
                    g.RenderState.Wrap2    = DefaultRenderState.Wrap2;
                    g.RenderState.Wrap3    = DefaultRenderState.Wrap3;
                    g.RenderState.Wrap4    = DefaultRenderState.Wrap4;
                    g.RenderState.Wrap5    = DefaultRenderState.Wrap5;
                    g.RenderState.Wrap6    = DefaultRenderState.Wrap6;
                    g.RenderState.Wrap7    = DefaultRenderState.Wrap7;
                    g.RenderState.Wrap8    = DefaultRenderState.Wrap8;
                    g.RenderState.Wrap9    = DefaultRenderState.Wrap9;
                    g.RenderState.Wrap10   = DefaultRenderState.Wrap10;
                    g.RenderState.Wrap11   = DefaultRenderState.Wrap11;
                    g.RenderState.Wrap12   = DefaultRenderState.Wrap12;
                    g.RenderState.Wrap13   = DefaultRenderState.Wrap13;
                    g.RenderState.Wrap14   = DefaultRenderState.Wrap14;
                    g.RenderState.Wrap15   = DefaultRenderState.Wrap15;

                    // Fog settings

                    #if WINDOWS

                        g.RenderState.FogEnable        = DefaultRenderState.FogEnable;
                        g.RenderState.RangeFogEnable   = DefaultRenderState.RangeFogEnable;
                        g.RenderState.FogColor         = DefaultRenderState.FogColor;
                        g.RenderState.FogDensity       = DefaultRenderState.FogDensity;
                        g.RenderState.FogEnd           = DefaultRenderState.FogEnd;
                        g.RenderState.FogStart         = DefaultRenderState.FogStart;
                        g.RenderState.FogTableMode     = DefaultRenderState.FogTableMode;
                        g.RenderState.FogVertexMode    = DefaultRenderState.FogVertexMode;

                    #endif

                    // Multi sampling

                    #if XBOX360

                        g.RenderState.MultiSampleAntiAlias  = DefaultRenderState.MultiSampleAntiAlias;
                        g.RenderState.MultiSampleMask       = DefaultRenderState.MultiSampleMask;

                    #endif 

                    // Point and point sprite settings

                    g.RenderState.PointSize            = DefaultRenderState.PointSize;
                    g.RenderState.PointSizeMin         = DefaultRenderState.PointSizeMin;
                    g.RenderState.PointSizeMax         = DefaultRenderState.PointSizeMax;
                    g.RenderState.PointSpriteEnable    = DefaultRenderState.PointSpriteEnable;                

                    // Color write channels for screen and render targets

                    g.RenderState.ColorWriteChannels   = DefaultRenderState.ColorWriteChannels;
                    g.RenderState.ColorWriteChannels1  = DefaultRenderState.ColorWriteChannels1;
                    g.RenderState.ColorWriteChannels2  = DefaultRenderState.ColorWriteChannels2;
                    g.RenderState.ColorWriteChannels3  = DefaultRenderState.ColorWriteChannels3;    

                    // Scissors test
                    
                    g.RenderState.ScissorTestEnable    = DefaultRenderState.ScissorTestEnable;

                    // Stencil buffers and stencil test
            
                    g.RenderState.StencilEnable        = DefaultRenderState.StencilEnable;
                    g.RenderState.TwoSidedStencilMode  = DefaultRenderState.TwoSidedStencilMode;
                    g.RenderState.ReferenceStencil     = DefaultRenderState.ReferenceStencil;
                    g.RenderState.StencilMask          = DefaultRenderState.StencilMask;
                    g.RenderState.StencilWriteMask     = DefaultRenderState.StencilWriteMask;

                    g.RenderState.StencilFunction                          = DefaultRenderState.StencilFunction;
                    g.RenderState.CounterClockwiseStencilFunction          = DefaultRenderState.CounterClockwiseStencilFunction;                
                    g.RenderState.StencilDepthBufferFail                   = DefaultRenderState.StencilDepthBufferFail;
                    g.RenderState.CounterClockwiseStencilDepthBufferFail   = DefaultRenderState.CounterClockwiseStencilDepthBufferFail;
                    g.RenderState.StencilPass                              = DefaultRenderState.StencilPass;
                    g.RenderState.CounterClockwiseStencilPass              = DefaultRenderState.CounterClockwiseStencilPass;
                    g.RenderState.StencilFail                              = DefaultRenderState.StencilFail;
                    g.RenderState.CounterClockwiseStencilFail              = DefaultRenderState.CounterClockwiseStencilFail;                
            }

        }

        //=========================================================================================
        /// <summary>
        /// Sets the graphics device's textures to their default sampling settings.
        /// </summary>
        //=========================================================================================

        public void SetDefaultSamplerState()
        {
            // Run through all the texture samplers:

            for ( int i = 0 ; i < m_device_manager.GraphicsDevice.GraphicsDeviceCapabilities.MaxSimultaneousTextures ; i++ )
            {
                // Set the sampler state:

                m_device_manager.GraphicsDevice.SamplerStates[i].AddressU      = DefaultSamplerState.AddressU;
                m_device_manager.GraphicsDevice.SamplerStates[i].AddressV      = DefaultSamplerState.AddressV;
                m_device_manager.GraphicsDevice.SamplerStates[i].AddressW      = DefaultSamplerState.AddressW;
                m_device_manager.GraphicsDevice.SamplerStates[i].BorderColor   = DefaultSamplerState.BorderColor;
                m_device_manager.GraphicsDevice.SamplerStates[i].MagFilter     = DefaultSamplerState.MagFilter;
                m_device_manager.GraphicsDevice.SamplerStates[i].MinFilter     = DefaultSamplerState.MinFilter;
                m_device_manager.GraphicsDevice.SamplerStates[i].MipFilter     = DefaultSamplerState.MipFilter;
                m_device_manager.GraphicsDevice.SamplerStates[i].MaxAnisotropy = DefaultSamplerState.MaxAnisotropy;
                m_device_manager.GraphicsDevice.SamplerStates[i].MaxMipLevel   = DefaultSamplerState.MaxMipLevel;
            }
                 
        }

        //=========================================================================================
        /// <summary>
        /// Loads the given texture.
        /// </summary>
        /// <param name="name"> Name of the texture to load </param>
        /// <returns> The newly loaded texture, or null on failure </returns>
        //=========================================================================================

        public Texture2D LoadTexture( string name )
        {
            // Get the locale specific version of the file:

            name = Locale.GetLocFile( name );

            // Save texture to load here

            Texture2D loadedTexture = null;

            // This might fail:

            try
            {
                // Attempt to load the texture

                loadedTexture = m_content_manager.Load<Texture2D>(name);
            }

            // If on windows debug then print what happened: otherwise ignore the error

            #if WINDOWS_DEBUG 

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                // Ignore if not in windows debug mode

                catch ( Exception ){;}

            #endif

            // Return the loaded texture

            return loadedTexture;
        }

        //=========================================================================================
        /// <summary>
        /// Loads the given effect.
        /// </summary>
        /// <param name="name"> Name of the effect to load </param>
        /// <returns> The newly loaded effect, or null on failure </returns>
        //=========================================================================================

        public Effect LoadEffect( string name )
        {
            // Get the locale specific version of the file:

            name = Locale.GetLocFile( name );

            // Save texture to load here

            Effect loadedEffect = null;

            // This might fail:

            try
            {
                // Attempt to load the effect

                loadedEffect = m_content_manager.Load<Effect>(name);
            }

            // If on windows debug then print what happened: otherwise ignore the error

            #if WINDOWS_DEBUG 

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                // Ignore if not in windows debug mode

                catch ( Exception ){;}

            #endif

            // Return the loaded texture

            return loadedEffect;
        }

    }   // end of class

}   // end of namespace
