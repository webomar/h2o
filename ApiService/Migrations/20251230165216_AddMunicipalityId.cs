using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddMunicipalityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing primary key on Code
            migrationBuilder.DropPrimaryKey(
                name: "PK_OgranisatorischeEenheid",
                table: "OgranisatorischeEenheid"
            );

            // Add new identity column
            migrationBuilder.AddColumn<int>(
                    name: "MunicipalityId",
                    table: "OgranisatorischeEenheid",
                    nullable: false,
                    defaultValue: 0
                )
                .Annotation("SqlServer:Identity", "1, 1");

            // Set new primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_OgranisatorischeEenheid",
                table: "OgranisatorischeEenheid",
                column: "MunicipalityId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_OgranisatorischeEenheid",
                table: "OgranisatorischeEenheid"
            );

            // Remove MunicipalityId column
            migrationBuilder.DropColumn(
                name: "MunicipalityId",
                table: "OgranisatorischeEenheid"
            );

            // Restore primary key on Code
            migrationBuilder.AddPrimaryKey(
                name: "PK_OgranisatorischeEenheid",
                table: "OgranisatorischeEenheid",
                column: "Code"
            );
        }
    }
}
