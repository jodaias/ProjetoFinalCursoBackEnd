using Library.Domain.Models;
using Library.Domain.Interfaces.Repositories;
using Library.Infrastructure;
using Library.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests;

public class LoanRepositoryTests
{
    private readonly LoanRepository _repo;
    private readonly LibraryContext _context;

    public LoanRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Loan").Options;
        _context = new LibraryContext(options);
        _repo = new LoanRepository(_context);
    }

    [Fact]
    public async Task AddLoan_AddsLoan()
    {
        var author = new Author { Name = "Test Author" };
        _context.Authors.Add(author);
        _context.SaveChanges();
        var book = new Book { Title = "Test Book", AuthorId = author.Id };
        _context.Books.Add(book);
        _context.SaveChanges();
        var loan = new Loan { BookId = book.Id, BorrowerName = "Test User" };
        await _repo.AddAsync(loan);
        var found = await _repo.GetByIdAsync(loan.Id);
        Assert.NotNull(found);
        Assert.Equal("Test User", found!.BorrowerName);
    }

    [Fact]
    public async Task GetAllLoans_ReturnsAll()
    {
        var author = new Author { Name = "A1" };
        _context.Authors.Add(author);
        _context.SaveChanges();
        var book = new Book { Title = "B1", AuthorId = author.Id };
        _context.Books.Add(book);
        _context.SaveChanges();
        _context.Loans.Add(new Loan { BookId = book.Id, BorrowerName = "L1" });
        _context.Loans.Add(new Loan { BookId = book.Id, BorrowerName = "L2" });
        _context.SaveChanges();
        var all = await _repo.GetAllAsync();
        Assert.True(all.Count() >= 2);
    }
}
