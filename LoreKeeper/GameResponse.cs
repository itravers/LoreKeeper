// GameResponse.cs
//
// Represents the output of the LLMInterface:
// - A natural language message to speak or show to the player
// - A structured GameCommand for the Ink engine to execute
//
// This forms the bridge between narrative interaction (LLM) and
// deterministic engine behavior (Ink).



public class GameResponse
{
    public string say_to_user { get; set; }
    public GameCommand command { get; set; }
}