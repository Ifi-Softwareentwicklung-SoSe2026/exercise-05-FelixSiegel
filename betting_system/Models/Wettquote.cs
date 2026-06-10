namespace wm;

public class Wettquote {
    public string Wetttyp {get; set;} = string.Empty;
    public double Quote {get; set;}

    public Wettquote() {}

    public Wettquote(string typ, double quote) {
        Wetttyp = typ;
        Quote = quote;
    }
}
