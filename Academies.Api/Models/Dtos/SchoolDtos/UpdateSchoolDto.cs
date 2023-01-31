using System.ComponentModel.DataAnnotations;

namespace Academies.Api.Models.Dtos.SchoolDtos
{
    public class UpdateSchoolDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Web { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
    }
}
