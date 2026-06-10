namespace wm;

public class Wette {
    public string Wetttyp {get; set;} = String.Empty;
    public double Quote {get; set;}
    public double Einsatz {get; set;}
    public bool IsEvaluated {get; set;}

    // Im UML gilt Beziehung: Wette → Benutzer und Wette → Spiel
    // Normales Speichern, aka `public Benutzer Benutzer` etc würde beim Speichern als JSON jedoch zu Problemen führen:
    // Wette hat eine Referenz auf Benutzer, Benutzer hat eine Liste mit Wette-Objekten → der JSON-Serializer 
    // läuft im Kreis und wirft eine Exception
    //
    // Deswegen speichern wir nur Benutzername und ID direkt und lösen diese dann beim Laden in LoadAllData diese in 
    // Objektreferenzen auf
    public string BenutzerName { get; set; } = string.Empty;
    public int SpielId { get; set; }


    public Wette() {}

    public Wette(string typ, double quote, double einsatz) {
        Wetttyp = typ;
        Quote = quote;
        Einsatz = einsatz;
    }

    public double evaluate(string actualResult) {
        if (IsEvaluated) return 0;
        IsEvaluated = true;

        bool won = determineWin(actualResult);

        // wenn gewonnen -> Einsatz*Quote, ansonsten bekommt man 0 zurück
        return won ? Einsatz * Quote : 0;
    }

    private bool determineWin(string actualResult) {
        // actualResult hat format: "2:1", "1:1", "0:3" etc.
        var parts = actualResult.Split(':');
        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0], out int home) || !int.TryParse(parts[1], out int away))
            return false;

        return Wetttyp switch
        {
            "1"    => home > away,   // Home win
            "X"    => home == away,  // Draw
            "2"    => away > home,   // Away win
            "1X"   => home >= away,  // Homewin or Draw
            "X2"   => away >= home,  // Away win or Draw
            "12"   => home != away,  // Any win (no Draw)
            _      => false          // unkown -> always loose
        };
 
    }
}
