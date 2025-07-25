using System.Net.Http.Json;
using Library.Api.Dtos;
using Xunit;

namespace Library.Tests;

public class BookApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Book_CreatesBook()
    {
        // Cria author
        var author = new CreateAuthorDto { Name = "Author Book API" };
        var authorResp = await _client.PostAsJsonAsync("/authors", author);
        var authorObj = await authorResp.Content.ReadFromJsonAsync<AuthorDto>();
        // Cria book
        var book = new CreateBookDto { Title = "Book API", AuthorId = authorObj!.Id };
        var postResp = await _client.PostAsJsonAsync("/books", book);
        postResp.EnsureSuccessStatusCode();
        var created = await postResp.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);
        Assert.Equal("Book API", created!.Title);
    }

    [Fact]
    public async Task Get_Book_ReturnsBook()
    {
        var author = new CreateAuthorDto { Name = "Author Book API" };
        var authorResp = await _client.PostAsJsonAsync("/authors", author);
        var authorObj = await authorResp.Content.ReadFromJsonAsync<AuthorDto>();
        var book = new CreateBookDto { Title = "Book API", AuthorId = authorObj!.Id };
        var postResp = await _client.PostAsJsonAsync("/books", book);
        var created = await postResp.Content.ReadFromJsonAsync<BookDto>();
        var getResp = await _client.GetAsync($"/books/{created!.Id}");
        getResp.EnsureSuccessStatusCode();
        var fetched = await getResp.Content.ReadFromJsonAsync<BookDto>();
        Assert.Equal("Book API", fetched!.Title);
    }
}
