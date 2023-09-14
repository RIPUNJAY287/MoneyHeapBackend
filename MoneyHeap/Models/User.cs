using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MoneyHeap.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }
        public User() { }

        public User(String name, String email) {
            Name = name;
            Email = email;
        }

    }
}
