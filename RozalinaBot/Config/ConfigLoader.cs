using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RozalinaBot.Config
{
    internal class ConfigLoader
    {
        private const string Config = "app.config";
        public ConfigData LoadedConfig;

        public void LoadConfig()
        {
            if (!File.Exists(Config))
                return;

            using (var fileStream = new FileStream(Config, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(ConfigData));
                LoadedConfig = (ConfigData) xmlSerializer.Deserialize(fileStream);
            }
        }
    }
}
