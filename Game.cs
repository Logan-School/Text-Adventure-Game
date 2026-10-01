// Purpose: Controls the overall game flow and startup menu.
// Game connects the story, game state, and save manager.

namespace PrisonEscapeTextAdventure;

public class Game
{
    private readonly SaveManager _saveManager;
    private GameState _state = new();

    // SaveManager is provided to the game so file handling stays separate.
    public Game(SaveManager saveManager)
    {
        _saveManager = saveManager;
    }

    // Starts the game and continues playing scenes until an ending is reached.
    public void Start()
    {
        ShowStartupMenu();

        while (!_state.IsGameOver)
        {
            // Story receives callbacks for status and saving. This keeps those responsibilities out of the Story class itself.
            Story story = new(_state, ShowStatus, SaveGame);
            story.PlayNextScene();
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }

    // Displays the choices available before starting or loading a game.
    private void ShowStartupMenu()
    {
        Console.WriteLine();
        Console.WriteLine("PRISON ESCAPE TEXT ADVENTURE");
        Console.WriteLine();

        if (_saveManager.SaveExists())
        {
            Console.WriteLine("1. Load saved game");
            Console.WriteLine("2. Start a new game");
            Console.WriteLine("3. Delete save and start over");
            Console.WriteLine("(Press q to quit at any time)");

            int choice = ReadMenuChoice(1, 3);

            switch (choice)
            {
                case 1:
                    _state = _saveManager.Load() ?? CreateNewGame();
                    break;
                case 2:
                    _state = CreateNewGame();
                    break;
                case 3:
                    _saveManager.DeleteSave();
                    _state = CreateNewGame();
                    break;
            }
        }
        else
        {
            Console.WriteLine("No save file was found.");
            Console.WriteLine("Starting a new game.");
            Console.WriteLine("(Press q to quit any time)");
            _state = CreateNewGame();
        }
    }

    // Creates a new player and game state.
    private GameState CreateNewGame()
    {
        Console.WriteLine();
        Console.Write("Enter your prisoner's name: ");
        string? name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Prisoner";
        }

        return GameState.CreateNew(name.Trim());
    }

    // Saves the current state when the player selects the save option.
    private void SaveGame()
    {
        _saveManager.Save(_state);
        Console.WriteLine();
        Console.WriteLine("Game saved.");
    }

    // Displays player information without changing the game state.
    private void ShowStatus()
    {
        Console.WriteLine();
        Console.WriteLine("--------------- STATUS ----------------");
        Console.WriteLine($"Name: {_state.Player.Name}");
        Console.WriteLine($"Health: {_state.Player.Health}");
        Console.WriteLine($"Noise level: {_state.Player.NoiseLevel}");
        Console.WriteLine($"Location: {_state.CurrentScene}");
        Console.WriteLine($"Alarm triggered: {_state.GuardAlarmed}");
        Console.WriteLine("----------------------------------------");
    }

    // Reads a startup-menu number and rejects invalid input.
    private static int ReadMenuChoice(int min, int max)
    {
        while (true)
        {
            Console.Write("Choose: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= min && choice <= max)
            {
                return choice;
            }

            Console.WriteLine($"Please enter a number from {min} to {max}.");
        }
    }
}
