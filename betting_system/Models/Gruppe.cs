namespace wm;

public class Gruppe {
    public string Name {get; set;} = string.Empty;
    private List<Mannschaft> teams  = new();

    // Wir exposen Teams als read only list nach außen und händeln intern über private teams (änderbar über methods)
    public IReadOnlyList<Mannschaft> Teams => teams.AsReadOnly();

    public Gruppe() {}

    public Gruppe(string name) {
        Name = name;
    }

    public void addTeam(Mannschaft team) {
        teams.Add(team);
    }
}
