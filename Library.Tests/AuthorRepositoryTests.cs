using Library.Domain.Models;
using Library.Domain.Interfaces.Repositories;
using Library.Infrastructure;
using Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests;

public class AuthorRepositoryTests
{
    private readonly AuthorRepository _repo;
    private readonly LibraryContext _context;

    public AuthorRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Author").Options;
        _context = new LibraryContext(options);
        _repo = new AuthorRepository(_context);
    }

    [Fact]
    public async Task AddAuthor_AddsAuthor()
    {
        var author = new Author { Name = "Test Author" };
        await _repo.AddAsync(author);
        var found = await _repo.GetByIdAsync(author.Id);
        Assert.NotNull(found);
        Assert.Equal("Test Author", found!.Name);
    }

    [Fact]
    public async Task GetAllAuthors_ReturnsAll()
    {
        _context.Authors.Add(new Author { Name = "A1" });
        _context.Authors.Add(new Author { Name = "A2" });
        _context.SaveChanges();
        var all = await _repo.GetAllAsync();
        Assert.True(all.Count() >= 2);
    }
}
