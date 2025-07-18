
using Tourly.TelegramBot;

namespace Tourly
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TelegramBotAdmin bot = new TelegramBotAdmin();
            await bot.StartAsync();
        }
    }
}
