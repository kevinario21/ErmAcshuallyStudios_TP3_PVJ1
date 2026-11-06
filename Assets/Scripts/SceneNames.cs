// Nombres de las escenas. Tienen que coincidir EXACTO con los de File > Build Settings.
public static class SceneNames
{
    public const string MainMenu = "MainMenu";
    public const string Tutorial = "ErmAcshuallyStudios_TP3_PVJ1";
    public const string Nivel1 = "Nivel1";
    public const string Level2 = "Level2";
    public const string Victory = "Victory";
    public const string GameOver = "GameOver";

    // Orden de los niveles jugables. El GameManager pasa de uno al siguiente.
    public static readonly string[] Levels = { Tutorial, Nivel1, Level2 };
}
