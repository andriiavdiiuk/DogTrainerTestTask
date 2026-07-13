using Microsoft.AspNetCore.Identity;

namespace DogTrainerTestTask.Data.Entities;

public class UserRole : IdentityRole<long>
{
    public UserRole() {}
    
    public UserRole(string role) : base(role) {}
}