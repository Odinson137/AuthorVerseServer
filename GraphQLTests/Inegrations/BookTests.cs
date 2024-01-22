using AuthorVerseServer.Services;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQLTests.Inegrations;

public class BookTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly GraphQLHttpClient _graphQLClient;

    public BookTests(WebApplicationFactory<Program> factory)
    {
        _graphQLClient =
            new GraphQLHttpClient("http://localhost:7069/graphql",
                new NewtonsoftJsonSerializer());
     
        var tokenService = factory.Services.GetRequiredService<CreateJWTtokenService>();
        var token = tokenService.GenerateAdminJwtToken("admin");

        _graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }
    
    [Fact]
    public async Task GetBooks_WithFilter_ReturnsFilteredBooks()
    {
        // Arrange
        var query = @"
            query {
                books(where: {bookId: {lt: 11}}) {
                    bookId
                    title
                    authorUserName
                }
            }";

        // Act
        var response = await _graphQLClient.SendQueryAsync<dynamic>(query);

        // Assert
        Assert.NotNull(response.Data);
        Assert.Null(response.Errors);

        var books = response.Data["books"];
        Assert.NotNull(books);
        // Проверка наличия полей в каждой книге
        foreach (var book in books)
        {
            Assert.NotNull(book["bookId"]);
            Assert.NotNull(book["title"]);
            Assert.NotNull(book["authorUserName"]);
        }
    }

    [Fact]
    public async Task CreateBook_SuccessfullyCreated_ReturnsBookId()
    {
        // Arrange
        var mutation = @"
            mutation {
                createBook(createBook: {
                    title: ""hello world"",
                    description: ""a big description for my tests book and with some strange letters sadfsdfasdsgdsdfsdfsdgdfg"",
                    ageRating: ALL,
                    genresId: [1, 2, 3],
                    tagsId: [1, 2, 3]
                }) 
            }";

        // Act
        var response = await _graphQLClient.SendMutationAsync<dynamic>(mutation);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);
        var bookId = response.Data["createBook"];
        Assert.NotNull(bookId);
    }
    
    [Fact]
    public async Task PatchBook_SuccessfullyUpdated_ReturnsBookId()
    {
        // Arrange
        var mutation = @"
        mutation {
            patchBook(bookId: 2, 
                updateBook: {
                    title: ""new title for book""
                    ageRating: SIX_PLUS,
                    genresId: [1, 2, 3, 4, 5],
                    tagsId: [1, 2, 3, 4, 5]
                }
            )
        }";

        // Act
        var response = await _graphQLClient.SendMutationAsync<dynamic>(mutation);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);
        var bookId = response.Data["patchBook"];
        Assert.NotNull(bookId);
    }


    //
    // [Fact]
    // public async Task LoadBookImage_WithFile_SuccessfullyUploaded_ReturnsBookId()
    // {
    //     // Arrange
    //     var mutation = @"
    //     mutation ($file: Upload!) {
    //         image(bookImage: $file)
    //     }";
    //
    //     var request = new GraphQLHttpRequest
    //     {
    //         Query = mutation,
    //         Variables = new
    //         {
    //             file = "javascript-it-юмор-geek-5682739.jpeg"
    //         }
    //     };
    //
    //     using var formData = new MultipartFormDataContent();
    //     formData.Add(
    //         new ByteArrayContent(File.ReadAllBytes("../../../Images/javascript-it-юмор-geek-5682739.jpeg")), "file",
    //         "javascript-it-юмор-geek-5682739.jpeg");
    //
    //     request.Variables = formData;
    //     
    //     // Act
    //     var response = await _graphQLClient.SendMutationAsync<dynamic>(request);
    //
    //     // Assert
    //     Assert.Null(response.Errors);
    //     Assert.NotNull(response.Data);
    //     var bookId = response.Data["image"];
    //     Assert.NotNull(bookId);
    // }
}