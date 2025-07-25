namespace Library.Domain.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    public bool IsLoaned { get; set; }
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
