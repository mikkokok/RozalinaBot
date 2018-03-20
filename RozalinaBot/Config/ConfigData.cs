using System.Collections.Generic;
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
        [XmlArray("OumanRegisteredUsers"), XmlArrayItem(typeof(User))]
        public List<User> OumanRegisteredUsers;

    }
    [XmlRoot("ConfigData")]
    public class User
    {
        [XmlElement]
        public int Id;
        [XmlElement]
        public string Username;
    }

}
