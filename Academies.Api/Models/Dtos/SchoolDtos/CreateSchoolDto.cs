using System.ComponentModel.DataAnnotations;

namespace Academies.Api.Models.Dtos.SchoolDtos
{
    public class CreateSchoolDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Web { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
