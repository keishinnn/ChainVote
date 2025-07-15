using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using ChainVote.Data;
using ChainVote.Models;
using ChainVote.Models.DatabaseEntities;

public class ElectionStatusUpdaterService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _services;

    // Constructor to get the IServiceProvider to access scoped services like DbContext
    public ElectionStatusUpdaterService(IServiceProvider services)
    {
        _services = services;
    }

    // This method is called when the hosted service starts
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Set up a timer that runs the UpdateElectionStatuses method every 1 minute
        _timer = new Timer(UpdateElectionStatuses, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    // This method updates the status of all election events based on current date and time
    private async void UpdateElectionStatuses(object state)
    {
        // Create a new service scope to safely access scoped services like DbContext
        using (var scope = _services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Get the current system time
            var now = DateTime.Now;

            // Retrieve all elections from the database
            var elections = await context.EventsData.ToListAsync();

            // Loop through each election to update its status
            foreach (var election in elections)
            {
                // If the current time is between the start and end dates, set status to InProgress
                if (now >= election.StartDate && now < election.EndDate)
                {
                    if (election.Status != ElectionStatus.InProgress)
                        election.Status = ElectionStatus.InProgress;
                }
                // If the current time is after the end date, set status to Completed
                else if (now >= election.EndDate)
                {
                    if (election.Status != ElectionStatus.Completed)
                        election.Status = ElectionStatus.Completed;
                }
                // If the current time is before the start date, set status to Awaiting
                else
                {
                    if (election.Status != ElectionStatus.Awaiting)
                        election.Status = ElectionStatus.Awaiting;
                }
            }

            // Save all changes made to the database
            await context.SaveChangesAsync();
        }
    }

    // This method is called when the hosted service stops
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop the timer when the service is stopped
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    // Dispose the timer to free up resources
    public void Dispose()
    {
        _timer?.Dispose();
    }
}
