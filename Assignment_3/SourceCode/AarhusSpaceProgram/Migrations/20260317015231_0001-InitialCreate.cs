using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AarhusSpaceProgram.Migrations
{
    /// <inheritdoc />
    public partial class _0001InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CelestialBodies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    BodyType = table.Column<string>(type: "NVARCHAR(20)", nullable: false),
                    PlanetType = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    FK_ParentPlanetID = table.Column<int>(type: "INT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CelestialBodies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CelestialBodies_CelestialBodies_FK_ParentPlanetID",
                        column: x => x.FK_ParentPlanetID,
                        principalTable: "CelestialBodies",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Crews",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crews", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Launchpads",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    MaxSupportedWeight = table.Column<int>(type: "INT", nullable: false),
                    CurrentStatus = table.Column<string>(type: "NVARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Launchpads", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rockets",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelName = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    FuelCapacity = table.Column<int>(type: "INT", nullable: false),
                    CrewCapacity = table.Column<int>(type: "INT", nullable: false),
                    NumberOfStages = table.Column<int>(type: "INT", nullable: false),
                    TotalWeight = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rockets", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Astronauts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightHours = table.Column<int>(type: "int", nullable: false),
                    Paygrade = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Astronauts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "JointAstronautCrews",
                columns: table => new
                {
                    CrewID = table.Column<int>(type: "INT", nullable: false),
                    AstronautID = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JointAstronautCrews", x => x.CrewID);
                    table.ForeignKey(
                        name: "FK_JointAstronautCrews_Astronauts_AstronautID",
                        column: x => x.AstronautID,
                        principalTable: "Astronauts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JointAstronautCrews_Crews_CrewID",
                        column: x => x.CrewID,
                        principalTable: "Crews",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    FK_managerID = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Emplyees",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    HireDate = table.Column<DateOnly>(type: "DATE", nullable: false),
                    FK_DepartmentID = table.Column<int>(type: "int", nullable: true),
                    DepartmentID = table.Column<int>(type: "INT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emplyees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Emplyees_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Managers_Emplyees_ID",
                        column: x => x.ID,
                        principalTable: "Emplyees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scientists",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scientists", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Scientists_Emplyees_ID",
                        column: x => x.ID,
                        principalTable: "Emplyees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Duration = table.Column<int>(type: "INT", nullable: false),
                    CurrentStatus = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Type = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    LaunchDate = table.Column<DateOnly>(type: "DATE", nullable: false),
                    FK_RocketID = table.Column<int>(type: "INT", nullable: false),
                    FK_launchpadID = table.Column<int>(type: "INT", nullable: false),
                    FK_CrewID = table.Column<int>(type: "INT", nullable: false),
                    FK_ManagerID = table.Column<int>(type: "INT", nullable: false),
                    FK_CelestialID = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Missions_CelestialBodies_FK_CelestialID",
                        column: x => x.FK_CelestialID,
                        principalTable: "CelestialBodies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_Crews_FK_CrewID",
                        column: x => x.FK_CrewID,
                        principalTable: "Crews",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_Launchpads_FK_launchpadID",
                        column: x => x.FK_launchpadID,
                        principalTable: "Launchpads",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_Managers_FK_ManagerID",
                        column: x => x.FK_ManagerID,
                        principalTable: "Managers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_Rockets_FK_RocketID",
                        column: x => x.FK_RocketID,
                        principalTable: "Rockets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JointScientistMissions",
                columns: table => new
                {
                    MissionID = table.Column<int>(type: "INT", nullable: false),
                    ScientistID = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JointScientistMissions", x => x.MissionID);
                    table.ForeignKey(
                        name: "FK_JointScientistMissions_Missions_MissionID",
                        column: x => x.MissionID,
                        principalTable: "Missions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JointScientistMissions_Scientists_ScientistID",
                        column: x => x.ScientistID,
                        principalTable: "Scientists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CelestialBodies_FK_ParentPlanetID",
                table: "CelestialBodies",
                column: "FK_ParentPlanetID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FK_managerID",
                table: "Departments",
                column: "FK_managerID");

            migrationBuilder.CreateIndex(
                name: "IX_Emplyees_DepartmentID",
                table: "Emplyees",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_JointAstronautCrews_AstronautID",
                table: "JointAstronautCrews",
                column: "AstronautID");

            migrationBuilder.CreateIndex(
                name: "IX_JointScientistMissions_ScientistID",
                table: "JointScientistMissions",
                column: "ScientistID");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_FK_CelestialID",
                table: "Missions",
                column: "FK_CelestialID");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_FK_CrewID",
                table: "Missions",
                column: "FK_CrewID");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_FK_launchpadID",
                table: "Missions",
                column: "FK_launchpadID");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_FK_ManagerID",
                table: "Missions",
                column: "FK_ManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_FK_RocketID",
                table: "Missions",
                column: "FK_RocketID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Astronauts_Emplyees_ID",
                table: "Astronauts",
                column: "ID",
                principalTable: "Emplyees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Managers_FK_managerID",
                table: "Departments",
                column: "FK_managerID",
                principalTable: "Managers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Emplyees_ID",
                table: "Managers");

            migrationBuilder.DropTable(
                name: "JointAstronautCrews");

            migrationBuilder.DropTable(
                name: "JointScientistMissions");

            migrationBuilder.DropTable(
                name: "Astronauts");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Scientists");

            migrationBuilder.DropTable(
                name: "CelestialBodies");

            migrationBuilder.DropTable(
                name: "Crews");

            migrationBuilder.DropTable(
                name: "Launchpads");

            migrationBuilder.DropTable(
                name: "Rockets");

            migrationBuilder.DropTable(
                name: "Emplyees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Managers");
        }
    }
}
