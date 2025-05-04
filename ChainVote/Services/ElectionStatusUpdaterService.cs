using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using ChainVote.Data; // adjust this namespace
using ChainVote.Models;
using ChainVote.Models.DatabaseEntities; // adjust if needed

public class ElectionStatusUpdaterService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _services;

    public ElectionStatusUpdaterService(IServiceProvider services)
    {
        _services = services;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(UpdateElectionStatuses, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // every 1 min
        return Task.CompletedTask;
    }

    private async void UpdateElectionStatuses(object state)
    {
        using (var scope = _services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = DateTime.Now;
            var elections = await context.EventsData.ToListAsync();

            foreach (var election in elections)
            {
                if (now >= election.StartDate && now < election.EndDate)
                {
                    if (election.Status != ElectionStatus.InProgress)
                        election.Status = ElectionStatus.InProgress;
                }
                else if (now >= election.EndDate)
                {
                    if (election.Status != ElectionStatus.Completed)
                        election.Status = ElectionStatus.Completed;
                }
                else
                {
                    if (election.Status != ElectionStatus.Awaiting)
                        election.Status = ElectionStatus.Awaiting;
                }
            }

            await context.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
