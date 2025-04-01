using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Grade
    {
        public string Grade1 { get; set; } = null!;
        public uint ClassId { get; set; }
        public string StudentId { get; set; } = null!;

        public virtual Class Class { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
