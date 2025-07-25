using System.Net.Http.Json;
using Library.Api.Dtos;
using Xunit;

namespace Library.Tests;

public class AuthorApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthorApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Author_CreatesAuthor()
    {
        var author = new CreateAuthorDto { Name = "Author API" };
        var postResp = await _client.PostAsJsonAsync("/authors", author);
        postResp.EnsureSuccessStatusCode();
        var created = await postResp.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(created);
        Assert.Equal("Author API", created!.Name);
    }

    [Fact]
    public async Task Get_Author_ReturnsAuthor()
    {
        var author = new CreateAuthorDto { Name = "Author API" };
        var postResp = await _client.PostAsJsonAsync("/authors", author);
        var created = await postResp.Content.ReadFromJsonAsync<AuthorDto>();
        var getResp = await _client.GetAsync($"/authors/{created!.Id}");
        getResp.EnsureSuccessStatusCode();
        var fetched = await getResp.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.Equal("Author API", fetched!.Name);
    }
}
