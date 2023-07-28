using System;

namespace Api.SteamAuth;

internal static class SteamAuthConstants
{
    public const long SteamId64Ident = 76561197960265728;
    public const string SteamIdTokenKey = "steamId";
    public const string SteamIdPrefix = "STEAM_0:";
    public static readonly string SiteBaseUrl = Environment.GetEnvironmentVariable("SITE_BASE_URL");
    public static readonly string ApiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
}