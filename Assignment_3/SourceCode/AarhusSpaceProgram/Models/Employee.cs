public class Employee {
    public int ID { get; set; }  // Primary
    public string FullName { get; set; }
    public DateOnly HireDate { get; set; }
    public int? FK_DepartmentID { get; set; }  // Foreign key to department
    public Department? department { get; set; }  // Navigation to Department
    public Astronaut? astronaut { get; set; }  // Nullable, not all employees are astronauts
    public Scientist? scientist { get; set; }  // Nullable, not all employees are scientists
    public Manager? manager { get; set; }  // Nullable, not all employees are managers
}