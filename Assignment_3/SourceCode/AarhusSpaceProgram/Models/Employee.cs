public class Employee
{
    public int ID { get; set; }  // Primary Key for Employee
    public string FullName { get; set; }
    public DateOnly HireDate { get; set; }
    public Department? Department { get; set; }  // Navigation to Department
    public Astronaut? Astronaut { get; set; }  // Nullable, not all employees are astronauts
    public Scientist? Scientist { get; set; }  // Nullable, not all employees are scientists
    public Manager? Manager { get; set; }  // Nullable, not all employees are managers
}