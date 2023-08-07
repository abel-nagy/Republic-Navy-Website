namespace Api.SteamAuth;

internal class ValidatedSteamLoginData
{
    public string SteamId { get; }
    public bool IsLoginValid { get; }

    public ValidatedSteamLoginData(string steamId, bool isLoginValid)
    {
        SteamId = steamId;
        IsLoginValid = isLoginValid;
    }
}