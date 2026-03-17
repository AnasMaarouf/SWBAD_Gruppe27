using Microsoft.EntityFrameworkCore;

public class MyDBContext : DbContext {
	private const string DbName = "AarhusSpaceProgram";
    private const string ConnectionString = $"Data Source=localhost;Initial Catalog={DbName};User ID=sa;Password=Abcd123456!;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Authentication=SqlPassword;Application Intent=ReadWrite;";
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(ConnectionString);

    // Models/Schemas
    public DbSet<CelestialBody> CelestialBodies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Astronaut> Astronauts { get; set; }
    public DbSet<Scientist> Scientists { get; set; }
    public DbSet<Crew> Crews { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Launchpad> Launchpads { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<Rocket> Rockets { get; set; }

    // Joints
    public DbSet<Joint_Astronaut_Crew> JointAstronautCrews { get; set; }
    public DbSet<Joint_Scientist_Mission> JointScientistMissions { get; set; }

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

            entity.Property(e => e.PlanetType)
                .HasColumnType("NVARCHAR(100)");

            // Self-referencing relationship
            entity.HasOne(moon => moon.ParentPlanet)
                .WithMany(planet => planet.Moons)
                .HasForeignKey(moon => moon.FK_ParentPlanetID);
        });


        // Model builder for Employee
        modelBuilder.Entity<Employee>(entity => {
            entity.ToTable("Emplyees");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.Property(e => e.FullName)
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            entity.Property(e => e.HireDate)
                .HasColumnType("DATE")
                .IsRequired();
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


        // Model builder for Crew.
        modelBuilder.Entity<Crew>(entity => {
            entity.ToTable("Crews");

            entity.HasKey(C => C.ID);
            entity.Property(C => C.ID)
                .HasColumnType("INT")
                .IsRequired();
        });

        // Model builder for Department.
        modelBuilder.Entity<Department>(entity => {
            entity.ToTable("Departments");

            // Primary Key.
            entity.HasKey(D => D.ID);
            entity.Property(D => D.ID)
                .HasColumnType("INT")
                .IsRequired();

            // Department Name.
            entity.Property(D => D.name)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();
            
            // Foreign Key for manager.
            entity.Property(D => D.FK_managerID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(D => D.manager)
                .WithMany(M => M.Departments)
                .HasForeignKey(D => D.FK_managerID);
        });

        // Model builder for Launchpad.
        modelBuilder.Entity<Launchpad>(entity => {
            entity.ToTable("Launchpads");

            // Primary Key.
            entity.HasKey(L => L.ID);
            entity.Property(L => L.ID)
                .HasColumnType("INT")
                .IsRequired();

            // Launchpad location variable constraints.
            entity.Property(L => L.Location)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();
            
            // Max Supported Weight variable constraints.
            entity.Property(L => L.MaxSupportedWeight)
                .HasColumnType("INT")
                .IsRequired();

            // Current status variable constraints.
            entity.Property(L => L.CurrentStatus)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();
        });

        // Model builder for Mission.
        modelBuilder.Entity<Mission>(entity => {
            entity.ToTable("Missions");

            // Primary Key.
            entity.HasKey(M => M.ID);
            entity.Property(M => M.ID)
                .HasColumnType("INT")
                .IsRequired();

            // Name variable constraints.
            entity.Property(M => M.Name)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            // Duration variable constraints.
            entity.Property(M => M.Duration)
                .HasColumnType("INT")
                .IsRequired();

            // Current status variable constraints.
            entity.Property(M => M.CurrentStatus)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            // Name variable constraints.
            entity.Property(M => M.Type)
                .HasColumnType("NVARCHAR(100)");

            // Launchdate variable constraints
            entity.Property(M => M.LaunchDate)
                .HasColumnType("DATE");

            // Foreign Key for Rocket, variable constraints.
            entity.Property(M => M.FK_RocketID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(M => M.AssignedRocket)
                .WithOne(R => R.Mission)
                .HasForeignKey<Mission>(M => M.FK_RocketID);
    
            // Foreign Key for Launchpad, variable constraints.
            entity.Property(M => M.FK_launchpadID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(M => M.launchpad)
                .WithMany(L => L.Missions)
                .HasForeignKey(M => M.FK_launchpadID);

            // Foreign Key for Crew, variable constraints.
            entity.Property(M => M.FK_CrewID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(M => M.crew)
                .WithMany(C => C.Missions)
                .HasForeignKey(M => M.FK_CrewID);

            // Foreign Key for Manager, variable constraints.
            entity.Property(M => M.FK_ManagerID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(Miss => Miss.manager)
                .WithMany(Man => Man.Missions)
                .HasForeignKey(Miss => Miss.FK_ManagerID);

            // Foreign Key for CelestialBody, variable constraints.
            entity.Property(M => M.FK_CelestialID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(M => M.celestialBody)
                .WithMany(C => C.Missions)
                .HasForeignKey(Miss => Miss.FK_CelestialID);
        });


        // Model builder for Launchpad.
        modelBuilder.Entity<Rocket>(entity => {
            entity.ToTable("Rockets");

            // Primary Key.
            entity.HasKey(R => R.ID);
            entity.Property(R => R.ID)
                .HasColumnType("INT")
                .IsRequired();

            // Modelname variable constraints
            entity.Property(R => R.ModelName)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            // FuelCapacity variable constraints
            entity.Property(R => R.FuelCapacity)
                .HasColumnType("INT")
                .IsRequired();

            // CrewCapacity variable constraints
            entity.Property(R => R.CrewCapacity)
                .HasColumnType("INT")
                .IsRequired();

            // Number of stages variable constraints
            entity.Property(R => R.NumberOfStages)
                .HasColumnType("INT")
                .IsRequired();

            // Total weight variable constraints
            entity.Property(R => R.TotalWeight)
                .HasColumnType("INT");

        });

        // JOINTS
        // Astronaut & Crew joint
        modelBuilder.Entity<Joint_Astronaut_Crew>(entity => {
            // Astronaut foreignkey
            entity.Property(AC => AC.AstronautID)
                .HasColumnType("INT").IsRequired();
            entity.HasOne(AC => AC.astronaut)
                .WithMany(A => A.joint_Astronaut_Crew)
                .HasForeignKey(AC => AC.AstronautID);

            // Crew foreignkey and as primary key
            entity.HasKey(AC => AC.CrewID);
            entity.Property(AC => AC.CrewID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(AC => AC.crew)
                .WithMany(C => C.joint_Astronaut_Crew)
                .HasForeignKey(AC => AC.CrewID);
        });

        // Scientist & Mission joint (for Many-To-Many Relationship)
        modelBuilder.Entity<Joint_Scientist_Mission>(entity => {
            // Scientist foreignkey
            entity.Property(SM => SM.ScientistID)
                .HasColumnType("INT").IsRequired();
            entity.HasOne(SM => SM.scientist)
                .WithMany(S => S.joint_scientist_missions)
                .HasForeignKey(AC => AC.ScientistID);

            // Mission foreignkey and as primary key
            entity.HasKey(SM => SM.MissionID);
            entity.Property(SM => SM.MissionID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(SM => SM.mission)
                .WithMany(M => M.joint_scientist_missions)
                .HasForeignKey(SM => SM.MissionID);
        });
    }
}