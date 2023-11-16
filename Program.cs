using Microsoft.EntityFrameworkCore;
using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Repository;
using Microsoft.AspNetCore.Identity;
using AuthorVerseServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthorVerseServer.Services;
using AuthorVerseServer.Interfaces.ServiceInterfaces;
using Microsoft.OpenApi.Models;
using AuthorVerseServer.Filters;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddControllers();
services.AddScoped<IChapterSection, ChapterSectionRepository>();
services.AddScoped<IComment, CommentRepository>();
services.AddScoped<IBookChapter, BookChapterRepository>();
services.AddScoped<IGenre, GenreRepository>();
services.AddScoped<IAccount, AccountRepository>();
services.AddScoped<ISectionChoice, SectionChoiceRepository>();
services.AddScoped<IBook, BookRepository>();
services.AddScoped<ICharacter, CharacterRepository>();
services.AddScoped<INote, NoteRepository>();
services.AddScoped<IUser, UserRepository>();
services.AddScoped<ITag, TagRepository>();

services.AddScoped<ILoadImage, LoadImageService>();
services.AddTransient<MailService>();
services.AddTransient<GenerateRandomName>();
services.AddSingleton<CreateJWTtokenService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "Author Verse", Version = "v1" });
    s.OperationFilter<SwaggerAuthorizedMiddleware>();
});

//string redisConnection = builder.Configuration.GetConnectionString("Redis");

//services.AddStackExchangeRedisCache(redisOptions =>
//{
//    string connection = redisConnection;
//    redisOptions.Configuration = connection;
//});


//services.AddSingleton<IDatabase>(provider =>
//{
//    var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnection);
//    return connectionMultiplexer.GetDatabase();
//});

services.AddMemoryCache();


//var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

var connectionString = $"Data Source=app-db;Initial Catalog=AuthorVerseDb;User ID=sa;Password=PaASasssword@#Q@123456";
services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(connectionString);
});

//services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(databaseName: "InMemoryDatabase"));

var policyName = "AllowReactApp";
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

services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        var builtInFactory = options.InvalidModelStateResponseFactory;

        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<Program>>();

            return builtInFactory(context);
        };
    });


services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
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

services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    await Seed.SeedData(dataContext, roleManager, userManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();

app.MapControllers();

app.UseCors(policyName);

app.Run();
public partial class Program { }