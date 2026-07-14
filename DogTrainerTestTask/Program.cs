using System.Text.Json.Serialization;
using DogTrainerTestTask.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DogTrainerTestTask.Data.Entities;
using DogTrainerTestTask.Middleware;
using DogTrainerTestTask.Services;
using DogTrainerTestTask.Services.Impl;
using Microsoft.AspNetCore.Http.Json;

namespace DogTrainerTestTask;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(o =>
        {
            o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

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

        builder.Services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


        builder.Services.AddTransient<ILittersService, LittersService>();
        builder.Services.AddTransient<IDataSeeder, DataSeeder>();
        builder.Services.AddTransient<INotificationService, NotificationService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

            var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbcontext.Database.EnsureCreated();
            seeder.Seed();
        }

        app.Run();
    }
}