// Purpose: Starts the prison escape text adventure.
// This file keeps the entry point small so the main game logic can be stored in the other classes.

namespace PrisonEscapeTextAdventure;

public static class Program
{
    // Main creates the file manager and game, then starts the game loop.
    public static void Main()
    {
        Console.Title = "Prison Escape - Text Adventure";

        SaveManager saveManager = new();
        Game game = new(saveManager);
        game.Start();
    }
}
