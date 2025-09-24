using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FixVendorCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Users_AddedByLeaderId",
                table: "Vendors");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Users_AddedByLeaderId",
                table: "Vendors",
                column: "AddedByLeaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Users_AddedByLeaderId",
                table: "Vendors");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Users_AddedByLeaderId",
                table: "Vendors",
                column: "AddedByLeaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
