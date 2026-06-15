using System.Text.Json;
using wm;

public class TurnierManager {
    private List<Spiel> games = new();
    private List<Mannschaft> teams = new();
    private List<Gruppe> groups = new();
    private List<Benutzer> users = new();
    private List<Wette> bets = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };


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

    public void saveAllData(string filename) {
        var data = new TurnierData {
            Mannschaften = teams.Select(t => new MannschaftDto { Name = t.Name }).ToList(),
            Gruppen = groups.Select(g => new GruppeDto {
                Name = g.Name,
                TeamNames = g.Teams.Select(t => t.Name).ToList()
            }).ToList(),
            Spiele = games.Select(s => new SpielDto {
                SpielId      = s.SpielId,
                HomeTeamName = s.HomeTeam.Name,
                AwayTeamName = s.AwayTeam.Name,
                Datum        = s.Datum,
                Uhrzeit      = s.Uhrzeit.ToString("HH:mm"),
                Ergebnis     = s.Ergebnis,
                Quotes       = s.Quotes.Select(q => new WettquoteDto {
                    Wetttyp = q.Wetttyp,
                    Quote   = q.Quote
                }).ToList()
            }).ToList(),
            Benutzer = users.Select(b => new BenutzerDto {
                Name     = b.Name,
                Guthaben = b.Guthaben
            }).ToList(),
            Wetten = bets.Select(w => new WetteDto {
                BenutzerName = w.BenutzerName,
                SpielId      = w.SpielId,
                Wetttyp      = w.Wetttyp,
                Quote        = w.Quote,
                Einsatz      = w.Einsatz,
                IsEvaluated  = w.IsEvaluated
            }).ToList()
        };

        var json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(filename, json);
        Console.WriteLine($"Daten gespeichert: {filename}");
    }

    public void loadAllData(string filename) {
        if (!File.Exists(filename))
            throw new FileNotFoundException($"Datei nicht gefunden: {filename}");

        var json = File.ReadAllText(filename);
        var data = JsonSerializer.Deserialize<TurnierData>(json, JsonOptions)
                   ?? throw new InvalidDataException("Ungültige JSON-Datei.");

        teams.Clear();
        groups.Clear();
        games.Clear();
        users.Clear();
        bets.Clear();

        // Rebuild teams
        foreach (var dto in data.Mannschaften)
            teams.Add(new Mannschaft(dto.Name));

        // Rebuild groups
        foreach (var dto in data.Gruppen)
        {
            var gruppe = new Gruppe(dto.Name);
            foreach (var tName in dto.TeamNames)
            {
                var team = teams.First(t => t.Name == tName);
                gruppe.addTeam(team);
            }
            groups.Add(gruppe);
        }

        // Rebuild games
        foreach (var dto in data.Spiele)
        {
            var home = teams.First(t => t.Name == dto.HomeTeamName);
            var away = teams.First(t => t.Name == dto.AwayTeamName);
            var uhrzeit = TimeOnly.ParseExact(dto.Uhrzeit, "HH:mm");
            var spiel = new Spiel(dto.SpielId, home, away, dto.Datum, uhrzeit)
            {
                Ergebnis = dto.Ergebnis
            };
            foreach (var q in dto.Quotes)
                spiel.setQuote(q.Wetttyp, q.Quote);
            games.Add(spiel);
        }

        // Rebuild users
        foreach (var dto in data.Benutzer)
            users.Add(new Benutzer(dto.Name, dto.Guthaben));

        // Rebuild bets
        foreach (var dto in data.Wetten)
        {
            var wette = new Wette(dto.Wetttyp, dto.Quote, dto.Einsatz, dto.BenutzerName, dto.SpielId)
            {
                IsEvaluated = dto.IsEvaluated
            };
            bets.Add(wette);
            var benutzer = users.FirstOrDefault(u => u.Name == dto.BenutzerName);
            benutzer?.addWette(wette);
        }

        Console.WriteLine($"Daten geladen: {filename}  ({teams.Count} Teams, {games.Count} Spiele)");
    }

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

