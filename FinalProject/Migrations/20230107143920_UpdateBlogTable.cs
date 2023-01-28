using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalProject.Migrations
{
    public partial class UpdateBlogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "Product_Size",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_Size_BlogId",
                table: "Product_Size",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Size_Blogs_BlogId",
                table: "Product_Size",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Size_Blogs_BlogId",
                table: "Product_Size");

            migrationBuilder.DropIndex(
                name: "IX_Product_Size_BlogId",
                table: "Product_Size");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "Product_Size");
        }
    }
}
