// LLMInterface.cs
//
// Simulates an LLM-based game master that interprets story state and user input,
// then returns either a command to run or a narrative message about what just happened.
//
// GetCommand(...) → returns intent as GameCommand
// NarrateResult(...) → returns narration for Ink result
//
// In production, this will be replaced with real LLM calls.

using System;
using System.Collections.Generic;

public class LLMInterface
{
    public GameCommand GetCommand(string storyText, List<string> choices, string userInput)
    {
        // Special-case command triggers
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
                name = "check_inventory"
            };
        }

        // Debug output
        Console.WriteLine("---- [LLM Debug Input] ----");
        Console.WriteLine(storyText.Trim());
        Console.WriteLine("\nAvailable choices:");
        for (int i = 0; i < choices.Count; i++)
        {
            Console.WriteLine($"  ({i}) {choices[i]}");
        }
        Console.WriteLine($"User input: \"{userInput}\"\n");

        // Try to match a keyword in choices
        int selectedIndex = -1;
        for (int i = 0; i < choices.Count; i++)
        {
            if (userInput.ToLower().Contains(choices[i].ToLower().Split(' ')[0]))
            {
                selectedIndex = i;
                break;
            }
        }

        // Fallback to first choice
        if (selectedIndex == -1 && choices.Count > 0)
        {
            selectedIndex = 0;
        }

        return new GameCommand
        {
            command = selectedIndex >= 0 ? "make_choice" : "continue",
            index = selectedIndex >= 0 ? selectedIndex : null
        };
    }

    public string NarrateResult(string inkResult)
    {
        if (string.IsNullOrWhiteSpace(inkResult))
            return "[No result from Ink function.]";

        return $"Here's what happened: {inkResult.Trim()}";
    }
}
