using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBookingManagementWeb.Migrations
{
    /// <inheritdoc />
    public partial class SwithToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Orders__UserID__17036CC0",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK__Payments__UserID__5812160E",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK__Reviews__UserID__5CD6CB2B",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Age",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Orders__UserID__17036CC0",
                table: "Orders",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Payments__UserID__5812160E",
                table: "Payments",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Reviews__UserID__5CD6CB2B",
                table: "Reviews",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Orders__UserID__17036CC0",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK__Payments__UserID__5812160E",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK__Reviews__UserID__5CD6CB2B",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCACDA95FF49", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D105349C68A34A",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Orders__UserID__17036CC0",
                table: "Orders",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__Payments__UserID__5812160E",
                table: "Payments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK__Reviews__UserID__5CD6CB2B",
                table: "Reviews",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");
        }
    }
}
