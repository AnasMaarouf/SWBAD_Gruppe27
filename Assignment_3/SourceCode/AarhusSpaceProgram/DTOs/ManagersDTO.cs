namespace AarhusSpaceProgram.DTOs;

public class ManagerDto
{
    public int ID { get; set; }
    public string? FullName { get; set; }
}

public class ManagerCreateDto
{
    public int EmployeeID { get; set; }  // For at knytte manager til en employee
}

public class ManagerUpdateDto
{
    public int ID { get; set; }
    public int EmployeeID { get; set; }
}