using Library.Domain.Models;
using Library.Domain.Interfaces.Repositories;
using Library.Infrastructure;
using Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests;

public class BookRepositoryTests
{
    private readonly BookRepository _repo;
    private readonly LibraryContext _context;

    public BookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Book").Options;
        _context = new LibraryContext(options);
        _repo = new BookRepository(_context);
    }

    [Fact]
    public async Task AddBook_AddsBook()
    {
        var author = new Author { Name = "Test Author" };
        _context.Authors.Add(author);
        _context.SaveChanges();
        var book = new Book { Title = "Test Book", AuthorId = author.Id };
        await _repo.AddAsync(book);
        var found = await _repo.GetByIdAsync(book.Id);
        Assert.NotNull(found);
        Assert.Equal("Test Book", found!.Title);
    }

    [Fact]
    public async Task GetAllBooks_ReturnsAll()
    {
        var author = new Author { Name = "A1" };
        _context.Authors.Add(author);
        _context.SaveChanges();
        _context.Books.Add(new Book { Title = "B1", AuthorId = author.Id });
        _context.Books.Add(new Book { Title = "B2", AuthorId = author.Id });
        _context.SaveChanges();
        var all = await _repo.GetAllAsync();
        Assert.True(all.Count() >= 2);
    }
}
