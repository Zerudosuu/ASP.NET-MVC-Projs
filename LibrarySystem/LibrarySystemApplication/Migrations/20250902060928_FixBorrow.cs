using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemApplication.Migrations
{
    /// <inheritdoc />
    public partial class FixBorrow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Borrows",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Borrows");
        }
    }
}
