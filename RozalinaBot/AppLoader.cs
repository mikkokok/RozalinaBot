using System;
using System.Threading;
using RozalinaBot.Config;

namespace RozalinaBot
{
    internal class AppLoader
    {
        private static volatile object _locker = new object();
        private static AppLoader _instance;
        private bool _stopping;
        private static InfoDeployers.Telegram.RozalinaBot _rozalinaBot;
        private static ConfigLoader _configLoader;
        public static ConfigData LoadedConfig => _configLoader.LoadedConfig;

        public static AppLoader Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new AppLoader();
                    }
                }
                return _instance;
            }
        }
        public static void LoadConfig()
        {
            if (_configLoader == null)
                _configLoader = new ConfigLoader();
            _configLoader.LoadConfig();
        }
        public static void LoadTelegramBot()
        {
            if (_rozalinaBot == null)
                _rozalinaBot = new InfoDeployers.Telegram.RozalinaBot(_configLoader.LoadedConfig.TelegramToken, _configLoader.LoadedConfig.OumanAddress);
        }

        private AppLoader()
        {
            new Thread(RunLoop).Start();
        }
        private void RunLoop()
        {
            while (!_stopping)
            {
                //Console.WriteLine("I'm in the loop");
                Thread.Sleep(10000);
            }
        }

        public void Stop()
        {
            _stopping = true;
        }
    }
}
