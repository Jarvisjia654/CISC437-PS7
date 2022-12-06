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
    public class ZipcodeDTO
    {
        [StringLength(5)]
        [Unicode(false)]
        public string Zip { get; set; } = null!;
        [StringLength(25)]
        [Unicode(false)]
        public string? City { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string? State { get; set; }
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
