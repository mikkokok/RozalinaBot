using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using RozalinaBot.Config;

namespace RozalinaBot
{
    class Program
    {

        static void Main(string[] args)
        {
            var appLoader = AppLoader.Instance;
            AppLoader.LoadConfig();
            AppLoader.LoadTelegramBot();
        }
    }
}
