using Microsoft.AspNetCore.SignalR;

namespace SampleApi.SignalrHubs;

public class StatisticHub(ILogger<StatisticHub> logger) : Hub
{
    private readonly ILogger<StatisticHub> logger = logger;

    public override Task OnConnectedAsync()
    {
        logger.LogInformation($"{nameof(OnConnectedAsync)} = ConnectionId: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) => await base.OnDisconnectedAsync(exception);
}
