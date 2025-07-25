using System.Net.Http.Json;
using Library.Api.Dtos;
using Xunit;

namespace Library.Tests;

public class LoanApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoanApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Loan_CreatesLoanAndBookIsLoaned()
    {
        // Cria author e book
        var author = new CreateAuthorDto { Name = "Author Loan" };
        var authorResp = await _client.PostAsJsonAsync("/authors", author);
        var authorObj = await authorResp.Content.ReadFromJsonAsync<AuthorDto>();
        var book = new CreateBookDto { Title = "Book Loan", AuthorId = authorObj!.Id };
        var bookResp = await _client.PostAsJsonAsync("/books", book);
        var bookObj = await bookResp.Content.ReadFromJsonAsync<BookDto>();
        // Realiza empréstimo
        var loan = new CreateLoanDto { BookId = bookObj!.Id, BorrowerName = "Fulano" };
        var loanResp = await _client.PostAsJsonAsync("/loans", loan);
        loanResp.EnsureSuccessStatusCode();
        var loanObj = await loanResp.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(loanObj);
        // Verifica book emprestado
        var bookGet = await _client.GetFromJsonAsync<BookDto>($"/books/{bookObj.Id}");
        Assert.True(bookGet!.IsLoaned);
    }

    [Fact]
    public async Task Post_Return_ReturnsBook()
    {
        // Cria author, book e empréstimo
        var author = new CreateAuthorDto { Name = "Author Loan" };
        var authorResp = await _client.PostAsJsonAsync("/authors", author);
        var authorObj = await authorResp.Content.ReadFromJsonAsync<AuthorDto>();
        var book = new CreateBookDto { Title = "Book Loan", AuthorId = authorObj!.Id };
        var bookResp = await _client.PostAsJsonAsync("/books", book);
        var bookObj = await bookResp.Content.ReadFromJsonAsync<BookDto>();
        var loan = new CreateLoanDto { BookId = bookObj!.Id, BorrowerName = "Fulano" };
        var loanResp = await _client.PostAsJsonAsync("/loans", loan);
        var loanObj = await loanResp.Content.ReadFromJsonAsync<LoanDto>();
        // Devolve book
        var returnResp = await _client.PostAsync($"/loans/{loanObj!.Id}/return", null);
        returnResp.EnsureSuccessStatusCode();
        var returnedLoan = await returnResp.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(returnedLoan!.ReturnDate);
        // Verifica book disponível
        var bookAvailable = await _client.GetFromJsonAsync<BookDto>($"/books/{bookObj.Id}");
        Assert.False(bookAvailable!.IsLoaned);
    }
}
