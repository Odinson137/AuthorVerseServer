using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorVerseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        
        public ICollection<Book> GetBooks()
        {
            return new List<Book>() { new Book() };
        }

        
    }
}
