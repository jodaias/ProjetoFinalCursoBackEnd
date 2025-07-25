namespace Library.Api.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public bool IsLoaned { get; set; }
}
