public class Launchpad {
    // Primary Key for Launchpad
    public int ID { get; set; }
    public string Location { get; set; }
    public int MaxSupportedWeight { get; set; }
    public int CurrentStatus { get; set; }
    public ICollection<Mission>? Missions {get; set;}
}
