// Purpose: Contains the story scenes, player choices, and gameplay events.
// Each scene changes GameState and then points the game toward the next scene.
// Status and saving are provided through callbacks from Game so this class does not directly manage files or the overall game loop.

namespace PrisonEscapeTextAdventure;

public class Story
{
    private readonly GameState _state;
    private readonly Action _showStatus;
    private readonly Action _saveGame;
    private readonly Random _random = new();

    // The constructor receives the current state and the utility actions.
    public Story(GameState state, Action showStatus, Action saveGame)
    {
        _state = state;
        _showStatus = showStatus;
        _saveGame = saveGame;
    }

    // Chooses the next scene based on the location stored in GameState.
    public bool PlayNextScene()
    {
        return _state.CurrentScene switch
        {
            "Cell" => CellScene(),
            "Tunnel" => TunnelScene(),
            "Yard" => YardScene(),
            "Wall" => WallScene(),
            "Outside" => OutsideScene(),
            "Caught" => CaughtScene(),
            _ => false
        };
    }

    // Cell Scene
    private bool CellScene()
    {
        PrintTitle("The Cell");
        Console.WriteLine("Midnight passes slowly. The prison has finally gone quiet.");
        Console.WriteLine("You have several possible ways to begin the escape.");
        Console.WriteLine();
        Console.WriteLine("1. Check status.");
        Console.WriteLine("2. Save game.");
        Console.WriteLine("3. Slip the loose vent cover from the wall.");
        Console.WriteLine("4. Sneak out when the guard makes his next round.");
        Console.WriteLine("5. Force the cell door and make a run for it.");
        Console.WriteLine("6. Stay still and listen for a safer opportunity.");

        int choice = ReadSceneChoice(4);

        switch (choice)
        {
            case 1:
                Console.WriteLine("You work the vent cover loose without making much sound.");
                _state.Player.NoiseLevel += 1;
                _state.CurrentScene = "Tunnel";
                break;
            case 2:
                Console.WriteLine("You time your movement with the guard's footsteps and slip into the hall.");
                _state.CurrentScene = "Yard";
                break;
            case 3:
                Console.WriteLine("You ram the door until the latch gives way. The whole corridor hears it.");
                _state.Player.NoiseLevel += 3;
                _state.GuardAlarmed = true;
                _state.CurrentScene = "Yard";
                break;
            case 4:
                Console.WriteLine("You wait. After several minutes, a maintenance alarm draws attention away from your block.");
                _state.Player.NoiseLevel = Math.Max(0, _state.Player.NoiseLevel - 1);
                _state.CurrentScene = "Tunnel";
                break;
        }

        return true;
    }

    // Tunnel Scene
    private bool TunnelScene()
    {
        PrintTitle("The Service Tunnel");
        Console.WriteLine("A narrow maintenance tunnel runs beneath the cell block.");
        Console.WriteLine("You can move carefully, or crawl quickly and risk making noise.");
        Console.WriteLine();
        Console.WriteLine("1. Check status.");
        Console.WriteLine("2. Save game.");
        Console.WriteLine("3. Crawl slowly and quietly.");
        Console.WriteLine("4. Move quickly through the pipes.");
        Console.WriteLine("5. Search the maintenance alcove.");

        int choice = ReadSceneChoice(3);

        switch (choice)
        {
            case 1:
                Console.WriteLine("You move one careful step at a time.");
                _state.CurrentScene = "Yard";
                break;
            case 2:
                Console.WriteLine("Your shoe strikes a pipe with a sharp metallic clang.");
                _state.Player.NoiseLevel += 2;
                _state.CurrentScene = "Yard";
                break;
            case 3:
                Console.WriteLine("You find a rusted service ladder and continue toward the yard.");
                _state.Player.NoiseLevel += 1;
                _state.CurrentScene = "Yard";
                break;
        }

        return true;
    }

    // Yard Scene
    private bool YardScene()
    {
        PrintTitle("The Prison Yard");
        Console.WriteLine("Cold air hits you as you step into the yard.");
        Console.WriteLine("A floodlight sweeps across the gravel every few seconds.");
        Console.WriteLine();
        Console.WriteLine("1. Check status.");
        Console.WriteLine("2. Save game.");
        Console.WriteLine("3. Cross the yard while the light turns away.");
        Console.WriteLine("4. Hide behind the equipment shed and wait.");
        Console.WriteLine("5. Run straight for the wall.");

        int choice = ReadSceneChoice(3);

        switch (choice)
        {
            case 1:
                Console.WriteLine("You time each movement with the rotating light.");
                _state.Player.NoiseLevel += 1;
                _state.CurrentScene = "Wall";
                break;
            case 2:
                Console.WriteLine("You wait in the shadows until the patrol passes.");
                _state.Player.NoiseLevel = Math.Max(0, _state.Player.NoiseLevel - 1);
                _state.CurrentScene = "Wall";
                break;
            case 3:
                Console.WriteLine("You sprint over the gravel. Your footsteps echo across the yard.");
                _state.Player.NoiseLevel += 3;
                _state.CurrentScene = "Wall";
                break;
        }

        return true;
    }

    // Wall Scene
    private bool WallScene()
    {
        PrintTitle("The Prison Wall");
        Console.WriteLine("You reach the outer wall. A guard's watch post sits directly above you.");
        Console.WriteLine($"Your current noise level is {_state.Player.NoiseLevel}.");
        Console.WriteLine();
        Console.WriteLine("1. Check status.");
        Console.WriteLine("2. Save game.");

        // This check uses a gameplay-altered value to determine the event.
        bool guardAwake = CheckGuardAwake();

        if (guardAwake)
        {
            Console.WriteLine("3. Climb anyway.");
            Console.WriteLine("4. Hide and wait for another chance.");

            int choice = ReadSceneChoice(2);

            if (choice == 1)
            {
                Console.WriteLine("You start climbing, but the guard spots you. An alarm begins to scream.");
                _state.GuardAlarmed = true;
                _state.CurrentScene = "Caught";
            }
            else
            {
                Console.WriteLine("You hide until the guard looks away, but the delay gives the patrol time to approach.");
                _state.Player.NoiseLevel = Math.Max(0, _state.Player.NoiseLevel - 1);
                _state.CurrentScene = "Wall";
            }
        }
        else
        {
            Console.WriteLine("3. Climb the wall.");
            Console.WriteLine("4. Search for a gate instead.");

            int choice = ReadSceneChoice(2);

            if (choice == 1)
            {
                Console.WriteLine("You climb slowly, pull yourself over the wall, and drop into the darkness beyond.");
                _state.CurrentScene = "Outside";
            }
            else
            {
                Console.WriteLine("You find a maintenance gate beneath the watch post. It is locked.");
                Console.WriteLine("You return to the wall and wait for another opportunity.");
                _state.Player.NoiseLevel += 1;
                _state.CurrentScene = "Wall";
            }
        }

        return true;
    }

    // Outside Scene
    private bool OutsideScene()
    {
        PrintTitle("Freedom");

        string[] endings =
        {
            "You disappear into the trees before the prison lights can find you.",
            "Rain begins to fall as you cross the road and leave the prison behind.",
            "For the first time in years, no walls surround you. You are free."
        };

        Console.WriteLine(endings[_random.Next(endings.Length)]);
        Console.WriteLine();
        Console.WriteLine("ESCAPE SUCCESSFUL.");

        _state.Escaped = true;
        _state.IsGameOver = true;
        return true;
    }

    // Caught Scene
    private bool CaughtScene()
    {
        PrintTitle("Caught");
        Console.WriteLine("The alarm draws guards from every direction.");
        Console.WriteLine("You are surrounded before you can reach the other side of the wall.");
        Console.WriteLine();
        Console.WriteLine("ESCAPE FAILED.");

        _state.IsGameOver = true;
        return true;
    }

    // This function determines whether the guard is awake based on values that were changed by the player's earlier choices.
    private bool CheckGuardAwake()
    {
        const int awakeThreshold = 3;
        return _state.Player.NoiseLevel >= awakeThreshold || _state.GuardAlarmed;
    }

    // Displays the two utility options and then returns the player's story choice.
    // Choosing status or save simply returns the player to this same menu.
    private int ReadSceneChoice(int storyOptionCount)
    {
        while (true)
        {
            Console.Write("Choose: ");
            string? input = Console.ReadLine();

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Game closed. Remember to save before quitting if you want to continue later.");
                Environment.Exit(0);
            }

            if (int.TryParse(input, out int choice))
            {
                if (choice == 1)
                {
                    _showStatus();
                    continue;
                }

                if (choice == 2)
                {
                    _saveGame();
                    continue;
                }

                int storyChoice = choice - 2;

                if (storyChoice >= 1 && storyChoice <= storyOptionCount)
                {
                    return storyChoice;
                }
            }

            Console.WriteLine("Enter a listed choice, or Q to quit.");
        }
    }

    // Prints a consistent heading for each major story section.
    private static void PrintTitle(string title)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine();
        Console.WriteLine();
    }
}
