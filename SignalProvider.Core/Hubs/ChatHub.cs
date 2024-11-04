using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SignalProvider.Core;

public class ChatHub(IHubContext<NotificationHub> notificationHub, ILogger<ChatHub> logger) : Hub
{
  private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
	private readonly ILogger<ChatHub> _logger = logger;

	public override async Task OnConnectedAsync()
	{
			await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
			await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
			await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
			await base.OnDisconnectedAsync(exception);
	}

	public async Task SendMessage(string message)
  {
    try
		{
				_logger.LogInformation($"Received message: {message}");
				await Clients.All.SendAsync("ReceiveMessage", $"{message + DateTime.Now}");		}
		catch (Exception ex)
		{
				_logger.LogError(ex, "Error in SendMessage");
				throw;
		}
  }
}