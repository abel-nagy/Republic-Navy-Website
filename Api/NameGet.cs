using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api;

public static class NameGet
{
    [FunctionName("NameGet")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string name = req.Query["name"];

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name ??= data?.name;

        var responseMessage = string.IsNullOrEmpty(name)
            ? "Guest"
            : name;

        var result = new OkObjectResult(responseMessage);
        if (!req.HttpContext.Request.Headers.ContainsKey("Origin"))
        {
            return result;
        }

        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", req.Headers["Origin"].ToString());
        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");

        return result;
    }
}