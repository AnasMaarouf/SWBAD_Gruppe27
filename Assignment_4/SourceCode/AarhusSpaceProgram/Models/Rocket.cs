public class Rocket
{
    public int ID { get; set; }  // Primary Key for Rocket
    public string ModelName { get; set; }
    public int? FuelCapacity { get; set; }
    public int? CrewCapacity { get; set; }
    public int? NumberOfStages { get; set; }
    public int? TotalWeight { get; set; }
    public Mission? Mission { get; set; }
}
