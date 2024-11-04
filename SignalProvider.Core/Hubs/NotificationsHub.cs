using Microsoft.AspNetCore.SignalR;

namespace SignalProvider.Core;

public class NotificationHub(IHubContext<ChatHub> chatHub) : Hub
{
  	private readonly IHubContext<ChatHub> _chatHub = chatHub;
	
	public async Task SendNotification(string userId, string message)
	{
		await Clients.User(userId).SendAsync("ReceiveNotification", message);
	}						
}