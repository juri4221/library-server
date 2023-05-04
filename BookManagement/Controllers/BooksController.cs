using BookManagement.Dtos;
using BookManagement.Infrastructure;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookManagement.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class BooksController : BaseController
    {
        private readonly BookDbContext _dbContext;
        public BooksController(BookDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var books = await _dbContext.Books
                .Include(book => book.Author)
                .Include(book => book.Categories)
                .ToListAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var book = _dbContext.Books
                .Include(book => book.Author)
                .Include(book => book.Categories)
                .SingleOrDefault(x => x.Id == id);
            
            if (book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(BookDto bookDto,[FromServices] IWebHostEnvironment env)
        {
            var author = await _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == bookDto.AuthorId);
            
            if (author is null)
                throw new Exception($"Author with id {bookDto.AuthorId} does not exist!");

            var categories = await _dbContext.Categories.Where(x => bookDto.CategoryIds.Contains(x.Id)).ToListAsync();
            
            if (categories.Count != bookDto.CategoryIds.Count)
                throw new Exception($"Categories with ids: {String.Join(",",bookDto.CategoryIds.Except(categories.Select(x => x.Id)))} do not exist");

            
            byte[] imageBytes = Convert.FromBase64String(bookDto.ImageSource);
            string fileName = Guid.NewGuid() + ".png";
            string filePath = Path.Combine(env.WebRootPath, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
            
            var newBook = new Book
            {
                Name = bookDto.Name,
                Description = bookDto.Description,
                ImageSource = fileName,
                Author = author,
                Categories = categories,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = UserId
            };

            await _dbContext.Books.AddAsync(newBook);
            await _dbContext.SaveChangesAsync();

            return Ok(newBook);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, BookDto bookDto,[FromServices] IWebHostEnvironment env)
        {
            var toBeUpdatedBook = await _dbContext.Books   
                .Include(book => book.Categories)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (toBeUpdatedBook is null)
            {
                return NotFound();
            }

            var author = await _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == bookDto.AuthorId);
            if (author is null)
                throw new Exception($"Author with id {bookDto.AuthorId} does not exist!");

            var categories = await _dbContext.Categories.Where(x => bookDto.CategoryIds.Contains(x.Id)).ToListAsync();
            if (categories.Count != bookDto.CategoryIds.Count)
                throw new Exception($"Categories with ids: {String.Join(",",bookDto.CategoryIds.Except(categories.Select(x => x.Id)))} do not exist");
            
            byte[] imageBytes = Convert.FromBase64String(bookDto.ImageSource);
            string fileName = Guid.NewGuid() + ".png";
            string filePath = Path.Combine(env.WebRootPath, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
            System.IO.File.Delete(toBeUpdatedBook.ImageSource);

            toBeUpdatedBook.Author = author;
            toBeUpdatedBook.Description = bookDto.Description;
            toBeUpdatedBook.Name = bookDto.Name;
            toBeUpdatedBook.ImageSource = fileName;
            toBeUpdatedBook.ModifiedAt = DateTime.UtcNow;
            toBeUpdatedBook.ModifiedBy = UserId;

            foreach (var child in toBeUpdatedBook.Categories)
            {
                var childEntity = toBeUpdatedBook.Categories.FirstOrDefault(c => c.Id == child.Id);
                if (childEntity != null)
                {
                    _dbContext.Entry(childEntity).CurrentValues.SetValues(child);
                }
                else
                {
                    _dbContext.Categories.Add(child);
                }
            }
            
            toBeUpdatedBook.Categories = categories;

            _dbContext.Books.Update(toBeUpdatedBook);
            await _dbContext.SaveChangesAsync();
            return Ok(toBeUpdatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var toBeDeletedBook = await _dbContext.Books.SingleOrDefaultAsync(x => x.Id == id);
            if (toBeDeletedBook is null)
            {
                return NotFound();
            }

            _dbContext.Books.Remove(toBeDeletedBook);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
