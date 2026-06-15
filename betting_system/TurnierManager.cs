using wm;

public class TurnierManager {
    private List<Spiel> games = new();
    private List<Mannschaft> teams = new();
    private List<Gruppe> groups = new();
    private List<Benutzer> users = new();
    private List<Wette> bets = new();


    // Helper methods to create objects
    private Gruppe CreateGroup(string name, params string[] teamNames) {
        var gruppe = new Gruppe(name);
        foreach (var tName in teamNames) {
            // Reuse existing team if already added (teams can appear in multiple groups in demo)
            var team = teams.FirstOrDefault(t => t.Name == tName) ?? new Mannschaft(tName);
            if (!teams.Contains(team))
                teams.Add(team);
            gruppe.AddTeam(team);
        }
        return gruppe;
    }

    private void AddMatch(ref int id, DateTime date, string time, string homeTeamName, string awayTeamName, Gruppe gruppe) {
        var home = GetOrCreateTeam(homeTeamName);
        var away = GetOrCreateTeam(awayTeamName);
        var uhrzeit = TimeOnly.ParseExact(time, "HH:mm");
        var spiel = new Spiel(id++, home, away, date, uhrzeit);
        games.Add(spiel);
        _ = gruppe; // gruppe is used for context; matches are stored globally
    }

    private Mannschaft GetOrCreateTeam(string name) {
        var team = teams.FirstOrDefault(t => t.Name == name);
        if (team == null) {
            team = new Mannschaft(name);
            teams.Add(team);
        }
        return team;
    }

    // end helpers

    public void initializeTurnier() {}

    public void saveAllData(string filename) {}

    public void loadAllData(string filename) {}

    public void printGames() {}

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

