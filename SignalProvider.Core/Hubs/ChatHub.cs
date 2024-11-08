using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalProvider.Core.Models;

namespace SignalProvider.Core;

public class ChatHub(IHubContext<NotificationHub> notificationHub, ILogger<ChatHub> logger) : Hub
{
  	private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
		private readonly ILogger<ChatHub> _logger = logger;

		private List<ChatMessageModel> messagesInWaitingRoom = new List<ChatMessageModel>();

	public override async Task OnConnectedAsync()
	{
		await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
		await Groups.AddToGroupAsync(Context.ConnectionId, "GuestSupportWaitingRoom");
		await Clients.Caller.SendAsync("AddedToGroup", "GuestSupportWaitingRoom");
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
	}

	public async Task RemoveFromGroup(string groupName)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
	}

	public async Task SendMessage(ChatMessageModel message)
	{
		try
		{
			if (message.GroupId == null)
			{
				await AddToGroup("GuestSupportWaitingRoom");
				await Clients.Caller.SendAsync("AddedToGroup", "GuestSupportWaitingRoom");

				message.GroupId = "GuestSupportWaitingRoom";				
				await Clients.Group("GuestSupportWaitingRoom").SendAsync("MessageReceived", message);
			}
			else 
			{
				await Clients.Group(message.GroupId).SendAsync("MessageReceived", message);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in SendMessage");
			throw;
		}
	}

	public async Task UserIsTyping()
	{
		try { await Clients.OthersInGroup("GuestSupportWaitingRoom").SendAsync("UserIsTyping", true); Console.WriteLine("typing");}			
		catch (Exception ex) { _logger.LogError(ex, "Error in UserIsTyping"); throw; }								
	}
	public async Task UserStoppedTyping()
	{
		try { await Clients.OthersInGroup("GuestSupportWaitingRoom").SendAsync("UserStoppedTyping", false); Console.WriteLine("not typing"); }			
		catch (Exception ex) { _logger.LogError(ex, "Error in UserStoppedTyping"); throw; }								
	}

}
