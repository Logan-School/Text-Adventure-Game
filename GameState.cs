// Purpose: Stores all information needed to stop and resume a game.
// The save system serializes this object to a JSON file.

namespace PrisonEscapeTextAdventure;

public class GameState
{
    // Player contains the name, health, and noise level.
    public Player Player { get; set; } = new();

    // CurrentScene tells the story which section should be played next.
    public string CurrentScene { get; set; } = "Cell";

    // These values track major events that affect the ending.
    public bool HasStarted { get; set; }
    public bool IsGameOver { get; set; }
    public bool Escaped { get; set; }
    public bool GuardAlarmed { get; set; }

    // Creates the starting state for a brand-new game.
    public static GameState CreateNew(string playerName)
    {
        return new GameState
        {
            Player = new Player { Name = playerName },
            CurrentScene = "Cell",
            HasStarted = true
        };
    }
}
