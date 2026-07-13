namespace DogTrainerTestTask.Data.Entities;

public class BreederBenefit
{
    public long BreederId { get; set; }
    public int FreeLimit { get; set; }
    public int UsedCount { get; set; }
    public User? Breeder {get; set;}
}