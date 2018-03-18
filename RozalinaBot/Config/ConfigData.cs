using System.Xml.Serialization;

namespace RozalinaBot.Config
{
    public class ConfigData
    {
        [XmlElement]
        public string TelegramToken;
        [XmlElement]
        public string OumanAddress;
        [XmlElement]
        public string OumanThumbPrint;
        [XmlElement]
        public string OumanUser;
        [XmlElement]
        public string OumanPassword;
    }
}
