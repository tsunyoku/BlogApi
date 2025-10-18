using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using BlogApi;
using BlogApi.Configuration;
using BlogApi.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddOptions<OsuSettings>()
    .Bind(builder.Configuration.GetSection("Osu"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var osuSettings = builder.Configuration.GetSection("Osu").Get<OsuSettings>()!;

builder.Services.AddDbContext<BlogDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("Database"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IBlogRepository, BlogRepository>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "osu";
    })
    .AddCookie("osu-cookie")
    .AddOAuth("osu", options =>
    {
        options.ClientId = osuSettings.ClientId;
        options.ClientSecret = osuSettings.ClientSecret;
        options.CallbackPath = "/auth/callback";

        options.SignInScheme = "osu-cookie";

        options.AuthorizationEndpoint = "https://osu.ppy.sh/oauth/authorize";
        options.TokenEndpoint = "https://osu.ppy.sh/oauth/token";
        options.UserInformationEndpoint = "https://osu.ppy.sh/api/v2/me";
        
        options.Scope.Add("identify");
        options.SaveTokens = true;
        
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Get, 
                    context.Options.UserInformationEndpoint);

                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    context.AccessToken);
                
                var response = await context.Backchannel.SendAsync(request);
                var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                
                context.RunClaimActions(json.RootElement);
            }
        };
        
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
        options.ClaimActions.MapJsonKey(OsuClaimTypes.AvatarUrl, "avatar_url");
        options.ClaimActions.MapJsonKey(OsuClaimTypes.CountryCode, "country_code");

        options.Validate();
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlogFrontend", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://blog.tsunyoku.xyz")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseCors("AllowBlogFrontend");
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
dbContext.Database.Migrate();

app.Run();