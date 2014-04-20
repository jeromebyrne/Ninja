using System;
using System.Collections.Generic;
using System.Xml;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    ///
    /// <summary> 
    /// 
    /// The grand-daddy of nearly all objects in the game. This abstract
    /// class represents all objects that can be created and modified through
    /// xml formatted data. 
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// All classes that want to be considered proper XML classes must fulfill
    /// the following requirements. Classes that fail to meet these requirements
    /// will not be creatable or modifiable through the XML system:
    /// 
    /// 1 - They must be provide a default constructor, so that they can be 
    /// created at all times- even with no data.
    /// 2 - They must implement ReadXML, even if it does nothing
    /// 3 - They must implement WriteXML, even if it does nothing
    /// </remarks>
    /// 
    //#############################################################################################

    public abstract class XmlObject
    {
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

        public virtual void ReadXml( XmlObjectData data ){;}

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

        public virtual void WriteXml( XmlObjectData data ){;}

    }   // end of class

}   // end of namespace
