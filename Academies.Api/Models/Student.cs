using System;
using System.Collections.Generic;

namespace Academies.Api.Models
{
    public partial class Student
    {
        public Student()
        {
            Grades = new HashSet<Grade>();
            StudentsCourses = new HashSet<StudentsCourse>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int SchoolId { get; set; }

        public virtual School School { get; set; } = null!;
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<StudentsCourse> StudentsCourses { get; set; }
    }
}
