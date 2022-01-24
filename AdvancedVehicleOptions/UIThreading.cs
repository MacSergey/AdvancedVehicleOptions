﻿using UnityEngine;
using ICities;
using ColossalFramework;


namespace AdvancedVehicleOptionsUID
{
    /// <summary>
    /// Threading to capture hotkeys.
    /// </summary>
    public class UIThreading : ThreadingExtensionBase
    {
        // Instance reference.
        private static UIThreading instance;

        // Flags.
        private bool operating = false;
        private bool processed = false;

        // Hotkey local reference.
        private readonly SavedInputKey savedKey;

        /// <summary>
        /// Constructor - sets instance reference.
        /// </summary>
        public UIThreading()
        {
            // Set instance reference.
            instance = this;

            // Set hotkey reference.
            savedKey = ModSettings.ToggleSavedKey;
        }


        /// <summary>
        /// Activates/deactivates hotkey.
        /// </summary>
        internal static bool Operating
        {
            set
            {
                if (instance != null)
                {
                    instance.operating = value;
                }
            }
        }


        /// <summary>
        /// Look for keypress to activate tool.
        /// </summary>
        /// <param name="realTimeDelta"></param>
        /// <param name="simulationTimeDelta"></param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Don't do anything if not active.
            if (operating)
            {
                // Has hotkey been pressed?
               if (savedKey.Key != KeyCode.None && Input.GetKey(savedKey.Key))
               {
                    // Check modifier keys according to settings.
                    bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.AltGr);
                    bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                    // Modifiers have to *exactly match* settings, e.g. "alt-E" should not trigger on "ctrl-alt-E".
                    bool altOkay = altPressed == savedKey.Alt;
                    bool ctrlOkay = ctrlPressed == savedKey.Control;
                    bool shiftOkay = shiftPressed == savedKey.Shift;

                    // Process keystroke.
                    if (altOkay && ctrlOkay && shiftOkay)
                    {
                        // Only process if we're not already doing so.
                        if (!processed)
                        {
                            // Set processed flag.
                            processed = true;

                            // Toggle AVO Mainpanel Visibility

                            if (!AdvancedVehicleOptions.m_mainPanel.isVisible)
                            {
                                AdvancedVehicleOptions.m_mainPanel.isVisible = true;
                                AdvancedVehicleOptions.m_mainPanel.m_fastList.DisplayAt(AdvancedVehicleOptions.m_mainPanel.m_fastList.listPosition);
                                AdvancedVehicleOptions.m_mainPanel.m_optionPanel.Show(AdvancedVehicleOptions.m_mainPanel.m_fastList.rowsData[AdvancedVehicleOptions.m_mainPanel.m_fastList.selectedIndex] as VehicleOptions);
                                AdvancedVehicleOptions.m_mainPanel.m_followVehicle.isVisible = AdvancedVehicleOptions.m_mainPanel.m_preview.parent.isVisible = true;
                                AdvancedVehicleOptions.UpdateOptionPanelInfo();
                            }
                            else AdvancedVehicleOptions.m_mainPanel.isVisible = false;
                        }
                    }
                    else
                    {
                        // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                        processed = false;
                    }
                }
                else
                {
                    // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                    processed = false;
                }
            }
        }
    }
}