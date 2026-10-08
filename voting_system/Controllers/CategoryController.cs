using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using voting_system.DTOS.CategoryDTO;
using voting_system.DTOS.CategoryDTOS;
using voting_system.DTOS.ExhibitorDTO;
using voting_system.DTOS.VoteDTOS;
using voting_system.Models;

namespace voting_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                var data = from category in _dbContext.Categories
                           select new CategoryDto
                           {
                               CategoryId = category.CategoryId,
                               Name = category.Name,

                               Exhibitors = category.Exhibitors
                                   .Select(x => new ExhibitorDto
                                   {
                                       ExhibitorsId = x.ExhibitorsId,
                                       Name = x.Name,
                                       Description = x.Description,
                                       PhotoUrl = x.PhotoUrl
                                   })
                                   .ToList(),

                               Votes = category.Votes
                                   .Select(x => new VoteDto
                                   {
                                       VoteId = x.VoteId,
                                       ExhibitorId = x.ExhibitorId,
                                       VisitorId = x.VisitorId,
                                       CreatedAt = x.CreatedAt,
                                       CategoryId = x.CategoryId
                                   })
                                   .ToList()
                           };

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Add")]
        public IActionResult Add(AddCategoryDto categoryDto)
        {
            try
            {
                var category = new Category
                {
                    Name = categoryDto.Name
                };

                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(long id)
        {
            try
            {
                var data = _dbContext.Categories
                    .Where(x => x.CategoryId == id)
                    .Select(category => new CategoryDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,

                        Exhibitors = category.Exhibitors
                            .Select(x => new ExhibitorDto
                            {
                                ExhibitorsId = x.ExhibitorsId,
                                Name = x.Name,
                                Description = x.Description,
                                PhotoUrl = x.PhotoUrl
                            })
                            .ToList(),

                        Votes = category.Votes
                            .Select(x => new VoteDto
                            {
                                VoteId = x.VoteId,
                                ExhibitorId = x.ExhibitorId,
                                VisitorId = x.VisitorId,
                                CreatedAt = x.CreatedAt,
                                CategoryId = x.CategoryId
                            })
                            .ToList()
                    })
                    .FirstOrDefault();

                if (data == null)
                    return NotFound("Category not found");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Update")]
        public IActionResult Update(UpdateCategoryDto categoryDto)
        {
            try
            {
                var category = _dbContext.Categories
                    .FirstOrDefault(x => x.CategoryId == categoryDto.CategoryId);

                if (category == null)
                {
                    return NotFound("Category not found");
                }

                category.Name = categoryDto.Name;

                _dbContext.SaveChanges();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Delete")]
        public IActionResult Delete(DeleteCategoryDto categoryDto)
        {
            try
            {
                var category = _dbContext.Categories
                    .FirstOrDefault(x => x.CategoryId == categoryDto.CategoryId);

                if (category == null)
                {
                    return NotFound("Category not found");
                }

                _dbContext.Categories.Remove(category);
                _dbContext.SaveChanges();

                return Ok("Category deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }


    }
 
