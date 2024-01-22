using AuthorVerseServer.Models;

namespace AuthorVerseServer.GraphQL.Types;

public class QueryUserDTO
{
    public QueryUserDTO()
    {
        
    }

    public QueryUserDTO(User user)
    {

        UserName = user.UserName;
        Name = user.Name;
        LastName = user.LastName;
        LogoUrl = user.LogoUrl;
        Description = user.Description;
        Email = user.Email;
    }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
}