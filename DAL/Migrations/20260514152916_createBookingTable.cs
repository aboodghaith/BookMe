using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class createBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_AspNetUsers_UserId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Services",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_UserId_IsDeleted",
                table: "Services",
                newName: "IX_Services_ServiceId_IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_AspNetUsers_ServiceId",
                table: "Services",
                column: "ServiceId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_AspNetUsers_ServiceId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Services",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServiceId_IsDeleted",
                table: "Services",
                newName: "IX_Services_UserId_IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_AspNetUsers_UserId",
                table: "Services",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
