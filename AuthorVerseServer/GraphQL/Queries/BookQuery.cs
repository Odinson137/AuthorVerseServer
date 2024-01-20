using System.Data.Entity;
using AuthorVerseServer.Data;
using AuthorVerseServer.DTO;
using AuthorVerseServer.GraphQL.Types;
using AuthorVerseServer.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace AuthorVerseServer.GraphQL.Queries;

public class BookQuery 
{
    private readonly ILogger<BookQuery> _logger;
    private readonly IMapper _mapper;
    
    public BookQuery(ILogger<BookQuery> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    [GraphQLName("books")]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<QueryBookDTO> GetBooks([Service] DataContext context)
    {
        _logger.LogInformation("get books");

        return context.Books.Select(c => new QueryBookDTO
        {
            BookId = c.BookId,
            Title = c.Title,
            Description = c.Description,
            Author = new QueryUserDTO()
            {
                UserName = c.Author.UserName,
                Name = c.Author.Name,
                LastName = c.Author.LastName,
                LogoUrl = c.Author.LogoUrl,
                Description = c.Author.Description,
                Email = c.Author.Email
            },
            Tags = c.Tags,
            Genres = c.Genres,
            Comments = c.Comments,
            Arts = c.Arts,
            Characters = c.Characters,
            BookChapters = c.BookChapters,
            ForumMessages = c.ForumMessages,
            UserSelectedBooks = c.UserSelectedBooks,
            BookCover = c.BookCover,
            BookQuotes = c.BookQuotes,
            Permission = c.Permission,
            NormalizedTitle = c.NormalizedTitle,
            AuthorId = c.AuthorId,
            Rating = c.Rating,
            AgeRating = c.AgeRating,
            Earnings = c.Earnings,
            CountRating = c.CountRating,
            PublicationData = c.PublicationData,
        });
    }
    
    [GraphQLName("pagingBooks")]
    [UsePaging]
    [UseProjection]
    public IQueryable<QueryBookDTO> GetPagingBooks([Service] DataContext context)
    {
        _logger.LogInformation("get page books");

        return context.Books.Select(c => new QueryBookDTO
        {
            BookId = c.BookId,
            Title = c.Title,
            Description = c.Description,
            Author = new QueryUserDTO()
            {
                UserName = c.Author.UserName,
                Name = c.Author.Name,
                LastName = c.Author.LastName,
                LogoUrl = c.Author.LogoUrl,
                Description = c.Author.Description,
                Email = c.Author.Email
            },
            Tags = c.Tags,
            Genres = c.Genres,
            Comments = c.Comments,
            Arts = c.Arts,
            Characters = c.Characters,
            BookChapters = c.BookChapters,
            ForumMessages = c.ForumMessages,
            UserSelectedBooks = c.UserSelectedBooks,
            BookCover = c.BookCover,
            BookQuotes = c.BookQuotes,
            Permission = c.Permission,
            NormalizedTitle = c.NormalizedTitle,
            AuthorId = c.AuthorId,
            Rating = c.Rating,
            AgeRating = c.AgeRating,
            Earnings = c.Earnings,
            CountRating = c.CountRating,
            PublicationData = c.PublicationData,
        });
    }
}
