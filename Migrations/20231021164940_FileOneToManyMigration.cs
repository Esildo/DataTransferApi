using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataTransferApi.Migrations
{
    /// <inheritdoc />
    public partial class FileOneToManyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedFiles_AspNetUsers_UserId",
                table: "SavedFiles");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SavedFiles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedFiles_AspNetUsers_UserId",
                table: "SavedFiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedFiles_AspNetUsers_UserId",
                table: "SavedFiles");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SavedFiles",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedFiles_AspNetUsers_UserId",
                table: "SavedFiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
