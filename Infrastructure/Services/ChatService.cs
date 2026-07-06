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

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsJsonAsync("/chat/message", request);
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException(
                $"No fue posible conectarse con FastAPI: {exception.Message}");
        }
        catch (TaskCanceledException)
        {
            throw new InvalidOperationException("FastAPI tardo demasiado en responder.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var normalizedContent = string.IsNullOrWhiteSpace(errorContent)
                ? "Sin detalle adicional."
                : errorContent;

            throw new InvalidOperationException(
                $"FastAPI respondio con {(int)response.StatusCode}: {normalizedContent}");
        }

        var chatbotResponse = await response.Content.ReadFromJsonAsync<ChatMessageResponse>();

        if (chatbotResponse is null)
        {
            throw new InvalidOperationException("El servicio de chatbot devolvio una respuesta vacia.");
        }

        return chatbotResponse;
    }
}
