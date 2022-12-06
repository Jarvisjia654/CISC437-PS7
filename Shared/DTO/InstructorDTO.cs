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
    public class InstructorDTO
    {
        [Precision(8)]
        public int SchoolId { get; set; }
        [Precision(8)]
        public int InstructorId { get; set; }
        [StringLength(5)]
        [Unicode(false)]
        public string? Salutation { get; set; }
        [StringLength(25)]
        [Unicode(false)]
        public string FirstName { get; set; } = null!;
        [StringLength(25)]
        [Unicode(false)]
        public string LastName { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string StreetAddress { get; set; } = null!;
        [StringLength(5)]
        [Unicode(false)]
        public string Zip { get; set; } = null!;
        [StringLength(15)]
        [Unicode(false)]
        public string? Phone { get; set; }
        [StringLength(30)]
        [Unicode(false)]
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        [StringLength(30)]
        [Unicode(false)]
        public string ModifiedBy { get; set; } = null!;
        public DateTime ModifiedDate { get; set; }
    }
}
