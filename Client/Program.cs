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
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"])
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        await builder.Build().RunAsync();
    }
}