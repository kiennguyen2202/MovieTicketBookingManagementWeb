using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingManagementWeb.Models
{
    public class CartItem
    {
        [Key]
        public int ID { get; set; }

        public int ShowtimeID { get; set; }
        public int MovieID { get; set; }

        [ForeignKey("ShowtimeID")]
        public virtual Showtime Showtime { get; set; }

        [Required]
        public int Quantity { get; set; } // Số lượng vé đặt

        [Required]
        public decimal Price { get; set; } // Giá của mỗi vé

        [NotMapped]
        public decimal TotalPrice => Quantity * Price; // Tổng tiền
    }
}
