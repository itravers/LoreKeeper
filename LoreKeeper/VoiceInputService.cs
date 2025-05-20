using System.Text.Json;

public class VoiceInputService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> GetVoiceInputAsync()
    {
        var response = await client.PostAsync("http://localhost:5003/listen", null);
        string json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<VoiceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return result?.Transcript?.Trim();
    }

    private class VoiceResponse
    {
        public string Transcript { get; set; }
    }
}
