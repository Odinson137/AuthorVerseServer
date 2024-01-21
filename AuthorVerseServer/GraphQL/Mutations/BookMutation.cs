using System.Security.Claims;
using AuthorVerseServer.Data;
using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.GraphQL.Types;
using AuthorVerseServer.Models;
using AuthorVerseServer.Services;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using CreateBookDTO = AuthorVerseServer.DTO.CreateBookDTO;

namespace AuthorVerseServer.GraphQL.Mutations;

public class BookMutation
{
    private readonly ILogger<BookMutation> _logger;
    private readonly LoadFileService _loadFile;
    public BookMutation(ILogger<BookMutation> logger, LoadFileService loadFile)
    {
        _logger = logger;
        _loadFile = loadFile;
    }
    
    [Authorize]
    [GraphQLName("createBook")]
    public async Task<int> CreateBook(
        [Service] DataContext context, 
        CreateBookGraphDTO createBook, 
        ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create book");

        var userId = claimsPrincipal.Claims.First().Value;

        if (createBook.Title == null || createBook.Title.Length > 200)
            throw new Exception("Title's length is too long");

        if (createBook.Description == null || createBook.Description.Length is > 1000 or < 50)
            throw new Exception("Description's length is not correct");

        if (createBook.GenresId is { Count: < 3 })
            throw new Exception("Genres are not select");

        if (createBook.TagsId is { Count: < 3 })
            throw new Exception("Tags are not select");

        if (createBook.GenresId!.Distinct().Count() != createBook.GenresId!.Count)
            throw new Exception("Genres have duplicate");
        
        if (createBook.TagsId!.Distinct().Count() != createBook.TagsId!.Count)
            throw new Exception("Tags have duplicate");
        
        if (createBook.AgeRating == AgeRating.NotSelected)
            throw new ArgumentException("The wrong age rating");

        Book book = new()
        {
            AuthorId = userId,
            Title = createBook.Title,
            Description = createBook.Description,
            AgeRating = createBook.AgeRating,
        };

        foreach (var genreId in createBook.GenresId!)
        {
            var genre = await context.Genres.FindAsync(genreId, cancellationToken);

            if (genre != null)
            {
                book.Genres.Add(genre);
            } else
            {
                throw new Exception("Genre not found");
            }
        }

        foreach (var tagId in createBook.TagsId!)
        {
            var tag = await context.Tags.FindAsync(tagId, cancellationToken);

            if (tag != null)
            {
                book.Tags.Add(tag);
            }
            else
            {
                throw new Exception("Tag not found");
            }
        }

        await context.AddAsync(book, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return book.BookId;
    }
    
    [Authorize]
    [GraphQLName("patchBook")]
    public async Task<int> UpdateBook(
        [Service] DataContext context, 
        int bookId,
        UpdateBookGraphDTO updateBook, 
        ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update book");

        var userId = claimsPrincipal.Claims.First().Value;

        var book = await context.Books
            .Where(b => b.BookId == bookId)
            .Include(b => b.Genres)
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (book == null)
        {
            throw new ArgumentException("Book not found");
        }

        if (book.AuthorId != userId)
        {
            throw new Exception("You are not the author");
        }
        
        if (!string.IsNullOrEmpty(updateBook.Title))
        {
            book.Title = updateBook.Title;
        }
        
        if (!string.IsNullOrEmpty(updateBook.Description))
        {
            book.Description = updateBook.Description;
        }

        if (updateBook.TagsId != null)
        {
            if (updateBook.TagsId.Count < 3)
            {
                throw new ArgumentException("The tag's count must be at least 3");
            } 

            var tagsId = book.Tags.Select(c => c.TagId).ToList();
            foreach (var tagId in updateBook.TagsId)
            {
                if (tagsId.Remove(tagId)) continue;
                var tag = await context.Tags.FindAsync(tagId, cancellationToken);

                if (tag != null)
                {
                    book.Tags.Add(tag);
                }
                else
                {
                    throw new Exception("Tag not found");
                }

            }

            foreach (var tagId in tagsId)
            {
                book.Tags.Remove(book.Tags.First(c => c.TagId == tagId));
            }
        }
        
        if (updateBook.GenresId != null)                                                                  
        {                                                                                               
            if (updateBook.GenresId.Count < 3)                                                            
            {                                                                                           
                throw new ArgumentException("The genre's count must be at least 3");                      
            }                                                                                           
                                                                                                        
            var genresId = book.Genres.Select(c => c.GenreId).ToList();                                       
            foreach (var genreId in updateBook.GenresId)                                                    
            {                                                                                           
                if (genresId.Remove(genreId)) continue;                                                   
                var genre = await context.Genres.FindAsync(genreId, cancellationToken);                       
                                                                                                        
                if (genre != null)                                                                        
                {                                                                                       
                    book.Genres.Add(genre);                                                                 
                } else                                                                                  
                {                                                                                       
                    throw new Exception("Genre not found");                                               
                }                                                                                       
            }                                                                                           
                                                                                                        
            foreach (var genreId in genresId)                                                               
            {                                                                                           
                book.Genres.Remove(book.Genres.First(c => c.GenreId == genreId));                               
            }                                                                                           
        }                                                                                               
        
        if (updateBook.AgeRating != AgeRating.NotSelected)
        {
            book.AgeRating = updateBook.AgeRating;
        }

        await context.SaveChangesAsync(cancellationToken);

        return book.BookId;
    }
}