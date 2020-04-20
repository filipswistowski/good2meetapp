using activitiesapp.Models;
using activitiesapp.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace activitiesapp.Controllers
{
    /// <summary>
    /// Management of categories
    /// </summary>
    [ApiController]
    [Route("/category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        /// <summary>
        /// Returns all categories in database
        /// </summary>
        /// <response code="200">Returns list of categories</response>
        /// <response code="500">If there is a server error</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDTO[]>> Get()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();

                return Ok(_mapper.Map<CategoryDTO[]>(categories));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        /// <summary>
        /// Returns category of given id
        /// </summary>
        /// <param name="CategoryId">Id of the category we want to get</param>
        /// <response code="200">Returns category by given id</response>
        /// <response code="404">If there is no category with given id</response>  
        /// <response code="500">If there is a server error</response>  
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{CategoryId:int}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int CategoryId)
        {
            {
                try
                {
                    var category = await _categoryRepository.GetCategoryByIdAsync(CategoryId);

                    if (category == null) return NotFound();

                    return Ok(_mapper.Map<CategoryDTO>(category));
                }
                catch (Exception)
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
                }
            }
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <response code="201">Creates category with given name</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> Post(CategoryDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);

                var location = _linkGenerator.GetPathByAction("Get", "Category", new { id = category.CategoryId });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not post current Category");
                }

                _categoryRepository.CreateCategory(category);
                if (await _categoryRepository.SaveChangeAsync())
                {
                    return Created($"/api/category/{category.CategoryId}", (_mapper.Map<CategoryDTO>(category)));
                }

                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        /// <summary>
        /// Changes name of category with given ID
        /// </summary>
        /// <param name="CategoryId">Id of the category we want to change name</param>
        /// <response code="200">Succesfully changed name of category</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{CategoryId:int}")]
        public async Task<ActionResult<CategoryDTO>> Put(int CategoryId, CategoryDTO model)
        {
            try
            {
                var oldCategory = await _categoryRepository.GetCategoryByIdAsync(CategoryId);
                if (oldCategory == null) return NotFound($"Could not find category with id of {CategoryId}");

                _mapper.Map(model, oldCategory);

                if (await _categoryRepository.SaveChangeAsync())
                {
                    return _mapper.Map<CategoryDTO>(oldCategory);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        /// <summary>
        /// Deletes category with given ID
        /// </summary>
        /// <param name="CategoryId">Id of the category we want to delete</param>
        /// <response code="200">Succesfully removed category</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{CategoryId:int}")]
        public async Task<ActionResult<CategoryDTO>> Delete(int CategoryId)
        {
            try
            {
                var oldCategory = await _categoryRepository.GetCategoryByIdAsync(CategoryId);
                if (oldCategory == null) return NotFound($"Could not find category with id of {CategoryId}");

                _categoryRepository.Delete(oldCategory);

                if (await _categoryRepository.SaveChangeAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }
    }
}