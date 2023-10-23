using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataTransferApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLoadCheckMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExpectedSize",
                table: "SavedFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedSize",
                table: "SavedFiles");
        }
    }
}
