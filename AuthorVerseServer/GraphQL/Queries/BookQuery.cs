using AuthorVerseServer.Data;
using AuthorVerseServer.GraphQL.Types;
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

        return context.Books.ProjectTo<QueryBookDTO>(_mapper.ConfigurationProvider);
    }
    
    [GraphQLName("pagingBooks")]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<QueryBookDTO> GetPagingBooks([Service] DataContext context)
    {
        _logger.LogInformation("get books");

        return context.Books.ProjectTo<QueryBookDTO>(_mapper.ConfigurationProvider);
    }
   
}
