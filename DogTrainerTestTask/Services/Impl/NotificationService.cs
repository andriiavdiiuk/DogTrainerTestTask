namespace DogTrainerTestTask.Services.Impl;

public class NotificationService : INotificationService
{
    public void Notify(string message)
    {
        Console.WriteLine(message);
    }
}