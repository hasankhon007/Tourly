using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Tourly.Constants;
using Tourly.Domain;
using Tourly.Enums;
using Tourly.Extentions;
using Tourly.Helpers;
using Tourly.Models.HotelModels;
using Tourly.Services.HotelService;
using Tourly.Services.UserServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tourly.TelegramBot;

public class TelegramBotAdmin
{
    private readonly HotelService hotelService;
    private readonly UserService userService;
    private readonly Dictionary<long, HotelCreateModel> hotelStates = new();
    private readonly Dictionary<long, int> hotelSteps = new();

    private readonly Dictionary<long, HotelUpdateModel> hotelUpdateStates = new();
    private readonly Dictionary<long, int> hotelUpdateSteps = new();
    private static readonly List<string> HotelImages = new()
{
    "https://i.pinimg.com/736x/e6/30/db/e630db9e931df9ea09a6090cf5dbfa89.jpg",
    "https://i.pinimg.com/736x/1a/55/ad/1a55ad12640a86cea9277c19cbc8b37f.jpg",
    "https://i.pinimg.com/736x/58/41/66/5841666be7be79dcefdcffa23b24b2ff.jpg"
};


    private TelegramBotClient botClient;
     
    public TelegramBotAdmin()
    {
        botClient = new TelegramBotClient("7782728604:AAFNSPq7xVq5qcyEoWuUWyrqeZFDKTyotdQ");
        userService = new UserService();
        hotelService = new HotelService();
    }
    public async Task StartAsync()
    {
        using var cts = new CancellationTokenSource();

        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } // Important!
            },
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMe();
        Console.WriteLine($"✅ Bot is running as @{me.Username}");
        Console.WriteLine("📡 Listening for updates...");
        Console.ReadLine();

        cts.Cancel();
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {exception.Message}");

        if (exception.InnerException != null)
            Console.WriteLine($"[INNER EXCEPTION] {exception.InnerException.Message}");

        Console.ResetColor();
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            await HandleCallbackQueryAsync(update.CallbackQuery,cancellation);
            return;
        }
        if (update.Type != UpdateType.Message || update.Message?.Text is null) return;

        var chatId = update.Message.Chat.Id;
        var message = update.Message.Text;

        if (hotelUpdateSteps.ContainsKey(chatId))
        {
            await HandleUpdateHotelSteps(chatId, message);
            return;
        }
        // Step-by-step Add Hotel
        if (hotelSteps.ContainsKey(chatId))
        {
            await HandleAddHotelSteps(chatId, message, botClient);
            return;
        }
        switch (message)
        {
            case "/start":
                await ShowMainMenu(botClient, chatId);
                break;
            case "Menu":
                await ShowMainMenu(botClient, chatId);
                break;

            case "🏨 Hotels":
                await botClient.SendMessage(chatId, "🏨 Hotel Menu:", replyMarkup: HotelMenu());
                break;
            case "👤 Users":
                await botClient.SendMessage(chatId, "👤 User Menu:", replyMarkup: UserMenu());
                break;

            case "➕ Add Hotel":
                hotelStates[chatId] = new HotelCreateModel();
                hotelSteps[chatId] = 0;
                await botClient.SendMessage(chatId, "📍 Enter hotel name:");
                break;
            case "✏️ Update Hotel":
                await UpdateHotelAsync(chatId);
                break;


            case "📋 View Hotels":
                await ViewHotelsAsync(chatId);
                break;

            case "❌ Delete Hotel":
                await DeleteHotelAsync(chatId, CancellationToken.None);
                break;
            case "👤 View Users":
                await ViewUsersAsync(botClient, chatId, cancellation);
                break;
            case "❌ Delete Users":
                await DeleteUsersAsync(botClient, chatId, cancellation);
                break;

            default:
                if (int.TryParse(message, out int id))
                {
                    try
                    {
                        hotelService.Delete(id);
                        await botClient.SendMessage(chatId, $"✅ Hotel with ID {id} deleted.");
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendMessage(chatId, $"❌ {ex.Message}");
                    }
                }
                else
                {
                    await botClient.SendMessage(chatId, "❗ Unknown command. Use /start.");
                }
                break;
        }
        
    }
    private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken token)
    {
        Console.WriteLine($"[CallbackQuery Received] => {callbackQuery.Data}");

        if (callbackQuery.Data.StartsWith("delete_hotel:"))
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, "Deleting...");

            int id = int.Parse(callbackQuery.Data.Split(':')[1]);
            hotelService.Delete(id);

            await botClient.SendMessage(callbackQuery.Message.Chat.Id, $"✅ Hotel {id} deleted.");
        }

        if (callbackQuery.Data.StartsWith("delete_user_"))
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, "Deleting user...");

            int id = int.Parse(callbackQuery.Data.Replace("delete_user_", ""));
            userService.Delete(id);

            await botClient.SendMessage(callbackQuery.Message.Chat.Id, $"✅ User {id} deleted.");
        }
    }


    private async Task HandleAddHotelSteps(long chatId, string message, ITelegramBotClient botClient)
    {
        var step = hotelSteps[chatId];
        var model = hotelStates[chatId];

        try
        {
            switch (step)
            {
                case 0:
                    model.Name = message;
                    await botClient.SendMessage(chatId, "🌍 Enter hotel location:");
                    break;

                case 1:
                    model.Location = message;
                    await botClient.SendMessage(chatId, "📞 Enter phone number (+998...):");
                    break;

                case 2:
                    if (!message.StartsWith("+998") || message.Length < 13)
                    {
                        await botClient.SendMessage(chatId, "❌ Please enter a valid Uzbek phone number starting with +998.");
                        return;
                    }
                    model.PhoneNumber = message;
                    await botClient.SendMessage(chatId, "⭐ Enter star count (1-5):");
                    break;

                case 3:
                    model.StarsCount = byte.Parse(message);
                    await botClient.SendMessage(chatId, "📝 Enter hotel description:");
                    break;

                case 4:
                    model.Description = message;
                    await botClient.SendMessage(chatId, "🏨 Enter room counts as:\n\n`Single,Double,Family,Deluxe,VIP`\nExample: `10,5,2,1,1`",
                        parseMode: ParseMode.Markdown);
                    break;

                case 5:
                    var counts = message.Split(',').Select(int.Parse).ToList();
                    var rooms = new List<Room>();
                    rooms.AddRange(RoomType.Single.Create(counts[0], GeneratorHelper.GenerateId(PathHolder.HotelsFilesPath)));
                    rooms.AddRange(RoomType.Double.Create(counts[1], GeneratorHelper.GenerateId(PathHolder.HotelsFilesPath)));
                    rooms.AddRange(RoomType.Family.Create(counts[2], GeneratorHelper.GenerateId(PathHolder.HotelsFilesPath)));
                    rooms.AddRange(RoomType.Deluxe.Create(counts[3], GeneratorHelper.GenerateId(PathHolder.HotelsFilesPath)));
                    rooms.AddRange(RoomType.VIP.Create(counts[4], GeneratorHelper.GenerateId(PathHolder.HotelsFilesPath)));
                    model.Rooms = rooms;

                    hotelService.Create(model);
                    await botClient.SendMessage(chatId, "✅ Hotel added successfully!");

                    hotelSteps.Remove(chatId);
                    hotelStates.Remove(chatId);
                    return;
            }

            hotelSteps[chatId]++;
        }
        catch (Exception ex)
        {
            await botClient.SendMessage(chatId, $"❌ Error: {ex.Message}");
            hotelSteps.Remove(chatId);
            hotelStates.Remove(chatId);
        }
    }

    private async Task ShowMainMenu(ITelegramBotClient botClient, long chatId)
    {
        var menu = new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("🏨 Hotels"), new KeyboardButton("👤 Users") }
        })
        {
            ResizeKeyboard = true
        };

        await botClient.SendMessage(chatId, "👋 Welcome to Tourly Admin Panel", replyMarkup: menu);
    }

    private async Task ViewHotelsAsync(long chatId)
    {
        var hotels = hotelService.GetAll("");

        if (hotels.Count == 0)
        {
            await botClient.SendMessage(chatId, "❌ No hotels found.");
            return;
        }

        foreach (var hotel in hotels)
        {
            var rooms = FileHelper.ReadFromFile(PathHolder.RoomsFilesPath).Convert<Room>()
                .Where(r => r.HotelId == hotel.ID)
                .ToList();
            string hotelInfo = $"🏨 *{hotel.Name}*\n" +
                               $"📍 Location: {hotel.Location}\n" +
                               $"📞 Phone: {hotel.PhoneNumber}\n" +
                               $"⭐  Stars: {hotel.StarsCount}\n" +
                               $"   Room Count: {rooms.Count}";

            var photoUrl = GetRandomHotelImage();

            await botClient.SendPhoto(
                chatId: chatId,
                photo: photoUrl,
                caption: hotelInfo,
                parseMode: ParseMode.Markdown);
        }
    }


    private async Task UpdateHotelAsync(long chatId)
    {
        hotelUpdateStates[chatId] = new HotelUpdateModel();
        hotelUpdateSteps[chatId] = 0;

        var hotels = hotelService.GetAll("");
        if (hotels.Count == 0)
        {
            await botClient.SendMessage(chatId, "❌ No hotels found to update.");
            return;
        }

        string list = "🏨 Available Hotels:\n";
        foreach (var hotel in hotels)
            list += $"ID: {hotel.ID} | {hotel.Name} | {hotel.Location}\n";

        await botClient.SendMessage(chatId, list + "\n\n🆔 Enter the *ID* of the hotel you want to update:", Telegram.Bot.Types.Enums.ParseMode.Markdown);
    }

    private ReplyMarkup HotelMenu()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("➕ Add Hotel")},
            new[] { new KeyboardButton("✏️ Update Hotel"), new KeyboardButton("📋 View Hotels") },
            new[] { new KeyboardButton("❌ Delete Hotel"), new KeyboardButton("Menu") }
        })
        {
            ResizeKeyboard = true
        };
    }

    private ReplyMarkup UserMenu()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("👤 View Users"), new KeyboardButton("❌ Delete Users") },
            new[] { new KeyboardButton("Menu") }
        })
        {
            ResizeKeyboard = true
        };
    }

    private async Task DeleteHotelAsync(long chatId, CancellationToken token)
    {
        var hotels = hotelService.GetAll("");

        if (hotels.Count == 0)
        {
            await botClient.SendMessage(chatId, "❌ No hotels found to delete.");
            return;
        }

        // Generate inline buttons for each hotel with its ID
        var inlineKeyboard = new List<List<InlineKeyboardButton>>();

        foreach (var hotel in hotels)
        {
            inlineKeyboard.Add(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData($"{hotel.Name} ({hotel.ID})", $"delete_hotel:{hotel.ID}")
        });
        }

        

        await botClient.SendMessage(
            chatId: chatId,
            text: "🗑 Select a hotel to delete:",
            replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
            cancellationToken: token);
    }

    private async Task ViewUsersAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var users = userService.GetALLforAdmin();

        if (users == null || users.Count == 0)
        {
            await botClient.SendMessage(chatId, "👤 No users found.");
            return;
        }

        foreach (var user in users)
        {
            string text = $"👤 *Name:* {user.FirstName} {user.LastName}\n" +
                          $"🆔 *ID:* `{user.Id}`\n" +
                          $"📞 *Phone:* {user.PhoneNumber}";

            await botClient.SendMessage(
                chatId: chatId,
                text: text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task DeleteUsersAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var users = userService.GetALLforAdmin();

        if (users == null || users.Count == 0)
        {
            await botClient.SendMessage(chatId, "❌ No users found to delete.");
            return;
        }

        foreach (var user in users)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            InlineKeyboardButton.WithCallbackData("🗑 Delete this user", $"delete_user_{user.Id}")
        });

            string userInfo = $"👤 *Name:* {user.FirstName} {user.LastName}\n" +
                              $"📞 *Phone:* {user.PhoneNumber}\n" +
                              $"🆔 *ID:* `{user.Id}`";

            await botClient.SendMessage(
                chatId: chatId,
                text: userInfo,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task HandleUpdateHotelSteps(long chatId, string message)
    {
        var step = hotelUpdateSteps[chatId];
        var model = hotelUpdateStates[chatId];

        try
        {
            switch (step)
            {
                case 0:
                    if (!int.TryParse(message, out int id))
                    {
                        await botClient.SendMessage(chatId, "❌ Invalid ID. Please enter a number.");
                        return;
                    }

                    var hotel = hotelService.Get(id);
                    if (hotel == null)
                    {
                        await botClient.SendMessage(chatId, "❌ Hotel not found.");
                        return;
                    }

                    model.Id = id;
                    await botClient.SendMessage(chatId, "📍 Enter new hotel location:");
                    break;

                case 1:
                    model.Location = message;
                    await botClient.SendMessage(chatId, "🏨 Enter new hotel name:");
                    break;

                case 2:
                    model.Name = message;
                    await botClient.SendMessage(chatId, "📞 Enter new phone number:");
                    break;

                case 3:
                    model.PhoneNumber = message;
                    await botClient.SendMessage(chatId, "📝 Enter new description:");
                    break;

                case 4:
                    model.Description = message;
                    await botClient.SendMessage(chatId, "⭐ Enter new star count (1–5):");
                    break;

                case 5:
                    if (!byte.TryParse(message, out byte stars) || stars < 1 || stars > 5)
                    {
                        await botClient.SendMessage(chatId, "❌ Invalid star count. Please enter 1 to 5.");
                        return;
                    }

                    model.StarsCount = stars;
                    hotelService.Update(model);
                    await botClient.SendMessage(chatId, "✅ Hotel updated successfully!");

                    hotelUpdateStates.Remove(chatId);
                    hotelUpdateSteps.Remove(chatId);
                    return;
            }

            hotelUpdateSteps[chatId]++;
        }
        catch (Exception ex)
        {
            await botClient.SendMessage(chatId, $"❌ Error: {ex.Message}");
            hotelUpdateStates.Remove(chatId);
            hotelUpdateSteps.Remove(chatId);
        }
    }

    private string GetRandomHotelImage()
    {
        var random = new Random();
        int index = random.Next(HotelImages.Count);
        return HotelImages[index];
    }

}
