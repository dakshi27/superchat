using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FinalSchemaSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Users_AddedByAdminId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PendingFirstName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PendingLastName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PendingPasswordHash",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "UpdatedByAdminId",
                table: "Vendors",
                newName: "UpdatedByLeaderId");

            migrationBuilder.RenameColumn(
                name: "AddedByAdminId",
                table: "Vendors",
                newName: "AddedByLeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendors_AddedByAdminId",
                table: "Vendors",
                newName: "IX_Vendors_AddedByLeaderId");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Vendors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Vendors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByLeaderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_Users_CreatedByLeaderId",
                        column: x => x.CreatedByLeaderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobVendor",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobVendor", x => new { x.JobId, x.VendorId });
                    table.ForeignKey(
                        name: "FK_JobVendor_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobVendor_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_PublicId",
                table: "Vendors",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PublicId",
                table: "Users",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobId",
                table: "Employees",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PublicId",
                table: "Employees",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job_CreatedByLeaderId",
                table: "Job",
                column: "CreatedByLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_PublicId",
                table: "Job",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobVendor_VendorId",
                table: "JobVendor",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Job_JobId",
                table: "Employees",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Employees_Job_JobId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Users_AddedByLeaderId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "JobVendor");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_PublicId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Users_PublicId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Employees_JobId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PublicId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "UpdatedByLeaderId",
                table: "Vendors",
                newName: "UpdatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "AddedByLeaderId",
                table: "Vendors",
                newName: "AddedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendors_AddedByLeaderId",
                table: "Vendors",
                newName: "IX_Vendors_AddedByAdminId");

            migrationBuilder.AddColumn<string>(
                name: "PendingFirstName",
                table: "Vendors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingLastName",
                table: "Vendors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingPasswordHash",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Users_AddedByAdminId",
                table: "Vendors",
                column: "AddedByAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
