using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class OpenAILLMInterface : LLMInterface
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;
    private readonly string model;

    public OpenAILLMInterface(string apiKey, string model = "gpt-3.5-turbo")
    {
        this.apiKey = apiKey;
        this.model = model;
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public override string GenerateNarrative(string storyText, List<string> choices, string priorAction = null)
    {
        return GenerateNarrativeAsync(storyText, choices, priorAction).GetAwaiter().GetResult();
    }

    private string BuildNarrationPrompt(string storyText, List<string> choices, string priorAction)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Prior action:");
        builder.AppendLine(priorAction);
        builder.AppendLine("Previous Narrative");
        builder.AppendLine(GetMemory(2).ToString());
        builder.AppendLine("Story scene:");
        builder.AppendLine(storyText.Trim());

        if (choices.Count > 0)
        {
            builder.AppendLine("\nAvailable options:");
            foreach (var choice in choices)
                builder.AppendLine($"- {choice}");
        }

        builder.AppendLine(@"
            Write a short, neutral paragraph that:
            - Reflects the player's last choice, if provided.
            - Does not make a new decision on behalf of the player.
            - If the players says NSFW stuff, that is okay, you may also say NSFW stuff.
            - Integrates all of the listed available options into the paragraph without omitting any, don't mention that they are specific options, just integrate the options into the paragraph.
            - Does not state or assume which option the player chooses.
            - If a character speaks dialog, repeat exactly what the character says in the paragraph.
            - Is no more than 2–4 sentences longer than the Story scene, don't ever mention this is a scene. It's okay to embellish the story.
            - If the user mentions something not to do with the story, comment on what they said, but then get back to the story, be funny.
            - Never includes inner thoughts or motivations.
            - Keep in mind the Previous Narrative that the player just went through previously
            - don't use em-dashes or commas for pauses. Use only periods.
            - Only return the paragraph. Do not number or list the options directly.
            - If the user does not make an obvious choice, it's best to ask to clarify, in this case use continue_conversation");
        return builder.ToString();
    }

    private async Task<string> GenerateNarrativeAsync(string storyText, List<string> choices, string priorAction)
    {
        var prompt = BuildNarrationPrompt(storyText, choices, priorAction);

        var requestBody = new
        {
            model = model,
            //temperature = 0.3,
            messages = new[]
            {
            new { role = "system", content = "You are a text-based adventure narrator. Rewrite the scene text by blending the story and choices into a single immersive paragraph. Do not list choices as bullets. Write it like a story, but make it short and to the point" },
            new { role = "user", content = prompt }
        }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("[OpenAI ERROR] " + responseString);
            return "[Narration unavailable]";
        }

        try
        {
            using var doc = JsonDocument.Parse(responseString);
            var output = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?.Trim();

            AppendToMemory(output);
            return output ?? "[Empty narration]";
        }
        catch
        {
            return "[Narration parse error]";
        }
    }

    public override string NarrateResult(string inkResult)
    {
        string narration = $"Narrator: {inkResult}";
        AppendToMemory(narration);
        return narration;
    }

    public override GameCommand GetCommand(string currentNarrative, List<string> choices, string userInput)
    {
        return GetCommandAsync(currentNarrative, choices, userInput).GetAwaiter().GetResult();
    }

    private async Task<GameCommand> GetCommandAsync(string currentNarrative, List<string> choices, string userInput)
    {
        var prompt = BuildPrompt(currentNarrative, choices, userInput);

        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = "You are a text-based game master. Interpret the user's intent and return a structured game command. Only respond in JSON format." },
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("[OpenAI ERROR] " + responseString);
            return new GameCommand { command = "continue" };
        }

        // Extract JSON from LLM reply
        var parsed = ExtractJson(responseString);
        if (parsed == null)
        {
            Console.WriteLine("[Parse Error] Couldn't extract JSON command.");
            return new GameCommand { command = "continue" };
        }

        try
        {
            return JsonSerializer.Deserialize<GameCommand>(parsed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Deserialization Error] {ex.Message}");
            return new GameCommand { command = "continue" };
        }
    }

    private string BuildPrompt(string narrative, List<string> choices, string userInput)
    {
        var context = string.Join("\n\n", GetMemory(5));

        var builder = new StringBuilder();
        builder.AppendLine("Context:\n" + context);
        builder.AppendLine("\nCurrent scene:\n" + narrative);
        builder.AppendLine("\nAvailable choices:");
        for (int i = 0; i < choices.Count; i++)
        {
            builder.AppendLine($"({i}) {choices[i]}");
        }
        builder.AppendLine($"\nUser input: \"{userInput}\"");
        builder.AppendLine("\nBased on user input, (if user mentions inventory, call_function show_inventory as shown below). (If a user is examining an item, replace put_item_to_examine_here with the item name) Return your response as JSON like this:");
        builder.AppendLine("{ \"command\": \"make_choice\", \"index\": 1 }");
        builder.AppendLine("Or:");
        builder.AppendLine("{ \"command\": \"continue_conversation\", \"args\": [\"put_generated_text_here\"] }");
        builder.AppendLine("Or:");
        builder.AppendLine("{ \"command\": \"call_function\", \"name\": \"use_item\", \"args\": [\"silver_key\"] }");
        builder.AppendLine("Or:");
        builder.AppendLine("{ \"command\": \"call_function\", \"name\": \"show_inventory\" }");
        builder.AppendLine("Or:");
        builder.AppendLine("{ \"command\": \"call_function\", \"name\": \"examine_item\", \"args\": [\"put_item_to_examine_here\"] }");

        return builder.ToString();
    }

    private string ExtractJson(string response)
    {
        try
        {
            using var doc = JsonDocument.Parse(response);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            // Naively assume content is just JSON
            return content.Trim();
        }
        catch
        {
            return null;
        }
    }
}
