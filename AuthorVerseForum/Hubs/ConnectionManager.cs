using AuthorVerseForum.Interfaces;

namespace AuthorVerseForum.Hubs
{
    public class ConnectionManager : IConnectionManager
    {

        public ConnectionManager()
        {
            
        }

        public Task<bool> AddUserIdAsync(string userId, string userConnectionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FindUserIdAsync(string userId, string userConnectionId)
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync()
        {
            throw new NotImplementedException();
        }
    }
}
