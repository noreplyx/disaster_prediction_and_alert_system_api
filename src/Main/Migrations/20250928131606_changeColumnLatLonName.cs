using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class changeColumnLatLonName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lon",
                table: "Regions",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "Lat",
                table: "Regions",
                newName: "Latitude");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Regions",
                newName: "Lon");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Regions",
                newName: "Lat");
        }
    }
}
