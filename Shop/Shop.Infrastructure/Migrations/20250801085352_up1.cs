using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    public partial class up1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Specifications",
                schema: "product",
                table: "Specifications");

            migrationBuilder.DropIndex(
                name: "IX_Specifications_ProductId",
                schema: "product",
                table: "Specifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                schema: "product",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ProductId",
                schema: "product",
                table: "Images");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specifications",
                schema: "product",
                table: "Specifications",
                columns: new[] { "ProductId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                schema: "product",
                table: "Images",
                columns: new[] { "ProductId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Specifications",
                schema: "product",
                table: "Specifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                schema: "product",
                table: "Images");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specifications",
                schema: "product",
                table: "Specifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                schema: "product",
                table: "Images",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Specifications_ProductId",
                schema: "product",
                table: "Specifications",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProductId",
                schema: "product",
                table: "Images",
                column: "ProductId");
        }
    }
}
