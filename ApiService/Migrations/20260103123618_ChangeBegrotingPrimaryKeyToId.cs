using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBegrotingPrimaryKeyToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Begrotingsregels_Begrotingen_BegrotingJaar",
                table: "Begrotingsregels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Begrotingen",
                table: "Begrotingen");

            // First, save Jaar values to a temporary column
            migrationBuilder.AddColumn<int>(
                name: "JaarTemp",
                table: "Begrotingen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Begrotingen SET JaarTemp = Jaar");

            // Drop Jaar column (which has identity)
            migrationBuilder.DropColumn(
                name: "Jaar",
                table: "Begrotingen");

            // Add new Id column as identity (now there's no identity column)
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Begrotingen",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Recreate Jaar without identity
            migrationBuilder.AddColumn<int>(
                name: "Jaar",
                table: "Begrotingen",
                type: "int",
                nullable: false);

            migrationBuilder.Sql("UPDATE Begrotingen SET Jaar = JaarTemp");

            migrationBuilder.DropColumn(
                name: "JaarTemp",
                table: "Begrotingen");

            // Set primary key on Id
            migrationBuilder.AddPrimaryKey(
                name: "PK_Begrotingen",
                table: "Begrotingen",
                column: "Id");

            // Rename BegrotingJaar to BegrotingId in Begrotingsregels
            migrationBuilder.RenameColumn(
                name: "BegrotingJaar",
                table: "Begrotingsregels",
                newName: "BegrotingId");

            migrationBuilder.RenameIndex(
                name: "IX_Begrotingsregels_BegrotingJaar",
                table: "Begrotingsregels",
                newName: "IX_Begrotingsregels_BegrotingId");

            // Update BegrotingId in Begrotingsregels to match new Id values
            // Map based on Jaar value: find Begroting by Jaar, use its Id
            migrationBuilder.Sql(@"
                UPDATE br
                SET br.BegrotingId = b.Id
                FROM Begrotingsregels br
                INNER JOIN Begrotingen b ON br.BegrotingId = b.Jaar
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Begrotingsregels_Begrotingen_BegrotingId",
                table: "Begrotingsregels",
                column: "BegrotingId",
                principalTable: "Begrotingen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Begrotingsregels_Begrotingen_BegrotingId",
                table: "Begrotingsregels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Begrotingen",
                table: "Begrotingen");

            // Update BegrotingId back to Jaar values
            migrationBuilder.Sql(@"
                UPDATE br
                SET br.BegrotingId = b.Jaar
                FROM Begrotingsregels br
                INNER JOIN Begrotingen b ON br.BegrotingId = b.Id
            ");

            // Rename BegrotingId back to BegrotingJaar
            migrationBuilder.RenameColumn(
                name: "BegrotingId",
                table: "Begrotingsregels",
                newName: "BegrotingJaar");

            migrationBuilder.RenameIndex(
                name: "IX_Begrotingsregels_BegrotingId",
                table: "Begrotingsregels",
                newName: "IX_Begrotingsregels_BegrotingJaar");

            // Drop Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Begrotingen");

            // Recreate Jaar as identity column
            migrationBuilder.AddColumn<int>(
                name: "JaarTemp",
                table: "Begrotingen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Begrotingen SET JaarTemp = Jaar");

            migrationBuilder.DropColumn(
                name: "Jaar",
                table: "Begrotingen");

            migrationBuilder.AddColumn<int>(
                name: "Jaar",
                table: "Begrotingen",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.Sql("UPDATE Begrotingen SET Jaar = JaarTemp");

            migrationBuilder.DropColumn(
                name: "JaarTemp",
                table: "Begrotingen");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Begrotingen",
                table: "Begrotingen",
                column: "Jaar");

            migrationBuilder.AddForeignKey(
                name: "FK_Begrotingsregels_Begrotingen_BegrotingJaar",
                table: "Begrotingsregels",
                column: "BegrotingJaar",
                principalTable: "Begrotingen",
                principalColumn: "Jaar",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
