using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers.BaseRating
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentRatingController : BaseRatingController
    {
        public CommentRatingController(ICommentRating rating,
            CreateJWTtokenService jWTtokenService) : base(rating, jWTtokenService, Data.Enums.RatingEntityType.Comment)
        {

        }


    }
}
