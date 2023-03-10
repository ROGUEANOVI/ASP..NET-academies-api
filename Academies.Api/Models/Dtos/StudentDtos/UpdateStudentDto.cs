using System.ComponentModel.DataAnnotations;

namespace Academies.Api.Models.Dtos.StudentDtos
{
    public class UpdateStudentDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public int SchoolId { get; set; }
    }
}
