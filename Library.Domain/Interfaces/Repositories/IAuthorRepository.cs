using Library.Domain.Models;

namespace Library.Domain.Interfaces.Repositories;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(int id);
    Task<IEnumerable<Author>> GetAllAsync();
    Task AddAsync(Author author);
    Task UpdateAsync(Author author);
    Task DeleteAsync(int id);
}
