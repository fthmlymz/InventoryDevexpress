using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "AssignedProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AssignedProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manager",
                table: "AssignedProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhysicalDeliveryOfficeName",
                table: "AssignedProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AssignedProduct",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "AssignedProduct");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "AssignedProduct");

            migrationBuilder.DropColumn(
                name: "Manager",
                table: "AssignedProduct");

            migrationBuilder.DropColumn(
                name: "PhysicalDeliveryOfficeName",
                table: "AssignedProduct");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AssignedProduct");
        }
    }
}
