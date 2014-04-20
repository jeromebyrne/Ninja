using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Simple AI Behaviour. Makes AI turn around and sight the player if the player approaches 
    /// from behind. The inclination towards this behaviour grows stronger as the player nears.
    /// </summary>
    //#############################################################################################

    public class AI_Turn_Around :AIBehaviour
    {
        //=========================================================================================
        // Constants
        //=========================================================================================

            /// <summary> Maximum distance at which the AI will turn around. </summary>

            private const float TURN_AROUND_DISTANCE = 160;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the behaviour. A valid control character MUST be passed in.
        /// </summary>
        /// <param name="i"> 
        /// Importance of the behaviour to this character. Behaviour scores are scaled by this. 
        /// </param>
        /// <param name="c"> Character the behaviour is controlling. </param>
        /// <param name="b"> Behaviour set the AIBehaviour belongs to. </param>
        //=========================================================================================

        public AI_Turn_Around( float i , Character c , AIBehaviourSet b ) : base ( i , c , b ){}

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public override float ScoreBehaviour()
        { 
            // Get the ai sight module:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            // If it isn't there then just return zero:

            if ( b_sight == null ) return 0;

            // If there is no player then abort:

            if ( b_sight.Player == null ) return 0;

            // If the player is marked as visible then this action is not relevant:

            if ( b_sight.PlayerInSight ) return 0;

            // Get the direction of the players line of vision:

            Vector2 vision_vec = Vector2.Transform( Vector2.UnitX , Matrix.CreateRotationZ( b_sight.Player.SurfaceRotation ) );

            if ( b_sight.Player.Flipped == false ) vision_vec.X *= -1; 

            // Now get a vector from the player to the ai

            Vector2 to_ai = ControlCharacter.Position - b_sight.Player.Position;

            // Do a dot product with the players direction vector to see if we must turn around. it the dot is positive then we must turrn around

            float dot = Vector2.Dot( to_ai , vision_vec );

            // See if we have to turn around:

            if ( dot > 0 )
            {
                // Ok: get the actual distance to the player and use this to determine if we should turn

                return ( TURN_AROUND_DISTANCE - to_ai.Length() ) * Importance;
            }
            else
            {
                // Don't have to turn around

                return 0;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public override void PerformBehaviour()
        {
            // Get the ai sight module:

            AI_Sight b_sight = (AI_Sight) BehaviourSet.GetBehaviour("AI_Sight");

            // If it isn't there then abort

            if ( b_sight == null ) return;

            // If there is no player then abort:

            if ( b_sight.Player == null ) return;

            // If the player is marked as visible then this action is not relevant:

            if ( b_sight.PlayerInSight ) return;

            // Get the direction of the players line of vision:

            Vector2 vision_vec = Vector2.Transform( Vector2.UnitX , Matrix.CreateRotationZ( b_sight.Player.SurfaceRotation ) );

            if ( b_sight.Player.Flipped == false ) vision_vec.X *= -1; 

            // Now get a vector from the player to the ai

            Vector2 to_ai = ControlCharacter.Position - b_sight.Player.Position;

            // Do a dot product with the players direction vector to see if we must turn around. it the dot is positive then we must

            float dot = Vector2.Dot( to_ai , vision_vec );

            // If the dot product is positive then we must turn around:

            if ( dot > 0 )
            {
                // Turn around the character:

                if ( ControlCharacter.Flipped )
                {
                    ControlCharacter.Accelerate( - 0.25f );
                }
                else
                {
                    ControlCharacter.Accelerate( 0.25f );
                }
            }
        }

    }   // end of class

}   // end of namespace
