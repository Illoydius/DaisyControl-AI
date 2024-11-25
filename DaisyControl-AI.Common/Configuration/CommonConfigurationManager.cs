using DaisyControl_AI.Common.Serialization;

namespace DaisyControl_AI.Common.Configuration
{
    public static class CommonConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string CONFIG_FILE_NAME = "config.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static DaisyControlConfig fDaisyControlConfig;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static DaisyControlConfig GetConfigFromMemory()
        {
            return fDaisyControlConfig ?? ReloadConfig();
        }

        public static DaisyControlConfig ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(CONFIG_FILE_NAME);
            fDaisyControlConfig = JsonCommonSerializer.DeserializeFromString<DaisyControlConfig>(_ConfigFileContent);
            return fDaisyControlConfig;
        }

        public static void SetConfigInMemory(DaisyControlConfig aDaisyControlConfig)
        {
            fDaisyControlConfig = aDaisyControlConfig;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fDaisyControlConfig == null)
                fDaisyControlConfig = new DaisyControlConfig();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fDaisyControlConfig);
            File.WriteAllText(CONFIG_FILE_NAME, _SerializedConfig);
        }
    }
}
