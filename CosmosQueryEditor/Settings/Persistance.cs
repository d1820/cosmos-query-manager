using System;
using System.IO;

namespace CosmosQueryEditor.Settings
{
    public class Persistance
    {
        private const string CONFIGURATION_FILE_NAME = "cosmosQueryEditor-settings-auto.cqe";

        public static readonly string ConfigurationFilePath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIGURATION_FILE_NAME);

        public static readonly string QueryFileFolder =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cosmosQueryEditor-docs");

        public bool TryLoadRaw(out string rawData)
        {
            if (File.Exists(ConfigurationFilePath))
            {
                try
                {
                    rawData = File.ReadAllText(ConfigurationFilePath);
                    return true;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            rawData = null;
            return false;
        }

        public bool TrySaveRaw(string rawData)
        {
            try
            {
                File.WriteAllText(ConfigurationFilePath, rawData);
                return true;
            }
            catch (Exception) { }

            return false;
        }
    }
}