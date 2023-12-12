using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class brandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brand_Company_CompanyId",
                table: "Brand");

            migrationBuilder.DropForeignKey(
                name: "FK_Model_Brand_BrandId",
                table: "Model");

            migrationBuilder.AlterColumn<int>(
                name: "BrandId",
                table: "Model",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Brand",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Brand",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_Company_CompanyId",
                table: "Brand",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Model_Brand_BrandId",
                table: "Model",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brand_Company_CompanyId",
                table: "Brand");

            migrationBuilder.DropForeignKey(
                name: "FK_Model_Brand_BrandId",
                table: "Model");

            migrationBuilder.AlterColumn<int>(
                name: "BrandId",
                table: "Model",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Brand",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Brand",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_Company_CompanyId",
                table: "Brand",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Model_Brand_BrandId",
                table: "Model",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
