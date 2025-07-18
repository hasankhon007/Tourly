using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Tourly.BookingModels;
using Tourly.Enums;
using Tourly.Models.UserModels;
using Tourly.Services.BookingServices;
using Tourly.Services.HotelService;
using Tourly.Services.UserServices;

namespace Tourly.Menu.UserPanel;

public class UserPanel
{
    private static readonly string Token = "7672157071:AAFrlYxHMI6O1mwXqBLx7Ra3sPEkNyF0SUg";
    private static TelegramBotClient botClient = new TelegramBotClient(Token);

    static Dictionary<long, string> userStates = new();
    static Dictionary<long, UserRegisterModel> userRegisterModels = new();
    static Dictionary<long, (string Phone, string Password)> userLoginModels = new();
    private static Dictionary<long, HotelBookingModel> hotelBookingModels = new();

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

            await bot.SendMessage(
                chatId,
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
        else if (message.Text == "🏨 Hotel bron qilish")
        {
            await bot.SendMessage(chatId, "Hotel bron qilish xizmati ishlanmoqda...", cancellationToken: cancellationToken);
        }
        else if (message.Text == "📄 Mening bronlarim")
        {
            await bot.SendMessage(chatId, "Sizning bron qilingan hotellar ro'yxati ishlanmoqda...", cancellationToken: cancellationToken);
        }
        else if (message.Text == "👤 Profilni ko‘rish")
        {
            // Sizda user ID bor deb hisoblaymiz
            var user = userRegisterModels.GetValueOrDefault(chatId);
            if (user != null)
            {
                string info = $"👤 Profil ma'lumotlari:\n" +
                              $"Ism: {user.FirstName}\n" +
                              $"Familiya: {user.LastName}\n" +
                              $"Telefon: {user.PhoneNumber}";
                await bot.SendMessage(chatId, info, cancellationToken: cancellationToken);
            }
            else
            {
                await bot.SendMessage(chatId, "Profil ma'lumotlari topilmadi.", cancellationToken: cancellationToken);
            }
        }
        else if (message.Text == "🚪 Chiqish")
        {
            // Logout
            if (userRegisterModels.ContainsKey(chatId))
                userRegisterModels.Remove(chatId);

            await bot.SendMessage(chatId, "Siz tizimdan chiqdingiz.", cancellationToken: cancellationToken);

            var keyboard = new ReplyKeyboardMarkup(new[]
            {
            new[] { new KeyboardButton("📝 Register"), new KeyboardButton("🔐 Login") }
        })
            { ResizeKeyboard = true };

            await bot.SendMessage(chatId, "Qaytadan tizimga kirish uchun birini tanlang 👇", replyMarkup: keyboard, cancellationToken: cancellationToken);
        }
        else
        {
            await HandleUserInput(bot, message, cancellationToken);
        }
    }


    private async Task HandleUserInput(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        if (!userStates.TryGetValue(chatId, out var state)) return;

        var service = new UserService();

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
                await bot.SendMessage(chatId, "Parolni kiriting:", cancellationToken: cancellationToken);
                break;

            case "register_password":
                userRegisterModels[chatId].Password = message.Text;

                try
                {
                    await service.SaveToFileAsync(userRegisterModels[chatId]);

                    await bot.SendMessage(chatId, "✅ Muvaffaqiyatli ro'yxatdan o'tdingiz!", cancellationToken: cancellationToken);

                    userStates[chatId] = "login_phone";
                    await bot.SendMessage(chatId, "Telefon raqamingizni kiriting:", cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    await bot.SendMessage(chatId, $"❌ Xatolik: {ex.Message}", cancellationToken: cancellationToken);
                }

                break;

            case "login_phone":
                userLoginModels[chatId] = (Phone: message.Text, Password: "");
                userStates[chatId] = "login_password";
                await bot.SendMessage(chatId, "Parolni kiriting:", cancellationToken: cancellationToken);
                break;

            case "login_password":
                var loginModel = userLoginModels[chatId];
                loginModel.Password = message.Text;

                try
                {
                    var user = await service.LoginAsync(loginModel.Phone, loginModel.Password);

                    if (user is not null)
                    {
                        userRegisterModels[chatId] = user; // login bo'lgach saqlaymiz

                        var replyKeyboard = new ReplyKeyboardMarkup(new[]
                        {
                        new KeyboardButton[] { "🏨 Hotel bron qilish", "📋 Mening bronlarim" },
                        new KeyboardButton[] { "👤 Profilni ko‘rish", "🚪 Chiqish" }
                    })
                        {
                            ResizeKeyboard = true
                        };

                        await bot.SendMessage(
                            chatId: chatId,
                            text: $"✅ Xush kelibsiz, {user.FirstName}!\nSiz tizimga kirdingiz.",
                            replyMarkup: replyKeyboard,
                            cancellationToken: cancellationToken
                        );
                    }
                    else
                    {
                        await bot.SendMessage(chatId, $"❌ Telefon yoki parol noto‘g‘ri!", cancellationToken: cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    await bot.SendMessage(chatId, $"❌ Xatolik: {ex.Message}", cancellationToken: cancellationToken);
                }

                userStates.Remove(chatId);
                userLoginModels.Remove(chatId);
                break;

            // 🔽 HOTEL BOOKING BOSHLANDI
            case "booking_hotel_id":
                if (!int.TryParse(message.Text, out int hotelId))
                {
                    await bot.SendMessage(chatId, "Noto‘g‘ri ID. Iltimos, son kiriting.", cancellationToken: cancellationToken);
                    return;
                }
                hotelBookingModels[chatId] = new HotelBookingModel { HotelId = hotelId };
                userStates[chatId] = "booking_room_type";
                await bot.SendMessage(chatId, "Xona turini tanlang (Single, Double, Suite):", cancellationToken: cancellationToken);
                break;

            case "booking_room_type":
                if (!Enum.TryParse<RoomType>(message.Text, true, out var roomType))
                {
                    await bot.SendMessage(chatId, "Noto‘g‘ri xona turi. (Single, Double, Suite)", cancellationToken: cancellationToken);
                    return;
                }

                hotelBookingModels[chatId].RoomType = roomType;
                userStates[chatId] = "booking_start_date";
                await bot.SendMessage(chatId, "Boshlanish sanasini kiriting (yyyy-mm-dd):", cancellationToken: cancellationToken);
                break;

            case "booking_start_date":
                if (!DateOnly.TryParse(message.Text, out var startDate))
                {
                    await bot.SendMessage(chatId, "Noto‘g‘ri format. yyyy-mm-dd ko‘rinishida kiriting.", cancellationToken: cancellationToken);
                    return;
                }

                hotelBookingModels[chatId].StartDate = startDate;
                userStates[chatId] = "booking_end_date";
                await bot.SendMessage(chatId, "Tugash sanasini kiriting (yyyy-mm-dd):", cancellationToken: cancellationToken);
                break;

            case "booking_end_date":
                if (!DateOnly.TryParse(message.Text, out var endDate))
                {
                    await bot.SendMessage(chatId, "Noto‘g‘ri format. yyyy-mm-dd ko‘rinishida kiriting.", cancellationToken: cancellationToken);
                    return;
                }

                var bookingModel = hotelBookingModels[chatId];
                bookingModel.EndDate = endDate;

                var hotelService = new HotelService();
                var allHotels = hotelService.GetAll(message.Text);
                var hotel = allHotels.FirstOrDefault(h => h.ID == bookingModel.HotelId);

                if (hotel == null)
                {
                    await bot.SendMessage(chatId, "Bunday ID li hotel topilmadi.", cancellationToken: cancellationToken);
                    return;
                }

                var bookingService = new BookingService();
                var userId = userRegisterModels[chatId].Id;

                var booking = bookingService.BookRoom(userId, hotel, bookingModel.RoomType, bookingModel.StartDate, bookingModel.EndDate);

                if (booking == null)
                {
                    await bot.SendMessage(chatId, "❌ Afsuski, tanlangan sana oralig‘ida bo‘sh xona yo‘q.", cancellationToken: cancellationToken);
                }
                else
                {
                    await bot.SendMessage(chatId,
                        $"✅ Hotel bron qilindi!\n🏨 {hotel.Name}\n🛏 {bookingModel.RoomType}\n📅 {bookingModel.StartDate} - {bookingModel.EndDate}\n💰 {booking.Price} so'm",
                        cancellationToken: cancellationToken);
                }

                // Holatlarni tozalash
                userStates.Remove(chatId);
                hotelBookingModels.Remove(chatId);
                break;
        }
    }


    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Botda xatolik: {exception.Message}");
        return Task.CompletedTask;
    }
}
