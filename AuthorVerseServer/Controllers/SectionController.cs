using AuthorVerseServer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthorVerseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly IDatabase _redis;
        private readonly ISection _section;
        public SectionController(IDatabase redis, ISection section)
        {
            _redis = redis;
            _section = section;
        }
    }
}
