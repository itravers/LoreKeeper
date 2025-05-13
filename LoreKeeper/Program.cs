using Ink.Runtime;
using System;
using System.IO;
using System.Collections.Generic;

namespace LoreKeeper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use argument or fallback to default
            string storyName = args.Length > 0 ? args[0] : "test3";

            string baseDir = AppContext.BaseDirectory;
            string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, "..", "..", ".."));
            string jsonPath = System.IO.Path.Combine(projectRoot, "CompiledInk", $"{storyName}.json");

            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"Error: Story file not found at: {jsonPath}");
                return;
            }

            Console.WriteLine($"Looking for: {System.IO.Path.GetFullPath(jsonPath)}");

            string inkJson = File.ReadAllText(jsonPath);
            var story = new Story(inkJson);
            story.ChoosePathString("main");

            var llm = new LLMInterface();

            while (true)
            {
                // Step 1: Continue story as far as possible
                while (story.canContinue)
                {
                    Console.WriteLine(story.Continue().Trim());
                }

                // Step 2: Get current choices
                var choices = new List<string>();
                foreach (var choice in story.currentChoices)
                    choices.Add(choice.text.Trim());

                // Step 3: Get user input
                Console.Write(">> ");
                string userInput = Console.ReadLine();

                string inkResult = null;

                // Step 4: Get command from LLM
                var cmd = llm.GetCommand(story.currentText, choices, userInput);

                // Step 5: Run command (set inkResult if calling a function)

                switch (cmd.command)
                {
                    case "make_choice":
                        if (cmd.index.HasValue && cmd.index.Value >= 0 && cmd.index.Value < story.currentChoices.Count)
                            story.ChooseChoiceIndex(cmd.index.Value);
                        else
                            Console.WriteLine("Invalid choice index from LLM.");
                        break;

                    case "continue":
                        if (!story.canContinue)
                            Console.WriteLine("No more content to continue.");
                        break;

                    case "restart":
                        story = new Story(inkJson);
                        Console.WriteLine("Story restarted.");
                        break;

                    case "call_function":
                        try
                        {
                            object result = cmd.args != null
                                ? story.EvaluateFunction(cmd.name, cmd.args.ToArray())
                                : story.EvaluateFunction(cmd.name);

                            if (result is string text && !string.IsNullOrWhiteSpace(text))
                            {
                                inkResult = text.Trim();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Error] Failed to call function '{cmd.name}': {ex.Message}");
                        }
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {cmd.command}");
                        break;
                }

                // Step 6: Let the LLM narrate the result
                if (!string.IsNullOrWhiteSpace(inkResult))
                {
                    string narration = llm.NarrateResult(inkResult);
                    Console.WriteLine($"LLM: {narration}");
                }
            }
        }
    }
}
