namespace wm;

public class Mannschaft {
    public string Name {get; set;} = string.Empty;

    public Mannschaft() {}

    public Mannschaft(string name) {
        Name = name;
    }

    public override string ToString() => Name;
}
