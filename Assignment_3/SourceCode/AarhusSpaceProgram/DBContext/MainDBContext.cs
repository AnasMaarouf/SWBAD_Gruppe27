using Microsoft.EntityFrameworkCore;

public class MyDBContext : DbContext {
	private const string DbName = "EFGetStarted";
    private const string ConnectionString = $"Data Source=localhost;Initial Catalog={DbName};User ID=sa;Password=Abcd123456!;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Authentication=SqlPassword;Application Intent=ReadWrite;";
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(ConnectionString);


    public DbSet<CelestialBody> CelestialBodies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Astronaut> Astronauts { get; set; }
    public DbSet<Scientist> Scientists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Model builder for celestial body
        modelBuilder.Entity<CelestialBody>(entity =>{
            entity.ToTable("CelestialBodies");

            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnType("NVARCHAR(100)")
                .HasColumnName("Name")
                .IsRequired();

            entity.Property(e => e.Distance)
                .HasColumnType("decimal(12,2)")
                .IsRequired();

            entity.Property(e => e.BodyType)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.PlanetType).HasColumnType("NVARCHAR(100)");

            // Self-referencing relationship
            entity.HasOne(e => e.ParentPlanet).WithMany(m => m.Moons).HasForeignKey();

            entity.ToTable("CelestialBodies", table => {
                table.HasCheckConstraint(
                    "CK_CelestialBodies_BodyType",
                    "(bodyType = 'Planet' AND PlanetType IS NOT NULL) OR (bodyType = 'Moon' AND PlanetType IS NULL)"
                );
            });
        });


        // Model builder for Employee
        modelBuilder.Entity<Employee>(entity =>{
            entity.ToTable("Emplyees");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.Property(e => e.HireDate)
                .HasColumnType("DATE")
                .IsRequired();

            entity.Property(e => e.FullName)
                .HasColumnType("VARCHAR(50)")
                .IsRequired();

        });

    }

}