using BookManagement.Dtos;
using BookManagement.Infrastructure;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AuthorsController : BaseController 
    {
        private readonly BookDbContext _dbContext;
        public AuthorsController(BookDbContext juri, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _dbContext = juri;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var authors = await _dbContext.Authors.Include(x=>x.Books).ToListAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var author = await _dbContext.Authors.SingleOrDefaultAsync(x => x.Id == id);
            if (author is null)
            {
                return NotFound();
            }
 
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(AuthorDto author)
        {
            //momenti kur klasa inicializohet
            var newAuthor = new Author()
            {
                Bio = author.Bio,
                FullName = author.FullName,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = UserId
            };

            await _dbContext.Authors.AddAsync(newAuthor);
            await _dbContext.SaveChangesAsync();

            return Ok(newAuthor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AuthorDto author)
        {
            var toBeUpdatedAuthor = _dbContext.Authors.SingleOrDefault(x => x.Id == id);
            if (toBeUpdatedAuthor is null)
            {
                return NotFound();
            }

            toBeUpdatedAuthor.Bio = author.Bio;
            toBeUpdatedAuthor.FullName = author.FullName;
            toBeUpdatedAuthor.ModifiedAt = DateTime.UtcNow;
            toBeUpdatedAuthor.ModifiedBy = UserId;

            _dbContext.Authors.Update(toBeUpdatedAuthor);
            await _dbContext.SaveChangesAsync();
            return Ok(toBeUpdatedAuthor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var toBeDeletedAuthor = _dbContext.Authors.SingleOrDefault(x => x.Id == id);
            if (toBeDeletedAuthor is null)
            {
                return NotFound();
            }

            _dbContext.Authors.Remove(toBeDeletedAuthor);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}