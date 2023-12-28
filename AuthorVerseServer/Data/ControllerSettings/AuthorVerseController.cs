using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Data.ControllerSettings
{
    public class AuthorVerseController() : ControllerBase
    {
        public string UserId => User.Identities.First().Claims.First().Value;

    }
}
