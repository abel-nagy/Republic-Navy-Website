using System.Threading.Tasks;
using Api.TokenSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Api;

public static class GetSteamId
{
    [FunctionName("GetSteamId")]
    public static Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "steamId")]
        HttpRequest request)
    {
        var userValidity = UserTokenValidator.ValidateToken(request);
        return userValidity.IsLoginValid ? Task.FromResult<IActionResult>(new OkObjectResult(userValidity.SteamId)) : Task.FromResult<IActionResult>(new UnauthorizedResult());
    }
}