namespace DogTrainerTestTask.Data.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public long EntityId { get; set; }     
    public string EntityName { get; set; }
    public string Action { get; set; }
    public string? OldValues { get; set; }        
    public string? NewValues { get; set; }      
    public long ModifiedBy { get; set; }       
    public DateTimeOffset CreatedAt { get; set; } 
    public User? ModifiedByUser { get; set; }
}