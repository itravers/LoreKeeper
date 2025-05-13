using Ink.Runtime;
using System;
using System.IO;

namespace LoreKeeper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use argument or fallback to default
            string storyName = args.Length > 0 ? args[0] : "test1";

            string baseDir = AppContext.BaseDirectory;
            string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            string jsonPath = System.IO.Path.Combine(projectRoot, "CompiledInk", $"{storyName}.json");



            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"Error: Story file not found at: {jsonPath}");
                return;
            }

            Console.WriteLine($"Looking for: {System.IO.Path.GetFullPath(jsonPath)}");

            string inkJson = File.ReadAllText(jsonPath);
            var story = new Story(inkJson);

            while (true)
            {
                while (story.canContinue)
                {
                    Console.WriteLine(story.Continue().Trim());
                }

                if (story.currentChoices.Count > 0)
                {
                    for (int i = 0; i < story.currentChoices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {story.currentChoices[i].text}");
                    }

                    Console.Write("Choose an option: ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int choiceIndex) &&
                        choiceIndex >= 1 && choiceIndex <= story.currentChoices.Count)
                    {
                        story.ChooseChoiceIndex(choiceIndex - 1);
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("== The End ==");
                    break;
                }
            }
        }
    }
}
