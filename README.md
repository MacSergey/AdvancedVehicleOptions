# CS-AdvancedVehicleOptions 1.9.2

Change the Mod Option Panel
- Compatibility : If TLM is present and active, disable some vehicle parameters; can be turned off, default setting: ON
- Compatibility : If IPT is present and active, disable some vehicle parameters; can be turned off, default setting: ON

Note: Disabling the option will make the values for editing available, however, the Spawn Control for Bus, Trolley Bus, Metro, Tram and Monorail is still under control of the game, TLM or IPT. The vehicles must be configured in the respective transport line managers.

Lean Modifier and Nod Mulltiplier are not saved if zero or negative value. Now remembers and loads values properly (eg. for trains or helicopters).

Trolley Bus to show next to Bus and Intercity Bus

# CS-AdvancedVehicleOptions 1.9.1
This version is 1.9.1, tested in a limited environment only. Adds compatibility patches to the game (vehicle spawning), ITP/TLM (vehicle spawning, vehicle parameters), Vehicle Color Expander (coloring parameters).

Adds compatibility patches to the game (vehicle spawning), ITP/TLM (vehicle spawning, vehicle parameters), Vehicle Color Expander (coloring parameters).

Adds compatibility for game controlled vehicle spawning (Hide Spawn option for game controlled vehicles (eg bus); if vehicle spawning is controlled by game, the vehicle will always be turned on (even if it was disabled before)

New Options are visible in the Mod Option panel of Cities Skylines.
 - Game Balancing : Hide capacity value for vehicles without passenger/cargo capacity; can be turned off, default setting: ON
 - Compatibility : If Color Vehicle Expander is present and active, disable Coloring; cannot be turned off, default setting: ON
 - Compatibility : If TLM is present and active, disable some vehicle parameters; cannot be turned off, default setting: ON
 - Compatibility : If IPT is present and active, disable some vehicle parameters; cannot be turned off, default setting: ON

New Category for Intercity Bus

Support for categorizing properly Bus trailers. SteamID config file 09-Apr-2020 for 1.9.1.

Hiding GUI works again

Added Mod Option Panel links to Wiki and Log directory 

# CS-AdvancedVehicleOptions 1.9.0

This version is 1.9.0, tested in a limited environment only. Last changes include a new logo (yeah) und the change of the namespace for the mod. This should stop annyoing messages when user are still subscribed to the original one. 

- Issue on load: Still error messages when opening an existing savegame (one time only due to exception on loading values)
- Added new vehicle categories for new DLC Sunset Harbor
- Compatibility towards More Vehicles, no more cargo train issues

# CS-AdvancedVehicleOptions

This version is 1.8.7, incorporating airatin's updates and additional changes intended to make this mod compatible with More Vechicles by @dymanoid, most importantly using ```Array16<T>::m_buffer.Length``` instead of ```Array16<T>::m_size``` to iterate over the filled length of ```VehicleManager.instance.m_vehicles``` and ```VehicleManager.instance.m_parkedVehicles``` to circumvent the use of preset vehicle limits.

# Testing

I've seen a normal frequency of functional cargo trains in multiple cities of different sizes while using this version locally with More Vehicles and many other mods (to be updated), but please test further before incorporating into the workshop version.
