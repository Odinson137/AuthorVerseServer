namespace AuthorVerseForum.Interfaces
{
    public interface IConnectionManager
    {
        public Task SendMessageAsync();
        public Task<bool> AddUserIdAsync(string userId, string userConnectionId);
        public Task<bool> FindUserIdAsync(string userId, string userConnectionId);
        public Task<bool> DeleteUserByIdAsync(string userId);
    }
}
