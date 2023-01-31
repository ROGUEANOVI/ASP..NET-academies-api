using System;
using System.Collections.Generic;

namespace Academies.Api.Models
{
    public partial class Course
    {
        public Course()
        {
            Grades = new HashSet<Grade>();
            StudentsCourses = new HashSet<StudentsCourse>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TeacherId { get; set; }

        public virtual Teacher Teacher { get; set; } = null!;
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<StudentsCourse> StudentsCourses { get; set; }
    }
}
