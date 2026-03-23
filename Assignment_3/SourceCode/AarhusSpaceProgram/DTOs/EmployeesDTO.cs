namespace AarhusSpaceProgram.DTOs;

public class EmployeeDto
{
    public int ID { get; set; }
    public string FullName { get; set; } = "";
    public DateOnly HireDate { get; set; }

    public string? Department { get; set; }

    public bool IsAstronaut { get; set; }
    public bool IsScientist { get; set; }
    public bool IsManager { get; set; }

    // Optional: Detailed role info (for GET by ID)
    public AstronautRoleDto? Astronaut { get; set; }
    public ScientistRoleDto? Scientist { get; set; }
    public ManagerRoleDto? Manager { get; set; }
}

public class EmployeeCreateDto
{
    public string FullName { get; set; } = "";
    public DateOnly HireDate { get; set; }
    public int? FK_DepartmentID { get; set; }
}

public class EmployeeUpdateDto
{
    public int ID { get; set; }
    public string FullName { get; set; } = "";
    public DateOnly HireDate { get; set; }
    public int? FK_DepartmentID { get; set; }
}

// Role DTOs
public class AstronautRoleDto
{
    public int ID { get; set; }
}

public class ScientistRoleDto
{
    public int ID { get; set; }
}

public class ManagerRoleDto
{
    public int ID { get; set; }
}