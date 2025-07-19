using System.Globalization; // DateOnly parse qilish uchun
using Telegram.Bot;
using Telegram.Bot.Polling;
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
    private static TelegramBotClient _botClient;

    private readonly UserService _userService;
    private readonly BookingService _bookingService;
    private readonly HotelService _hotelService; 

    private static readonly Dictionary<long, string> UserStates = new();
    private static readonly Dictionary<long, UserRegisterModel> UserRegisterModels = new();
    private static readonly Dictionary<long, (string Phone, string Password)> UserLoginModels = new();
    private static readonly Dictionary<long, HotelBookingModel> UserBookingModels = new();
    private static readonly Dictionary<long, int> UserSelectedHotelId = new(); // Bron qilish uchun tanlangan hotel ID

    private static readonly Dictionary<long, UserRegisterModel> LoggedInUsers = new();

    public UserPanel()
    {
        _botClient = new TelegramBotClient(Token);
        _userService = new UserService();
        _bookingService = new BookingService();
        _hotelService = new HotelService(); 
    }

    public async Task Start()
    {
        using var cts = new CancellationTokenSource();

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions: new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } },
            cancellationToken: cts.Token
        );

        var me = await _botClient.GetMe(cts.Token);
        Console.WriteLine($"✅ User Bot @{me.Username} ishlashni boshladi.");
        await Task.Delay(-1, cts.Token); // Botni doimiy ishlash holatida ushlab turish
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[XATOLIK] {exception.Message}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null) return;

        var message = update.Message;
        var chatId = message.Chat.Id;
        var text = message.Text;


        Console.WriteLine($"[{DateTime.Now}] Received message '{text}' from {chatId}");

        // 1. Foydalanuvchi biror jarayon ichidami (masalan, parol kirityaptimi)?
        if (UserStates.ContainsKey(chatId))
        {
            await HandleStatefulInputAsync(bot, message, cancellationToken);
            return;
        }

        // 2. Foydalanuvchi tizimga kirganmi?
        if (LoggedInUsers.ContainsKey(chatId))
        {
            await HandleLoggedInCommandsAsync(bot, chatId, text, cancellationToken);
        }
        else // 3. Foydalanuvchi tizimga kirmagan
        {
            await HandleLoggedOutCommandsAsync(bot, chatId, text, cancellationToken);
        }
    }

    private async Task HandleLoggedOutCommandsAsync(ITelegramBotClient bot, long chatId, string text, CancellationToken cancellationToken)
    {
        Task action = text switch
        {
            "/start" => SendWelcomeMessageAsync(bot, chatId, cancellationToken),
            "📝 Register" => StartRegistrationProcessAsync(bot, chatId, cancellationToken),
            "🔐 Login" => StartLoginProcessAsync(bot, chatId, cancellationToken),
            _ => bot.SendMessage(chatId, "Noma'lum buyruq. Iltimos, ro'yxatdan o'ting yoki tizimga kiring.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken)
        };
        await action;
    }

    private async Task HandleLoggedInCommandsAsync(ITelegramBotClient bot, long chatId, string text, CancellationToken cancellationToken)
    {
        Task action = text switch
        {
            "🏢 Hotellarni ko'rish" => ViewAllHotelsAsync(bot, chatId, cancellationToken),
            "🔍 Hotel qidirish" => StartHotelSearchAsync(bot, chatId, cancellationToken),
            "🏨 Hotelni bron qilish" => StartBookingProcessAsync(bot, chatId, cancellationToken),
            "📋 Mening bronlarim" => ShowMyBookingsAsync(bot, chatId, cancellationToken),
            "✏️ Bronni yangilash" => UpdateBookingAsync(bot, chatId, cancellationToken),
            "👤 Profilni ko‘rish" => ShowProfileAsync(bot, chatId, cancellationToken),
            "🚪 Chiqish" => ProcessLogoutAsync(bot, chatId, cancellationToken),
            _ => bot.SendMessage(chatId, "Noma'lum buyruq. Menyudagi tugmalardan birini tanlang.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken)
        };
        await action;
    }

    private ReplyMarkup GetLoggedOutMenu()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("📝 Register"), new KeyboardButton("🔐 Login") }
        })
        { ResizeKeyboard = true };
    }

    private ReplyMarkup GetLoggedInMenu()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🏢 Hotellarni ko'rish", "🔍 Hotel qidirish" },
            new KeyboardButton[] { "🏨 Hotelni bron qilish" },
            new KeyboardButton[] { "📋 Mening bronlarim", "✏️ Bronni yangilash" },
            new KeyboardButton[] { "👤 Profilni ko‘rish", "🚪 Chiqish" }
        })
        { ResizeKeyboard = true };
    }

    private ReplyKeyboardMarkup GetRoomTypeKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { RoomType.Single.ToString(), RoomType.Deluxe.ToString() },
            new KeyboardButton[] { RoomType.Family.ToString(), RoomType.Double.ToString(), RoomType.VIP.ToString()}
        })
        { ResizeKeyboard = true, OneTimeKeyboard = true };
    }


    // --- Boshlang'ich Methodlar ---
    private async Task SendWelcomeMessageAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        await bot.SendMessage(chatId, "Assalomu alaykum! Tourly botiga xush kelibsiz!", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
    }

    private async Task StartRegistrationProcessAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        UserStates[chatId] = "register_firstname";
        UserRegisterModels[chatId] = new UserRegisterModel(); // Yangi ro'yxatga olish modelini yaratish
        await bot.SendMessage(chatId, "Ismingizni kiriting:", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
    }

    private async Task StartLoginProcessAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        UserStates[chatId] = "login_phone";
        UserLoginModels[chatId] = (Phone: "", Password: ""); // Login modelini boshlash
        await bot.SendMessage(chatId, "Telefon raqamingizni kiriting (+998xxxxxxxxx):", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
    }

    // --- Tizimga kirgandan keyingi Methodlar ---
    private async Task ShowProfileAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        if (LoggedInUsers.TryGetValue(chatId, out var user))
        {
            string info = $"👤 Sizning profilingiz:\n\n" +
                          $"Ism: *{user.FirstName}*\n" +
                          $"Familiya: *{user.LastName}*\n" +
                          $"Telefon: `{user.PhoneNumber}`";
            await bot.SendMessage(chatId, info, parseMode: ParseMode.Markdown, replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
        }
        else
        {
            await bot.SendMessage(chatId, "Profil ma'lumotlarini olishda xatolik. Iltimos, qaytadan tizimga kiring.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
            LoggedInUsers.Remove(chatId); // Agar qandaydir sababga ko'ra kirgan bo'lsa ham, o'chiramiz
        }
    }

    private async Task ViewAllHotelsAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken, string searchQuery = "")
    {
        try
        {
            var hotels = _hotelService.GetAll(searchQuery);
            if (hotels == null || hotels.Count == 0)
            {
                await bot.SendMessage(chatId, "Hozircha mehmonxonalar mavjud emas.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
                return;
            }

            await bot.SendMessage(chatId, $"Natijalar ({hotels.Count} ta topildi):", cancellationToken: cancellationToken);
            foreach (var hotel in hotels)
            {
                string info = $"🏨 *{hotel.Name}* (ID: {hotel.ID})\n" +
                              $"📍 Manzil: {hotel.Location}\n" +
                              $"⭐️ Reyting: {new string('⭐', hotel.StarsCount)}";
                await bot.SendMessage(chatId, info, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);
            }
            await bot.SendMessage(chatId, "Bosh menyu:", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[XATOLIK] Hotellarni ko'rishda: {ex.Message}");
            await bot.SendMessage(chatId, "Hotellarni yuklashda xatolik yuz berdi. Iltimos, keyinroq urinib ko'ring.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
        }
    }

    private async Task StartHotelSearchAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        UserStates[chatId] = "search_query";
        await bot.SendMessage(chatId, "🔍 Qidirish uchun hotel nomini yoki manzilini yozing:", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
    }

    private async Task StartBookingProcessAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        UserStates[chatId] = "booking_hotel_id";
        UserBookingModels[chatId] = new HotelBookingModel(); // Yangi booking modelini boshlash
        UserSelectedHotelId.Remove(chatId); // Oldingi tanlovni tozalash
        await bot.SendMessage(chatId, "Bron qilish uchun Hotel ID raqamini kiriting:", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
    }

    private async Task ShowMyBookingsAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        if (!LoggedInUsers.TryGetValue(chatId, out var currentUser))
        {
            await bot.SendMessage(chatId, "Siz tizimga kirmagansiz. Iltimos, avval tizimga kiring.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
            return;
        }

        try
        {
            var bookings = _bookingService.GetBookings(currentUser.Id);
            if (bookings == null || bookings.Count == 0)
            {
                await bot.SendMessage(chatId, "📋 Sizda hech qanday bron topilmadi.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
                return;
            }

            await bot.SendMessage(chatId, $"📋 Sizning bronlaringiz ({bookings.Count} ta):", cancellationToken: cancellationToken);
            foreach (var booking in bookings)
            {
                string info = $"Booking ID: *{booking.idFinder(currentUser.Id)}*\n" +
                              $"Hotel: *{booking.HotelName}*\n" +
                              $"Xona turi: *{booking.RoomType}*\n" +
                              $"Kirish sanasi: `{booking.StartDate.ToShortDateString()}`\n" +
                              $"Chiqish sanasi: `{booking.EndDate.ToShortDateString()}`\n" +
                              $"Narxi: *{booking.Price:C}*"; // Valyuta formatini o'zgartirishingiz mumkin
                await bot.SendMessage(chatId, info, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);
            }
            await bot.SendMessage(chatId, "Bosh menyu:", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[XATOLIK] Bronlarni ko'rishda: {ex.Message}");
            await bot.SendMessage(chatId, $"Bronlarni yuklashda xatolik yuz berdi: {ex.Message}", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
        }
    }

    private async Task UpdateBookingAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        await bot.SendMessage(chatId, "✏️ Bronni yangilash funksiyasi hozirda ishlab chiqilmoqda.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
    }

    private async Task ProcessLogoutAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        LoggedInUsers.Remove(chatId);
        UserStates.Remove(chatId); // Chiqishda barcha holatlarni tozalash
        UserRegisterModels.Remove(chatId);
        UserLoginModels.Remove(chatId);
        UserBookingModels.Remove(chatId);
        UserSelectedHotelId.Remove(chatId);
        await bot.SendMessage(chatId, "Siz tizimdan muvaffaqiyatli chiqdingiz.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
    }

    private async Task HandleStatefulInputAsync(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        if (!UserStates.TryGetValue(message.Chat.Id, out var state)) return;

        // Holat qaysi jarayonga tegishli ekanligini aniqlaymiz
        Task action = state switch
        {
            _ when state.StartsWith("register_") => ProcessRegistrationStepAsync(bot, message, state, cancellationToken),
            _ when state.StartsWith("login_") => ProcessLoginStepAsync(bot, message, state, cancellationToken),
            _ when state.StartsWith("booking_") => ProcessBookingStepAsync(bot, message, state, cancellationToken),
            "search_query" => ProcessSearchQueryAsync(bot, message, cancellationToken),
            _ => Task.CompletedTask
        };
        await action;
    }

    private async Task ProcessRegistrationStepAsync(ITelegramBotClient bot, Message message, string state, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var text = message.Text;
        var currentUserRegisterModel = UserRegisterModels.GetValueOrDefault(chatId, new UserRegisterModel());

        try
        {
            switch (state)
            {
                case "register_firstname":
                    currentUserRegisterModel.FirstName = text;
                    UserStates[chatId] = "register_lastname";
                    await bot.SendMessage(chatId, "Familiyangizni kiriting:", cancellationToken: cancellationToken);
                    break;
                case "register_lastname":
                    currentUserRegisterModel.LastName = text;
                    UserStates[chatId] = "register_phone";
                    await bot.SendMessage(chatId, "Telefon raqamingizni kiriting (+998xxxxxxxxx):", cancellationToken: cancellationToken);
                    break;
                case "register_phone":
                    // Telefon raqami formatini tekshirish (soddalashtirilgan)
                    if (!text.StartsWith("+998") || text.Length != 13 || !long.TryParse(text.Substring(1), out _))
                    {
                        await bot.SendMessage(chatId, "Noto'g'ri telefon raqami formati. Iltimos, +998xxxxxxxxx formatida kiriting:", cancellationToken: cancellationToken);
                        return;
                    }

                    // Telefon raqami allaqachon mavjudligini tekshirish
                    var allUsers = await _userService.GetAllUsersAsync();
                    if (allUsers.Any(u => u.PhoneNumber == text))
                    {
                        await bot.SendMessage(chatId, "Bu telefon raqami allaqachon ro'yxatdan o'tgan. Iltimos, boshqa raqam kiriting yoki tizimga kiring.", cancellationToken: cancellationToken);
                        UserStates.Remove(chatId);
                        UserRegisterModels.Remove(chatId);
                        await SendWelcomeMessageAsync(bot, chatId, cancellationToken);
                        return;
                    }

                    currentUserRegisterModel.PhoneNumber = text;
                    UserStates[chatId] = "register_password";
                    await bot.SendMessage(chatId, "Parol yarating (kamida 6 belgi):", cancellationToken: cancellationToken);
                    break;
                case "register_password":
                    if (text.Length < 6)
                    {
                        await bot.SendMessage(chatId, "Parol kamida 6 belgidan iborat bo'lishi kerak. Qaytadan kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    currentUserRegisterModel.Password = text;
                    await _userService.SaveToFileAsync(currentUserRegisterModel); // Yangi foydalanuvchini saqlash
                    await bot.SendMessage(chatId, "✅ Muvaffaqiyatli ro'yxatdan o'tdingiz! Endi tizimga kiring.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
                    UserStates.Remove(chatId);
                    UserRegisterModels.Remove(chatId);
                    // Ro'yxatdan o'tgandan keyin avtomatik login jarayoniga o'tish shart emas, foydalanuvchi o'zi "Login" tugmasini bosishi mumkin.
                    // await StartLoginProcessAsync(bot, chatId, cancellationToken); // Agar avtomatik login kerak bo'lsa
                    break;
            }
            UserRegisterModels[chatId] = currentUserRegisterModel; // Har qadamda modelni yangilash
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[XATOLIK] Ro'yxatdan o'tishda: {ex.Message}");
            await bot.SendMessage(chatId, $"Ro'yxatdan o'tishda xatolik yuz berdi: {ex.Message}. Iltimos, qaytadan urinib ko'ring.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
            UserStates.Remove(chatId);
            UserRegisterModels.Remove(chatId);
        }
    }

    private async Task ProcessLoginStepAsync(ITelegramBotClient bot, Message message, string state, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var text = message.Text;
        var currentLoginModel = UserLoginModels.GetValueOrDefault(chatId, (Phone: "", Password: ""));

        try
        {
            switch (state)
            {
                case "login_phone":
                    // Telefon raqami formatini tekshirish (soddalashtirilgan)
                    if (!text.StartsWith("+998") || text.Length != 13 || !long.TryParse(text.Substring(1), out _))
                    {
                        await bot.SendMessage(chatId, "Noto'g'ri telefon raqami formati. Iltimos, +998xxxxxxxxx formatida kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    currentLoginModel.Phone = text;
                    UserStates[chatId] = "login_password";
                    await bot.SendMessage(chatId, "Parolni kiriting:", cancellationToken: cancellationToken);
                    break;

                case "login_password":
                    currentLoginModel.Password = text;
                    var user = await _userService.LoginAsync(currentLoginModel.Phone, currentLoginModel.Password);

                    if (user != null)
                    {
                        LoggedInUsers[chatId] = user; // Foydalanuvchini "kirganlar" ro'yxatiga qo'shamiz
                        UserStates.Remove(chatId);    // Jarayon tugadi
                        UserLoginModels.Remove(chatId);

                        await bot.SendMessage(chatId, $"Xush kelibsiz, {user.FirstName}!", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await bot.SendMessage(chatId, "❌ Telefon yoki parol noto‘g‘ri! Qaytadan telefon raqamingizni kiriting:", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                        UserStates[chatId] = "login_phone"; // Qaytadan telefon raqamini so'rash
                    }
                    break;
            }
            UserLoginModels[chatId] = currentLoginModel; // Har qadamda modelni yangilash
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[XATOLIK] Login jarayonida: {ex.Message}");
            await bot.SendMessage(chatId, $"Login jarayonida xatolik yuz berdi: {ex.Message}. Iltimos, qaytadan urinib ko'ring.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
            UserStates.Remove(chatId);
            UserLoginModels.Remove(chatId);
        }
    }

    private async Task ProcessSearchQueryAsync(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        UserStates.Remove(message.Chat.Id);
        await ViewAllHotelsAsync(bot, message.Chat.Id, cancellationToken, searchQuery: message.Text);
    }

    private async Task ProcessBookingStepAsync(ITelegramBotClient bot, Message message, string state, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var text = message.Text;
        var currentBookingModel = UserBookingModels.GetValueOrDefault(chatId, new HotelBookingModel());

        if (!LoggedInUsers.TryGetValue(chatId, out var currentUser))
        {
            await bot.SendMessage(chatId, "Bron qilish uchun tizimga kirishingiz kerak.", replyMarkup: GetLoggedOutMenu(), cancellationToken: cancellationToken);
            UserStates.Remove(chatId);
            UserBookingModels.Remove(chatId);
            UserSelectedHotelId.Remove(chatId);
            return;
        }

        try
        {
            switch (state)
            {
                case "booking_hotel_id":
                    if (!int.TryParse(text, out int hotelId))
                    {
                        await bot.SendMessage(chatId, "Noto'g'ri Hotel ID. Iltimos, raqam kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    var hotel = _hotelService.Get(hotelId);
                    if (hotel == null)
                    {
                        await bot.SendMessage(chatId, "Bunday ID raqamli hotel topilmadi. Iltimos, to'g'ri ID kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    UserSelectedHotelId[chatId] = hotelId;
                    currentBookingModel.HotelId = hotelId;
                    currentBookingModel.HotelName = hotel.Name; // Hotel nomini ham saqlaymiz
                    currentBookingModel.UserId = currentUser.Id; // Foydalanuvchi ID ni saqlash
                    UserStates[chatId] = "booking_room_type";
                    await bot.SendMessage(chatId, $"Siz *{hotel.Name}* hotelini tanladingiz. Xona turini tanlang:", parseMode: ParseMode.Markdown, replyMarkup: GetRoomTypeKeyboard(), cancellationToken: cancellationToken);
                    break;

                case "booking_room_type":
                    if (!Enum.TryParse<RoomType>(text, true, out var roomType))
                    {
                        await bot.SendMessage(chatId, "Noto'g'ri xona turi. Iltimos, menyudagi tugmalardan birini tanlang:", replyMarkup: GetRoomTypeKeyboard(), cancellationToken: cancellationToken);
                        return;
                    }
                    currentBookingModel.RoomType = roomType;
                    UserStates[chatId] = "booking_start_date";
                    await bot.SendMessage(chatId, "Bron qilish boshlanish sanasini kiriting (YYYY-MM-DD formatida):", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                    break;

                case "booking_start_date":
                    if (!DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                    {
                        await bot.SendMessage(chatId, "Noto'g'ri sana formati. Iltimos, YYYY-MM-DD formatida kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    if (startDate < DateOnly.FromDateTime(DateTime.Today))
                    {
                        await bot.SendMessage(chatId, "Boshlanish sanasi bugundan oldin bo'lmasligi kerak. Iltimos, to'g'ri sana kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    currentBookingModel.StartDate = startDate;
                    UserStates[chatId] = "booking_end_date";
                    await bot.SendMessage(chatId, "Bron qilish tugash sanasini kiriting (YYYY-MM-DD formatida):", cancellationToken: cancellationToken);
                    break;

                case "booking_end_date":
                    if (!DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
                    {
                        await bot.SendMessage(chatId, "Noto'g'ri sana formati. Iltimos, YYYY-MM-DD formatida kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    if (endDate <= currentBookingModel.StartDate)
                    {
                        await bot.SendMessage(chatId, "Tugash sanasi boshlanish sanasidan keyin bo'lishi kerak. Iltimos, to'g'ri sana kiriting:", cancellationToken: cancellationToken);
                        return;
                    }
                    currentBookingModel.EndDate = endDate;

                    // Bronni yakunlash
                    var selectedHotel = _hotelService.Get(UserSelectedHotelId[chatId]);
                    if (selectedHotel == null)
                    {
                        await bot.SendMessage(chatId, "Hotel ma'lumotlari topilmadi. Iltimos, qaytadan urinib ko'ring.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
                        UserStates.Remove(chatId);
                        UserBookingModels.Remove(chatId);
                        UserSelectedHotelId.Remove(chatId);
                        return;
                    }

                    // Xona mavjudligini tekshirish va bron qilish
                    var bookedRoom = _bookingService.BookRoom(
                        currentUser.Id,
                        selectedHotel,
                        currentBookingModel.RoomType,
                        currentBookingModel.StartDate,
                        currentBookingModel.EndDate
                    );

                    if (bookedRoom != null)
                    {
                        await bot.SendMessage(chatId, $"✅ Hotel muvaffaqiyatli bron qilindi!\n" +
                                                      $"Hotel: *{selectedHotel.Name}*\n" +
                                                      $"Xona turi: *{currentBookingModel.RoomType}*\n" +
                                                      $"Kirish: `{currentBookingModel.StartDate.ToShortDateString()}`\n" +
                                                      $"Chiqish: `{currentBookingModel.EndDate.ToShortDateString()}`\n" +
                                                      $"Umumiy narx: *{bookedRoom.Price:C}*", // bookedRoom.Price ni ishlatamiz
                                                      parseMode: ParseMode.Markdown, replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await bot.SendMessage(chatId, "Afsuski, tanlangan sanalarda va xona turida bo'sh xona topilmadi. Iltimos, boshqa sanalarni yoki xona turini tanlang.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
                    }

                    UserStates.Remove(chatId);
                    UserBookingModels.Remove(chatId);
                    UserSelectedHotelId.Remove(chatId);
                    break;
            }
            UserBookingModels[chatId] = currentBookingModel; // Har qadamda modelni yangilash
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[XATOLIK] Bron qilish jarayonida: {ex.Message}");
            await bot.SendMessage(chatId, $"Bron qilish jarayonida xatolik yuz berdi: {ex.Message}. Iltimos, qaytadan urinib ko'ring.", replyMarkup: GetLoggedInMenu(), cancellationToken: cancellationToken);
            UserStates.Remove(chatId);
            UserBookingModels.Remove(chatId);
            UserSelectedHotelId.Remove(chatId);
        }
    }
}
