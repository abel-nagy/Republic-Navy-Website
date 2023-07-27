using System.Web;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Client.Authentication;

public class AuthenticationService
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;

    public AuthenticationService(NavigationManager navigationManager, ILocalStorageService localStorage)
    {
        _navigationManager = navigationManager;
        _localStorage = localStorage;
    }

    public async Task<bool> SaveTokenAfterSteamLoginRedirectAsync()
    {
        if (await IsTokenPresentOnStorage())
        {
            return true;
        }

        var query = new Uri(_navigationManager.Uri).Query;
        var token = HttpUtility.ParseQueryString(query).Get(AuthenticationConstants.TokenKey);
        if (token == null)
        {
            return false;
        }

        token = token.Replace("'", "");
        await _localStorage.SetItemAsStringAsync(AuthenticationConstants.TokenKey, token);

        return await IsTokenPresentOnStorage();
    }

    public async Task DestroyAuthToken()
    {
        await _localStorage.RemoveItemAsync(AuthenticationConstants.TokenKey);
    }

    public async Task<bool> IsTokenPresentOnStorage()
    {
        var localToken = await GetLocalTokenAsync();
        return !string.IsNullOrWhiteSpace(localToken);
    }

    private async Task<string> GetLocalTokenAsync()
    {
        return await _localStorage.GetItemAsStringAsync(AuthenticationConstants.TokenKey);
    }
}