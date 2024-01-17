using AuthorVerseServer.DTO;
using Grpc.Net.Client;
using GrpcClientApp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ServerTests.Integrations
{
    public class ForumServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Forum.ForumClient _client;
        private readonly IDatabase _redis;

        public ForumServiceIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5288");

            var redisConnection = factory.Services.GetRequiredService<IConnectionMultiplexer>();
            _redis = redisConnection.GetDatabase();

            _client = new Forum.ForumClient(channel);
        }
        
        [Fact]
        public async Task AddMessage_AddToNullParent_ReturnsOkResult()
        {
            // Arrange
            string newGuid = Guid.NewGuid().ToString();
            string key = $"add_message:{newGuid}";

            SendForumMessageDTO sendMessage = new SendForumMessageDTO
            {
                BookId = 1,
                Text = "Hello",
                UserId = "admin",
                AnswerId = null,
            };
    
            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(sendMessage), TimeSpan.FromSeconds(10));

            var response = _client.InsertMessage(new Request
            {
                Key = newGuid,
            });

            Assert.NotNull(response);
            Assert.True(response.MessageId > 0);
        }
        
        [Fact]
        public async Task AddMessage_AddWithParent_ReturnsOkResult()
        {
            // Arrange
            Guid newGuid = Guid.NewGuid();
            string key = $"add_message:{newGuid}";

            SendForumMessageDTO sendMessage = new SendForumMessageDTO
            {
                BookId = 1,
                Text = "Hello",
                UserId = "admin",
                AnswerId = 1,
            };

            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(sendMessage), TimeSpan.FromSeconds(10));

            var response = _client.InsertMessage(new Request
            {
                Key = newGuid.ToString(),
            });

            Assert.NotNull(response);
            Assert.True(response.MessageId > 0);
        }
        
        [Fact]
        public async Task PutMessage_CheckOkRequest_ReturnsOkResult()
        {
            // Arrange
            Guid newGuid = Guid.NewGuid();
            string key = $"put_message:{newGuid}";

            var changeTextMessage = new ChangeTextDTO
            {
                MessageId = 1,
                NewText = "Hello from new message text",
            };

            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(changeTextMessage), TimeSpan.FromSeconds(10));

            var response = _client.PatchMessageAsync(new Request
            {
                Key = newGuid.ToString(),
            });

            Assert.NotNull(response);
        }

        private async Task<int> GetNewMessageIdAsync(int? answerId = null)
        {
            string newGuid = Guid.NewGuid().ToString();
            string key = $"add_message:{newGuid}";

            SendForumMessageDTO sendMessage = new SendForumMessageDTO
            {
                BookId = 1,
                Text = "Hello",
                UserId = "admin",
                AnswerId = answerId,
            };

            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(sendMessage), TimeSpan.FromSeconds(10));

            var response = _client.InsertMessage(new Request
            {
                Key = newGuid,
            });

            return response.MessageId;
        }

        [Fact]
        public async Task Delete_CheckOkRequest_ReturnsOkResult()
        {
            // Arrange
            int messageId = await GetNewMessageIdAsync();

            Guid newGuid = Guid.NewGuid();
            string key = $"delete_message:{newGuid}";

            var changeTextMessage = new DeleteMessageDTO
            {
                MessageId = messageId,
                UserId = "admin",
            };

            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(changeTextMessage), TimeSpan.FromSeconds(10));

            var response = _client.DeleteMessage(new Request
            {
                Key = newGuid.ToString(),
            });

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Delete_CheckOkRequestWithParent_ReturnsOkResult()
        {
            // Arrange
            var messageId = await GetNewMessageIdAsync();
            await GetNewMessageIdAsync(messageId);

            var newGuid = Guid.NewGuid();
            var key = $"delete_message:{newGuid}";

            var changeTextMessage = new DeleteMessageDTO
            {
                MessageId = messageId,
                UserId = "admin",
            };

            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(changeTextMessage), TimeSpan.FromSeconds(10));

            var response = _client.DeleteMessage(new Request
            {
                Key = newGuid.ToString(),
            });

            Assert.NotNull(response);
        }



    }
}
