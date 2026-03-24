public class Department {
    public int ID {get; set;}
    public string name {get; set;}
    public int FK_managerID {get; set;}
    public Manager manager {get; set;}
    public ICollection<Employee>? employees {get; set;}
}