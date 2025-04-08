using System.Collections.Generic;
using System.Linq;

namespace MovieTicketBookingManagementWeb.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i =>
                i.ShowtimeID == item.ShowtimeID &&
                i.SeatID == item.SeatID);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(int showtimeId, int seatId)
        {
            // Xóa toàn bộ item trong giỏ hàng dựa trên showtimeId và seatId
            Items?.RemoveAll(i =>
                i.ShowtimeID == showtimeId &&
                i.SeatID == seatId);
        }




        public void UpdateQuantity(int showtimeId, int seatId, int popcornDrinkItemId, int quantity)
        {
            var item = Items.FirstOrDefault(i =>
                i.ShowtimeID == showtimeId &&
                i.SeatID == seatId);

            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(i => (i.ShowtimePrice + i.PopcornDrinkCardItems.Sum(p => p.Quantity * p.Price) ) * i.Quantity);
        }
    }
}