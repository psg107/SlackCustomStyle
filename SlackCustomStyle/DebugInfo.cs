using System.Text.Json.Serialization;

namespace SlackCustomStyle;

public class DebugInfo
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("devtoolsFrontendUrl")]
    public string DevtoolsFrontendUrl { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("webSocketDebuggerUrl")]
    public string WebSocketDebuggerUrl { get; set; }
}
