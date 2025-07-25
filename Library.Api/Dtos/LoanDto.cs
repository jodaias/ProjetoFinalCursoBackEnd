namespace Library.Api.Dtos;

public class LoanDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string? BorrowerName { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
