namespace ProjectNetIa.Api.Configuration;

public sealed class ChatbotOptions
{
    public const string SectionName = "Chatbot";

    public string BaseUrl { get; set; } = "http://localhost:8000";

    public int TimeoutSeconds { get; set; } = 30;
}
