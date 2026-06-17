namespace wm;

public static class CommandHandler {
    private const string DefaultFileName = "data.json";

    public static void Handle(TurnierManager manager, string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Bitte einen command angeben.");
            return;
        }

        var command = args[0].ToLowerInvariant();

        switch (command) {
            case "new":
                HandleNew(manager, args);
                break;
            case "print":
                PrintGame(manager, args);
                break;
            case "help":
                PrintUsage();
                break;
            default:
                Console.WriteLine($"Unbekannter Befehl: '{command}'");
                PrintUsage();
                break;
        }
    ;}

    public static void HandleNew(TurnierManager manager, string[] args) {
        var filename = args.Length > 1 ? args[1] : DefaultFileName;

        Console.WriteLine("Neues Turnier wird erstellt...");
        manager.initializeTurnier();
        manager.saveAllData(filename);
        Console.WriteLine($"Neues Turnier wurde erstellt und in '{filename}' gespeichert.");
    }

    public static void PrintGame(TurnierManager manager, string[] args) {
        var filename = args.Length > 1 ? args[1] : DefaultFileName;

        if (File.Exists(filename)) {
            Console.WriteLine($"Lade Turnier aus '{filename}'...");
            manager.loadAllData(filename);
        } else {
            Console.WriteLine($"Keine Datei '{filename}' gefunden.");
            return;
        }
        manager.printGames();
    }

    private static void PrintUsage() {
        Console.WriteLine();
        Console.WriteLine("Usage:  dotnet run -- <Befehl> [file]");
        Console.WriteLine();
        Console.WriteLine("  new   [file]   Neues Turnier anlegen und speichern");
        Console.WriteLine($"                  Standard-Datei: {DefaultFileName}.json");
        Console.WriteLine();
        Console.WriteLine("  print [file]   Spielplan des gespeicherten Turniers ausgeben");
        Console.WriteLine($"                  Standard-Datei: {DefaultFileName}.json");
        Console.WriteLine();
    }

}
