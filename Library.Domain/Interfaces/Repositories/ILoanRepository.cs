using Library.Domain.Models;

namespace Library.Domain.Interfaces.Repositories;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(int id);
    Task<IEnumerable<Loan>> GetAllAsync();
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task DeleteAsync(int id);
}
