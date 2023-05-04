using System.ComponentModel.DataAnnotations;

namespace BookManagement.Dtos;

public class BookDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageSource { get; set; }
    public List<int> CategoryIds { get; set; }
    public int AuthorId { get; set; }
}