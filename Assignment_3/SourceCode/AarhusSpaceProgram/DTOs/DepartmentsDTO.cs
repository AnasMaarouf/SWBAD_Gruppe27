namespace AarhusSpaceProgram.DTOs;

public class DepartmentDto
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public string? ManagerName { get; set; }
}

public class DepartmentCreateDto
{
    public string Name { get; set; } = "";
    public int? FK_ManagerID { get; set; }  // Optional, kan oprettes uden manager
}

public class DepartmentUpdateDto
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public int? FK_ManagerID { get; set; }  // Opdatering af manager hvis ønsket
}