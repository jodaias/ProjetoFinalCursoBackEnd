namespace Library.Api.Dtos;

public class CreateLoanDto
{
    public int BookId { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
}
