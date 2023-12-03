using AuthorVerseForum.Data;
using AuthorVerseForum.Hubs;
using AuthorVerseForum.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.

services.AddScoped<UncodingJwtoken>();
services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379,abortConnect=false"));
//services.AddSingleton<IConnectionManager, ConnectionManager>(); // можно сделать Transient

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddSignalR();

services.AddSwaggerGen();

ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

//services.AddDbContext<DataContext>(options =>
//{
//    //#if !DEBUG
//    //#else
//    //    string str = $"Server=localhost;Initial Catalog=AuthorVerseDb;Persist Security Info=False;User ID=sa;Password=S3cur3P@ssW0rd!;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=True";
//    //#endif

//    string str = $"Server=db;Initial Catalog=AuthorVerseForumDb;Persist Security Info=False;User ID=sa;Password=S3cur3P@ssW0rd!;Encrypt=False;TrustServerCertificate=False;Connection Timeout=50;MultipleActiveResultSets=True";
//    options.UseSqlServer(str);
//});

//#if !DEBUG

//services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379,abortConnect=false"));

//services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = "redis:6379,abortConnect=false";
//    options.InstanceName = "RedisCache";
//});


//#else

//services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));

services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379,abortConnect=false";
    options.InstanceName = "RedisCache";
});


//#endif


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


var app = builder.Build();

//#if !DEBUG

//using (var scope = app.Services.CreateScope())
//{
//    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
//    await Seed.SeedData(dataContext);
//}

//#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowReactApp");

app.MapHub<ForumHub>("/forum");
app.Run();
