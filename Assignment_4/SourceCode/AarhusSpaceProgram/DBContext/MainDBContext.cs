using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

public class MainDBContext : IdentityDbContext<ApiUser> {
    public MainDBContext(DbContextOptions<MainDBContext> options) : base(options){ }
    
    
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
    public DbSet<Experiment> Experiments { get; set; }

    // Joints
    public DbSet<Joint_Astronaut_Crew> JointAstronautCrews { get; set; }
    public DbSet<Joint_Scientist_Mission> JointScientistMissions { get; set; }
    public DbSet<Joint_Scientist_Experiment> JointScientistExperiments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ApiUser>(entity => {
            entity.Property(u => u.FullName)
                  .HasMaxLength(100);
        });

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
                .HasForeignKey(moon => moon.FK_ParentPlanetID)
                .OnDelete(DeleteBehavior.NoAction);
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
                .WithOne(e => e.manager)
                .HasForeignKey<Manager>(m => m.ID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Model builder for Astronaut
        modelBuilder.Entity<Astronaut>(entity => {
            entity.ToTable("Astronauts");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.HasOne(a => a.employee)
                .WithOne(e => e.astronaut)
                .HasForeignKey<Astronaut>(a => a.ID)
                .OnDelete(DeleteBehavior.Cascade);
            
        });

        // Model builder for Scientist
        modelBuilder.Entity<Scientist>(entity => {
            entity.ToTable("Scientists");

            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID)
                .HasColumnType("INT")
                .IsRequired();

            entity.HasOne(s => s.Employee)
                .WithOne(e => e.scientist)
                .HasForeignKey<Scientist>(s => s.ID)
                .OnDelete(DeleteBehavior.Cascade);
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
                .HasForeignKey(D => D.FK_managerID)
                .OnDelete(DeleteBehavior.Restrict);
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
                .HasColumnType("int")
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

            // Current mission status variable constraints.
            entity.Property(M => M.CurrentStatus)
                .HasColumnType("int")
                .IsRequired();

            // Name variable constraints.
            entity.Property(M => M.Type)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            // Launchdate variable constraints
            entity.Property(M => M.LaunchDate)
                .HasColumnType("DATE");

            // Foreign Key for Rocket, variable constraints.
            entity.Property(M => M.FK_RocketID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(M => M.AssignedRocket)
                .WithOne(R => R.Mission)
                .HasForeignKey<Mission>(M => M.FK_RocketID)
                .OnDelete(DeleteBehavior.Restrict);
    
            // Foreign Key for Launchpad, variable constraints.
            entity.Property(M => M.FK_launchpadID)
                .HasColumnType("INT");
            entity.HasOne(M => M.launchpad)
                .WithMany(L => L.Missions)
                .HasForeignKey(M => M.FK_launchpadID)
                .OnDelete(DeleteBehavior.SetNull);

            // Foreign Key for Crew, variable constraints.
            entity.Property(M => M.FK_CrewID)
                .HasColumnType("INT");

            entity.HasOne(M => M.crew)
                .WithMany(C => C.Missions)
                .HasForeignKey(M => M.FK_CrewID)
                .OnDelete(DeleteBehavior.SetNull);

            // Foreign Key for Manager, variable constraints.
            entity.Property(M => M.FK_ManagerID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(Miss => Miss.manager)
                .WithMany(Man => Man.Missions)
                .HasForeignKey(Miss => Miss.FK_ManagerID)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign Key for CelestialBody, variable constraints.
            entity.Property(M => M.FK_CelestialID)
                .HasColumnType("INT");
            entity.HasOne(M => M.celestialBody)
                .WithMany(C => C.Missions)
                .HasForeignKey(Miss => Miss.FK_CelestialID)
                .OnDelete(DeleteBehavior.NoAction);
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

        // Model builder for Experiment.
        modelBuilder.Entity<Experiment>(entity => {
            entity.ToTable("Experiments");

            // Primary Key.
            entity.HasKey(E => E.ID);
            entity.Property(E => E.ID)
                .HasColumnType("INT")
                .IsRequired();

            // Name variable constraints
            entity.Property(E => E.Name)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            // Description variable constraints
            entity.Property(E => E.Description)
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            // CreationDate variable constraints
            entity.Property(E => E.CreationDate)
                .HasColumnType("DATE")
                .IsRequired();

            // Mission Foreignkey
            entity.Property(E => E.FK_MissionID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(E => E.mission)
                .WithMany(M => M.Experiments)
                .HasForeignKey(E => E.FK_MissionID)
                .OnDelete(DeleteBehavior.NoAction);

        });

        // JOINTS
        // Astronaut & Crew joint
        modelBuilder.Entity<Joint_Astronaut_Crew>(entity => {
            // Crew foreignkey and as primary key
            entity.HasKey(AC => AC.CrewID);
            entity.Property(AC => AC.CrewID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(AC => AC.crew)
                .WithMany(C => C.joint_Astronaut_Crew)
                .HasForeignKey(AC => AC.CrewID)
                .OnDelete(DeleteBehavior.Cascade);

            // Astronaut foreignkey
            entity.Property(AC => AC.AstronautID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(AC => AC.astronaut)
                .WithMany(A => A.joint_Astronaut_Crew)
                .HasForeignKey(AC => AC.AstronautID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Scientist & Mission joint (for Many-To-Many Relationship)
        modelBuilder.Entity<Joint_Scientist_Mission>(entity => {
            // Mission foreignkey and as primary key
            entity.HasKey(j => new { j.ScientistID, j.MissionID });
            entity.Property(SM => SM.MissionID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(SM => SM.mission)
                .WithMany(M => M.joint_scientist_missions)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Scientist foreignkey
            entity.Property(SM => SM.ScientistID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(SM => SM.scientist)
                .WithMany(S => S.joint_scientist_missions)
                .HasForeignKey(AC => AC.ScientistID)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // Scientist & Experiment joint (for Many-To-Many Relationship)
        modelBuilder.Entity<Joint_Scientist_Experiment>(entity => {
            // Experiment foreignkey and as primary key
            entity.HasKey(j => new { j.ScientistID, j.ExperimentID });
            entity.Property(SE => SE.ExperimentID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(SE => SE.experiment)
                .WithMany(E => E.joint_scientist_experiment)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Scientist foreignkey
            entity.Property(SE => SE.ScientistID)
                .HasColumnType("INT")
                .IsRequired();
            entity.HasOne(SE => SE.scientist)
                .WithMany(S => S.joint_scientist_experiments)
                .HasForeignKey(AC => AC.ScientistID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    
        base.OnModelCreating(modelBuilder);



        // ----------------------
        // Employees (REQUIRED BASE)
        // ----------------------
        modelBuilder.Entity<Employee>().HasData(
            new Employee { ID = 1, FullName = "Neil Armstrong" },
            new Employee { ID = 2, FullName = "Buzz Aldrin" },
            new Employee { ID = 3, FullName = "Sally Ride" },
            new Employee { ID = 4, FullName = "Carl Sagan" },
            new Employee { ID = 5, FullName = "Jane Foster" },
            new Employee { ID = 6, FullName = "Alice Johnson" },
            new Employee { ID = 7, FullName = "Bob Smith" }
        );

        // ----------------------
        // Astronauts (3+)
        // ----------------------
        modelBuilder.Entity<Astronaut>().HasData(
            new Astronaut { ID = 1, Rank = "Commander", FlightHours = 1200, Paygrade = "A1" },
            new Astronaut { ID = 2, Rank = "Pilot", FlightHours = 900, Paygrade = "A2" },
            new Astronaut { ID = 3, Rank = "Specialist", FlightHours = 700, Paygrade = "A3" }
        );

        // ----------------------
        // Scientists (2+)
        // ----------------------
        modelBuilder.Entity<Scientist>().HasData(
            new Scientist { ID = 4, Title = "Dr.", Specialty = "Astrophysics" },
            new Scientist { ID = 5, Title = "Dr.", Specialty = "Planetary Science" }
        );

        // ----------------------
        // Managers (2+)
        // ----------------------
        modelBuilder.Entity<Manager>().HasData(
            new Manager { ID = 6},
            new Manager { ID = 7}
        );

        // ----------------------
        // Rockets (2+)
        // ----------------------
        modelBuilder.Entity<Rocket>().HasData(
            new Rocket { ID = 1, ModelName = "Falcon 9", FuelCapacity = 500000, CrewCapacity = 7, NumberOfStages = 2, TotalWeight = 549000 },
            new Rocket { ID = 2, ModelName = "Saturn V", FuelCapacity = 950000, CrewCapacity = 3, NumberOfStages = 3, TotalWeight = 2970000 },
            new Rocket {ID = 3, ModelName = "Ares I-X", FuelCapacity = 650000, CrewCapacity = 4, NumberOfStages = 2, TotalWeight = 800000 },
            new Rocket {ID = 4, ModelName = "Ares II", FuelCapacity = 600000, CrewCapacity = 4, NumberOfStages = 2, TotalWeight = 820000 }
        );

        // ----------------------
        // Launchpads (2+)
        // ----------------------
        modelBuilder.Entity<Launchpad>().HasData(
            new Launchpad { ID = 1, Location = "Cape Canaveral" },
            new Launchpad { ID = 2, Location = "Vandenberg" }
        );

        // ----------------------
        // Celestial Bodies (3+ with hierarchy)
        // ----------------------
        modelBuilder.Entity<CelestialBody>().HasData(
            new CelestialBody { ID = 1, Name = "Earth", Distance = 0, BodyType = "Planet", PlanetType = "Rocky" },
            new CelestialBody { ID = 2, Name = "Mars", Distance = 225000000, BodyType = "Planet", PlanetType = "Rocky" },
            new CelestialBody { ID = 3, Name = "Moon", Distance = 384400, BodyType = "Moon",  PlanetType = "Rocky", FK_ParentPlanetID = 1 }
        );

        // ----------------------
        // Crews (needed for missions)
        // ----------------------
        modelBuilder.Entity<Crew>().HasData(
            new Crew { ID = 1 },
            new Crew { ID = 2 }
        );

        // ----------------------
        // Missions (2+)
        // ----------------------
        modelBuilder.Entity<Mission>().HasData(
            new Mission
            {
                ID = 1,
                Name = "Apollo 11",
                Duration = 8,
                CurrentStatus = Mission.Status.Completed,
                Type = "Lunar Landing",
                LaunchDate = new DateOnly(1969, 7, 16),

                FK_RocketID = 2,
                FK_launchpadID = 1,
                FK_CrewID = 1,
                FK_ManagerID = 7,
                FK_CelestialID = 3
            },
            new Mission
            {
                ID = 2,
                Name = "Mars Explorer",
                Duration = 300,
                CurrentStatus = Mission.Status.Planned,
                Type = "Mars Mission",
                LaunchDate = new DateOnly(2030, 3, 1),

                FK_RocketID = 1,
                FK_launchpadID = 2,
                FK_CrewID = 2,
                FK_ManagerID = 6,
                FK_CelestialID = 2
            },
            new Mission
            {
                ID = 3,
                Name = "ISS Resupply Alpha",
                Duration = 30,
                CurrentStatus = Mission.Status.Active,
                Type = "Resupply",
                LaunchDate = new DateOnly(2026, 4, 15),

                
                FK_RocketID = 3,

                FK_launchpadID = 1,
                FK_CrewID = 2,
                FK_ManagerID = 6,
                FK_CelestialID = 1
            }
        );

        // ----------------------
        // Scientist ↔ Mission (Many-to-Many)
        // ----------------------
        modelBuilder.Entity<Joint_Scientist_Mission>().HasData(
            new Joint_Scientist_Mission { ScientistID = 4, MissionID = 1 },
            new Joint_Scientist_Mission { ScientistID = 5, MissionID = 2 }
        );
    }
}
