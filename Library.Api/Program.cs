
using Microsoft.EntityFrameworkCore;
using Library.Api.Dtos;
using Library.Domain.Models;
using Library.Domain.Interfaces.Repositories;
using Library.Infrastructure;
using Library.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Remover AddOpenApi e MapOpenApi, adicionar Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Author endpoints
app.MapGet("/authors", async (IAuthorRepository repo) =>
{
    var authors = await repo.GetAllAsync();
    return Results.Ok(authors.Select(a => new AuthorDto { Id = a.Id, Name = a.Name }));
});
app.MapGet("/authors/{id}", async (int id, IAuthorRepository repo) =>
    await repo.GetByIdAsync(id) is Author author ? Results.Ok(new AuthorDto { Id = author.Id, Name = author.Name }) : Results.NotFound());
app.MapPost("/authors", async (CreateAuthorDto dto, IAuthorRepository repo) =>
{
    var author = new Author { Name = dto.Name };
    await repo.AddAsync(author);
    var result = new AuthorDto { Id = author.Id, Name = author.Name };
    return Results.Created($"/authors/{author.Id}", result);
});
app.MapPut("/authors/{id}", async (int id, Author author, IAuthorRepository repo) =>
{
    if (id != author.Id) return Results.BadRequest();
    await repo.UpdateAsync(author);
    return Results.NoContent();
});
app.MapDelete("/authors/{id}", async (int id, IAuthorRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});
app.MapGet("/authors/{id}/books", async (int id, IAuthorRepository authorRepo, IBookRepository bookRepo) =>
{
    var author = await authorRepo.GetByIdAsync(id);
    if (author == null)
        return Results.NotFound();
    var books = await bookRepo.GetAllAsync();
    var booksByAuthor = books.Where(b => b.AuthorId == id).Select(b => new BookDto
    {
        Id = b.Id,
        Title = b.Title,
        AuthorId = b.AuthorId,
        AuthorName = b.Author?.Name,
        IsLoaned = b.IsLoaned
    });
    return Results.Ok(booksByAuthor);
});

// Book endpoints
app.MapGet("/books", async (IBookRepository repo) =>
{
    var books = await repo.GetAllAsync();
    return Results.Ok(books.Select(b => new BookDto
    {
        Id = b.Id,
        Title = b.Title,
        AuthorId = b.AuthorId,
        AuthorName = b.Author?.Name,
        IsLoaned = b.IsLoaned
    }));
});
app.MapGet("/books/{id}", async (int id, IBookRepository repo) =>
    await repo.GetByIdAsync(id) is Book book ? Results.Ok(new BookDto
    {
        Id = book.Id,
        Title = book.Title,
        AuthorId = book.AuthorId,
        AuthorName = book.Author?.Name,
        IsLoaned = book.IsLoaned
    }) : Results.NotFound());
app.MapPost("/books", async (CreateBookDto dto, IBookRepository repo) =>
{
    var book = new Book { Title = dto.Title, AuthorId = dto.AuthorId };
    await repo.AddAsync(book);
    var b = await repo.GetByIdAsync(book.Id);
    var result = new BookDto
    {
        Id = b!.Id,
        Title = b.Title,
        AuthorId = b.AuthorId,
        AuthorName = b.Author?.Name,
        IsLoaned = b.IsLoaned
    };
    return Results.Created($"/books/{book.Id}", result);
});
app.MapPut("/books/{id}", async (int id, Book book, IBookRepository repo) =>
{
    if (id != book.Id) return Results.BadRequest();
    await repo.UpdateAsync(book);
    return Results.NoContent();
});
app.MapDelete("/books/{id}", async (int id, IBookRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});
app.MapGet("/available-books", async (IBookRepository repo) => Results.Ok(await repo.GetAvailableAsync()));

// Loan endpoints
app.MapGet("/loans", async (ILoanRepository repo) => {
    var loans = await repo.GetAllAsync();
    return Results.Ok(loans.Select(e => new LoanDto {
        Id = e.Id,
        BookId = e.BookId,
        BorrowerName = e.BorrowerName,
        LoanDate = e.LoanDate,
        ReturnDate = e.ReturnDate
    }));
});
app.MapGet("/loans/{id}", async (int id, ILoanRepository repo) =>
    await repo.GetByIdAsync(id) is Loan loan ? Results.Ok(new LoanDto {
        Id = loan.Id,
        BookId = loan.BookId,
        BorrowerName = loan.BorrowerName,
        LoanDate = loan.LoanDate,
        ReturnDate = loan.ReturnDate
    }) : Results.NotFound());
app.MapPost("/loans", async (CreateLoanDto dto, ILoanRepository repo, IBookRepository bookRepo) =>
{
    var book = await bookRepo.GetByIdAsync(dto.BookId);
    if (book == null || book.IsLoaned)
        return Results.BadRequest("Livro não disponível para empréstimo.");
    book.IsLoaned = true;
    await bookRepo.UpdateAsync(book);
    var loan = new Loan { BookId = dto.BookId, BorrowerName = dto.BorrowerName, LoanDate = DateTime.Now, ReturnDate = null };
    await repo.AddAsync(loan);
    var result = new LoanDto {
        Id = loan.Id,
        BookId = loan.BookId,
        BorrowerName = loan.BorrowerName,
        LoanDate = loan.LoanDate,
        ReturnDate = loan.ReturnDate
    };
    return Results.Created($"/loans/{loan.Id}", result);
});
app.MapPost("/loans/{id}/return", async (int id, ILoanRepository repo, IBookRepository bookRepo) =>
{
    var loan = await repo.GetByIdAsync(id);
    if (loan == null || loan.ReturnDate != null)
        return Results.BadRequest("Empréstimo não encontrado ou já devolvido.");
    loan.ReturnDate = DateTime.Now;
    await repo.UpdateAsync(loan);
    var book = await bookRepo.GetByIdAsync(loan.BookId);
    if (book != null)
    {
        book.IsLoaned = false;
        await bookRepo.UpdateAsync(book);
    }
    var result = new LoanDto {
        Id = loan.Id,
        BookId = loan.BookId,
        BorrowerName = loan.BorrowerName,
        LoanDate = loan.LoanDate,
        ReturnDate = loan.ReturnDate
    };
    return Results.Ok(result);
});

// Seed de dados iniciais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    db.Database.EnsureCreated();
    if (!db.Authors.Any())
    {
        var authors = new[]
        {
            new Author { Name = "Robert C. Martin" },
            new Author { Name = "Martin Fowler" },
            new Author { Name = "Eric Evans" },
            new Author { Name = "Kent Beck" },
            new Author { Name = "Andrew S. Tanenbaum" }
        };
        db.Authors.AddRange(authors);
        db.SaveChanges();

        var books = new[]
        {
            new Book { Title = "Clean Code", Author = authors[0] },
            new Book { Title = "Clean Architecture", Author = authors[0] },
            new Book { Title = "The Clean Coder", Author = authors[0] },
            new Book { Title = "Agile Principles, Patterns, and Practices in C#", Author = authors[0] },
            new Book { Title = "UML for Java Programmers", Author = authors[0] },

            new Book { Title = "Refactoring", Author = authors[1] },
            new Book { Title = "Patterns of Enterprise Application Architecture", Author = authors[1] },
            new Book { Title = "UML Distilled", Author = authors[1] },
            new Book { Title = "Continuous Integration", Author = authors[1] },
            new Book { Title = "NoSQL Distilled", Author = authors[1] },

            new Book { Title = "Domain-Driven Design", Author = authors[2] },
            new Book { Title = "Domain-Driven Design Reference", Author = authors[2] },
            new Book { Title = "Implementing Domain-Driven Design", Author = authors[2] },
            new Book { Title = "DDD Distilled", Author = authors[2] },
            new Book { Title = "Domain-Driven Design Quickly", Author = authors[2] },

            new Book { Title = "Test-Driven Development: By Example", Author = authors[3] },
            new Book { Title = "Extreme Programming Explained", Author = authors[3] },
            new Book { Title = "Implementation Patterns", Author = authors[3] },
            new Book { Title = "Planning Extreme Programming", Author = authors[3] },
            new Book { Title = "Test-Driven Development in Microsoft .NET", Author = authors[3] },

            new Book { Title = "Computer Networks", Author = authors[4] },
            new Book { Title = "Operating Systems: Design and Implementation", Author = authors[4] },
            new Book { Title = "Modern Operating Systems", Author = authors[4] },
            new Book { Title = "Distributed Systems: Principles and Paradigms", Author = authors[4] },
            new Book { Title = "Structured Computer Organization", Author = authors[4] },

            new Book { Title = "Software Engineering: A Practitioner's Approach", Author = authors[1] },
            new Book { Title = "Continuous Delivery", Author = authors[1] },
            new Book { Title = "Patterns, Principles, and Practices of Domain-Driven Design", Author = authors[2] },
            new Book { Title = "Working Effectively with Legacy Code", Author = authors[0] },
            new Book { Title = "Design Patterns: Elements of Reusable Object-Oriented Software", Author = authors[1] }
        };
        db.Books.AddRange(books);
        db.SaveChanges();
    }
}

app.Run();

public partial class Program { }
