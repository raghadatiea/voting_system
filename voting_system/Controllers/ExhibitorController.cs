using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using voting_system.DTOS.CategoryDTO;
using voting_system.DTOS.ExhibitorDTO;
using voting_system.DTOS.ExhibitorDTOS;
using voting_system.Models;

namespace voting_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitorController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ExhibitorController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Exhibitor
        [HttpGet("GettAll")]
        public IActionResult GetAll()
        {
            var exhibitors = _dbContext.Exhibtiors
                .Include(x => x.Categories)
                .Select(x => new ExhibitorDto
                {
                    ExhibitorsId = x.ExhibitorsId,
                    Name = x.Name,
                    Description = x.Description,
                    PhotoUrl = x.PhotoUrl,

                    Categories = x.Categories
                        .Select(c => new CategoryDto
                        {
                            CategoryId = c.CategoryId,
                            Name = c.Name
                        })
                        .ToList()
                })
                .ToList();

            return Ok(exhibitors);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var exhibitor = _dbContext.Exhibtiors
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.ExhibitorsId == id);

            if (exhibitor == null)
            {
                return NotFound("Exhibitor not found");
            }

            var result = new ExhibitorDto
            {
                ExhibitorsId = exhibitor.ExhibitorsId,
                Name = exhibitor.Name,
                Description = exhibitor.Description,
                PhotoUrl = exhibitor.PhotoUrl,

                Categories = exhibitor.Categories
                    .Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    })
                    .ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Add(AddExhibitorDto dto)
        {
            var exhibitor = new Exhibtior
            {
                Name = dto.Name,
                Description = dto.Description,
                PhotoUrl = dto.PhotoUrl
            };

            var categories = _dbContext.Categories
                .Where(x => dto.CategoryIds.Contains(x.CategoryId))
                .ToList();

            if (categories.Count != dto.CategoryIds.Count)
            {
                return BadRequest("One or more Category IDs are invalid");
            }

            exhibitor.Categories = categories;

            _dbContext.Exhibtiors.Add(exhibitor);

            _dbContext.SaveChanges();

            return Ok(new
            {
                Message = "Exhibitor added successfully",
                ExhibitorsId = exhibitor.ExhibitorsId,
                Name = exhibitor.Name,
                Description = exhibitor.Description,
                PhotoUrl = exhibitor.PhotoUrl,
                CategoryIds = exhibitor.Categories
                    .Select(x => x.CategoryId)
                    .ToList()
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, UpdateExhibitorDto dto)
        {
            var exhibitor = _dbContext.Exhibtiors
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.ExhibitorsId == id);

            if (exhibitor == null)
            {
                return NotFound("Exhibitor not found");
            }

            exhibitor.Name = dto.Name;
            exhibitor.Description = dto.Description;
            exhibitor.PhotoUrl = dto.PhotoUrl;

            var categories = _dbContext.Categories
                .Where(x => dto.CategoryIds.Contains(x.CategoryId))
                .ToList();

            if (categories.Count != dto.CategoryIds.Count)
            {
                return BadRequest("One or more Category IDs are invalid");
            }

            // Remove old relationships
            exhibitor.Categories.Clear();

            // Add new relationships
            foreach (var category in categories)
            {
                exhibitor.Categories.Add(category);
            }

            _dbContext.SaveChanges();

            return Ok("Exhibitor updated successfully");
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var exhibitor = _dbContext.Exhibtiors
                .FirstOrDefault(x => x.ExhibitorsId == id);

            if (exhibitor == null)
            {
                return NotFound("Exhibitor not found");
            }

            _dbContext.Exhibtiors.Remove(exhibitor);

            _dbContext.SaveChanges();

            return Ok("Exhibitor deleted successfully");
        }

        [HttpPost("{id}/categories")]
        public IActionResult AssignCategories( long id,AssignCategoriesDTO dto)
        {
            var exhibitor = _dbContext.Exhibtiors
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.ExhibitorsId == id);

            if (exhibitor == null)
            {
                return NotFound("Exhibitor not found");
            }

            var categories = _dbContext.Categories
                .Where(x => dto.CategoryIds.Contains(x.CategoryId))
                .ToList();

            if (categories.Count != dto.CategoryIds.Count)
            {
                return BadRequest("One or more Category IDs are invalid");
            }

            foreach (var category in categories)
            {
                if (!exhibitor.Categories.Contains(category))
                {
                    exhibitor.Categories.Add(category);
                }
            }

            _dbContext.SaveChanges();

            return Ok("Categories assigned successfully");
        }
    }
}
