public class Scientist
{
    public int ID { get; set; }  // Primary Key
    public Employee Employee { get; set; }  // Navigation to Employee
    public string Title { get; set; }
    public string Specialty { get; set; }
}