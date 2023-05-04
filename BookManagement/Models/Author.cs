namespace BookManagement.Models
{
    public class Author : BaseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Bio { get; set; }
        
        public ICollection<Book> Books { get; } = new List<Book>(); 
    }
}
