// LLMInterface.cs
//
// Simulates an LLM-based game master that interprets story state and user input,
// then returns a structured response with both a command for the Ink engine
// and natural language feedback for the player.
//
// In production, this mock will be replaced by a real LLM integration
// (e.g. OpenAI, DeepSeek, etc.) that performs intent recognition and command generation.
//
// Input:
// - Current story text
// - List of available choices
// - Freeform user input
//
// Output:
// - A GameResponse containing a message to the user and a GameCommand for the engine

using System;
using System.Collections.Generic;

public class LLMInterface
{
    public GameResponse GetResponse(string storyText, List<string> choices, string userInput)
    {
        Console.WriteLine("---- [LLM Debug Input] ----");
        Console.WriteLine(storyText.Trim());
        Console.WriteLine("\nAvailable choices:");
        for (int i = 0; i < choices.Count; i++)
        {
            Console.WriteLine($"  ({i}) {choices[i]}");
        }
        Console.WriteLine($"User input: \"{userInput}\"\n");

        // 🔍 Try to match a choice by keyword
        int selectedIndex = -1;
        for (int i = 0; i < choices.Count; i++)
        {
            if (userInput.ToLower().Contains(choices[i].ToLower().Split(' ')[0]))
            {
                selectedIndex = i;
                break;
            }
        }

        // 🧠 Fallback: just pick the first choice
        if (selectedIndex == -1 && choices.Count > 0)
        {
            selectedIndex = 0;
        }

        // 🎭 Mock response to the user
        string responseText = (selectedIndex >= 0)
            ? $"Sounds good. We'll choose: \"{choices[selectedIndex]}\"."
            : "No choices to make — continuing the story.";

        // 🧱 Build the command
        var command = new GameCommand
        {
            command = selectedIndex >= 0 ? "make_choice" : "continue",
            index = selectedIndex >= 0 ? selectedIndex : null
        };

        return new GameResponse
        {
            say_to_user = responseText,
            command = command
        };
    }
}