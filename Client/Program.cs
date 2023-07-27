using Blazored.LocalStorage;
using Client.Authentication;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddMudServices();
        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddScoped<AuthenticationService>();

        builder.Services.AddScoped(sp =>
        {
            var baseUri = builder.HostEnvironment.BaseAddress;
            var localStorage = sp.GetRequiredService<ILocalStorageService>();

            if (builder.HostEnvironment.IsDevelopment())
            {
                if (builder.Configuration["Api:BaseUrl"] != null)
                {
                    baseUri = builder.Configuration["Api:BaseUrl"] ?? string.Empty;
                }
            }

            var client = new AuthenticatedHttpClient(localStorage)
            {
                BaseAddress = new Uri(baseUri)
            };

            return client;
        });

        await builder.Build().RunAsync();
    }
}