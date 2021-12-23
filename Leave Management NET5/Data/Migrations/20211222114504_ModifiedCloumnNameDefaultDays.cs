using Microsoft.EntityFrameworkCore.Migrations;

namespace Leave_Management_NET5.Data.Migrations
{
    public partial class ModifiedCloumnNameDefaultDays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefalutDays",
                table: "LeaveTypes",
                newName: "DefaultDays");

            migrationBuilder.AddColumn<int>(
                name: "DefaultDays",
                table: "LeaveTypeVM",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultDays",
                table: "LeaveTypeVM");

            migrationBuilder.RenameColumn(
                name: "DefaultDays",
                table: "LeaveTypes",
                newName: "DefalutDays");
        }
    }
}
