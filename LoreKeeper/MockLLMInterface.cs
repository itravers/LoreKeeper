using System;
using System.Collections.Generic;
using System.Text;

public class MockLLMInterface : LLMInterface
{
    public override string GenerateNarrative(string storyText, List<string> choices, string priorAction = null)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Narrator:");
        builder.AppendLine(storyText.Trim());

        if (choices.Count > 0)
        {
            builder.AppendLine("\nYou may:");
            for (int i = 0; i < choices.Count; i++)
                builder.AppendLine($"- {choices[i]}");
        }

        string output = builder.ToString().Trim();
        AppendToMemory(output);
        return output;
    }

    public override GameCommand GetCommand(string currentNarrative, List<string> choices, string userInput)
    {
        // Simple hardcoded mock logic
        if (userInput.ToLower().Contains("use"))
        {
            return new GameCommand
            {
                command = "call_function",
                name = "use_item",
                args = new List<string> { "silver_key" }
            };
        }

        if (userInput.ToLower().Contains("inventory"))
        {
            return new GameCommand
            {
                command = "call_function",
                name = "show_inventory"
            };
        }

        // Match choice by keyword
        int selectedIndex = -1;
        for (int i = 0; i < choices.Count; i++)
        {
            if (userInput.ToLower().Contains(choices[i].ToLower().Split(' ')[0]))
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex == -1 && choices.Count > 0)
            selectedIndex = 0;

        return new GameCommand
        {
            command = selectedIndex >= 0 ? "make_choice" : "continue",
            index = selectedIndex >= 0 ? selectedIndex : null
        };
    }

    public override string NarrateResult(string inkResult)
    {
        string narration = $"Narrarator: {inkResult}";
        AppendToMemory(narration);
        return narration;
    }
}
