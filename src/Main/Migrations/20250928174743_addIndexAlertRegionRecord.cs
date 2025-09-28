using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Migrations
{
    /// <inheritdoc />
    public partial class addIndexAlertRegionRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegionAlertRecord_DisasterTypes_DisasterTypeId",
                table: "RegionAlertRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_RegionAlertRecord_Regions_RegionId",
                table: "RegionAlertRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegionAlertRecord",
                table: "RegionAlertRecord");

            migrationBuilder.RenameTable(
                name: "RegionAlertRecord",
                newName: "RegionAlertRecords");

            migrationBuilder.RenameIndex(
                name: "IX_RegionAlertRecord_RegionId",
                table: "RegionAlertRecords",
                newName: "IX_RegionAlertRecords_RegionId");

            migrationBuilder.RenameIndex(
                name: "IX_RegionAlertRecord_DisasterTypeId",
                table: "RegionAlertRecords",
                newName: "IX_RegionAlertRecords_DisasterTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegionAlertRecords",
                table: "RegionAlertRecords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RegionAlertRecords_CreateDate_RegionId",
                table: "RegionAlertRecords",
                columns: new[] { "CreateDate", "RegionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RegionAlertRecords_DisasterTypes_DisasterTypeId",
                table: "RegionAlertRecords",
                column: "DisasterTypeId",
                principalTable: "DisasterTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegionAlertRecords_Regions_RegionId",
                table: "RegionAlertRecords",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegionAlertRecords_DisasterTypes_DisasterTypeId",
                table: "RegionAlertRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_RegionAlertRecords_Regions_RegionId",
                table: "RegionAlertRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegionAlertRecords",
                table: "RegionAlertRecords");

            migrationBuilder.DropIndex(
                name: "IX_RegionAlertRecords_CreateDate_RegionId",
                table: "RegionAlertRecords");

            migrationBuilder.RenameTable(
                name: "RegionAlertRecords",
                newName: "RegionAlertRecord");

            migrationBuilder.RenameIndex(
                name: "IX_RegionAlertRecords_RegionId",
                table: "RegionAlertRecord",
                newName: "IX_RegionAlertRecord_RegionId");

            migrationBuilder.RenameIndex(
                name: "IX_RegionAlertRecords_DisasterTypeId",
                table: "RegionAlertRecord",
                newName: "IX_RegionAlertRecord_DisasterTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegionAlertRecord",
                table: "RegionAlertRecord",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegionAlertRecord_DisasterTypes_DisasterTypeId",
                table: "RegionAlertRecord",
                column: "DisasterTypeId",
                principalTable: "DisasterTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegionAlertRecord_Regions_RegionId",
                table: "RegionAlertRecord",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
