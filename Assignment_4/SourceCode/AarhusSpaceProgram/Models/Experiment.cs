public class Experiment {
    public int ID { get; set;}  // Primary Key
    public string Name {get; set;}
    public string Description {get; set;}
    public DateOnly CreationDate {get; set;}
    public int FK_MissionID {get; set;}
    public Mission? mission {get; set;}
    public ICollection<Joint_Scientist_Experiment>? joint_scientist_experiment {get; set;}
}