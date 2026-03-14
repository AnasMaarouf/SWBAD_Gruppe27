public class Department {
    public int ID {get; set;}
    public string name {get; set;}
    public int FK_manager {get; set;}
    public Manager manager {get; set;}
    ICollection<Employee> employees {get; set;}
}