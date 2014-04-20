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
    /// This class creates debris that bounces around the level for a bit before 
    /// vanishing. It takes a shape and splits it up into several pieces. 
    /// </summary>
    //#############################################################################################

    public class Debris : GameObject
    {
        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> The position of each piece of debris. </summary>

            private DebrisPiece[] m_pieces = null;

            /// <summary> Texture used to render the pieces. </summary>

            private Texture2D m_texture = null;

            /// <summary> Effect used to render the piece. </summary>

            private Effect m_effect = null;

            /// <summary> Amount of gravity to apply to each piece. </summary>

            private float m_gravity = 0;

            /// <summary> Vertex array to render all the pieces with. Two triangles per piece. </summary>

            private VertexPositionColorTexture[] m_vertices = null;

            /// <summary> Should we flip the debris piece horizontally ? </summary>

            private bool m_flipped = false;
            
        //=========================================================================================
        // Structs
        //=========================================================================================

            /// <summary> Represents one piece of debris in the debris bunch </summary>

            private struct DebrisPiece
            {
                /// <summary> Position of center of this debris piece in the world. </summary>

                public Vector2 Position;

                /// <summary> Texture coordinate for the piece. Top left. </summary>

                public Vector2 TopLeftTextureCoordinate;

                /// <summary> Texture coordinate for the piece. Bottom right. </summary>

                public Vector2 BottomRightTextureCoordinate;

                /// <summary> Size of the piece from its center. </summary>

                public Vector2 Size;

                /// <summary> Current rotation for the piece. </summary>

                public float Rotation;

                /// <summary> Current angular velocity for the piece. </summary>

                public float AngularVelocity;

                /// <summary> Current velocity for the piece. </summary>

                public Vector2 Velocity;

                /// <summary> Amount of time the piece can live for. </summary>

                public float LiveTime;

                /// <summary> How long the piece has been alive for. </summary>

                public float TimeAlive;
            };

        //=========================================================================================
        /// <summary>
        /// Default constructor for the debris. Debris is not intended to be read from a file so 
        /// and constructed like most xml objects so this is just to keep the xml construction
        /// system happy. 
        /// </summary>
        //=========================================================================================

        public Debris() : base(true,false,true,false){}

        //=========================================================================================
        /// <summary>
        /// Default constructor for the debris. Debris is not intended to be read from a file so 
        /// and constructed like most xml objects so this is just to keep the xml construction
        /// system happy.
        /// </summary>
        /// 
        /// <param name="position">             Position of the debris                      </param>
        /// <param name="size">                 Size of the debris from its center          </param>
        /// <param name="flipped">              Should the debris be flipped horizontally ? </param>
        /// <param name="inital_orientation">   Initial orientation of the debris.          </param>
        /// <param name="min_velocity">         Minimum debris velocity                     </param>
        /// <param name="max_velocity">         Maximum debris velocity                     </param>
        /// <param name="min_angular_velocity"> Minimum debris angular velocity             </param>
        /// <param name="max_angular_velocity"> Maximum debris angular velocity             </param>
        /// <param name="min_live_time">        Minimum derbis live time                    </param>
        /// <param name="max_live_time">        Maximum debris live time                    </param>
        /// <param name="pieces">               Number of pieces of debris to make          </param>
        /// <param name="gravity">              Amount of gravity to apply to each piece.   </param>
        /// <param name="texture">              Texture to draw the debris with             </param>
        /// <param name="effect">               Effect to draw the debris with              </param>
        //=========================================================================================

        public Debris
        (
            Vector2     position                ,
            Vector2     size                    ,
            bool        flipped                 ,
            float       inital_orientation      ,
            float       min_velocity            ,
            float       max_velocity            ,
            float       min_angular_velocity    ,
            float       max_angular_velocity    ,
            float       min_live_time           ,
            float       max_live_time           ,
            int         pieces                  ,
            float       gravity                 ,
            Texture2D   texture                 ,
            Effect      effect                  
        ) 
        : base( true ,false ,true ,false )
        {
            // Set the position of this object:

            this.Position = position;

            // Save if we are flipped:

            m_flipped = flipped;

            // If no pieces were specified then abort:

            if ( pieces <= 0 ) return;

            // If no texture or effect was given then also abort:

            if ( texture == null || effect == null ) return;

            // Save the texture and effect given:

            m_effect = effect; m_texture = texture;

            // Create a new array for all the pieces:

            m_pieces = new DebrisPiece[pieces];

            // Save gravity:

            m_gravity = gravity;

            // Create the first piece:
                
            m_pieces[0].Position                        = position;
            m_pieces[0].Size                            = size;
            m_pieces[0].Rotation                        = inital_orientation;
            m_pieces[0].AngularVelocity                 = RandomBetween( min_angular_velocity , max_angular_velocity );
            m_pieces[0].Velocity                        = RandomDir() * RandomBetween( min_velocity , max_velocity );
            m_pieces[0].TimeAlive                       = 0;
            m_pieces[0].LiveTime                        = RandomBetween( min_live_time , max_live_time );
            m_pieces[0].TopLeftTextureCoordinate        = Vector2.Zero;
            m_pieces[0].BottomRightTextureCoordinate    = Vector2.One;

            // Make sure live time is ok:

            if ( m_pieces[0].LiveTime < 0.001f ) m_pieces[0].LiveTime = 0.001f;

            // Create subsequent pieces from splitting one of the prior pieces:

            for ( int i = 1 ; i < m_pieces.Length ; i++ )
            {
                // Pick a piece to split:

                int split_i = Core.Random.Next(0,i-1);

                // Decide on either a horizontal or vertical split:

                if ( ( Core.Random.Next() & 1 ) == 0 )
                {
                    // Do a horizontal split: 

                    m_pieces[i] = m_pieces[split_i];

                        // Pick a x new size for both pieces (half divide)

                        float sx = m_pieces[i].Size.X * 0.5f;

                        // Pick the x texture coordinate halfway between the two pieces:

                        float tx = ( m_pieces[i].BottomRightTextureCoordinate.X + m_pieces[i].TopLeftTextureCoordinate.X ) * 0.5f;

                        // Resize and reposition both pieces so that they are joined together                            

                        m_pieces[i].Size.X                              = sx;
                        m_pieces[i].Position.X                         -= sx;
                        m_pieces[i].BottomRightTextureCoordinate.X      = tx;
                        m_pieces[split_i].Size.X                        = sx;
                        m_pieces[split_i].Position.X                   += sx;
                        m_pieces[split_i].TopLeftTextureCoordinate.X    = tx;
                    
                        // Now pick the rest of the settings for this new piece randomly:

                        m_pieces[i].AngularVelocity = RandomBetween( min_angular_velocity , max_angular_velocity );
                        m_pieces[i].Velocity        = RandomDir() * RandomBetween( min_velocity , max_velocity );
                        m_pieces[i].LiveTime        = RandomBetween( min_live_time , max_live_time );

                        // Make sure live time is ok:

                        if ( m_pieces[i].LiveTime < 0.001f ) m_pieces[i].LiveTime = 0.001f;
                }
                else
                {
                    // Do a vertical split:

                    m_pieces[i] = m_pieces[split_i];

                        // Pick a new y size for both pieces (half divide)

                        float sy = m_pieces[i].Size.Y * 0.5f;

                        // Pick the y texture coordinate halfway between the two pieces:

                        float ty = ( m_pieces[i].BottomRightTextureCoordinate.Y + m_pieces[i].TopLeftTextureCoordinate.Y ) * 0.5f;

                        // Resize and reposition both pieces so that they are joined together                            

                        m_pieces[i].Size.Y                                  = sy;
                        m_pieces[i].Position.Y                             -= sy;
                        m_pieces[i].TopLeftTextureCoordinate.Y              = ty;
                        m_pieces[split_i].Size.Y                            = sy;
                        m_pieces[split_i].Position.Y                       += sy;
                        m_pieces[split_i].BottomRightTextureCoordinate.Y    = ty;
                    
                        // Now pick the rest of the settings for this new piece randomly:

                        m_pieces[i].AngularVelocity = RandomBetween( min_angular_velocity , max_angular_velocity );
                        m_pieces[i].Velocity        = RandomDir() * RandomBetween( min_velocity , max_velocity );
                        m_pieces[i].LiveTime        = RandomBetween( min_live_time , max_live_time );

                        // Make sure live time is ok:

                        if ( m_pieces[i].LiveTime < 0.001f ) m_pieces[i].LiveTime = 0.001f;
                }

            }   // end for all the pieces to make

            // Rotate the position of all the pieces about the origin of the debris part:

            for ( int i = 0 ; i < m_pieces.Length ; i++ )
            {
                // Rotate this position:

                m_pieces[i].Position = Vector2.Transform
                (
                    m_pieces[i].Position - position ,
                    Matrix.CreateRotationZ(inital_orientation)
                );

                m_pieces[i].Position += position;
            }

            // Ok: create a vertex array for us to render with later on:

            m_vertices = new VertexPositionColorTexture[ 6 * m_pieces.Length ];

            // Set the texture coordinates for all the pieces:

            for ( int i = 0 ; i < m_pieces.Length ; i++ )
            {
                // See if we are flipped: and set texture coords appropriately

                if ( m_flipped == false )
                {
                    m_vertices[ i * 6 + 0 ].TextureCoordinate.X = m_pieces[i].TopLeftTextureCoordinate.X;
                    m_vertices[ i * 6 + 0 ].TextureCoordinate.Y = m_pieces[i].TopLeftTextureCoordinate.Y;
                    m_vertices[ i * 6 + 1 ].TextureCoordinate.X = m_pieces[i].BottomRightTextureCoordinate.X;
                    m_vertices[ i * 6 + 1 ].TextureCoordinate.Y = m_pieces[i].TopLeftTextureCoordinate.Y;
                    m_vertices[ i * 6 + 2 ].TextureCoordinate.X = m_pieces[i].BottomRightTextureCoordinate.X;
                    m_vertices[ i * 6 + 2 ].TextureCoordinate.Y = m_pieces[i].BottomRightTextureCoordinate.Y;
                    m_vertices[ i * 6 + 3 ].TextureCoordinate.X = m_pieces[i].BottomRightTextureCoordinate.X;
                    m_vertices[ i * 6 + 3 ].TextureCoordinate.Y = m_pieces[i].BottomRightTextureCoordinate.Y;
                    m_vertices[ i * 6 + 4 ].TextureCoordinate.X = m_pieces[i].TopLeftTextureCoordinate.X;
                    m_vertices[ i * 6 + 4 ].TextureCoordinate.Y = m_pieces[i].BottomRightTextureCoordinate.Y;
                    m_vertices[ i * 6 + 5 ].TextureCoordinate.X = m_pieces[i].TopLeftTextureCoordinate.X;
                    m_vertices[ i * 6 + 5 ].TextureCoordinate.Y = m_pieces[i].TopLeftTextureCoordinate.Y;
                }
                else
                {
                    m_vertices[ i * 6 + 0 ].TextureCoordinate.X = m_pieces[i].BottomRightTextureCoordinate.X;
                    m_vertices[ i * 6 + 0 ].TextureCoordinate.Y = m_pieces[i].TopLeftTextureCoordinate.Y;
                    m_vertices[ i * 6 + 1 ].TextureCoordinate.X = m_pieces[i].TopLeftTextureCoordinate.X;
                    m_vertices[ i * 6 + 1 ].TextureCoordinate.Y = m_pieces[i].TopLeftTextureCoordinate.Y;
                    m_vertices[ i * 6 + 2 ].TextureCoordinate.X = m_pieces[i].TopLeftTextureCoordinate.X;
                    m_vertices[ i * 6 + 2 ].TextureCoordinate.Y = m_pieces[i].BottomRightTextureCoordinate.Y;
                    m_vertices[ i * 6 + 3 ].TextureCoordinate.X = m_pieces[i].TopLeftTextureCoordinate.X;
                    m_vertices[ i * 6 + 3 ].TextureCoordinate.Y = m_pieces[i].BottomRightTextureCoordinate.Y;
                    m_vertices[ i * 6 + 4 ].TextureCoordinate.X = m_pieces[i].BottomRightTextureCoordinate.X;
                    m_vertices[ i * 6 + 4 ].TextureCoordinate.Y = m_pieces[i].BottomRightTextureCoordinate.Y;
                    m_vertices[ i * 6 + 5 ].TextureCoordinate.X = m_pieces[i].BottomRightTextureCoordinate.X;
                    m_vertices[ i * 6 + 5 ].TextureCoordinate.Y = m_pieces[i].TopLeftTextureCoordinate.Y;
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Is visible function for debris. No occlusion culling is performed on debris.
        /// </summary>
        /// <param name="c"> Camera the scene is being viewed from. </param>
        /// <returns> True, as we never cull debris. </returns>
        //=========================================================================================

        public override bool IsVisible(Camera c){ return true; }

        //=========================================================================================
        /// <summary>
        /// Update function for the debris. Moves all the pieces and updates their positions.
        /// </summary>
        //=========================================================================================

        public override void OnUpdate()
        {
            // Do base class update

            base.OnUpdate();

            // If this object has no pieces, texture or shader then remove it from the level:

            if ( m_pieces == null || m_effect == null || m_texture == null )
            {
                // Remove this object:

                Core.Level.Data.Remove(this);

                // Abort:

                return;
            }

            // Update each piece:

            for ( int i = 0 ; i < m_pieces.Length ; i++ )
            {
                // Update the time alive:

                m_pieces[i].TimeAlive += Core.Timing.ElapsedTime;

                // If past the maximum time alive then clamp:

                if ( m_pieces[i].TimeAlive > m_pieces[i].LiveTime ) m_pieces[i].TimeAlive = m_pieces[i].LiveTime;

                // Apply gravity to the piece:

                m_pieces[i].Velocity.Y -= m_gravity;

                // Good, now rotate and move the piece:

                m_pieces[i].Position += m_pieces[i].Velocity;
                m_pieces[i].Rotation += m_pieces[i].AngularVelocity;
            }

            // If no pieces are left alive then kill this object:

            bool piece_alive = false;

            for ( int i = 0 ; i < m_pieces.Length ; i++ )
            {
                // If past the maximum time alive then clamp:

                if ( m_pieces[i].TimeAlive < m_pieces[i].LiveTime )
                {
                    // This piece is alive: do not kill this object

                    piece_alive = true;
                }
            }

            // See if there are no pieces left alive:

            if ( piece_alive == false )
            {
                // Nothing left: kill the debris object

                Core.Level.Data.Remove(this);
            }

        }

        //=========================================================================================
        /// <summary>
        /// Draw function. Draws all the pieces of debris.
        /// </summary>
        //=========================================================================================

        public override void OnDraw()
        {
            // Abort if there are no pieces:

            if ( m_pieces == null || m_vertices == null ) return;

            // Abort if we are missing a shader or texture:

            if ( m_effect == null || m_texture == null ) return;

            // Fill up the data in the vertex array for all the pieces of debris:

            for ( int i = 0 ; i <  m_pieces.Length ; i++ )
            {
                // Rotate the four points of the piece locally about it's center

                    Vector2 p1 = Vector2.One;
                    Vector2 p2 = Vector2.One;
                    Vector2 p3 = Vector2.One;
                    Vector2 p4 = Vector2.One;

                    {
                        // Get piece size:

                        Vector2 s = m_pieces[i].Size;

                        // Scale points by it:

                        p1.X = -s.X;    p1.Y = +s.Y;
                        p2.X = +s.X;    p2.Y = +s.Y;
                        p3.X = +s.X;    p3.Y = -s.Y;
                        p4.X = -s.X;    p4.Y = -s.Y;
                    }

                    Matrix rot = Matrix.CreateRotationZ( m_pieces[i].Rotation );

                    p1 = Vector2.Transform( p1 , rot );
                    p2 = Vector2.Transform( p2 , rot );
                    p3 = Vector2.Transform( p3 , rot );
                    p4 = Vector2.Transform( p4 , rot );

                // Now transform to world position:

                p1 += m_pieces[i].Position;
                p2 += m_pieces[i].Position;
                p3 += m_pieces[i].Position;
                p4 += m_pieces[i].Position;

                // Choose an alpha value for the piece depending on how long it has left to live:

                float a = 1.0f - ( m_pieces[i].TimeAlive / m_pieces[i].LiveTime );

                // Figure out the color for the pieces:

                Vector4 col_vec = Vector4.One; col_vec.W = a;

                // Make a color for the pieces:

                Color col = new Color(col_vec);

                // Now fill in all the data for the piece:

                {
                    // Base index for the 6 vertices:

                    int index = i * 6;

                    // Fill in the positions and colors:

                    m_vertices[ index + 0 ].Position.X = p1.X;
                    m_vertices[ index + 0 ].Position.Y = p1.Y;
                    m_vertices[ index + 1 ].Position.X = p2.X;
                    m_vertices[ index + 1 ].Position.Y = p2.Y;
                    m_vertices[ index + 2 ].Position.X = p3.X;
                    m_vertices[ index + 2 ].Position.Y = p3.Y;
                    m_vertices[ index + 3 ].Position.X = p3.X;
                    m_vertices[ index + 3 ].Position.Y = p3.Y;
                    m_vertices[ index + 4 ].Position.X = p4.X;
                    m_vertices[ index + 4 ].Position.Y = p4.Y;
                    m_vertices[ index + 5 ].Position.X = p1.X;
                    m_vertices[ index + 5 ].Position.Y = p1.Y;

                    m_vertices[ index + 0 ].Color = col;
                    m_vertices[ index + 1 ].Color = col;
                    m_vertices[ index + 2 ].Color = col;
                    m_vertices[ index + 3 ].Color = col;
                    m_vertices[ index + 4 ].Color = col;
                    m_vertices[ index + 5 ].Color = col;
                }

            }   // end for all the pieces

            // Get view camera transforms:
            
            Matrix view_projection = Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection;

            // Set the texture on the shader:

            EffectParameter param_tex = m_effect.Parameters[ "Texture"               ];
            EffectParameter param_wvp = m_effect.Parameters[ "WorldViewProjection"   ];

            if ( param_tex != null ) param_tex.SetValue( m_texture          );
            if ( param_wvp != null ) param_wvp.SetValue( view_projection    );

            // Begin drawing with the shader:
            
            m_effect.Begin();

                // Set vertex declaration on graphics device:

                Core.Graphics.Device.VertexDeclaration = Core.Graphics.VertexPositionColorTextureDeclaration;

                // Begin pass:

                m_effect.CurrentTechnique.Passes[0].Begin();

                    // Draw all the pieces:

                    Core.Graphics.Device.DrawUserPrimitives<VertexPositionColorTexture>
                    (
                        PrimitiveType.TriangleList  ,
                        m_vertices                  ,
                        0                           ,
                        m_pieces.Length * 2
                    );

                // End pass:

                m_effect.CurrentTechnique.Passes[0].End();

            // End drawing with the shader:

            m_effect.End();
        }

        //=========================================================================================
        /// <summary>
        /// Picks a random float bewteen the two given numbers
        /// </summary>
        /// <returns> A random float between the two given numbers. </returns>
        //=========================================================================================

        private float RandomBetween( float num1 , float num2 )
        {
            // Get a random float from 0-1:

            float t = (float) Core.Random.NextDouble();

            // Mix between the two numbers and return the result:

            return num1 * t + num2 * ( 1.0f - t );
        }

        //=========================================================================================
        /// <summary>
        /// Picks a random direction
        /// </summary>
        /// <returns> A random direction. </returns>
        //=========================================================================================

        private Vector2 RandomDir()
        {
            // Pick two random floats:

            while ( true )
            {
                // Pick 'em

                float f1 = (float) Core.Random.NextDouble();
                float f2 = (float) Core.Random.NextDouble();

                // Bring to -0.5 to 0.5 range:

                f1 -= 0.5f;
                f2 -= 0.5f;

                // See if not zero:

                if ( f1 != 0 || f2 != 0 )
                {
                    // Good: make a vector

                    Vector2 v = new Vector2(f1,f2);

                    // Normalise and return 

                    v.Normalize(); return v;
                }
            }
        }

    }   // end of class

}   // end of namespace
