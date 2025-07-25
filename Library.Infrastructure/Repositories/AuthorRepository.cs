using Library.Domain.Models;
using Library.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryContext _context;
    public AuthorRepository(LibraryContext context) => _context = context;

    public async Task<Author?> GetByIdAsync(int id) => await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
    public async Task<IEnumerable<Author>> GetAllAsync() => await _context.Authors.Include(a => a.Books).ToListAsync();
    public async Task AddAsync(Author author) { _context.Authors.Add(author); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(Author author) { _context.Authors.Update(author); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(int id) { var author = await _context.Authors.FindAsync(id); if (author != null) { _context.Authors.Remove(author); await _context.SaveChangesAsync(); } }
}
