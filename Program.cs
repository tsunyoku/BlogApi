using BlogApi;
using BlogApi.Abstractions;
using BlogApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<BlogDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("Database"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IBlogDbContext>(sp => sp.GetRequiredService<BlogDbContext>());
builder.Services.AddScoped<IBlogRepository, BlogRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // apply migrations
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseExceptionHandler();

app.Run();