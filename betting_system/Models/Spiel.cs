namespace wm;

public class Spiel {
    public int SpielId {get; set;}
    public Mannschaft HomeTeam {get; set;} = new();
    public Mannschaft AwayTeam {get; set;} = new();
    public DateTime Datum {get; set;}
    public TimeOnly Uhrzeit {get; set;}
    public string Ergebnis {get; set;} = "-";
    private List<Wettquote> quotes = new();

    // Wir belassen quotes als private (Änderungen nur über setQuote etc) aber andere Klassen dürfen den Wert auslesen
    public IReadOnlyList<Wettquote> Quotes => quotes.AsReadOnly();

    public Spiel() {}

    public Spiel(int spielId, Mannschaft homeTeam, Mannschaft awayTeam, DateTime datum, TimeOnly uhrzeit)
    {
        SpielId = spielId;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        Datum = datum;
        Uhrzeit = uhrzeit;
    }

    public void setQuote(string type, double value) {
        var existing = quotes.FirstOrDefault(q => q.Wetttyp == type);
        if (existing != null)
            existing.Quote = value;
        else
            quotes.Add(new Wettquote(type, value));
    }

    public double getQuote(string type) {
        var quote = quotes.FirstOrDefault(q => q.Wetttyp == type);
        return quote?.Quote ?? 0.0;
    }

    public void setResult(string score) {
        Ergebnis = score;
    }

    public override string ToString()
    {
        return $"[{SpielId:D2}] {Datum:dd.MM.yyyy} {Uhrzeit:HH:mm}  {HomeTeam.Name,-20} vs  {AwayTeam.Name,-20}  Ergebnis: {Ergebnis}";
    }
}
