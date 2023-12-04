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
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


// Add services to the container.
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
services.AddScoped<IQuote, QuoteRepository>();
services.AddScoped<IForumMessage, ForumMessageRepository>();

services.AddScoped<ILoadImage, LoadImageService>();
services.AddTransient<MailService>();
services.AddTransient<GenerateRandomNameService>();
services.AddSingleton<CreateJWTtokenService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

services.AddControllers(options =>
    {
        options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        var builtInFactory = options.InvalidModelStateResponseFactory;

        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<Program>>();

            return builtInFactory(context);
        };
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;

        options.SerializerSettings.Error = (sender, args) =>
        {
            args.ErrorContext.Handled = true;
        };

        options.SerializerSettings.Error += (sender, args) =>
        {
            args.ErrorContext.Handled = true;
        };
    });

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme.",

        Scheme = "bearer",
        BearerFormat = "JWT",

        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
});

ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(); 
    builder.AddDebug();
});



services.AddDbContext<DataContext>(options =>
{
#if !DEBUG
    string str = $"Server=db;Initial Catalog=AuthorVerseDb;Persist Security Info=False;User ID=sa;Password=S3cur3P@ssW0rd!;Encrypt=False;TrustServerCertificate=False;Connection Timeout=50;MultipleActiveResultSets=True";
#else
    string str = $"Server=localhost;Initial Catalog=AuthorVerseDb;Persist Security Info=False;User ID=sa;Password=S3cur3P@ssW0rd!;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=True";
#endif

    options.UseSqlServer(str);
});

#if !DEBUG

services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379,abortConnect=false"));

services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379,abortConnect=false";
    options.InstanceName = "RedisCache";
});


#else

services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));

services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379,abortConnect=false";
    options.InstanceName = "RedisCache";
});


#endif


services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    options.AddPolicy("AllowForumApp", builder =>
    {
        builder.WithOrigins("http://localhost:7069/")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
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

//#if !DEBUG

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    await Seed.SeedData(dataContext, roleManager, userManager);
}

//#endif

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

app.UseCors("AllowReactApp");
app.UseCors("AllowForumApp");

app.Run();
public partial class Program { }