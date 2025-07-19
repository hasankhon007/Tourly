using Tourly.Menu.UserPanel;
using Tourly.TelegramBot;
namespace Tourly;
class Program
{
    public static async Task Main(string[] args)
    {
        UserPanel userPanel = new UserPanel();
        TelegramBotAdmin admin = new TelegramBotAdmin();

        await Task.WhenAll(userPanel.Start(), admin.StartAsync());
    }

}
