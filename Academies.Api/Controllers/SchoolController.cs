using Academies.Api.Models;
using Academies.Api.Models.Dtos.SchoolDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ILogger<SchoolController> _logger;
        private readonly ACADEMIES_DBContext _dbContext;
        public SchoolController(ILogger<SchoolController> logger, ACADEMIES_DBContext dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SchoolDto>>> GetAll()
        {
            _logger.LogInformation("Obtener las escuelas");

            return Ok(await _dbContext.Schools.ToListAsync());
        }


        [HttpGet("{id:int}", Name ="GetSchoolById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SchoolDto>> Get(int id)
        {
            if (id == 0) 
            {
                _logger.LogError($"Error al intentar traer una escuela con id: {id}");
                return BadRequest();
            }

            var school = await _dbContext.Schools.FirstOrDefaultAsync(s => s.Id ==id);
            
            if (school == null)
            {
                return NotFound();  
            }

            return Ok(school);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SchoolDto>> Create([FromBody] CreateSchoolDto schoolDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var schoolExists = await _dbContext.Schools.FirstOrDefaultAsync(s => s.Name.ToLower() == schoolDto.Name.ToLower());
            if (schoolExists != null)   
            {
                ModelState.AddModelError("Nombre Existente", $"Ya existe una escuela con el nombre {schoolDto.Name}");
                return BadRequest(ModelState);
            }

            if (schoolDto == null)
            {
                return BadRequest(schoolDto);
            }



            School schoolModel = new School()
            {
                Name = schoolDto.Name,
                Web = schoolDto.Web,
                Email = schoolDto.Email,
                Phone = schoolDto.Phone
            };

            await _dbContext.AddAsync(schoolModel);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetSchoolById", new { id = schoolModel.Id}, schoolModel);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSchoolDto schoolDto)
        { 
            if ((schoolDto == null) || (id  != schoolDto.Id))
            {
                return BadRequest();
            }

            School schoolModel = new() 
            {   
                Id = schoolDto.Id,
                Name = schoolDto.Name, 
                Web = schoolDto.Web, 
                Email = schoolDto.Email, 
                Phone = schoolDto.Phone
            };

            _dbContext.Update(schoolModel);
            await _dbContext.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartial(int id, JsonPatchDocument<UpdateSchoolDto> patchDto)
        {
            if ((patchDto == null) || (id == 0))
            {
                return BadRequest();
            }

            var school = await _dbContext.Schools.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

            if(school == null) return BadRequest();

            UpdateSchoolDto schoolDto = new()
            {
                Id = school.Id,
                Name = school.Name,
                Web = school.Web,
                Email = school.Email,
                Phone = school.Phone
            };

            patchDto.ApplyTo(schoolDto);

            School schoolModel = new()
            {
                Id = schoolDto.Id,
                Name = schoolDto.Name,
                Web = schoolDto.Web,
                Email = schoolDto.Email,
                Phone = schoolDto.Phone
            };

            _dbContext.Schools.Update(schoolModel);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Remove(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var school = await _dbContext.Schools.FirstOrDefaultAsync(s => s.Id == id);
            
            if (school == null)
            {
                return NoContent();
            }

            _dbContext.Schools.Remove(school);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
