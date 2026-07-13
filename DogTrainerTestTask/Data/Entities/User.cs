using Microsoft.AspNetCore.Identity;

namespace DogTrainerTestTask.Data.Entities;

public class User : IdentityUser<long>
{
    public ICollection<Litter>? Litters { get; set; }
    public BreederBenefit? BreederBenefit { get; set; }
    public ICollection<AuditLog>? AuditLogs { get; set; }
}