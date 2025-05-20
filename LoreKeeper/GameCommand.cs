// GameCommand.cs
// 
// Defines the structure of a command sent from the LLM to the Ink game engine.
// This command is the structured output of natural language input,
// telling the engine what action to take next.
//
// Supported commands include:
// - "make_choice" (select a choice by index)
// - "continue" (advance story when no choice is needed)
// - "call_function" (invoke a named Ink function with arguments)
// - "restart" (reset the story state)
//
// This class is designed to be serialized/deserialized as JSON,
// allowing easy integration with LLMs or external input sources.

using System.Collections.Generic;

public class GameCommand
{
    public string command { get; set; }           // e.g. "make_choice", "continue"
    public int? index { get; set; }               // used if command == "make_choice"
    public string name { get; set; }              // used if command == "call_function"
    public List<string> args { get; set; }        // used if command == "call_function"
}