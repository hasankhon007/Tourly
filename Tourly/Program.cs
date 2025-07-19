using Tourly.Menu.UserPanel;
namespace Tourly;
class Program
{
    public static async Task Main(string[] args)
    {
         UserPanel userpanel = new UserPanel();
         await userpanel.Start();

    }
}
