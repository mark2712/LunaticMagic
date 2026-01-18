public static class GameLocations
{
    public const string None = "None";

    public const string Forest1 = "Forest1";
    public const string Forest2 = "Forest2";
    public const string Forest3 = "Forest3";
    public const string Forest4 = "Forest4";
    public const string Forest5 = "Forest5";
    public const string Forest6 = "Forest6";

    public const string DesertForest1 = "DesertForest1";
    public const string DesertForest2 = "DesertForest2";

    public const string Desert1 = "Desert1";
    public const string Desert2 = "Desert2";

    public const string Castle1 = "Castle1";
    public const string Castle2 = "Castle2";

    public const string Field1 = "Field1";
    public const string Field2 = "Field2";

    public const string Desert3 = "Desert3";
    public const string Desert4 = "Desert4";

    public const string Wasteland1 = "Wasteland1";
}


public class GameChunks
{
    public string[,] Chunks = new string[,]
    {
        { GameLocations.Forest1, GameLocations.Forest2, GameLocations.DesertForest1, GameLocations.None },
        { GameLocations.None, GameLocations.Castle1, GameLocations.DesertForest2, GameLocations.Castle2 },
        { GameLocations.Field1, GameLocations.Field2, GameLocations.Desert3, GameLocations.Desert2 },
        { GameLocations.Forest5, GameLocations.Forest6, GameLocations.Desert4, GameLocations.Wasteland1 },
    };

    public GameChunks()
    {
        // WorldManager.GoTo(GameScenes.Level1, GameLocations.Forest1, GameChunks.Thicket1);
    }
}

