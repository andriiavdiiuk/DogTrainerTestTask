namespace DogTrainerTestTask.Data.Entities;

public class Litter
{
    public long Id { get; set; }
    public long BreederId { get; set; }
    public LitterStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public User? Breeder { get; set; }
}