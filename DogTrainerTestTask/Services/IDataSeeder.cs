using Microsoft.EntityFrameworkCore;

namespace DogTrainerTestTask.Services;

public interface IDataSeeder
{
    void Seed();
}