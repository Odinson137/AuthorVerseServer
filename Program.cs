using Microsoft.EntityFrameworkCore;
using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Repository;
using Microsoft.AspNetCore.Identity;
using AuthorVerseServer.Models;
using AuthorVerseServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddControllers();
services.AddScoped<IChapterSection, ChapterSectionRepository>();
services.AddScoped<IComment, CommentRepository>();
services.AddScoped<IBookChapter, BookChapterRepository>();
services.AddScoped<IGenre, GenreRepository>();
services.AddScoped<IUserSelectedBook, UserSelectedBookRepository>();
services.AddScoped<ISectionChoice, SectionChoiceRepository>();
services.AddScoped<IBook, BookRepository>();
services.AddScoped<ICharacter, CharacterRepository>();
services.AddScoped<INote, NoteRepository>();
services.AddScoped<IUser, UserRepository>();
/*builder.Services.AddScoped<IFriendship, FriendshipRepository>();*/
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(connectionString);
});


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

services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

await Seed.SeedData(app);

var scope = app.Services.CreateScope();
var scopedContext = scope.ServiceProvider.GetRequiredService<DataContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();

app.MapControllers();

app.UseCors(policyName);

app.Run();
