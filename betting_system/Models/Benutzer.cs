namespace wm;

public class Benutzer {
    public string Name {get; set;} = string.Empty;
    public double Guthaben {get; set;}
    private List<Wette> myBets = new();

    public Benutzer() {}

    public Benutzer(string name, double guthaben = 100.0) {
        Name = name;
        Guthaben = guthaben;
    }

    public void updateBalance(double amount) {
        Guthaben += amount;
    }

    public void addWette(Wette wette) {
        myBets.Add(wette);
    }
}
