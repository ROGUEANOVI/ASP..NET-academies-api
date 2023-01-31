using Academies.Api.Models;
using Academies.Api.Models.Dtos;
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
        public ActionResult<IEnumerable<SchoolDto>> GetAll()
        {
            _logger.LogInformation("Obtener las escuelas");

            return Ok(_dbContext.Schools.ToList());
        }


        [HttpGet("{id:int}", Name ="GetSchoolById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SchoolDto> Get(int id)
        {
            if (id == 0) 
            {
                _logger.LogError($"Error al intentar traer una escuela con id: {id}");
                return BadRequest();
            }

            var school = _dbContext.Schools.FirstOrDefault(s => s.Id ==id);
            
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
        public ActionResult<SchoolDto> Create([FromBody] SchoolDto schoolDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var schoolExists = _dbContext.Schools.FirstOrDefault(s => s.Name.ToLower() == schoolDto.Name.ToLower());
            if (schoolExists != null)   
            {
                ModelState.AddModelError("Nombre Existente", $"Ya existe una escuela con el nombre {schoolDto.Name}");
                return BadRequest(ModelState);
            }

            if (schoolDto == null)
            {
                return BadRequest(schoolDto);
            }

            if (schoolDto.Id > 0) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }

            School schoolModel = new School()
            {
                Name = schoolDto.Name,
                Web = schoolDto.Web,
                Email = schoolDto.Email,
                Phone = schoolDto.Phone
            };

            _dbContext.Add(schoolModel);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetSchoolById", new { id = schoolDto.Id}, schoolDto);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(int id, [FromBody] SchoolDto schoolDto)
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
            _dbContext.SaveChanges();

            return NoContent();

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartial(int id, JsonPatchDocument<SchoolDto> patchDto)
        {
            if ((patchDto == null) || (id == 0))
            {
                return BadRequest();
            }

            var school = _dbContext.Schools.AsNoTracking().FirstOrDefault(s => s.Id == id);

            if(school == null) return BadRequest();

            SchoolDto schoolDto = new()
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
            _dbContext.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Remove(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var school = _dbContext.Schools.FirstOrDefault(s => s.Id == id);
            
            if (school == null)
            {
                return NoContent();
            }

            _dbContext.Schools.Remove(school);
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
