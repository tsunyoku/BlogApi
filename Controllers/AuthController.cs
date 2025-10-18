using System.Security.Claims;
using BlogApi.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public ChallengeHttpResult Login([FromQuery(Name = "redirect_uri")] string redirectUri = "/")
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri
        };

        return TypedResults.Challenge(properties, ["osu"]);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<RedirectHttpResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return TypedResults.Redirect("/");
    }

    [HttpGet("user")]
    [Authorize]
    public Ok<GetUserResponse> GetUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var username = User.FindFirst(ClaimTypes.Name)!.Value;
        var avatarUrl = User.FindFirst(OsuClaimTypes.AvatarUrl)!.Value;
        var countryCode = User.FindFirst(OsuClaimTypes.CountryCode)!.Value;

        return TypedResults.Ok(new GetUserResponse
        {
            Id = userId,
            Username = username,
            AvatarUrl = avatarUrl,
            CountryCode = countryCode,
        });
    }
}