using Ink.Runtime;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetEnv;


namespace LoreKeeper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Use argument or fallback to default
            string storyName = args.Length > 0 ? args[0] : "isaacsday";

            string baseDir = AppContext.BaseDirectory;
            string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, "..", "..", ".."));
            string jsonPath = System.IO.Path.Combine(projectRoot, "CompiledInk", $"{storyName}.json");

            var voice = new VoiceOutputService();

            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"Error: Story file not found at: {jsonPath}");
                return;
            }

            Console.WriteLine($"Looking for: {System.IO.Path.GetFullPath(jsonPath)}");

            string inkJson = File.ReadAllText(jsonPath);
            var story = new Story(inkJson);
            story.ChoosePathString("main");

            Console.WriteLine("directory: " + Directory.GetCurrentDirectory());

            string envPath = System.IO.Path.Combine(projectRoot, ".env");
            DotNetEnv.Env.Load(envPath);
            var openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            //var llm = new MockLLMInterface();
            var llm = new OpenAILLMInterface(openaiKey, model: "gpt-4.1-mini");
            var voiceInput = new VoiceInputService();
            string priorChoice = null;
            bool skipStoryNarration = false;
            string generatedNarrative = "";
            var choices = new List<string>();
            

            while (true)
            {
                
                if (!skipStoryNarration)
                {
                    // STEP 1: Continue the Ink story and collect text
                    var storyTextBuilder = new System.Text.StringBuilder();
                    while (story.canContinue)
                    {
                        storyTextBuilder.AppendLine(story.Continue().Trim());
                    }
                    string storyText = storyTextBuilder.ToString().Trim();

                    // STEP 2: Collect all available choices
                    choices = new List<string>();
                    foreach (var choice in story.currentChoices)
                        choices.Add(choice.text.Trim());

                    // STEP 3: Generate LLM-style narration from story + choices
                    generatedNarrative = llm.GenerateNarrative(storyText, choices, priorChoice);
                    Console.WriteLine($"\nNarrator: {generatedNarrative}");
                    await voice.SpeakAsync(generatedNarrative, speaker: "p236");
                }

                // STEP 4: Get user input
                Console.Write(">> ");
                //string userInput = Console.ReadLine();
                string userInput = await voiceInput.GetVoiceInputAsync();

                // STEP 5: LLM decides what to do based on input, narrative, and choices
                var cmd = llm.GetCommand(generatedNarrative, choices, userInput);

                // STEP 6: Execute the command
                string inkResult = null;

                skipStoryNarration = false;

                switch (cmd.command)
                {
                    case "make_choice":
                        if (cmd.index.HasValue && cmd.index.Value >= 0 && cmd.index.Value < choices.Count){
                            priorChoice = story.currentChoices[cmd.index.Value].text;
                            story.ChooseChoiceIndex(cmd.index.Value);
                        }else{
                            Console.WriteLine("Invalid choice index from LLM.");
                        }
                            
                        break;

                    case "continue":
                        if (!story.canContinue)
                            Console.WriteLine("No more content to continue.");
                        break;

                    case "restart":
                        story = new Story(inkJson);
                        story.ChoosePathString("main");
                        llm.ResetMemory();
                        Console.WriteLine("Story restarted.");
                        break;

                    case "call_function":
                        try
                        {
                            string functionOutput = null;

                            object result = cmd.args != null
                                ? story.EvaluateFunction(cmd.name, out functionOutput, cmd.args.ToArray())
                                : story.EvaluateFunction(cmd.name, out functionOutput);


                            if (!string.IsNullOrWhiteSpace(functionOutput))
                            {
                                inkResult = functionOutput.Trim();  // ✅ This is what you want to pass to the LLM
                            }

                            if (result is string text && !string.IsNullOrWhiteSpace(result.ToString().Trim()))
                            {
                                inkResult = result.ToString().Trim();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Error] Failed to call function '{cmd.name}': {ex.Message}");
                        }
                        break;

                    case "continue_conversation":
                        if (cmd.args != null && cmd.args.Count > 0)
                        {
                            skipStoryNarration = true;
                            string clarification = cmd.args[0].Trim();
                            Console.WriteLine($"Narrator: {clarification}");
                            await voice.SpeakAsync(clarification, speaker: "p236");
                        }
                        else
                        {
                            Console.WriteLine("[LLM] continue_conversation was triggered but no clarification text was provided.");
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
                    
                    Console.WriteLine($"{narration}");
                }
            }
        }
    }
}
