using Ligonine.Auth.Model;
using System.ComponentModel.DataAnnotations;

namespace Ligonine.Data.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DoctorId { get; set; }

        [Required]
        public required string UserId { get; set; }
        public ForumRestUser User { get; set; }
    }
}
