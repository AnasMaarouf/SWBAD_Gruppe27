public class Manager
{
    public Employee Employee { get; set; }  // Navigation to Employee (1:1)
    public ICollection<Department>? Departments { get; set; }  // Departments managed by this manager (nullable)
    public ICollection<Mission>? Missions { get; set; }  // Missions managed by this manager (nullable)
}