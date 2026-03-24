public class DetailedDepartmentResponseDTO
{
    public int ID { get; set; }
    public string DepartmentName {get; set;}
    public string Manager {get; set;}
    public List<string> employees {get; set;}
}

public class DepartmentResponseDTO
{
    public int ID { get; set; }
    public string DepartmentName {get; set;}
    public string Manager {get; set;}
}

public class CreateDepartmentDTO
{
    public string DepartmentName {get; set;}
    public int ManagerID {get; set;}
}

public class UpdateDepartmentDTO
{
    public string DepartmentName {get; set;}
    public int ManagerID {get; set;}
}