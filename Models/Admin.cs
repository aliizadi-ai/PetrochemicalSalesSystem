using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetrochemicalSalesSystem.Models
{
    [Table("Admins")]
    public class Admins
    {
        [Key]
        public long AdminID { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public bool IsSuperAdmin { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime LastLogin { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public string UserType => "Admin";
    }
}