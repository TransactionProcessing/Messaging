using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingService.Common;

public sealed class SubscriptionPreWarmHostedService : IHostedLifecycleService
{
    private readonly IConfiguration configuration;
    private readonly IServiceProvider services;

    public SubscriptionPreWarmHostedService(IConfiguration configuration, IServiceProvider services)
    {
        this.configuration = configuration;
        this.services = services;
    }

    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Extensions.PreWarm(this.configuration, this.services, cancellationToken);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
