namespace MovieTicketBookingManagementWeb.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.MovieID == item.MovieID);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(int movieId)
        {
            Items.RemoveAll(i => i.MovieID == movieId);
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(i => i.Price * i.Quantity);
        }

    }
}
