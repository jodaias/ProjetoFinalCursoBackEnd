using Library.Domain.Models;
using Library.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;
    public BookRepository(LibraryContext context) => _context = context;

    public async Task<Book?> GetByIdAsync(int id) => await _context.Books.Include(l => l.Author).Include(l => l.Loans).FirstOrDefaultAsync(l => l.Id == id);
    public async Task<IEnumerable<Book>> GetAllAsync() => await _context.Books.Include(l => l.Author).Include(l => l.Loans).ToListAsync();
    public async Task AddAsync(Book book) { _context.Books.Add(book); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(Book book) { _context.Books.Update(book); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(int id) { var book = await _context.Books.FindAsync(id); if (book != null) { _context.Books.Remove(book); await _context.SaveChangesAsync(); } }
    public async Task<IEnumerable<Book>> GetAvailableAsync() => await _context.Books.Where(l => !l.IsLoaned).ToListAsync();
}
