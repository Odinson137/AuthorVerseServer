using AuthorVerseForum.Data;
using AuthorVerseForum.Hubs;
using AuthorVerseForum.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.

services.AddScoped<UncodingJwtoken>();
services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379,abortConnect=false"));
services.AddHttpClient();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddSignalR();


services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});



services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379,abortConnect=false";
    options.InstanceName = "RedisCache";
});

services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
    };
});

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowReactApp");

app.MapHub<ForumHub>("/forum");
app.Run();
