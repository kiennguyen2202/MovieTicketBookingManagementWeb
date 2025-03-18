using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBookingManagementWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMoviePoster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cinemas__59C926260B1BBE46", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Movies__4BD2943AA05B8B91", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCACDA95FF49", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CinemaID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SeatCapacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rooms__3286391982D9C9F8", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Rooms__CinemaID__3F466844",
                        column: x => x.CinemaID,
                        principalTable: "Cinemas",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MoviePosters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__9B556A588F0B63FC", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Payments__UserID__5812160E",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    MovieID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reviews__74BC79AEE3371759", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Reviews__MovieID__5DCAEF64",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Reviews__UserID__5CD6CB2B",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomID = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SeatType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seats__311713D3FAA4D100", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Seats__RoomID__45F365D3",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Showtimes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieID = table.Column<int>(type: "int", nullable: false),
                    RoomID = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Showtime__32D31FC00CFB611F", x => x.ID);
                    table.ForeignKey(
                        name: "FK__Showtimes__Movie__4222D4EF",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Showtimes__RoomI__4316F928",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ShowtimeID = table.Column<int>(type: "int", nullable: false),
                    SeatID = table.Column<int>(type: "int", nullable: false),
                    TicketType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    FinalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BookingTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PopcornQuantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    DrinkQuantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    PopcornPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    DrinkPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    PaymentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tickets__712CC627F99AA1C7", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tickets_Payments",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__Tickets__SeatID__52593CB8",
                        column: x => x.SeatID,
                        principalTable: "Seats",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Tickets__Showtim__5165187F",
                        column: x => x.ShowtimeID,
                        principalTable: "Showtimes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Tickets__UserID__5070F446",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoviePosters_MovieId",
                table: "MoviePosters",
                column: "MovieId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserID",
                table: "Payments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieID",
                table: "Reviews",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserID",
                table: "Reviews",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CinemaID",
                table: "Rooms",
                column: "CinemaID");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_RoomID",
                table: "Seats",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_MovieID",
                table: "Showtimes",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_RoomID",
                table: "Showtimes",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PaymentID",
                table: "Tickets",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SeatID",
                table: "Tickets",
                column: "SeatID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ShowtimeID",
                table: "Tickets",
                column: "ShowtimeID");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserID",
                table: "Tickets",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D105349C68A34A",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviePosters");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Showtimes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Cinemas");
        }
    }
}
