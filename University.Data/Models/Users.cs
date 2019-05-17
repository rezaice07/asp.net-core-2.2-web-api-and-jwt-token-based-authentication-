using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace University.Data.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string CellPhone { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDateUtc { get; set; }
        
        [ForeignKey("RoleId")]
        public UserRole UserRole { get; set; }

        [NotMapped]
        public string RoleName { get; set; }
    }
}
