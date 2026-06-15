namespace wm;

/// <summary>
/// Data Transfer Object for JSON persistence of all tournament data.
/// Flattened structure to avoid circular references.
/// </summary>
public class TurnierData
{
    public List<MannschaftDto> Mannschaften { get; set; } = new();
    public List<GruppeDto> Gruppen { get; set; } = new();
    public List<SpielDto> Spiele { get; set; } = new();
    public List<BenutzerDto> Benutzer { get; set; } = new();
    public List<WetteDto> Wetten { get; set; } = new();
}

public class MannschaftDto
{
    public string Name { get; set; } = string.Empty;
}

public class GruppeDto
{
    public string Name { get; set; } = string.Empty;
    public List<string> TeamNames { get; set; } = new();
}

public class WettquoteDto
{
    public string Wetttyp { get; set; } = string.Empty;
    public double Quote { get; set; }
}

public class SpielDto
{
    public int SpielId { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public string AwayTeamName { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
    public string Uhrzeit { get; set; } = "00:00";
    public string Ergebnis { get; set; } = "-";
    public List<WettquoteDto> Quotes { get; set; } = new();
}

public class BenutzerDto
{
    public string Name { get; set; } = string.Empty;
    public double Guthaben { get; set; }
}

public class WetteDto
{
    public string BenutzerName { get; set; } = string.Empty;
    public int SpielId { get; set; }
    public string Wetttyp { get; set; } = string.Empty;
    public double Quote { get; set; }
    public double Einsatz { get; set; }
    public bool IsEvaluated { get; set; }
}

