public static class AuthHelper
{
    public static bool IsAdmin(long chatId)
    {
        return BotAdmins.AdminIds.Contains(chatId);
    }
}
