using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataTransferApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileGroupId",
                table: "SavedFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FileGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedFiles_FileGroupId",
                table: "SavedFiles",
                column: "FileGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedFiles_FileGroups_FileGroupId",
                table: "SavedFiles",
                column: "FileGroupId",
                principalTable: "FileGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedFiles_FileGroups_FileGroupId",
                table: "SavedFiles");

            migrationBuilder.DropTable(
                name: "FileGroups");

            migrationBuilder.DropIndex(
                name: "IX_SavedFiles_FileGroupId",
                table: "SavedFiles");

            migrationBuilder.DropColumn(
                name: "FileGroupId",
                table: "SavedFiles");
        }
    }
}
