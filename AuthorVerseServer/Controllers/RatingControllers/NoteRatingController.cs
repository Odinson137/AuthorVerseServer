using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers.BaseRating
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NoteRatingController : BaseRatingController
    {
        public NoteRatingController(ICommentRating rating, CreateJWTtokenService jWTtokenService) 
            : base(rating, jWTtokenService, Data.Enums.RatingEntityType.Note)
        {
        }
    }
}
