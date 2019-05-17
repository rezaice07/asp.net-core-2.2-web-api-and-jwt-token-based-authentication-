using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace University.Data.Models
{
    [Table("Class")]
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime? ChangedDateUtc { get; set; }
    }
}
