using Academies.Api.Models.Dtos;
using Academies.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ILogger<SchoolController> _logger;
        private readonly ACADEMIES_DBContext _dbContext;
        public TeacherController(ILogger<SchoolController> logger, ACADEMIES_DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TeacherDto>> GetAll()
        {
            _logger.LogInformation("Obtener todos los profesores");

            return Ok(_dbContext.Teachers.ToList());
        }


        [HttpGet("{id:int}", Name = "GetTeacherById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TeacherDto> Get(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"Error al intentar traer un profesor con id: {id}");
                return BadRequest();
            }

            var teacher = _dbContext.Teachers.FirstOrDefault(s => s.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return Ok(teacher);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<SchoolDto> Create([FromBody] TeacherDto teacherDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacherExists = _dbContext.Teachers.FirstOrDefault(s => (s.FirstName.ToLower() == teacherDto.FirstName.ToLower()) && (s.LastName.ToLower() == teacherDto.LastName.ToLower()) && (s.Email == teacherDto.Email));
            if (teacherExists != null)
            {
                ModelState.AddModelError("Nombre Existente", $"Ya existe un profesor con estos datos registrados");
                return BadRequest(ModelState);
            }

            if (teacherDto == null)
            {
                return BadRequest(teacherDto);
            }

            if (teacherDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Teacher teacherModel = new Teacher()
            {
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Email = teacherDto.Email,
                Phone = teacherDto.Phone,
                SchoolId = teacherDto.SchoolId
            };

            _dbContext.Add(teacherModel);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetTeacherById", new { id = teacherDto.Id }, teacherDto);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(int id, [FromBody] TeacherDto teacherDto)
        {
            if ((teacherDto == null) || (id != teacherDto.Id))
            {
                return BadRequest();
            }

            Teacher teacherModel = new()
            {
                Id = teacherDto.Id,
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Email = teacherDto.Email,
                Phone = teacherDto.Phone,
                SchoolId= teacherDto.SchoolId
            };

            _dbContext.Update(teacherModel);
            _dbContext.SaveChanges();

            return NoContent();

        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartial(int id, JsonPatchDocument<TeacherDto> patchDto)
        {
            if ((patchDto == null) || (id == 0))
            {
                return BadRequest();
            }

            var teacher = _dbContext.Teachers.AsNoTracking().FirstOrDefault(s => s.Id == id);

            if (teacher == null) return BadRequest();

            TeacherDto teacherDto = new()
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                Phone = teacher.Phone,
                SchoolId = teacher.SchoolId
            };

            patchDto.ApplyTo(teacherDto);

            Teacher teacherModel = new()
            {
                Id = teacherDto.Id,
                FirstName= teacherDto.FirstName,
                LastName= teacherDto.LastName,
                Email = teacherDto.Email,
                Phone = teacherDto.Phone,
                SchoolId = teacherDto.SchoolId
            };

            _dbContext.Teachers.Update(teacherModel);
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

            var teacher = _dbContext.Teachers.FirstOrDefault(s => s.Id == id);

            if (teacher == null)
            {
                return NoContent();
            }

            _dbContext.Teachers.Remove(teacher);
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
