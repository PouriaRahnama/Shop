using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    public partial class up2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                schema: "user",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                schema: "user",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                schema: "role",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleId",
                schema: "role",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                schema: "user",
                table: "Addresses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                schema: "user",
                table: "Wallets",
                columns: new[] { "UserId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                schema: "user",
                table: "Roles",
                columns: new[] { "UserId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                schema: "role",
                table: "Permissions",
                columns: new[] { "RoleId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                schema: "user",
                table: "Addresses",
                columns: new[] { "UserId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                schema: "user",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                schema: "user",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                schema: "role",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                schema: "user",
                table: "Addresses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                schema: "user",
                table: "Wallets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                schema: "user",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                schema: "role",
                table: "Permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                schema: "user",
                table: "Addresses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                schema: "role",
                table: "Permissions",
                column: "RoleId");
        }
    }
}
