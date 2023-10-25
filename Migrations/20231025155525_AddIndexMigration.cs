using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataTransferApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "TokenLinks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SavedFileName",
                table: "SavedFiles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FileGroups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TokenLinks_Token",
                table: "TokenLinks",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_SavedFiles_SavedFileName",
                table: "SavedFiles",
                column: "SavedFileName");

            migrationBuilder.CreateIndex(
                name: "IX_FileGroups_Name",
                table: "FileGroups",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TokenLinks_Token",
                table: "TokenLinks");

            migrationBuilder.DropIndex(
                name: "IX_SavedFiles_SavedFileName",
                table: "SavedFiles");

            migrationBuilder.DropIndex(
                name: "IX_FileGroups_Name",
                table: "FileGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "TokenLinks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SavedFileName",
                table: "SavedFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FileGroups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
