namespace BookManagement.Models
{
    public class Category : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public ICollection<Book> Books { get; set; }
    }
    
}
