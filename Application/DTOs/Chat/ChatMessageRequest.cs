namespace ProjectNetIa.Application.DTOs.Chat;

public sealed class ChatMessageRequest
{
    public string SessionId { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? CustomerName { get; set; }

    public string? CustomerDocument { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerPhone { get; set; }
}
