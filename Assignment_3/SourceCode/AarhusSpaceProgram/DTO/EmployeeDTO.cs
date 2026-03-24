// Object to list table using http get
public class DetailedEmployeeResponseDTO {
    public int Id { get; set; }
    public string FullName { get; set; }
    public DateOnly HireDate { get; set; }
    public string DepartmentName { get; set; }
    public string Role { get; set; }
}

public class EmployeeResponseDTO {
    public int Id { get; set; }
    public string FullName { get; set; }
    public string DepartmentName { get; set; }
}

// Object to create table using http post
public class CreateEmployeeDTO {
    public string FullName { get; set; }
    public DateOnly HireDate { get; set; }
    public int? DepartmentId { get; set; }
}

public class UpdateEmployeeDTO {
    public string FullName { get; set; }
    public DateOnly HireDate { get; set; }
    public int? DepartmentID { get; set; }
}
