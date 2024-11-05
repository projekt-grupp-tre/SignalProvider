using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalProvider.Core.Models;

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


	public async Task AddToGroup(string groupName)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
		await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
	}

	public async Task RemoveFromGroup(string groupName)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
	}

	public async Task SendMessage(ChatMessageModel message)
  	{
		try
		{
			_logger.LogInformation($"Received message: {message}");

			if (message.GroupId == null)
			{
				await AddToGroup("GuestSupportWaitingRoom");
				await Clients.Caller.SendAsync("AddedToGroup", "GuestSupportWaitingRoom");
				await Clients.Group("GuestSupportWaitingRoom").SendAsync("MessageReceived", $"{message.UserName}: från chathub - ${DateTime.Now}");
				_logger.LogInformation($"Added client to waiting group");
			}
			else 
			{
				await AddToGroup(message.GroupId);
				await Clients.Group(message.GroupId).SendAsync("MessageReceived", $"{message.UserName}: från chathub - ${DateTime.Now}");
				_logger.LogInformation($"Added client to last group");
			}

			// _logger.LogInformation($"Received message: {message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in SendMessage");
			throw;
		}
	}
}
