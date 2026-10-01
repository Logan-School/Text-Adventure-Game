// Purpose: Stores information that belongs directly to the player.
// Keeping these values in their own class makes the player's data easy to use from the story and easy to save as part of the game state.

namespace PrisonEscapeTextAdventure;

public class Player
{
    // Basic player information.
    public string Name { get; set; } = "Prisoner";

    // Health is included as a value that can be implemented later.
    public int Health { get; set; } = 3;

    // NoiseLevel changes during the escape and affects later events.
    public int NoiseLevel { get; set; } = 0;
}
