using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace University.Data.Models
{
    [Table("StudentEnrolledClass")]
    public class StudentEnrolledClass
    {
        [Key]
        public int Id { get; set; }

        public int ClassId { get; set; }

        public int UserId { get; set; }
        public DateTime EnrolledDate { get; set; }
    }
}
