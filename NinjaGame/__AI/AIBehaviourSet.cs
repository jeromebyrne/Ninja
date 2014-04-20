using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that manages a set of a AIBehaviours. Controls which behaviour is currently 
    /// selected and manages the transition between behaviours.
    /// </summary>
    //#############################################################################################

    public class AIBehaviourSet
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================

            /// <summary> Character the behaviour set is for. </summary>
        
            public Character ControlCharacter { get { return m_character; } }

            /// <summary> Current behaviour in use by the behaviour set. </summary>

            public AIBehaviour CurrentBehaviour { get { return m_current_behaviour; } }

            /// <summary> Amount of time the current behaviour has been held for. </summary>

            public float CurrentBehaviourTime { get { return m_current_behaviour_time; } } 

            /// <summary> How much to smooth the scores of AI behaviors over time, 0-1. 0 means no smoothing. </summary>

            public float BehaviourSmooth { get { return m_behaviour_smooth; } set { m_behaviour_smooth = MathHelper.Clamp(value,0,1); } }

            /// <summary> Minimum amount of time in seconds that a behaviour must be held for. Default is 0. </summary>

            public float BehaviourHoldTime { get { return m_behaviour_hold_time; } set { m_behaviour_hold_time = MathHelper.Clamp(value,0,1000000.0f); } }

        //=========================================================================================
        // Variables
        //=========================================================================================
            
            /// <summary> A hash containing all of the behaviours for the behaviour set. </summary>

            private Dictionary<string,AIBehaviour> m_behaviours = new Dictionary<string,AIBehaviour>();

            /// <summary> A hash containing the current score for all behaviours. This score may be smoothed. </summary>
        
            private Dictionary<string,float> m_behaviour_scores = new Dictionary<string,float>();

            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// A list containing the behaviours to be processed by the ai when processing. This exists only because 
            /// a behaviour could be removed whilst we are enumerating the normal behaviours dictionary. That would 
            /// cause an exception.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            private List<AIBehaviour> m_behaviours_to_process = new List<AIBehaviour>();

            /// <summary> Character the behaviour set is for. </summary>
        
            private Character m_character = null;

            /// <summary> Current behaviour in use by the behaviour set. </summary>

            private AIBehaviour m_current_behaviour = null;

            /// <summary> Amount of time the current behaviour has been held for. </summary>

            private float m_current_behaviour_time = 0;

            /// <summary> How much to smooth the scores of AI behaviors over time, 0-1. 0 means no smoothing. </summary>

            private float m_behaviour_smooth = 0;

            /// <summary> Minimum amount of time in seconds that a behaviour must be held for. Default is 0. </summary>

            private float m_behaviour_hold_time = 0;

        //=========================================================================================
        /// <summary>
        /// Creates the AI behaviour set. Must be given the character to which this behaviour 
        /// set belongs.
        /// </summary>
        /// <param name="control_character"> Character the AI behaviour is for. </param>
        //=========================================================================================

        public AIBehaviourSet( Character control_character )
        {
            // On windows debug if the control character is null then throw an exception:

            #if WINDOWS_DEBUG

                if ( control_character == null )
                {
                    throw new Exception("AIBehaviourSet must have a valid control character !!!");
                }

            #endif

            // Save the control character:

            m_character = control_character;
        }

        //=========================================================================================
        /// <summary>
        /// Adds a behaviour to the behaviour set. The behaviour must be a valid AIBehaviour class 
        /// name and the name given is case sensitive. 
        /// </summary>
        /// <param name="behaviour_class_name"> Name of the behaviour to add - E.G "AI_Rest"</param>
        /// <param name="importance"> 
        /// How important the behaviour is. How much to scale behaviour scores by.
        /// </param>
        //=========================================================================================

        public void AddBehaviour( string behaviour_class_name , float importance )
        {
            // Try and get the class type:

            try
            {
                // Try and get the type:

                Type behaviour_type = Type.GetType( "NinjaGame." + behaviour_class_name);

                // If it couldn't be found then throw an exception:

                if ( behaviour_type == null ) throw new ArgumentException("Invalid behaviour class name given");

                // Get the root behaviour class

                Type root_behaviour_type = Type.GetType("NinjaGame.AIBehaviour");

                // Make sure it is not abstract:

                if ( behaviour_type.IsAbstract ) throw new Exception("Behaviour class for AI cannot be abstract!");

                // Make sure this is of type AIBehaviour

                if ( behaviour_type.IsSubclassOf(root_behaviour_type) == false ) 
                {
                    throw new Exception("AI Behaviour given is not of type 'AIBehaviour'");
                }

                // Get the constructor for the object:

                ConstructorInfo constructor = behaviour_type.GetConstructor
                (
                    new Type[]
                    {
                        Type.GetType("System.Single")               ,
                        Type.GetType("NinjaGame.Character")         ,
                        Type.GetType("NinjaGame.AIBehaviourSet")   
                    }
                );

                // Good: now create the object

                AIBehaviour behaviour = (AIBehaviour) constructor.Invoke( new object[]{ importance , m_character , this } );

                // Add it to the list of behaviours:

                m_behaviours[behaviour_type.Name] = behaviour;
                
                // Set current behaviour score to zero:

                m_behaviour_scores[behaviour_type.Name] = 0;

                // If there is no current behaviour then make this the current behaviour

                if ( m_current_behaviour == null ) m_current_behaviour = behaviour;
            }
            
            #if WINDOWS_DEBUG

                catch ( Exception e ){ DebugConsole.PrintException(e); }

            #else

                catch ( Exception ){}

            #endif
        }

        //=========================================================================================
        /// <summary>
        /// Removes a behaviour from the behaviour set.
        /// </summary>
        /// <param name="behaviour_class_name"> Name of the behaviour to remove. </param>
        //=========================================================================================

        public void RemoveBehaviour( string behaviour_class_name )
        {
            // See if this behaviour exists in the behaviour set:

            if ( m_behaviours.ContainsKey(behaviour_class_name) )
            {
                // Good: now remove it from the behaviours

                m_behaviours.Remove(behaviour_class_name);
                m_behaviour_scores.Remove(behaviour_class_name);

                // If this behaviour is the current behaviour then pick another one:

                if ( m_current_behaviour.GetType().Name == behaviour_class_name )
                {
                    if ( m_behaviours.Count > 0 )
                    {
                        // Get enumerator into behaviours:

                        Dictionary<string,AIBehaviour>.Enumerator e = m_behaviours.GetEnumerator();

                        // Move onto next one:

                        e.MoveNext();

                        // Set new current behaviour

                        m_current_behaviour = e.Current.Value;
                    }
                    else
                    {
                        // No behaviours left:

                        m_current_behaviour = null;
                    }
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Retrieves a specified behaviour from the behaviour set.
        /// </summary>
        /// <param name="behaviour_class_name"> Class name of the behaviour to retrieve. </param>
        /// <returns> The behaviour, or null on failure. </returns>
        //=========================================================================================

        public AIBehaviour GetBehaviour( string behaviour_class_name )
        {
            // See if the behaviour exists:

            if ( m_behaviours.ContainsKey(behaviour_class_name) )
            {
                return m_behaviours[behaviour_class_name];
            }
            else
            {
                return null;
            }
        }

        //=========================================================================================
        /// <summary>
        /// Updates the scores for all behaviours, and selects a new behaviour if neccessary. 
        /// Also runs the currently selected behaviour.
        /// </summary>
        //=========================================================================================

        public void RunBehaviours()
        {
            // If there are no behaviours then abort:

            if ( m_behaviours.Count <= 0 ) return;

            // Add to the list of all the behaviours being processed: 

            {
                // Get enumerator into behaviours dictionary:

                Dictionary<string,AIBehaviour>.Enumerator e = m_behaviours.GetEnumerator();

                // Run through list:

                while ( e.MoveNext() )
                {
                    // Add this behaviour into list of behaviours to proccess:

                    m_behaviours_to_process.Add( e.Current.Value );
                }
            }

            // There should be a current behaviour at this point:

            #if WINDOWS_DEBUG

                if ( m_current_behaviour == null )
                {
                    throw new Exception( "AIBehaviourSet.RunBehaviours() - no current behaviour !!" );
                }

            #endif  

            // Process the list of behaviours, one by one:

            {
                // Save the behaviour with the highest score here:

                float highest_score = 0; AIBehaviour highest_score_behaviour = m_current_behaviour;

                // Get enumerator into list of behaviours to process:

                List<AIBehaviour>.Enumerator e = m_behaviours_to_process.GetEnumerator();

                // Run through the list:

                while ( e.MoveNext() )
                {
                    // Get this behaviour:

                    AIBehaviour behaviour = e.Current;

                    // Get it's current score:

                    float score = behaviour.ScoreBehaviour();

                    // Get the saved score for the behaviour:

                    float last_score = m_behaviour_scores[behaviour.Name];

                    // Caculate it's new score:

                    float new_score = ( last_score * m_behaviour_smooth ) + ( 1.0f - m_behaviour_smooth ) * score;

                    // Save the new score:

                    m_behaviour_scores[behaviour.Name] = new_score;

                    // Ok, see if the new score is higher than the current behaviour:

                    if ( new_score > highest_score )
                    {
                        // New higher ranked behaviour: save it

                        highest_score = new_score; highest_score_behaviour = behaviour;
                    }
                }

                // Now see can we switch behaviours:

                if ( m_current_behaviour_time >= m_behaviour_hold_time )
                {
                    // Can switch behaviours: do it 

                    if ( highest_score_behaviour != m_current_behaviour )
                    {
                        // Switch

                        m_current_behaviour = highest_score_behaviour;

                        // Reset time in current behaviour

                        m_current_behaviour_time = 0;
                    }
                }

                // Increase the time the current behaviour has been active for:

                m_current_behaviour_time += Core.Timing.ElapsedTime;

                // Finally.. peform the current behaviour action

                m_current_behaviour.PerformBehaviour();

                // Clear the list of behaviours:

                m_behaviours_to_process.Clear();
            }
        }

        //=========================================================================================
        /// <summary>
        /// Performs the next most important behaviour from the given behaviour. Allows the most 
        /// important behaviour to reliquinish total control and allows for combinations of 
        /// behaviours. 
        /// </summary>
        /// <param name="behaviour"> Behaviour to peform next most important behaviour for. </param>
        //=========================================================================================

        public void PerformNextMostImportantBehaviour( AIBehaviour behaviour )
        {
            // If behaviour is null then don't bother:

            if ( behaviour == null ) return;

            // Make sure this behaviour exists in the map:

            if ( m_behaviour_scores.ContainsKey(behaviour.Name) )
            {
                // Good get it's score:

                float behaviour_score = m_behaviour_scores[behaviour.Name];

                // Store the next lowest score here:

                float next_lowest_score = float.NegativeInfinity;

                // Store the behaviour with the next lowest score here:

                AIBehaviour next_lowest_behaviour = null;

                // Try and find the next lowest score:

                Dictionary<string,float>.Enumerator e = m_behaviour_scores.GetEnumerator();

                while ( e.MoveNext() )
                {
                    // Make sure this isn't the same behaviour as what we passed in:

                    if ( e.Current.Key == behaviour.Name ) continue;

                    // See if this score is lower or equal to the current behaviour and higher than the current lowest:

                    if ( e.Current.Value >= next_lowest_score && e.Current.Value <= behaviour_score )
                    {
                        // Better match for the next lowest score: save

                        next_lowest_score = e.Current.Value; next_lowest_behaviour = m_behaviours[e.Current.Key];
                    }
                }

                // See if we found a next lowest scored behaviour:

                if ( next_lowest_behaviour != null )
                {
                    // Do the next lowest behaviour

                   next_lowest_behaviour.PerformBehaviour();
                }
            }
        }

        //=========================================================================================
        /// <summary>
        /// Does debug drawing for the AI behavior set.
        /// </summary>
        //=========================================================================================

        #if DEBUG

            public void OnDebugDraw()
            {
                // Abort if no current behaviour

                if ( m_current_behaviour == null ) return;

                // Draw the name of the current behaviour on the control character:

                Core.DebugFont.DrawString
                (
                    m_current_behaviour.Name                                                , 
                    m_character.Position + Vector2.UnitY * 32                               ,
                    Core.Level.Renderer.Camera.View * Core.Level.Renderer.Camera.Projection
                );

                // Draw debug stuff for the current behaviour:

                m_current_behaviour.OnDebugDraw();
            }

        #endif  // #if DEBUG

    }   // end of class

}   // end of namepsace
