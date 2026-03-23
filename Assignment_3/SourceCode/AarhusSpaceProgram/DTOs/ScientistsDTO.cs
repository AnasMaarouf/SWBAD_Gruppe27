namespace AarhusSpaceProgram.DTOs;

public class ScientistDto
{
    public int ID { get; set; }
    public string FullName { get; set; } = null!;
    public List<string>? Missions { get; set; }
}

public class ScientistCreateDto
{
    public int ID { get; set; }          // Skal matche Employee.ID
    public int FK_EmployeeID { get; set; }  // Reference til Employee
}

public class ScientistUpdateDto
{
    public int ID { get; set; }          // Skal matche Employee.ID
    public int FK_EmployeeID { get; set; }  // Reference til Employee
}