using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkerService.Migrations
{
    /// <inheritdoc />
    public partial class JaarKeyFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Begrotingen",
                columns: table => new
                {
                    Jaar = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Totaalbedrag = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Begrotingen", x => x.Jaar);
                });

            migrationBuilder.CreateTable(
                name: "Functies",
                columns: table => new
                {
                    Functiecode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Functienaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Schaall = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Functies", x => x.Functiecode);
                });

            migrationBuilder.CreateTable(
                name: "Medewerkers",
                columns: table => new
                {
                    Nummer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalNummer = table.Column<int>(type: "int", nullable: false),
                    Achternaam = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medewerkers", x => x.Nummer);
                });

            migrationBuilder.CreateTable(
                name: "OrganisatorischeEenheden",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Omschrijving = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCode = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisatorischeEenheden", x => x.Code);
                    table.ForeignKey(
                        name: "FK_OrganisatorischeEenheden_OrganisatorischeEenheden_ParentCode",
                        column: x => x.ParentCode,
                        principalTable: "OrganisatorischeEenheden",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Periodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jaar = table.Column<int>(type: "int", nullable: false),
                    Maand = table.Column<int>(type: "int", nullable: false),
                    Verwerking = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dienstverbanden",
                columns: table => new
                {
                    Nummer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalNummer = table.Column<int>(type: "int", nullable: false),
                    MedewerkerNummer = table.Column<int>(type: "int", nullable: false),
                    Functiecode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumInDienst = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatumUitDienst = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ancienniteit = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dienstverbanden", x => x.Nummer);
                    table.ForeignKey(
                        name: "FK_Dienstverbanden_Functies_Functiecode",
                        column: x => x.Functiecode,
                        principalTable: "Functies",
                        principalColumn: "Functiecode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dienstverbanden_Medewerkers_MedewerkerNummer",
                        column: x => x.MedewerkerNummer,
                        principalTable: "Medewerkers",
                        principalColumn: "Nummer",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Crediteur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedewerkerNummer = table.Column<int>(type: "int", nullable: true),
                    OrganisatorischeEenheidCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rekening = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracten_Medewerkers_MedewerkerNummer",
                        column: x => x.MedewerkerNummer,
                        principalTable: "Medewerkers",
                        principalColumn: "Nummer",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contracten_OrganisatorischeEenheden_OrganisatorischeEenheidCode",
                        column: x => x.OrganisatorischeEenheidCode,
                        principalTable: "OrganisatorischeEenheden",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Kostenplaatsen",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Omschrijving = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganisatorischeEenheidCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kostenplaatsen", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Kostenplaatsen_OrganisatorischeEenheden_OrganisatorischeEenheidCode",
                        column: x => x.OrganisatorischeEenheidCode,
                        principalTable: "OrganisatorischeEenheden",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transacties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bedrag = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacties_Contracten_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Begrotingsregels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BegrotingJaar = table.Column<int>(type: "int", nullable: false),
                    MedewerkerNummer = table.Column<int>(type: "int", nullable: true),
                    KostenplaatsCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Kostensoort = table.Column<int>(type: "int", nullable: false),
                    Bedrag = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Begrotingsregels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Begrotingsregels_Begrotingen_BegrotingJaar",
                        column: x => x.BegrotingJaar,
                        principalTable: "Begrotingen",
                        principalColumn: "Jaar",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Begrotingsregels_Kostenplaatsen_KostenplaatsCode",
                        column: x => x.KostenplaatsCode,
                        principalTable: "Kostenplaatsen",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Begrotingsregels_Medewerkers_MedewerkerNummer",
                        column: x => x.MedewerkerNummer,
                        principalTable: "Medewerkers",
                        principalColumn: "Nummer",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Inhuurkosten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodeId = table.Column<int>(type: "int", nullable: false),
                    KostenplaatsCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Bedrag = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inhuurkosten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inhuurkosten_Kostenplaatsen_KostenplaatsCode",
                        column: x => x.KostenplaatsCode,
                        principalTable: "Kostenplaatsen",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inhuurkosten_Periodes_PeriodeId",
                        column: x => x.PeriodeId,
                        principalTable: "Periodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Begrotingsregels_BegrotingJaar",
                table: "Begrotingsregels",
                column: "BegrotingJaar");

            migrationBuilder.CreateIndex(
                name: "IX_Begrotingsregels_KostenplaatsCode",
                table: "Begrotingsregels",
                column: "KostenplaatsCode");

            migrationBuilder.CreateIndex(
                name: "IX_Begrotingsregels_MedewerkerNummer",
                table: "Begrotingsregels",
                column: "MedewerkerNummer");

            migrationBuilder.CreateIndex(
                name: "IX_Contracten_MedewerkerNummer",
                table: "Contracten",
                column: "MedewerkerNummer");

            migrationBuilder.CreateIndex(
                name: "IX_Contracten_OrganisatorischeEenheidCode",
                table: "Contracten",
                column: "OrganisatorischeEenheidCode");

            migrationBuilder.CreateIndex(
                name: "IX_Dienstverbanden_Functiecode",
                table: "Dienstverbanden",
                column: "Functiecode");

            migrationBuilder.CreateIndex(
                name: "IX_Dienstverbanden_MedewerkerNummer",
                table: "Dienstverbanden",
                column: "MedewerkerNummer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inhuurkosten_KostenplaatsCode",
                table: "Inhuurkosten",
                column: "KostenplaatsCode");

            migrationBuilder.CreateIndex(
                name: "IX_Inhuurkosten_PeriodeId",
                table: "Inhuurkosten",
                column: "PeriodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Kostenplaatsen_OrganisatorischeEenheidCode",
                table: "Kostenplaatsen",
                column: "OrganisatorischeEenheidCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisatorischeEenheden_ParentCode",
                table: "OrganisatorischeEenheden",
                column: "ParentCode");

            migrationBuilder.CreateIndex(
                name: "IX_Transacties_ContractId",
                table: "Transacties",
                column: "ContractId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Begrotingsregels");

            migrationBuilder.DropTable(
                name: "Dienstverbanden");

            migrationBuilder.DropTable(
                name: "Inhuurkosten");

            migrationBuilder.DropTable(
                name: "Transacties");

            migrationBuilder.DropTable(
                name: "Begrotingen");

            migrationBuilder.DropTable(
                name: "Functies");

            migrationBuilder.DropTable(
                name: "Kostenplaatsen");

            migrationBuilder.DropTable(
                name: "Periodes");

            migrationBuilder.DropTable(
                name: "Contracten");

            migrationBuilder.DropTable(
                name: "Medewerkers");

            migrationBuilder.DropTable(
                name: "OrganisatorischeEenheden");
        }
    }
}
