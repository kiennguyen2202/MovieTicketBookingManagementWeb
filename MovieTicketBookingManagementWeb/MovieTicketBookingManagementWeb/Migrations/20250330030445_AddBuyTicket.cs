using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBookingManagementWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddBuyTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovieID",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "Seats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MovieID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeatID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_MovieID",
                table: "Tickets",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MovieID",
                table: "OrderDetails",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_RoomID",
                table: "OrderDetails",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_SeatID",
                table: "OrderDetails",
                column: "SeatID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ShowtimeID",
                table: "OrderDetails",
                column: "ShowtimeID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Movies_MovieID",
                table: "OrderDetails",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Rooms_RoomID",
                table: "OrderDetails",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Seats_SeatID",
                table: "OrderDetails",
                column: "SeatID",
                principalTable: "Seats",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Showtimes_ShowtimeID",
                table: "OrderDetails",
                column: "ShowtimeID",
                principalTable: "Showtimes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Movies_MovieID",
                table: "Tickets",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Movies_MovieID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Rooms_RoomID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Seats_SeatID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Showtimes_ShowtimeID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Movies_MovieID",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_MovieID",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_MovieID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_RoomID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_SeatID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ShowtimeID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MovieID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MovieID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "RoomID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SeatID",
                table: "OrderDetails");
        }
    }
}
