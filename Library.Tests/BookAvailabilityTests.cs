using Library.Domain.Models;
using Library.Infrastructure;
using Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests;

public class BookAvailabilityTests
{
    [Fact]
    public async Task GetAvailableBooks_ReturnsOnlyNotLoaned()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_BookAvailability").Options;
        using var context = new LibraryContext(options);
        var repo = new BookRepository(context);
        context.Books.Add(new Book { Title = "B1", AuthorId = 1, IsLoaned = false });
        context.Books.Add(new Book { Title = "B2", AuthorId = 1, IsLoaned = true });
        context.SaveChanges();
        var available = await repo.GetAvailableAsync();
        Assert.Single(available);
        Assert.Equal("B1", available.First().Title);
    }
}
