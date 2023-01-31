using Academies.Api.Models.Dtos.TeacherDtos;
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
        private readonly ILogger<TeacherController> _logger;
        private readonly ACADEMIES_DBContext _dbContext;
        public TeacherController(ILogger<TeacherController> logger, ACADEMIES_DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public  async Task<ActionResult<IEnumerable<TeacherDto>>> GetAll()
        {
            _logger.LogInformation("Obtener todos los profesores");

            return Ok(await _dbContext.Teachers.ToListAsync());
        }


        [HttpGet("{id:int}", Name = "GetTeacherById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDto>> Get(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"Error al intentar traer un profesor con id: {id}");
                return BadRequest();
            }

            var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(s => s.Id == id);

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
        public async Task<ActionResult<TeacherDto>> Create([FromBody] CreateTeacherDto teacherDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacherExists = await _dbContext.Teachers.FirstOrDefaultAsync(s => (s.FirstName.ToLower() == teacherDto.FirstName.ToLower()) && (s.LastName.ToLower() == teacherDto.LastName.ToLower()) && (s.Email == teacherDto.Email));
            if (teacherExists != null)
            {
                ModelState.AddModelError("Nombre Existente", $"Ya existe un profesor con estos datos registrados");
                return BadRequest(ModelState);
            }

            if (teacherDto == null)
            {
                return BadRequest(teacherDto);
            }



            Teacher teacherModel = new Teacher()
            {
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Email = teacherDto.Email,
                Phone = teacherDto.Phone,
                SchoolId = teacherDto.SchoolId
            };

            await _dbContext.AddAsync(teacherModel);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetTeacherById", new { id = teacherModel.Id }, teacherModel);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeacherDto teacherDto)
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
            await _dbContext.SaveChangesAsync();

            return NoContent();

        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartial(int id, JsonPatchDocument<UpdateTeacherDto> patchDto)
        {
            if ((patchDto == null) || (id == 0))
            {
                return BadRequest();
            }

            var teacher = await _dbContext.Teachers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

            if (teacher == null) return BadRequest();

            UpdateTeacherDto teacherDto = new()
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

            var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(s => s.Id == id);

            if (teacher == null)
            {
                return NoContent();
            }

            _dbContext.Teachers.Remove(teacher);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
