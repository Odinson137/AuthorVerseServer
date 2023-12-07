using AuthorVerseServer.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumHubTests.StressTests
{
    public class ForumStressTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IDatabase _redis;
        private readonly WebApplicationFactory<Program> _factory;
        public ForumStressTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var redisConnection = factory.Services.GetRequiredService<IConnectionMultiplexer>();

            _redis = redisConnection.GetDatabase();
            _client = factory.CreateClient();
        }

        public async Task<string> GetTokenAsync()
        {
            UserLoginDTO user = new UserLoginDTO()
            {
                UserName = "admin",
                Password = "Password@123",
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/User/Login", requestContent);

            return await response.Content.ReadAsStringAsync();
        }


        private async Task<HubConnection> AuthorizeAsync()
        {
            string token = await GetTokenAsync();
            int bookId = 1;

            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:7070/forum")
                .Build();
            await connection.StartAsync();
            await connection.InvokeAsync("AuthorizationConnection", token, bookId);
            return connection;
        }


        [Fact]
        public async Task SendMessage_SendMessageWithNullParrent_Ok()
        {
            // Arrange
            var connection = await AuthorizeAsync();
            string message = "Hello from test";

            Stopwatch stopwatch = new Stopwatch();

            // Начинаем измерение времени
            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
                await connection.InvokeAsync("SendMessage", message, null);
            }

            stopwatch.Stop();

            var a = stopwatch.ElapsedMilliseconds;

            // Выводим результат
            Console.WriteLine($"Время выполнения: {a} миллисекунд");

            Assert.True(connection.State == HubConnectionState.Connected);
        }

    }
}
