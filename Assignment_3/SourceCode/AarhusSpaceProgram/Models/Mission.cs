using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;
public class Mission {
    

}

[Table("CelestialBodies")]
public class CelestialBodies {
    [Key]
    public string ID {get; set;}
    [NotNull]
    public string Name {get; set;}

    [NotNull]
    public int Distance {get; set;}
    
    [NotNull]
    public string BodyType {get; set;}
    
    [NotNull]
    public string PlanetType {get; set;}

    [ForeignKey("ID")]
    public string MoonID {get; set;}
}