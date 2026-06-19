using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceProviderIdToServiceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_AspNetUsers_ServiceId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Services",
                newName: "ServiceProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServiceId_IsDeleted",
                table: "Services",
                newName: "IX_Services_ServiceProviderId_IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_AspNetUsers_ServiceProviderId",
                table: "Services",
                column: "ServiceProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_AspNetUsers_ServiceProviderId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceProviderId",
                table: "Services",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServiceProviderId_IsDeleted",
                table: "Services",
                newName: "IX_Services_ServiceId_IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_AspNetUsers_ServiceId",
                table: "Services",
                column: "ServiceId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
