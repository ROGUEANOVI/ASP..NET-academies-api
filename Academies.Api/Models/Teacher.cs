using System;
using System.Collections.Generic;

namespace Academies.Api.Models
{
    public partial class Teacher
    {
        public Teacher()
        {
            Courses = new HashSet<Course>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int SchoolId { get; set; }

        public virtual School School { get; set; } = null!;
        public virtual ICollection<Course> Courses { get; set; }
    }
}
