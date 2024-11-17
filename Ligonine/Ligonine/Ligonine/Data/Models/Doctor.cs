using Ligonine.Auth.Model;
using System.ComponentModel.DataAnnotations;

namespace Ligonine.Data.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string BloodType { get; set; }
        public int DepartmentId { get; set; }

        [Required]
        public required string UserId { get; set; }
        public ForumRestUser User { get; set; }

    }
}
