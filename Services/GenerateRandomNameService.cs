using System.Text;

namespace AuthorVerseServer.Services
{
    public class GenerateRandomNameService
    {
        private static string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        public string GenerateRandomUsername()
        {
            const string prefix = "User";

            Random random = new Random();
            StringBuilder usernameBuilder = new StringBuilder(prefix);

            for (int i = 0; i < 10; i++)
            {
                char randomChar = allowedCharacters[random.Next(allowedCharacters.Length)];
                usernameBuilder.Append(randomChar);
            }

            return usernameBuilder.ToString();
        }

    }
}
