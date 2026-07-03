using ProjectNetIa.Application.DTOs.Chat;

namespace ProjectNetIa.Application.Interfaces;

public interface IChatService
{
    Task<ChatMessageResponse> SendMessageAsync(ChatMessageRequest request);
}
