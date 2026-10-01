// Purpose: Handles all file operations for the game.
// This class is kept separate from the story so the story does not need to know how save files are created, loaded, or deleted.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrisonEscapeTextAdventure;

public class SaveManager
{
    private readonly string _savePath;
    private readonly JsonSerializerOptions _jsonOptions;

    // The constructor sets the save-file name and JSON formatting options.
    public SaveManager(string savePath = "savegame.json")
    {
        _savePath = savePath;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    // Checks whether a save file already exists.
    public bool SaveExists()
    {
        return File.Exists(_savePath);
    }

    // Converts the current game state to JSON and writes it to the save file.
    public void Save(GameState state)
    {
        string json = JsonSerializer.Serialize(state, _jsonOptions);
        File.WriteAllText(_savePath, json);
    }

    // Reads and converts the save file back into a GameState object.
    public GameState? Load()
    {
        if (!SaveExists())
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            return JsonSerializer.Deserialize<GameState>(json, _jsonOptions);
        }
        catch (JsonException)
        {
            // A damaged save file should not crash the game.
            return null;
        }
        catch (IOException)
        {
            // File access problems are handled the same way as a bad save.
            return null;
        }
    }

    // Deletes the current save file when the player chooses to start over.
    public bool DeleteSave()
    {
        if (!SaveExists())
        {
            return false;
        }

        File.Delete(_savePath);
        return true;
    }
}
