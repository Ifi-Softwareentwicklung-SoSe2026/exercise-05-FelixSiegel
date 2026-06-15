using System.Text.Json;
using wm;

public class TurnierManager {
    private List<Spiel> games = new();
    private List<Mannschaft> teams = new();
    private List<Gruppe> groups = new();
    private List<Benutzer> users = new();
    private List<Wette> bets = new();


    // Helper methods to create objects
    private Gruppe createGroup(string name, params string[] teamNames) {
        var gruppe = new Gruppe(name);
        foreach (var tName in teamNames) {
            // Reuse existing team if already added (teams can appear in multiple groups in demo)
            var team = teams.FirstOrDefault(t => t.Name == tName) ?? new Mannschaft(tName);
            if (!teams.Contains(team))
                teams.Add(team);
            gruppe.addTeam(team);
        }
        return gruppe;
    }

    private void addMatch(ref int id, DateTime date, string time, string homeTeamName, string awayTeamName, Gruppe gruppe) {
        var home = getOrCreateTeam(homeTeamName);
        var away = getOrCreateTeam(awayTeamName);
        var uhrzeit = TimeOnly.ParseExact(time, "HH:mm");
        var spiel = new Spiel(id++, home, away, date, uhrzeit);
        games.Add(spiel);
        _ = gruppe; // gruppe is used for context; matches are stored globally
    }

    private Mannschaft getOrCreateTeam(string name) {
        var team = teams.FirstOrDefault(t => t.Name == name);
        if (team == null) {
            team = new Mannschaft(name);
            teams.Add(team);
        }
        return team;
    }

    // end helpers

    public void initializeTurnier() {
        teams.Clear();
        groups.Clear();
        games.Clear();

        // --- Group A ---
        var groupA = createGroup("A",
            "Deutschland", "Mexiko", "Schweden", "Südkorea");

        // --- Group B ---
        var groupB = createGroup("B",
            "Portugal", "Spanien", "Marokko", "Iran");

        // --- Group C ---
        var groupC = createGroup("C",
            "Frankreich", "Australien", "Peru", "Dänemark");

        // --- Group D ---
        var groupD = createGroup("D",
            "Argentinien", "Island", "Kroatien", "Nigeria");

        // --- Group E ---
        var groupE = createGroup("E",
            "Brasilien", "Schweiz", "Costa Rica", "Serbien");

        // --- Group F ---
        var groupF = createGroup("F",
            "Deutschland", "Mexiko", "Schweden", "Südkorea");

        groups.AddRange(new[] { groupA, groupB, groupC, groupD, groupE, groupF });

        // --- Spiele: Gruppenphase (Auswahl) ---
        int id = 1;
        var baseDate = new DateTime(2026, 6, 11);

        // Group A matches
        addMatch(ref id, baseDate.AddDays(0),  "14:00", "Deutschland",  "Mexiko",       groupA);
        addMatch(ref id, baseDate.AddDays(0),  "17:00", "Schweden",     "Südkorea",     groupA);
        addMatch(ref id, baseDate.AddDays(5),  "14:00", "Deutschland",  "Schweden",     groupA);
        addMatch(ref id, baseDate.AddDays(5),  "17:00", "Südkorea",     "Mexiko",       groupA);
        addMatch(ref id, baseDate.AddDays(10), "16:00", "Südkorea",     "Deutschland",  groupA);
        addMatch(ref id, baseDate.AddDays(10), "16:00", "Mexiko",       "Schweden",     groupA);

        // Group B matches
        addMatch(ref id, baseDate.AddDays(1),  "14:00", "Portugal",     "Spanien",      groupB);
        addMatch(ref id, baseDate.AddDays(1),  "17:00", "Marokko",      "Iran",         groupB);
        addMatch(ref id, baseDate.AddDays(6),  "14:00", "Portugal",     "Marokko",      groupB);
        addMatch(ref id, baseDate.AddDays(6),  "17:00", "Iran",         "Spanien",      groupB);
        addMatch(ref id, baseDate.AddDays(11), "16:00", "Iran",         "Portugal",     groupB);
        addMatch(ref id, baseDate.AddDays(11), "16:00", "Spanien",      "Marokko",      groupB);

        // Group C matches
        addMatch(ref id, baseDate.AddDays(2),  "14:00", "Frankreich",   "Australien",   groupC);
        addMatch(ref id, baseDate.AddDays(2),  "17:00", "Peru",         "Dänemark",     groupC);
        addMatch(ref id, baseDate.AddDays(7),  "14:00", "Frankreich",   "Peru",         groupC);
        addMatch(ref id, baseDate.AddDays(7),  "17:00", "Dänemark",     "Australien",   groupC);
        addMatch(ref id, baseDate.AddDays(12), "16:00", "Dänemark",     "Frankreich",   groupC);
        addMatch(ref id, baseDate.AddDays(12), "16:00", "Australien",   "Peru",         groupC);

        // Set some default quotes for early games
        foreach (var game in games.Take(6))
        {
            game.setQuote("1",  1.8);
            game.setQuote("X",  3.5);
            game.setQuote("2",  4.2);
        }

        Console.WriteLine($"Turnier initialisiert: {teams.Count} Mannschaften, {groups.Count} Gruppen, {games.Count} Spiele.");
    }

    public void saveAllData(string filename) {}

    public void loadAllData(string filename) {}

    public void printGames() {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              FIFA Weltmeisterschaft 2026 – Gruppenphase Spielplan                ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════╣");

        var byGroup = new Dictionary<string, List<Spiel>>();

        foreach (var gruppe in groups)
        {
            var groupGames = games.Where(g =>
                gruppe.Teams.Any(t => t.Name == g.HomeTeam.Name) &&
                gruppe.Teams.Any(t => t.Name == g.AwayTeam.Name)).ToList();
            byGroup[gruppe.Name] = groupGames;
        }

        // Games not assigned to any group
        var assignedIds = byGroup.Values.SelectMany(l => l).Select(g => g.SpielId).ToHashSet();
        var unassigned  = games.Where(g => !assignedIds.Contains(g.SpielId)).ToList();

        foreach (var (groupName, groupGames) in byGroup.OrderBy(kv => kv.Key))
        {
            if (!groupGames.Any()) continue;

            Console.WriteLine($"║  Gruppe {groupName}                                                                        ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════╣");

            foreach (var spiel in groupGames.OrderBy(s => s.Datum).ThenBy(s => s.Uhrzeit))
            {
                var quotes = spiel.Quotes.Any()
                    ? $"  Quote 1/X/2: {spiel.getQuote("1"):F2} / {spiel.getQuote("X"):F2} / {spiel.getQuote("2"):F2}"
                    : "";
                Console.WriteLine($"║  {spiel,-80}║");
                if (!string.IsNullOrEmpty(quotes))
                    Console.WriteLine($"║  {quotes,-80}║");
            }

            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════╣");
        }

        if (unassigned.Any())
        {
            Console.WriteLine("║  Weitere Spiele                                                                  ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════╣");
            foreach (var spiel in unassigned.OrderBy(s => s.SpielId))
                Console.WriteLine($"║  {spiel,-80}║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════╣");
        }

        Console.WriteLine($"║  Gesamt: {games.Count} Spiele  |  {teams.Count} Mannschaften  |  {groups.Count} Gruppen" +
                          $"{"",29}║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

    }

    public void setQuote(int spielId, string type, double quote) {
        var spiel = getSpielById(spielId);
        spiel.setQuote(type, quote);
    }

    public double getQuote(int spielId, string type) {
        return getSpielById(spielId).getQuote(type);
    }

    public void placeBid(string playerName, int spielId, string type, double amount) {
        var benutzer = getOrCreateBenutzer(playerName);
        var spiel    = getSpielById(spielId);
        var quote    = spiel.getQuote(type);

        if (quote <= 0)
            throw new InvalidOperationException($"Keine Quote für Typ '{type}' in Spiel {spielId}.");
        if (benutzer.Guthaben < amount)
            throw new InvalidOperationException($"Nicht genug Guthaben. Verfügbar: {benutzer.Guthaben:F2}€");

        benutzer.updateBalance(-amount);
        var wette = new Wette(type, quote, amount, playerName, spielId);
        bets.Add(wette);
        benutzer.addWette(wette);
    }

    public void setResult(int spielId, string score) {
        getSpielById(spielId).setResult(score);
    }

    public Spiel getSpielById(int id) {
        return games.FirstOrDefault(g => g.SpielId == id)
               ?? throw new KeyNotFoundException($"Spiel mit ID {id} nicht gefunden.");
    }

    public Benutzer getBenutzerByName(string name) {
        return users.FirstOrDefault(u => u.Name == name)
               ?? throw new KeyNotFoundException($"Benutzer '{name}' nicht gefunden.");
    }

    private Benutzer getOrCreateBenutzer(string name) {
        var benutzer = users.FirstOrDefault(u => u.Name == name);
        if (benutzer == null)
        {
            benutzer = new Benutzer(name);
            users.Add(benutzer);
        }
        return benutzer;
    }

}

