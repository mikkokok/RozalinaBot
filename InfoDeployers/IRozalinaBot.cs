using System.Threading.Tasks;

namespace RozalinaBot.InfoDeployers
{
    public interface IRozalinaBot
    {

        Task SendAdminMessages(string message, string from);

        Task SendToAll(string message, string from = "");
    }
}