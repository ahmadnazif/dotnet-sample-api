using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SampleApi.SignalrHubs;

public class ChatHub(ILogger<ChatHub> logger) : Hub
{
    private readonly ILogger<ChatHub> logger = logger;

    public override Task OnConnectedAsync()
    {
        logger.LogInformation($"{nameof(OnConnectedAsync)} = ConnectionId: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) => await base.OnDisconnectedAsync(exception);
}
