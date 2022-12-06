using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNICKERS.Shared.DTO
{
    public class SectionDTO
    {
        [Precision(8)]
        public int SectionId { get; set; }
        [Precision(8)]
        public int CourseNo { get; set; }
        [Precision(3)]
        public byte SectionNo { get; set; }
        public DateTime? StartDateTime { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Location { get; set; }
        [Precision(8)]
        public int InstructorId { get; set; }
        [Precision(3)]
        public byte? Capacity { get; set; }
        [StringLength(30)]
        [Unicode(false)]
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        [StringLength(30)]
        [Unicode(false)]
        public string ModifiedBy { get; set; } = null!;
        public DateTime ModifiedDate { get; set; }
        [Key]
        [Precision(8)]
        public int SchoolId { get; set; }
    }
}
