using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Media;

public class VoiceOutputService
{
    private static readonly HttpClient client = new();

    public async Task SpeakAsync(string text, string speaker = null, string emotion = null)
    {
        var requestData = new
        {
            text,
            speaker,
            emotion
        };

        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync("http://localhost:5002/narrate", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TTS Error] HTTP {(int)response.StatusCode}: {responseBody}");
                return;
            }

            var result = JsonSerializer.Deserialize<NarrateResponse>(responseBody);
            if (result.status == "ok" && File.Exists(result.path))
            {
                using var player = new SoundPlayer(result.path);
                player.PlaySync();
            }
            else
            {
                Console.WriteLine($"[TTS Error] Audio not generated. Server said: {result?.message ?? "unknown error"}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TTS Exception] {ex.Message}");
        }
    }

    private class NarrateResponse
    {
        public string status { get; set; }
        public string path { get; set; }
        public string message { get; set; } // optional for error details
    }
}
