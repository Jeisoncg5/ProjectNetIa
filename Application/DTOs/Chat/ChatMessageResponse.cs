namespace ProjectNetIa.Application.DTOs.Chat;

public sealed class ChatMessageResponse
{
    public string Response { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string? InvoiceNumber { get; set; }

    public string? SaleOrigin { get; set; }
}
