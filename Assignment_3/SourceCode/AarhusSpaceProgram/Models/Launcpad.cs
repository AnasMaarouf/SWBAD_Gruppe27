public class Launchpad {
    // Primary Key for Launchpad
    public int ID { get; set; }
    public string Location { get; set; }
    public uint MaxSupportedWeight { get; set; }
    public string CurrentStatus { get; set; }
    public ICollection<Mission>? Missions {get; set;}
}
