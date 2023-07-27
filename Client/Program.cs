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
        try
        {
            var baseUri = string.Empty;

            if (builder.HostEnvironment.IsProduction())
            {
                baseUri = builder.HostEnvironment.BaseAddress;
            } 
            else if (builder.HostEnvironment.IsDevelopment())
            {
                baseUri = builder.Configuration["Api:BaseUrl"];
            }

            Console.WriteLine($"BaseURI: {baseUri}");
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(baseUri) });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        await builder.Build().RunAsync();
    }
}