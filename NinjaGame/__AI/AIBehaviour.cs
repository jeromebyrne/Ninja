using System;
using System.Collections.Generic;
using System.Text;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that represents a generic AI behavior. Contains functions for ranking 
    /// how important this behaviour should be an action function for doing the 
    /// behaviour if the behaviour is currently selected.
    /// </summary>
    //#############################################################################################

    public abstract class AIBehaviour
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> This is the character the AI behaviour will control. </summary>

            public Character ControlCharacter { get { return m_control_character; } }

            /// <summary> This is the behaviour set the behaviour belongs to. </summary>

            public AIBehaviourSet BehaviourSet { get { return m_behaviour_set; } }

            /// <summary> Gets/sets how much to scale the behaviour scores by. Subclasses should obey this and scale their scores by it. </summary>

            public float Importance { get { return m_importance; } set { m_importance = value; } }

            /// <summary> Class name or name of the behaviour. </summary>

            public string Name { get { return m_name; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> This is the character the AI behaviour will control. </summary>

            private Character m_control_character = null;

            /// <summary> This is the AI behaviour set the behaviour belongs to. </summary>

            private AIBehaviourSet m_behaviour_set = null;

            /// <summary> How much to scale the behaviour scores by. Subclasses should obey this and scale their scores by it. </summary>

            private float m_importance = 1;

            /// <summary> Class name or name of the behaviour. </summary>

            private string m_name = null;

        //=========================================================================================
        /// <summary>
        /// Constructor. Creates the behaviour. A valid control character MUST be passed in.
        /// </summary>
        /// <param name="importance"> 
        /// Importance of the behaviour to this character. Behaviour scores are scaled by this. 
        /// </param>
        /// <param name="control_character"> Character the behaviour is controlling. </param>
        /// <param name="behaviour_set"> Behaviour set the AIBehaviour belongs to. </param>
        //=========================================================================================

        public AIBehaviour( float importance , Character control_character , AIBehaviourSet behaviour_set )
        {
            // On windows debug if the control character is null then throw an exception:

            #if WINDOWS_DEBUG

                if ( control_character == null )
                {
                    throw new Exception("AIBehaviour must have a valid control character !!!");
                }

            #endif

            // On windows debug if the behaviour set is null then throw an exception:

            #if WINDOWS_DEBUG

                if ( behaviour_set == null )
                {
                    throw new Exception("AIBehaviour must be part of a valid AIBeaviourSet!!!");
                }

            #endif

            // Save importance:

            m_importance = importance;

            // Save the control character:

            m_control_character = control_character;

            // Save behaviour set:

            m_behaviour_set = behaviour_set;

            // Save the name of the behaviour:

            m_name = GetType().Name;
        }

        //=========================================================================================
        /// <summary>
        /// Scores the current behaviour and returns how relevant the AI thinks this action is
        /// </summary>
        /// <returns> A score for this current behaviour. </returns>
        //=========================================================================================

        public virtual float ScoreBehaviour(){ return 0; }

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to peform its action if it is selected as the current AI behaviour.
        /// </summary>
        //=========================================================================================

        public virtual void PerformBehaviour(){}

        //=========================================================================================
        /// <summary>
        /// Allows the behaviour to do debug drawing.
        /// </summary>
        //=========================================================================================

        #if DEBUG

            public virtual void OnDebugDraw(){}

        #endif  // #if DEBUG

    }   // end of class

}   // end of namespace
