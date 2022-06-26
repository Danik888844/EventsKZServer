using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsServer.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public Gender Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateOfReg { get; set; } = DateTime.Now;
        public Role[] Roles { get; set; } = new Role[] {Role.User};
        public List<EventUser> EventUsers { get; set; } = new List<EventUser>();
    }

    public enum Role
    {
        User,
        Admin
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
