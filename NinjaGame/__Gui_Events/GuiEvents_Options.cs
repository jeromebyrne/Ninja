using System;
using System.Collections.Generic;
using System.Text;

//#################################################################################################
//#################################################################################################

namespace NinjaGame
{
    //#############################################################################################
    /// <summary>
    /// Collection of events for the options menu.
    /// </summary>
    //#############################################################################################

    public class GuiEvents_Options
    {
        //=========================================================================================
        /// <summary>
        /// Gui event. Sets slider values for the options menu.
        /// </summary>
        /// <param name="widget"> Widget that the event is happening on. </param>
        //=========================================================================================

        public static void Event_Set_Options_Sliders( GuiWidget widget )
        { 
            // Set all settings:

            try
            {
                // Try to find this object:

                Gui_Slider s = (Gui_Slider) Core.Gui.Search.FindByName("Slider_Brightness");

                // Set it's value:

                if ( s != null ) s.CurrentValue = CorePreferences.Brightness;
            }
            catch ( Exception ){}

            try
            {
                // Try to find this object:

                Gui_Slider s = (Gui_Slider) Core.Gui.Search.FindByName("Slider_Music_Volume");

                // Set it's value:

                if ( s != null ) s.CurrentValue = CorePreferences.MusicVolume;
            }
            catch ( Exception ){}

            try
            {
                // Try to find this object:

                Gui_Slider s = (Gui_Slider) Core.Gui.Search.FindByName("Slider_Sound_Volume");

                // Set it's value:

                if ( s != null ) s.CurrentValue = CorePreferences.SoundVolume;
            }
            catch ( Exception ){}
        }

        //=========================================================================================
        /// <summary>
        /// Gui event. Happens when sound volume slider is moved.
        /// </summary>
        /// <param name="widget"> Widget that the event is happening on. </param>
        //=========================================================================================

        public static void Event_Sound_Volume_Changed( GuiWidget widget )
        { 
            // See if this is a slider:

            if ( widget != null )
            {
                try
                {
                    // Cast to a slider:

                    Gui_Slider s = (Gui_Slider) widget;

                    // Set sound volume:

                    CorePreferences.SoundVolume = s.CurrentValue;
                }
                catch ( Exception ){}
            }
        }

        //=========================================================================================
        /// <summary>
        /// Gui event. Happens when music volume slider is moved.
        /// </summary>
        /// <param name="widget"> Widget that the event is happening on. </param>
        //=========================================================================================

        public static void Event_Music_Volume_Changed( GuiWidget widget )
        { 
            // See if this is a slider:

            if ( widget != null )
            {
                try
                {
                    // Cast to a slider:

                    Gui_Slider s = (Gui_Slider) widget;

                    // Set music volume:

                    CorePreferences.MusicVolume = s.CurrentValue;
                }
                catch ( Exception ){}
            }
        }

        //=========================================================================================
        /// <summary>
        /// Gui event. Happens when brightness slider is moved.
        /// </summary>
        /// <param name="widget"> Widget that the event is happening on. </param>
        //=========================================================================================

        public static void Event_Brightness_Changed( GuiWidget widget )
        { 
            // See if this is a slider:

            if ( widget != null )
            {
                try
                {
                    // Cast to a slider:

                    Gui_Slider s = (Gui_Slider) widget;

                    // Set brightness:

                    CorePreferences.Brightness = s.CurrentValue;
                }
                catch ( Exception ){}
            }
        }

    }   // end of namespace

}   // end of class 
