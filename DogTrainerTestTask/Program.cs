using DogTrainerTestTask.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DogTrainerTestTask.Data.Entities;
using DogTrainerTestTask.Middleware;
using DogTrainerTestTask.Services;
using DogTrainerTestTask.Services.Impl;

namespace DogTrainerTestTask;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(o =>
        {
            o.UseInMemoryDatabase("InMemoryDatabase");

            if (builder.Environment.IsDevelopment())
            {
                o.EnableDetailedErrors();
                o.EnableSensitiveDataLogging();
            }
        });

        builder.Services.AddIdentity<User, UserRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<UserRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        
        builder.Services.AddTransient<IDataSeeder, DataSeeder>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        // empty lambda function is needed to prevent application startup crash.
        app.UseExceptionHandler(_ => { });
        
        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        using var scope = app.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

        seeder.Seed();
        
        app.Run();
    }
}