using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Grades = new HashSet<Grade>();
        }

        public uint Year { get; set; }
        public string Location { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public uint CourseId { get; set; }
        public uint ClassId { get; set; }
        public string Season { get; set; } = null!;
        public string? ProfessorId { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual Professor? Professor { get; set; }
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }
}
