using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Tourly.Models.UserModels;
using Tourly.Services.UserServices;

namespace Tourly.Menu.UserPanel;

public class UserPanel
{
    private static readonly string Token = "7672157071:AAFrlYxHMI6O1mwXqBLx7Ra3sPEkNyF0SUg";
    private static TelegramBotClient botClient = new TelegramBotClient(Token);

    static Dictionary<long, string> userStates = new();
    static Dictionary<long, UserRegisterModel> userRegisterModels = new();
    static Dictionary<long, UserLoginModel> userLoginModels = new();

    public void Start()
    {
        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync);
        Console.WriteLine("Bot ishlayapti...");
        Console.ReadLine();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;

        var message = update.Message;
        var chatId = message.Chat.Id;

        if (message.Text == "/start")
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] { new KeyboardButton("📝 Register"), new KeyboardButton("🔐 Login") }
            })
            { ResizeKeyboard = true };

            await bot.SendMessage(chatId,
                "Xush kelibsiz! Quyidagi tugmalardan birini tanlang 👇",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }
        else if (message.Text == "📝 Register")
        {
            await bot.SendMessage(chatId, "Ismingizni kiriting:", cancellationToken: cancellationToken);
            userStates[chatId] = "register_first_name";
        }
        else if (message.Text == "🔐 Login")
        {
            await bot.SendMessage(chatId, "Telefon raqamingizni kiriting:", cancellationToken: cancellationToken);
            userStates[chatId] = "login_phone";
        }
        else
        {
            await HandleUserInput(bot, message, cancellationToken);
        }
    }

    private async Task HandleUserInput(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        if (!userStates.ContainsKey(chatId)) return;

        var state = userStates[chatId];

        switch (state)
        {
            case "register_first_name":
                userRegisterModels[chatId] = new UserRegisterModel { FirstName = message.Text };
                userStates[chatId] = "register_last_name";
                await bot.SendMessage(chatId, "Familiyangizni kiriting:", cancellationToken: cancellationToken);
                break;

            case "register_last_name":
                userRegisterModels[chatId].LastName = message.Text;
                userStates[chatId] = "register_phone";
                await bot.SendMessage(chatId, "Telefon raqamingizni kiriting:", cancellationToken: cancellationToken);
                break;

            case "register_phone":
                userRegisterModels[chatId].PhoneNumber = message.Text;
                userStates[chatId] = "register_password";
                await bot.SendMessage(chatId, "Parol kiriting:", cancellationToken: cancellationToken);
                break;

            case "register_password":
                userRegisterModels[chatId].Password = message.Text;
                try
                {
                    var service = new UserService();
                    service.Register(userRegisterModels[chatId]);
                    await bot.SendMessage(chatId, "✅ Muvaffaqiyatli ro'yxatdan o'tdingiz! Endi tizimga kiring:", cancellationToken: cancellationToken);

                    userStates[chatId] = "login_phone";
                    await bot.SendMessage(chatId, "Telefon raqamingizni kiriting:", cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    await bot.SendMessage(chatId, $"❌ Xatolik: {ex.Message}", cancellationToken: cancellationToken);
                    userStates.Remove(chatId);
                    userRegisterModels.Remove(chatId);
                }
                break;

            case "login_phone":
                userLoginModels[chatId] = new UserLoginModel { PhoneNumber = message.Text };
                userStates[chatId] = "login_password";
                await bot.SendMessage(chatId, "Parolni kiriting:", cancellationToken: cancellationToken);
                break;

            case "login_password":
                userLoginModels[chatId].Password = message.Text;
                try
                {
                    var service = new UserService();
                    var userId = service.Login(userLoginModels[chatId]);

                    await bot.SendMessage(chatId, $"✅ Tizimga muvaffaqiyatli kirdingiz. UserID: {userId}\nBooking jarayoniga o'tamiz...", cancellationToken: cancellationToken);
                    // Keyingi bosqich: Booking menyusiga yo'naltirish
                }
                catch (Exception ex)
                {
                    await bot.SendMessage(chatId, $"❌ Xatolik: {ex.Message}", cancellationToken: cancellationToken);
                }
                userStates.Remove(chatId);
                userLoginModels.Remove(chatId);
                break;
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Botda xatolik: {exception.Message}");
        return Task.CompletedTask;
    }
}
