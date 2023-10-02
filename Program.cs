using Microsoft.EntityFrameworkCore;
using AuthorVerseServer.Data;
using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Repository;
using Microsoft.AspNetCore.Identity;
using AuthorVerseServer.Models;
using AuthorVerseServer.Enums;
using AuthorVerseServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IChapterSection, ChapterSectionRepository>();
builder.Services.AddScoped<IComment, CommentRepository>();
builder.Services.AddScoped<IBookChapter, BookChapterRepository>();
builder.Services.AddScoped<IGenre, GenreRepository>();
builder.Services.AddScoped<IUserSelectedBook, UserSelectedBookRepository>();
builder.Services.AddScoped<ISectionChoice, SectionChoiceRepository>();
builder.Services.AddScoped<IBook, BookRepository>();
builder.Services.AddScoped<ICharacter, CharacterRepository>();
builder.Services.AddScoped<INote, NoteRepository>();
builder.Services.AddScoped<IUser, UserRepository>();
/*builder.Services.AddScoped<IFriendship, FriendshipRepository>();*/
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(connectionString);
});


var policyName = "AllowReactApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000") 
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
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

builder.Services.AddIdentity<User, IdentityRole>()
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

app.UseAuthorization();

app.MapControllers();

app.UseCors(policyName);

app.Run();
