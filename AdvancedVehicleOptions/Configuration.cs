﻿using ColossalFramework.IO;

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace AdvancedVehicleOptionsUID
{
    [XmlType("ArrayOfVehicleOptions")]
    [Serializable]
    public class Configuration : IDataContainer
    {
        public class VehicleData
        {
            public bool oldversion = true;
            #region serialized
            [XmlAttribute("name")]
            public string name;
            [DefaultValue(true)]
            public bool enabled = true;
            [DefaultValue(false)]
            public bool addBackEngine = false;
            public float maxSpeed;
            public float acceleration;
            public float braking;
            public float turning;
            public float springs;
            public float dampers;
            public float leanMultiplier;
            public float nodMultiplier;
            [DefaultValue(true)]
            public bool useColorVariations = true;
            public HexaColor color0;
            public HexaColor color1;
            public HexaColor color2;
            public HexaColor color3;
            [DefaultValue(-1)]
            public int capacity = -1;
            [DefaultValue(-1)]
            public int specialcapacity = -1;
            [DefaultValue(false)]
            public bool isLargeVehicle = false;
            public string classname;
            [DefaultValue(true)]
            #endregion

            public bool isCustomAsset
            {
                get
                {
                    return name.Contains(".");
                }
            }
        }

        [XmlElement("VehicleOptions")]
        public VehicleData[] data;

        [XmlIgnore]
        public VehicleOptions[] options;

        private List<VehicleData> m_defaultVehicles = new List<VehicleData>();

        // Serialize to save
        public void Serialize(DataSerializer s)
        {
            try
            {
                int count = options.Length;
                s.WriteInt32(count);

                for (int i = 0; i < count; i++)
                {
                    s.WriteUniqueString(options[i].name);
                    s.WriteBool(options[i].enabled);
                    s.WriteBool(options[i].addBackEngine);
                    s.WriteFloat(options[i].maxSpeed);
                    s.WriteFloat(options[i].acceleration);
                    s.WriteFloat(options[i].braking);
                    s.WriteFloat(options[i].turning);
                    s.WriteFloat(options[i].springs);
                    s.WriteFloat(options[i].dampers);
                    s.WriteFloat(options[i].leanMultiplier);
                    s.WriteFloat(options[i].nodMultiplier);
                    s.WriteBool(options[i].useColorVariations);
                    s.WriteUniqueString(options[i].color0.Value);
                    s.WriteUniqueString(options[i].color1.Value);
                    s.WriteUniqueString(options[i].color2.Value);
                    s.WriteUniqueString(options[i].color3.Value);
                    s.WriteInt32(options[i].capacity);
                    s.WriteInt32(options[i].specialcapacity);
                    s.WriteBool(options[i].isLargeVehicle);
                    s.WriteUniqueString(options[i].classname);
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e);
            }
        }

        public void Deserialize(DataSerializer s)
        {
            try
            {
                options = null;
                data = null;

                int count = s.ReadInt32();
                data = new VehicleData[count];

                Logging.Message("AVO Savegame Version " + s.version);

                for (int i = 0; i < count; i++)
                {
                    data[i] = new VehicleData();
                    data[i].name = s.ReadUniqueString();
                    data[i].enabled = s.ReadBool();
                    data[i].addBackEngine = s.ReadBool();
                    data[i].maxSpeed = s.ReadFloat();
                    data[i].acceleration = s.ReadFloat();
                    data[i].braking = s.ReadFloat();
                    
                    if (s.version >= 2)                                          // Skip loading new vehicle propertiers for all versions below 1.9.0
                    {
                        data[i].turning = s.ReadFloat();
                        data[i].springs = s.ReadFloat();
                        data[i].dampers = s.ReadFloat();
                        data[i].leanMultiplier = s.ReadFloat();
                        data[i].nodMultiplier = s.ReadFloat();
                    }

                    data[i].useColorVariations = s.ReadBool();
                    data[i].color0 = new HexaColor(s.ReadUniqueString());
                    data[i].color1 = new HexaColor(s.ReadUniqueString());
                    data[i].color2 = new HexaColor(s.ReadUniqueString());
                    data[i].color3 = new HexaColor(s.ReadUniqueString());
                    data[i].capacity = s.ReadInt32();

                    if (s.version >= 3)                                         // Skip loading Special Capacity for all versions below 1.9.3
                    {
                        data[i].specialcapacity = s.ReadInt32();             
                        data[i].isLargeVehicle = s.ReadBool();
                    }

                    if (s.version >= 4)                                         // Skip loading Special Capacity for all versions below 1.9.8
                    {
                        data[i].classname = s.ReadUniqueString();
                    }
                }
            }
            catch (Exception e)
            {
                // Couldn't Deserialize
                Logging.Error("Couldn't deserialize");
                Logging.LogException(e);
            }
        }

        public void AfterDeserialize(DataSerializer s)
        {
        }

        // Serialize to file
        public void Serialize(string filename)
        {
            try
            {
                if (AdvancedVehicleOptions.isGameLoaded) OptionsToData();

                // Add back default vehicle options that might not exist on the map
                // I.E. Snowplow on non-snowy maps
                if (m_defaultVehicles.Count > 0)
                {
                    List<VehicleData> new_data = new List<VehicleData>(data);

                    for (int i = 0; i < m_defaultVehicles.Count; i++)
                    {
                        bool found = false;
                        for (int j = 0; j < data.Length; j++)
                        {
                            if (m_defaultVehicles[i].name == data[j].name)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            new_data.Add(m_defaultVehicles[i]);
                        }
                    }

                    data = new_data.ToArray();
                }

                using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    stream.SetLength(0); // Emptying the file !!!
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
                    xmlSerializer.Serialize(stream, this);
                    Logging.Message("XML Stream Vehicle Configuration successfully saved");
                }
            }
            catch (Exception e)
            {
                Logging.Error("Couldn't save configuration at \"" + Directory.GetCurrentDirectory() + "\"");
                Logging.LogException(e);
            }
        }

        public void Deserialize(string filename)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            Configuration config = null;

            options = null;
            data = null;

            try
            {
                // Trying to Deserialize the configuration file
                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {
                    config = xmlSerializer.Deserialize(stream) as Configuration;
                }
            }
            catch (Exception e)
            {
                // Couldn't Deserialize (XML malformed?)
                Logging.Error("Couldn't load configuration (XML malformed?)");
                Logging.LogException(e);

                config = null;
            }

            if(config != null)
            {
                data = config.data;

                if(data != null)
                {
                    // Saves all default vehicle options that might not exist on the map
                    // I.E. Snowplow on non-snowy maps
                    m_defaultVehicles.Clear();
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != null && !data[i].isCustomAsset)
                            m_defaultVehicles.Add(data[i]);
                    }
                }


                if (AdvancedVehicleOptions.isGameLoaded) DataToOptions();
            }
        }

        public void OptionsToData()
        {
            if (options == null) return;

            data = new VehicleData[options.Length];

            for (int i = 0; i < options.Length; i++)
            {
                data[i] = new VehicleData();
                data[i].name = options[i].name;
                data[i].enabled = options[i].enabled;
                data[i].addBackEngine = options[i].addBackEngine;
                data[i].maxSpeed = options[i].maxSpeed;
                data[i].acceleration = options[i].acceleration;
                data[i].braking = options[i].braking;
                data[i].turning = options[i].turning;
                data[i].springs = options[i].springs;
                data[i].dampers = options[i].dampers;
                data[i].leanMultiplier = options[i].leanMultiplier;
                data[i].nodMultiplier = options[i].nodMultiplier;
                data[i].useColorVariations = options[i].useColorVariations;
                data[i].color0 = options[i].color0;
                data[i].color1 = options[i].color1;
                data[i].color2 = options[i].color2;
                data[i].color3 = options[i].color3;
                data[i].capacity = options[i].capacity;
                data[i].specialcapacity = options[i].specialcapacity;
                data[i].isLargeVehicle = options[i].isLargeVehicle;
                data[i].classname = options[i].classname;
            }
        }

        public void DataToOptions()
        {
        if (data == null) return;

            options = new VehicleOptions[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].name == null) continue;

                options[i] = new VehicleOptions();
                options[i].name = data[i].name;
                options[i].enabled = data[i].enabled;
                options[i].addBackEngine = data[i].addBackEngine;
                options[i].maxSpeed = data[i].maxSpeed;
                options[i].acceleration = data[i].acceleration;
                options[i].braking = data[i].braking;
                options[i].turning = data[i].turning;
                options[i].springs = data[i].springs;
                options[i].dampers = data[i].dampers;
                options[i].leanMultiplier = data[i].leanMultiplier;
                options[i].nodMultiplier = data[i].nodMultiplier;
                options[i].useColorVariations = data[i].useColorVariations;
                options[i].color0 = data[i].color0;
                options[i].color1 = data[i].color1;
                options[i].color2 = data[i].color2;
                options[i].color3 = data[i].color3;
                options[i].capacity = data[i].capacity;
                options[i].specialcapacity = data[i].specialcapacity;
                options[i].isLargeVehicle = data[i].isLargeVehicle;

                if (options[i].prefab != null)
                {
                    if (AdvancedVehicleOptions.hasAirportDLC)
                    {
                        if ((data[i].classname != null) && (data[i].classname == "Airplane Vehicle Small" || data[i].classname == "Airplane Vehicle" || data[i].classname == "Airplane Vehicle Large"))
                        {
                            if (options[i].prefab.m_class != ItemClassCollection.FindClass(data[i].classname))
                                { 
                                 ItemClass oldClass = options[i].prefab.m_class;
                                 options[i].prefab.m_class = ItemClassCollection.FindClass(data[i].classname);
                                 Logging.Message("Itemclass Change successful: " + options[i].prefab.name + " / " + oldClass + " -> " + options[i].prefab.m_class.name);
                                }
                        }
                    }
                }
            }
                
            VehicleOptions.UpdateTransfertVehicles();
        }
    }
}
