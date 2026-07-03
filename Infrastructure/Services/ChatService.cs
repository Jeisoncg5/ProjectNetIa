using System.Net.Http.Json;
using ProjectNetIa.Application.DTOs.Chat;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChatMessageResponse> SendMessageAsync(ChatMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new InvalidOperationException("El identificador de sesion es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new InvalidOperationException("El mensaje es obligatorio.");
        }

        var response = await _httpClient.PostAsJsonAsync("/chat/message", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Error comunicandose con el servicio de chatbot en FastAPI.");
        }

        var chatbotResponse = await response.Content.ReadFromJsonAsync<ChatMessageResponse>();

        if (chatbotResponse is null)
        {
            throw new InvalidOperationException("El servicio de chatbot devolvio una respuesta vacia.");
        }

        return chatbotResponse;
    }
}
