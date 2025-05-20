// LLMInterface.cs
//
// Simulates an LLM-based game master that interprets story state and user input,
// then returns either a command to run or a narrative message about what just happened.
//
// GetCommand(...) → returns intent as GameCommand
// NarrateResult(...) → returns narration for Ink result
//
// In production, this will be replaced with real LLM calls.

using System.Collections.Generic;
using System.Text;

public abstract class LLMInterface
{
    protected List<string> memoryLog = new List<string>();

    public abstract string GenerateNarrative(string storyText, List<string> choices, string priorAction =  null);

    public abstract GameCommand GetCommand(string currentNarrative, List<string> choices, string userInput);

    public abstract string NarrateResult(string inkResult);

    public virtual void ResetMemory()
    {
        memoryLog.Clear();
    }

    public IEnumerable<string> GetMemory(int maxEntries = 3)
    {
        return memoryLog.Count <= maxEntries
            ? memoryLog
            : memoryLog.GetRange(memoryLog.Count - maxEntries, maxEntries);
    }

    protected void AppendToMemory(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
            memoryLog.Add(text.Trim());
    }
}