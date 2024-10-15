using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailCampaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class roleUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Role",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ContactGroup",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ContactGroup",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ContactGroup",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "ContactGroup",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "ContactGroup",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ContactGroup");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ContactGroup");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ContactGroup");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ContactGroup");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ContactGroup");
        }
    }
}
