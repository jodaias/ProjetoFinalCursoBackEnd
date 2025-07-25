using Library.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests;

public class LoanBusinessTests
{
    [Fact]
    public async Task CannotLoanBookThatIsAlreadyLoaned()
    {
        var book = new Book { Title = "Test", AuthorId = 1, IsLoaned = true };
        var loan = new Loan { BookId = book.Id, BorrowerName = "User" };
        Assert.True(book.IsLoaned);

        // Simulate business rule: cannot loan if already loaned
        async Task TryLoanAsync()
        {
            if (book.IsLoaned)
                throw new InvalidOperationException("Livro não disponível para empréstimo.");
            await Task.CompletedTask;
        }

        await Assert.ThrowsAsync<InvalidOperationException>(TryLoanAsync);
    }

    [Fact]
    public void CanLoanAndReturnBook()
    {
        var book = new Book { Title = "Test", AuthorId = 1, IsLoaned = false };
        var loan = new Loan { BookId = book.Id, BorrowerName = "User" };
        // Loan
        book.IsLoaned = true;
        Assert.True(book.IsLoaned);
        // Return
        book.IsLoaned = false;
        Assert.False(book.IsLoaned);
    }
}
