using System.ComponentModel.DataAnnotations;

namespace Academies.Api.Models.Dtos.StudentDtos
{
    public class CreateStudentDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int SchoolId { get; set; }
    }
}
