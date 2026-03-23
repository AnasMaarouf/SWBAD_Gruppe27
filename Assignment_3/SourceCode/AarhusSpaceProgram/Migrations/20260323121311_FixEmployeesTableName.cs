using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AarhusSpaceProgram.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeesTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Astronauts_Emplyees_ID",
                table: "Astronauts");

            migrationBuilder.DropForeignKey(
                name: "FK_Emplyees_Departments_DepartmentID",
                table: "Emplyees");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Emplyees_ID",
                table: "Managers");

            migrationBuilder.DropForeignKey(
                name: "FK_Scientists_Emplyees_ID",
                table: "Scientists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Emplyees",
                table: "Emplyees");

            migrationBuilder.RenameTable(
                name: "Emplyees",
                newName: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Emplyees_DepartmentID",
                table: "Employees",
                newName: "IX_Employees_DepartmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Astronauts_Employees_ID",
                table: "Astronauts",
                column: "ID",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentID",
                table: "Employees",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Employees_ID",
                table: "Managers",
                column: "ID",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scientists_Employees_ID",
                table: "Scientists",
                column: "ID",
                principalTable: "Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Astronauts_Employees_ID",
                table: "Astronauts");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentID",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Employees_ID",
                table: "Managers");

            migrationBuilder.DropForeignKey(
                name: "FK_Scientists_Employees_ID",
                table: "Scientists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Emplyees");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_DepartmentID",
                table: "Emplyees",
                newName: "IX_Emplyees_DepartmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emplyees",
                table: "Emplyees",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Astronauts_Emplyees_ID",
                table: "Astronauts",
                column: "ID",
                principalTable: "Emplyees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Emplyees_Departments_DepartmentID",
                table: "Emplyees",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Emplyees_ID",
                table: "Managers",
                column: "ID",
                principalTable: "Emplyees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scientists_Emplyees_ID",
                table: "Scientists",
                column: "ID",
                principalTable: "Emplyees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
