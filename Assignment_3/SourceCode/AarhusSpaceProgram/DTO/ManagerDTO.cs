public class ManagerResponseDTO
{
    public int Id { get; set; }
    public string FullName { get; set; }

    public List<string> Departments { get; set; }
    public List<string> Missions { get; set; }
}


public class CreateManagerDTO {
    public int EmployeeId { get; set; }
}
