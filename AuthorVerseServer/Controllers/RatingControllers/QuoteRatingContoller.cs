using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers.BaseRating
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteRatingController : BaseRatingController
    {
        public QuoteRatingController(ICommentRating rating, CreateJWTtokenService jWTtokenService)
            : base(rating, jWTtokenService, Data.Enums.RatingEntityType.Quote)
        {
        }
    }
}
