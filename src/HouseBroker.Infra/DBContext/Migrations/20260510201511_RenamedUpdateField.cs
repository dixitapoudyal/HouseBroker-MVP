using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseBroker.Infra.DBContext.Migrations
{
    public partial class RenamedUpdateField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedOn",
                table: "PropertyInfo",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "ModifiedOn",
                table: "CommissionPrice",
                newName: "UpdatedOn");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "PropertyInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "PropertyInfo");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "PropertyInfo",
                newName: "ModifiedOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "CommissionPrice",
                newName: "ModifiedOn");
        }
    }
}
