using Microsoft.EntityFrameworkCore;
using AuthorVerseServer.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // В случае, если переменная окружения не установлена, можно использовать стандартное значение.
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

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

var app = builder.Build();

var scope = app.Services.CreateScope();
var scopedContext = scope.ServiceProvider.GetRequiredService<DataContext>();
DataContext.Seed(scopedContext);

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
