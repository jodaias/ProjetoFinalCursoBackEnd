using Library.Domain.Models;
using Library.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryContext _context;
    public LoanRepository(LibraryContext context) => _context = context;

    public async Task<Loan?> GetByIdAsync(int id) => await _context.Loans.Include(e => e.Book).FirstOrDefaultAsync(e => e.Id == id);
    public async Task<IEnumerable<Loan>> GetAllAsync() => await _context.Loans.Include(e => e.Book).ToListAsync();
    public async Task AddAsync(Loan loan) { _context.Loans.Add(loan); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(Loan loan) { _context.Loans.Update(loan); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(int id) { var loan = await _context.Loans.FindAsync(id); if (loan != null) { _context.Loans.Remove(loan); await _context.SaveChangesAsync(); } }
}
