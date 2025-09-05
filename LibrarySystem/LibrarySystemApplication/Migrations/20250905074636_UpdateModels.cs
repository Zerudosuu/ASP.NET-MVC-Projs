using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystemApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublishedDate",
                table: "Books",
                newName: "DateAdded");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Books",
                newName: "Publisher");

            migrationBuilder.RenameColumn(
                name: "CoverImageUrl",
                table: "Books",
                newName: "CoverUrl");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Books",
                newName: "Categories");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Borrows",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublishYear",
                table: "Books",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublishYear",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Publisher",
                table: "Books",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "DateAdded",
                table: "Books",
                newName: "PublishedDate");

            migrationBuilder.RenameColumn(
                name: "CoverUrl",
                table: "Books",
                newName: "CoverImageUrl");

            migrationBuilder.RenameColumn(
                name: "Categories",
                table: "Books",
                newName: "Category");
        }
    }
}
