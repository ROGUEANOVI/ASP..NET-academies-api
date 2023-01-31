using Academies.Api.Models;
using Academies.Api.Models.Dtos.StudentDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly ACADEMIES_DBContext _dbContext;
        public StudentController(ILogger<StudentController> logger, ACADEMIES_DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            _logger.LogInformation("Obtener todos los estudiantes");

            return Ok(await _dbContext.Students.ToListAsync());
        }


        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDto>> Get(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"Error al intentar traer un estudiante con id: {id}");
                return BadRequest();
            }

            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto studentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var studentExists = await _dbContext.Students.FirstOrDefaultAsync(s => (s.FirstName.ToLower() == studentDto.FirstName.ToLower()) && (s.LastName.ToLower() == studentDto.LastName.ToLower()) && (s.Email == studentDto.Email));
            if (studentExists != null)
            {
                ModelState.AddModelError("Nombre Existente", $"Ya existe un estudiante con estos datos registrados");
                return BadRequest(ModelState);
            }

            if (studentDto == null)
            {
                return BadRequest(studentDto);
            }



            Student studentModel = new Student()
            {
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Email = studentDto.Email,
                Phone = studentDto.Phone,
                SchoolId = studentDto.SchoolId
            };

            await _dbContext.AddAsync(studentModel);
            await _dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetStudentById", new { id = studentModel.Id }, studentModel);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto studentDto)
        {
            if ((studentDto == null) || (id != studentDto.Id))
            {
                return BadRequest();
            }

            Student studentModel = new()
            {
                Id = studentDto.Id,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Email = studentDto.Email,
                Phone = studentDto.Phone,
                SchoolId = studentDto.SchoolId
            };

            _dbContext.Update(studentModel);
            await _dbContext.SaveChangesAsync();

            return NoContent();

        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartial(int id, JsonPatchDocument<UpdateStudentDto> patchDto)
        {
            if ((patchDto == null) || (id == 0))
            {
                return BadRequest();
            }

            var student = await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return BadRequest();

            UpdateStudentDto studentDto = new()
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Phone = student.Phone,
                SchoolId = student.SchoolId
            };

            patchDto.ApplyTo(studentDto);

            Student studentModel = new()
            {
                Id = studentDto.Id,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Email = studentDto.Email,
                Phone = studentDto.Phone,
                SchoolId = studentDto.SchoolId
            };

            _dbContext.Students.Update(studentModel);
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

            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NoContent();
            }

            _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
