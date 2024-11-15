namespace SignalProvider.Core.Models;

public class ChatMessageModel
{
	public string UserName { get; set; } = null!;
	public string? SenderUserId { get; set; }
	public string? GroupId { get; set; }
	public string MessageContent { get; set; } = null!;
	public DateTime MessageSentAt = DateTime.UtcNow;
	public DateTime? MessageReceivedAt {  get; set; }
	public List<string>? AttachmentUrls { get; set; }
}