using System;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Class that implements a heap. Can be used for priority queue implementation. 
    /// Heap elements are sorted using the greater than operator, with the highest element being 
    /// at the top of the heap. Internally implemented using arrays to represent a flattened 
    /// tree structure.
    /// </summary>
    //#############################################################################################

    public class Heap<TValue>
    {
        //=========================================================================================
        // Attributes
        //=========================================================================================
            
            /// <summary> The maximum number of elements that can be placed in the heap. </summary>

            public int Capacity { get { return m_values.Length; } }

            /// <summary> The number of elements currently in the heap. </summary>

            public int Count { get { return m_num_elements; } }
            
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            /// <summary> 
            /// The default value of elements in the heap. When an item is removed the arrays are 
            /// internally set to this value. Also when a remove operation is performed and nothing 
            /// is there to remove, this value is returned.
            /// </summary>
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public TValue DefaultValue { get { return m_default_value; } }

        //=========================================================================================
        // Variables
        //=========================================================================================

            /// <summary> Array representing the heap and all it's keys. </summary>

            private int[] m_sort_keys = null;

            /// <summary> Array representing the heap and all it's values. This is a complete tree flattened to an array. </summary>

            private TValue[] m_values = null;

            /// <summary> Number of elements currently in the heap. This is a complete tree flattened to an array. </summary>

            private int m_num_elements = 0;

            /// <summary> Value a heap entry is set to when it is removed.</summary>

            private TValue m_default_value;

        //=========================================================================================
        /// <summary>
        /// Constructor creates a heap.
        /// </summary>
        /// <param name="max_depth"> 
        /// Maximum depth of the heap. Determines the number of elements that can be added to the 
        /// heap. for example a depth of 4 allows for 15 elements. A depth of of 5 allows for 31 
        /// elements and so on.. Maximum allowed depth is 24. Minimum depth is 8. 
        /// </param>
        /// <param name="default_value"> 
        /// Default value to clear all heap entries with. The heap uses arrays internally so if 
        /// you are storing objects you should use 'null' as a default value so that object 
        /// references will be removed when an item is taken off the heap. This value is also 
        /// returned if a remove operation is performed when there is nothing left in the heap.
        /// </param>
        //=========================================================================================

        public Heap( int max_depth , TValue default_value )
        {
            // Make sure depth is in range

            if ( max_depth < 8  ) max_depth = 8;
            if ( max_depth > 24 ) max_depth = 24;

            // Calculate the size of the heap arrays from the maximum depth:

            int capacity = ( 1 << max_depth ) - 1;

            // Create the heap arrays:

            m_sort_keys = new int[ capacity ]; m_values = new TValue[ capacity ];

            // Save the default heap value:

            m_default_value = default_value;

            // Clear the heap array to the default heap value:

            for ( int i = 0 ; i < m_sort_keys.Length ; i++ )
            {
                m_values[i] = m_default_value;
            }
        }

        //=========================================================================================
        /// <summary> Adds an element to the heap. </summary>
        /// <param name="sort_key"> 
        /// Sort key for the element that determines it's position in the heap.
        /// </param>
        /// <param name="value"> 
        /// Value of the element.
        /// </param>
        //=========================================================================================

        public void Add( int sort_key , TValue value )
        {
            // If the number of elements has reached the max then abort:

            if ( m_num_elements >= m_values.Length ) return;

            // Add the element as the deepest rightmost node:

            m_values[m_num_elements] = value; m_sort_keys[m_num_elements] = sort_key;

            // Upheap the node to maintain heap order:

            UpHeap(m_num_elements);

            // Increase number of elements:

            m_num_elements++;
        }

        //=========================================================================================
        /// <summary> 
        /// Removes the top element from the heap. Returns the default value on failure. 
        /// </summary>
        /// <returns> Element that is at the top of the heap. </returns>
        //=========================================================================================

        public TValue Remove()
        {
            // If the heap is empty then return the default value:

            if ( m_num_elements == 0 ) return m_default_value;

            // Save the value to be removed:

            TValue remove_value = m_values[0];

            // Swap the last value into it's place:

            m_values[0] = m_values[ m_num_elements - 1 ]; m_sort_keys[0] = m_sort_keys[ m_num_elements - 1 ];

            // Set the last value to the default value:

            m_values[ m_num_elements - 1 ] = m_default_value;

            // Peform down heap sorting for the last value that was swapped to the top of the heap:

            DownHeap();

            // Decrement the number of elements

            m_num_elements--;

            // Return the value at the top of the heap:

            return remove_value;
        }

        //=========================================================================================
        /// <summary> Clears the heap and removes all values from it. </summary>
        //=========================================================================================

        public void Clear()
        {
            // Run through all elements and set their default value:

            for ( int i = 0 ; i < m_num_elements ; i++ )
            {
                m_values[i] = m_default_value;
            }

            // Set the number of elements back to zero

            m_num_elements = 0;
        }

        //=========================================================================================
        /// <summary> 
        /// Performs down heap sorting for the item at the top of the heap. This is called after 
        /// an item is removed from the heap and the last element is put to the top. This function 
        /// restores the heap order property.
        /// </summary>
        //=========================================================================================

        private void DownHeap()
        {
            // This is the current index of the node in the heap:

            int index = 0;

            // Keep going till the bottom of the heap has been reached or order has been restored:

            while ( true )
            {
                // Calculate the index where the two child nodes are:

                int children_index = ( ( index + 1 ) << 1 ) - 1;

                // If this is past the end of the heap then we are done:

                if ( children_index >= m_num_elements )
                {
                    // No child nodes under this: abort

                    break;
                }
                else if ( children_index == m_num_elements - 1 )
                {
                    // Only a left child to worry about: see if we are less than it

                    if ( m_sort_keys[index] < m_sort_keys[children_index] )
                    {
                        // Need to swap the two keys:

                        int t1 = m_sort_keys[index]; TValue t2 = m_values[index];

                        // Do the swap:

                        m_sort_keys[index]          = m_sort_keys[children_index]; 
                        m_values[index]             = m_values[children_index];
                        m_sort_keys[children_index] = t1;
                        m_values[children_index]    = t2;

                        // Save the new index of the node we are down heaping

                        index = children_index;
                    }
                    else
                    {
                        // Not less - heap is in order. Abort.

                        break;
                    }
                }
                else
                {
                    // Both left and right children: get sort key values of both

                    int l_key = m_sort_keys[ children_index     ];
                    int r_key = m_sort_keys[ children_index + 1 ];

                    // Get the largest child key:

                    int largest_i = children_index; if ( l_key < r_key ) largest_i++;

                    // See if this item is smaller than the largest child key - if so then swap:

                    if ( m_sort_keys[index] < m_sort_keys[largest_i] )
                    {
                        // Need to swap the two keys:

                        int t1 = m_sort_keys[index]; TValue t2 = m_values[index];

                        // Do the swap:

                        m_sort_keys[index]          = m_sort_keys[largest_i]; 
                        m_values[index]             = m_values[largest_i];
                        m_sort_keys[largest_i]      = t1;
                        m_values[largest_i]         = t2;

                        // Save the new index of the node we are down heaping

                        index = largest_i;
                    }
                    else
                    {
                        // Not less- heap is in order. Abort.

                        break;
                    }
                }

            }   // end while heap is unordered
        }

        //=========================================================================================
        /// <summary> 
        /// Peforms upheaping for the element at the specified index in the array, restoring the 
        /// heap order property when an item is added to the heap.
        /// </summary>
        /// <param name="index"> index of the element to upheap</param>
        //=========================================================================================

        private void UpHeap( int index )
        {
            // Keep going until we reach the top of the heap:

            while ( index > 0 )
            {
                // Get the index of the parent node:

                int parent_index = ( ( index + 1 ) >> 1 ) - 1;

                // See if we are bigger than the parent node:

                if ( m_sort_keys[index] > m_sort_keys[parent_index] )
                {
                    // Bigger: do a swap

                    int t1 = m_sort_keys[index]; TValue t2 = m_values[index];

                    m_sort_keys[index]          = m_sort_keys[parent_index];
                    m_sort_keys[parent_index]   = t1;
                    m_values[index]             = m_values[parent_index];
                    m_values[parent_index]      = t2;

                    // Set the index of the node as the parent index:

                    index = parent_index;
                }
                else
                {
                    // Not bigger: abort upheaping

                    break;
                }
            }
        }

    }   // end of class

}   // end of namespace
