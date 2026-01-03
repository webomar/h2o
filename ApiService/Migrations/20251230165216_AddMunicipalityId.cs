using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddMunicipalityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add MunicipalityId column (non-nullable with default value)
            migrationBuilder.AddColumn<int>(
                name: "MunicipalityId",
                table: "OrganisatorischeEenheden",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove MunicipalityId column
            migrationBuilder.DropColumn(
                name: "MunicipalityId",
                table: "OrganisatorischeEenheden");
        }
    }
}
