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
            case "set":
                HandleSet(manager, args);
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

    public static void HandleSet(TurnierManager manager, string[] args) {
        // set <spielId> <wetttyp> <quote> [datei]
        if (args.Length < 4) {
            Console.WriteLine("Verwendung: set <spielId> <Wetttyp> <Quote> [datei]");
            Console.WriteLine("Beispiel:   set 1 1 1.85");
            Console.WriteLine("Wetttypen:  1 (Heimsieg), X (Unentschieden), 2 (Auswärtssieg)");
            Console.WriteLine("            1X, X2, 12 (Doppelte Chance)");
            return;
        }

        if (!int.TryParse(args[1], out int spielId)) {
            Console.WriteLine($"Unfültige Spiel-Id: '{args[1]}' muss ganzzahlig sein.");
            return;
        }

        var wetttyp = args[2].ToUpperInvariant();

        if (!double.TryParse(args[3], out double quote) || quote <= 1.0) {
            Console.WriteLine($"Ungültige Quote: '{args[3]}' muss dezimal sein und größer als 1.0.");
            return;
        }

        var filename = args.Length > 4 ? args[4] : DefaultFileName;

        try {
            // load existing data to make sure we dont override anything
            if (File.Exists(filename)) {
                manager.loadAllData(filename);
            } else {
                Console.WriteLine($"Keine Datei '{filename}' gefunden. Initialisiere neues Turnier.");
                manager.initializeTurnier();
            }

            manager.setQuote(spielId, wetttyp, quote);
            Console.WriteLine($"Quote gesetzt: Spiel {spielId} | {wetttyp} = {quote:F2}");
            manager.saveAllData(filename);
        } catch (Exception e) {
            Console.WriteLine($"Error: {e.Message}");
        }
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
