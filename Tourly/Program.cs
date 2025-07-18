
using Tourly.TelegramBot;

namespace Tourly
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TelegramBotAdmin botAdmin = new TelegramBotAdmin();
            await botAdmin.StartAsync();
        }
    }
}
