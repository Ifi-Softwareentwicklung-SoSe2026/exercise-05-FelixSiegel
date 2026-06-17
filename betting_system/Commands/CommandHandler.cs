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
            default:
                Console.WriteLine($"Unbekannter Befehl: '{command}'");
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
}
