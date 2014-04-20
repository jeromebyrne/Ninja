using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// This gui object loads a game level, displays and updates the game to the user. 
    /// It's simple but essential nonetheless. It also calls up the end of level guis when needed 
    /// and displays a fadeout/blur style effect when the game is ending.
    /// </summary>
    //#############################################################################################

    public class Gui_Game : GuiWidget
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Last state of the keyboard. </summary>

            private KeyboardState m_last_keyboard_state;

            /// <summary> Last state of the gamepad. </summary>

            private GamePadState m_last_gamepad_state;

            /// <summary> Vertices used for rendering the screen fade out effect. </summary>
            
            private VertexPositionColorTexture[] m_vertices = new VertexPositionColorTexture[4];
            
            /// <summary> Effect to render the game with when fading out the screen. </summary>

            private Effect m_fade_effect = null;

            /// <summary> Name of the string to show when the game is won. </summary>

            private string m_game_won_string = "";

            /// <summary> Name of the string to show when the game is lost. </summary>

            private string m_game_lost_string = "";

            /// <summary> Gui screen to go to when the game is won. </summary>

            private string m_game_won_gui = "";

            /// <summary> Gui screen to go to when the game is lost. </summary>

            private string m_game_lost_gui = "";

            /// <summary> Name of the font to use for the game lost / won messages </summary>

            private Font m_message_font = null;

            /// <summary> Name of the font to use for the game lost / won messages </summary>

            private string m_message_font_name = "";

            /// <summary> Set to true if the game has finished and we have attempted to load the end game gui. </summary>

            private bool m_attempted_to_load_end_gui = false;

        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Time it takes for the blur to happen when the level ends. </summary>

            private const float LEVEL_FINISHED_FADE_TIME = 2.0f;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the object.
        /// </summary>
        //=========================================================================================

        public Gui_Game()
        {
            // Save the current kb and gamepad state;

            m_last_gamepad_state    = GamePad.GetState(PlayerIndex.One);
            m_last_keyboard_state   = Keyboard.GetState();

            // Setup the vertices for rendering the screen fade out effect:

            m_vertices[0].Position = new Vector3( -1 , +1 , 0.5f );
            m_vertices[1].Position = new Vector3( -1 , -1 , 0.5f );
            m_vertices[2].Position = new Vector3( +1 , +1 , 0.5f );
            m_vertices[3].Position = new Vector3( +1 , -1 , 0.5f );

            m_vertices[0].TextureCoordinate = Vector2.Zero;
            m_vertices[1].TextureCoordinate = Vector2.UnitY;
            m_vertices[2].TextureCoordinate = Vector2.UnitX;
            m_vertices[3].TextureCoordinate = Vector2.One;

            m_vertices[0].Color = Color.White;
            m_vertices[1].Color = Color.White;
            m_vertices[2].Color = Color.White;
            m_vertices[3].Color = Color.White;

            // Load the fade out effect:

            m_fade_effect = Core.Graphics.LoadEffect("Effects\\paused_blur");
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

            data.ReadString( "GameLostString"   , ref m_game_lost_string    ,  "No_string"                      );
            data.ReadString( "GameWonString"    , ref m_game_won_string     ,  "No_string"                      );
            data.ReadString( "MessageFont"      , ref m_message_font_name   ,  "Content\\Fonts\\Game_32px.xml"  );
            data.ReadString( "GameWonGui"       , ref m_game_won_gui        ,  ""                               );
            data.ReadString( "GameLostGui"      , ref m_game_lost_gui       ,  ""                               );

            // Load the message font:

            m_message_font = new Font( m_message_font_name );
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

            data.Write( "GameLostString"    , m_game_lost_string    );
            data.Write( "GameWonString"     , m_game_won_string     );
            data.Write( "MessageFont"       , m_message_font_name   );
            data.Write( "GameWonGui"        , m_game_won_gui        );
            data.Write( "GameLostGui"       , m_game_lost_gui       );
        }

        //=========================================================================================
        /// <summary> 
        /// Update function for the GUI. Updates the game.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Update the game:

            Core.Level.Update();

            // Get the current keyboard and gamepad states:

            KeyboardState kbs = Keyboard.GetState(); GamePadState gps = GamePad.GetState(PlayerIndex.One);

            // If the pause button is pressed then run the game paused gui

            if 
            ( 
                ( m_last_keyboard_state.IsKeyUp(Keys.Pause) && kbs.IsKeyDown(Keys.Pause) )
                ||
                ( m_last_keyboard_state.IsKeyUp(Keys.P) && kbs.IsKeyDown(Keys.P) )
                ||
                ( m_last_keyboard_state.IsKeyUp(Keys.Escape) && kbs.IsKeyDown(Keys.Escape) )
                ||
                ( m_last_gamepad_state.IsButtonUp(Buttons.Start) && gps.IsButtonDown(Buttons.Start) )                    
            )
            {
                // Pause game by loading paused game gui: but only if the game is not over

                if ( GameIsOver() == false ) Core.Gui.Load("Content\\Guis\\Game_Paused.xml");
            }

            // Save keyboard and gamepad state:

            m_last_gamepad_state    = gps;
            m_last_keyboard_state   = kbs;

            // See if the game is over:

            if ( GameIsOver() )
            {
                // Game is over: get the level rules object

                LevelRules rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");
                
                // See if there:

                if ( rules != null )
                {
                    // Cool: see if enough time has passed to change to the lost/won screen:

                    if ( rules.TimeSinceLevelOver > LEVEL_FINISHED_FADE_TIME )
                    {
                        // Only do this if we haven't attempted to load the end game gui:

                        if ( m_attempted_to_load_end_gui == false )
                        {
                            // Right: see if we won or lost

                            if ( rules.CurrentGameState == LevelRules.GameState.WON )
                            {
                                // Won:

                                Core.Gui.Load(m_game_won_gui);
                            }
                            else
                            {
                                // Lost:

                                Core.Gui.Load(m_game_lost_gui);
                            }
                        }

                        // We had a shot at this: do no more

                        m_attempted_to_load_end_gui = true;
                    }
                }
            }
        }

        //=========================================================================================
        /// <summary> 
        /// Tells if the game is over.
        /// </summary>
        /// <returns> True if the game is over. </returns>
        //=========================================================================================

        private bool GameIsOver()
        {
            // See if the game has been won or lost:

            bool game_over = false;

            // Try and get the level rules object:

            LevelRules rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

            // See if there:

            if ( rules != null )
            {
                // Good: see if the game is over

                if ( rules.CurrentGameState == LevelRules.GameState.LOST || rules.CurrentGameState == LevelRules.GameState.WON )
                {
                    // Game is over:

                    game_over = true;
                }
            }

            // Return if the game is over:

            return game_over;
        }

        //=========================================================================================
        /// <summary> 
        /// Draw function for the GUI. Draws the game.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // See if the game is over:

            bool game_over = GameIsOver();
 
            // If the game is over we will render the game to a render target:

            RenderTarget2D render_target = null; 

            // See if game is over:

            if ( game_over == true )
            {
                // Pick a width to render the game to:

                int w = 0;
                int h = 0;

                // Set width and height as back buffer settings:

                w = Core.Graphics.Device.Viewport.Width;
                h = Core.Graphics.Device.Viewport.Height;

                // If they are past the max then clamp:

                if ( w > Core.Graphics.Device.GraphicsDeviceCapabilities.MaxTextureWidth ) 
                {
                    w = Core.Graphics.Device.GraphicsDeviceCapabilities.MaxTextureWidth;
                }

                if ( h > Core.Graphics.Device.GraphicsDeviceCapabilities.MaxTextureHeight ) 
                {
                    h = Core.Graphics.Device.GraphicsDeviceCapabilities.MaxTextureHeight;
                }

                // Good: make a render target to render to

                render_target = new RenderTarget2D
                (
                    Core.Graphics.Device    , 
                    w                       ,
                    h                       ,
                    1                       ,
                    SurfaceFormat.Color
                );

                // Set this as the current render target:

                Core.Graphics.Device.SetRenderTarget(0,render_target);
            }

            // Draw the game:

            Core.Level.Draw();

            // If the game is over finish up rendering the render target:

            if ( game_over == true )
            {   
                // Clear the render target on the device:   
   
                Core.Graphics.Device.SetRenderTarget(0,null);

                // Grab a texture from the render target:

                Texture2D target_texture = render_target.GetTexture();

                // Render the fade out effect:

                RenderFadeOut( target_texture );

                // Dispose of all resources:

                render_target.Dispose();
                target_texture.Dispose();

                render_target   = null;
                target_texture  = null;
            }
        }

        //=========================================================================================
        /// <summary> 
        /// Renders the fade out effect for when the game ends.
        /// </summary>
        /// <param name="game_render"> A render of the game in a texture </param>
        //=========================================================================================

        private void RenderFadeOut( Texture2D game_render )
        {
            // If we have no effect or texture then abort:

            if ( game_render == null || m_fade_effect == null ) return;

            // Get the level rules object:

            LevelRules level_rules = (LevelRules) Core.Level.Search.FindByType("LevelRules");

            // Good: now setup the device

            Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

            // Disable alpha blending:

            Core.Graphics.Device.RenderState.AlphaBlendEnable = false;

            // Setup the shader:

                // Blur intensity:

                {
                    // Get the param:

                    EffectParameter p = m_fade_effect.Parameters["Intensity"];

                    // Calculate the intensity of the blur:

                    float intensity = 0;

                    // Set intensity

                    intensity = level_rules.TimeSinceLevelOver / LEVEL_FINISHED_FADE_TIME;

                    // Clamp

                    intensity = MathHelper.Clamp( intensity , 0 , 1 );

                    // Set if there:

                    if ( p != null ){ p.SetValue( intensity ); }
                }

                // Texture

                {
                    // Get the param:

                    EffectParameter p = m_fade_effect.Parameters["Texture"];

                    // Set if there:

                    if ( p != null ){ p.SetValue( game_render ); }
                }

            // Now draw the game render to the screen with the blur effect:

            m_fade_effect.Begin(); m_fade_effect.CurrentTechnique.Passes[0].Begin();

                // Do the drawing:
                
                Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                (
                    PrimitiveType.TriangleStrip ,
                    m_vertices                  ,
                    0                           ,
                    2
                );

            m_fade_effect.End(); m_fade_effect.CurrentTechnique.Passes[0].End();

            // Restore alpha blending:

            Core.Graphics.Device.RenderState.AlphaBlendEnable = GraphicsSystem.DefaultRenderState.AlphaBlendEnable;

            // Now draw the message to show that the level is over:

            if ( m_message_font != null && level_rules != null )
            {
                // Draw the level ended message: figure out the size of the font:

                float size = ( level_rules.TimeSinceLevelOver / LEVEL_FINISHED_FADE_TIME ) * m_message_font.Size;

                // Make sure it is in range:

                if ( size < 0                   ) size = 0;
                if ( size > m_message_font.Size ) size = m_message_font.Size;

                // Cool: now make up a custom font settings block:

                Font.CustomFontSettings settings = m_message_font.GetSettings();

                // Set the size:

                settings.Size = size;

                // Pick the string to render:

                string message = "";

                // See if won or lost:

                if ( level_rules.CurrentGameState == LevelRules.GameState.WON )
                {
                    // Won:

                    message = StringDatabase.GetString(m_game_won_string);
                }
                else
                {
                    // Lost:

                    message = StringDatabase.GetString(m_game_lost_string);
                }

                // Get the view settings for camera:

                Matrix view_projection = Core.Gui.Camera.View * Core.Gui.Camera.Projection;

                // Draw this string at the center of the screen:

                m_message_font.DrawCustomStringCentered
                (
                    message         ,
                    Vector2.Zero    ,
                    view_projection ,
                    settings        
                );
            }
        }

    }   // end of class

}   // end of namespace
