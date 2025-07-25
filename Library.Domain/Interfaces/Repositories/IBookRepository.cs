using Library.Domain.Models;

namespace Library.Domain.Interfaces.Repositories;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<IEnumerable<Book>> GetAvailableAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
}
