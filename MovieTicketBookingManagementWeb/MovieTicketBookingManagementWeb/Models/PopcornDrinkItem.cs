﻿namespace MovieTicketBookingManagementWeb.Models
{
    public class PopcornDrinkItem
    {
        public int ID { get; set; }
        public string? PictureUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
