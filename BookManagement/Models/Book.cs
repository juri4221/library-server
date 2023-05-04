namespace BookManagement.Models
{
    public class Book : BaseModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string ImageSource { get; set; }
        public ICollection<Category> Categories { get; set; }
        public Author Author { get; set; }
    }
}
