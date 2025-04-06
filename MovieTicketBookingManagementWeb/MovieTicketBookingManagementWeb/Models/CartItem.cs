using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketBookingManagementWeb.Models
{
    public class PopcornDrinkCardItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; } // Thêm thuộc tính này
        public decimal Price { get; set; }
    }
    public class CartItem
    {
        public int ShowtimeID { get; set; }
        public int RoomID { get; set; }
        public int CinemaID { get; set; }
        public string RoomName { get; set; }
        public string CinemaName { get; set; }
        public string MovieTitle { get; set; }
        public DateTime StartTime { get; set; }
        public decimal ShowtimePrice { get; set; }
        public int SeatID { get; set; }
        public string SeatNumber { get; set; }
        public List<PopcornDrinkCardItem> PopcornDrinkCardItems { get; set; } = new List<PopcornDrinkCardItem>();
        public decimal TotalPrice => (ShowtimePrice + PopcornDrinkCardItems.Sum(p => p.Quantity * p.Price));

        public int Quantity { get; set; }
    }
}