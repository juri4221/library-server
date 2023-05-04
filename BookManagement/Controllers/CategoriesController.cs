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
    //kjo sintakse percakton nje dekorator
    //dekoratoret jane funksione te cilet shtojne funksionalitetet e metodave ne te cilat perdoret
    
    //ky dekorator ben t mundur qe metodat e ksaj klase te aksesohen nepermjet nje requesti api
    [ApiController]
    //qe te aksesohen metodat brenda kesaj klase, useri duhet te jete i autorizuar dhe duhet te kete rolin: Admin
    [Authorize(Roles = "Admin")]
    //ky dekorator ben te mundur qe te gjithe metodat brenda kesaj klase ti pergjigjen nje root-i te caktuar, te kete rast 'controller'
    [Route("[controller]")]
    public class CategoriesController : BaseController
    {
        private readonly BookDbContext _dbContext;
        //ketu perdorim DbContext e cila eshte bere inject nga Program.cs
        public CategoriesController(BookDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _dbContext = dbContext;
            
        }

        [HttpGet]
        //ne kete metode kodi merr te gjitha kategorite nga DB 
        public async Task<IActionResult> Index()
        {
            var categories = await _dbContext.Categories.Include((x)=>x.Books).ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        //root i eshte categories/id, id eshte nje root parameter
        public async Task<IActionResult> Get(int id)
        {
            var category = await _dbContext.Categories.Include((x)=>x.Books).SingleOrDefaultAsync(x => x.Id == id);
            if (category is null)
            {
                return NotFound(); //404
            }

            return Ok(category);
        }

        [HttpPost]
        //Ky kod është një metodë qe trajton HTTP POST requests për të krijuar një kategori të re në një DB.
        //Ai pret një objekt "categoryDto" si parameter, i cili përmban informacionin për kategorinë e re.
        //krijon një objekt të ri "Categories", vendos vetitë e tij me vlerat nga objekti "categoryDto"
        //cakton datën e krijimit në kohën aktuale UTC dhe vendos ID-në e krijuesit në ID-në e përdoruesit aktual.
        public async Task<ActionResult> Create(CategoryDto categoryDto)
        {
            var newCategory = new Category()
            {
                Name = categoryDto.Name,
                Priority = categoryDto.Priority,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = UserId
            };
            
            await _dbContext.Categories.AddAsync(newCategory);
            await _dbContext.SaveChangesAsync();

            return Ok(newCategory);
        }

        [HttpPut("{id}")]
        //Ky kod eshte nje metode qe trajton kerkesat httpPut per te bere update nje kategori DB.
        //Merr parametrin id ne URL dhe CategoryDto ne request body qe permban informacionin e update uar
        //kerkon te gjeje kategorine qe do behet update me ane te id ne db
        //nese egziston i ben update me vlerat nga objekti CategoriesDto
        public async Task<ActionResult> Update(int id, CategoryDto categoryDto)
        {
            var toBeUpdatedCategory = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (toBeUpdatedCategory is null)
            {
                return NotFound();
            }

            toBeUpdatedCategory.Name= categoryDto.Name;
            toBeUpdatedCategory.Priority = categoryDto.Priority;
            toBeUpdatedCategory.ModifiedAt = DateTime.UtcNow;
            toBeUpdatedCategory.ModifiedBy = UserId;

            _dbContext.Categories.Update(toBeUpdatedCategory);
            await _dbContext.SaveChangesAsync();
            
            return Ok(toBeUpdatedCategory);
        }

        [HttpDelete("{id}")]
        //qellimi i ketij kodi eshte te fshije nje kategori te caktuar ne baze te id ne DB
        public async Task<IActionResult> Delete(int id)
        {
            var toBeDeletedCategory = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (toBeDeletedCategory is null)
            {
                return NotFound();
            }
            
            
            if (toBeDeletedCategory.Books.Count > 0)
                throw new Exception($"This category can't be deleted since there are books assigned to it.");
            

            _dbContext.Categories.Remove(toBeDeletedCategory);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
