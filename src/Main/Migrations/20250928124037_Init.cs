using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisasterTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisasterTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Lon = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionDisasterConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegionId = table.Column<int>(type: "integer", nullable: false),
                    DisasterTypeId = table.Column<int>(type: "integer", nullable: false),
                    Threshold = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionDisasterConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionDisasterConfigurations_DisasterTypes_DisasterTypeId",
                        column: x => x.DisasterTypeId,
                        principalTable: "DisasterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegionDisasterConfigurations_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisasterTypes_Name",
                table: "DisasterTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RegionDisasterConfigurations_DisasterTypeId",
                table: "RegionDisasterConfigurations",
                column: "DisasterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionDisasterConfigurations_RegionId",
                table: "RegionDisasterConfigurations",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionDisasterConfigurations");

            migrationBuilder.DropTable(
                name: "DisasterTypes");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
