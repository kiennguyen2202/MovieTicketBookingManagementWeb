using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBookingManagementWeb.Migrations
{
    /// <inheritdoc />
    public partial class z : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapImageUrl",
                table: "Cinemas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Cinemas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapImageUrl",
                table: "Cinemas");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Cinemas");
        }
    }
}
