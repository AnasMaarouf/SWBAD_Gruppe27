using Microsoft.EntityFrameworkCore;

public class MyDBContext : DbContext {
	private const string DbName = "EFGetStarted";
    private const string ConnectionString = $"Data Source=localhost;Initial Catalog={DbName};User ID=sa;Password=Abcd123456!;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Authentication=SqlPassword;Application Intent=ReadWrite;";
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(ConnectionString);


    public DbSet<CelestialBody> CelestialBodies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Manager> Managers { get; set; }
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
                .HasColumnType("NVARCHAR(20)")
                .IsRequired();

            entity.Property(e => e.PlanetType).HasColumnType("NVARCHAR(100)");

            // Self-referencing relationship
            entity.HasOne(moon => moon.ParentPlanet).WithMany(planet => planet.Moons).HasForeignKey(moon => moon.FK_ParentPlanetID);

            // CONSTRAINT NEEDS TO BE FIXED/REVIEWED !!!!!!!!!!!!!!!!!!!
            entity.ToTable("CelestialBodies", table => {
                table.HasCheckConstraint(
                    "CK_CelestialBodies_BodyType",
                    "(bodyType = 'Planet' AND PlanetType IS NOT NULL) OR (bodyType = 'Moon' AND PlanetType IS NULL)"
                );
            });
        });


        // Model builder for Employee
        modelBuilder.Entity<Employee>(entity => {
            entity.ToTable("Emplyees");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.Property(e => e.FullName)
                .HasColumnType("VARCHAR(50)")
                .IsRequired();

            entity.Property(e => e.HireDate)
                .HasColumnType("DATE")
                .IsRequired();

            // Department relationship
            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.FK_DepartmentID)
                .OnDelete(DeleteBehavior.SetNull);

        });

        // Model builder for Manager
        modelBuilder.Entity<Manager>(entity => {
            entity.ToTable("Managers");

            entity.HasKey(m => m.ID);
            entity.Property(m => m.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.HasOne(m => m.Employee)
                .WithOne(e => e.Manager)
                .HasForeignKey<Manager>(m => m.ID);
        });

        // Model builder for Astronaut
        modelBuilder.Entity<Astronaut>(entity => {
            entity.ToTable("Astronauts");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.HasOne(a => a.Employee)
                .WithOne(e => e.Astronaut)
                .HasForeignKey<Astronaut>(a => a.ID);
        });

        // Model builder for Scientist
        modelBuilder.Entity<Scientist>(entity => {
            entity.ToTable("Scientists");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.HasOne(s => s.Employee)
                .WithOne(e => e.Scientist)
                .HasForeignKey<Scientist>(s => s.ID);
        });
    }
}