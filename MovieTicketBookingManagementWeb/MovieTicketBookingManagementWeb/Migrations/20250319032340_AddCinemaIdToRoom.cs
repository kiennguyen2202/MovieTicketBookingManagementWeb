using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBookingManagementWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddCinemaIdToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropTable(
                name: "MoviePosters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoviePosters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviePosters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoviePosters_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoviePosters_MovieId",
                table: "MoviePosters",
                column: "MovieId",
                unique: true);
        }
    }
}
