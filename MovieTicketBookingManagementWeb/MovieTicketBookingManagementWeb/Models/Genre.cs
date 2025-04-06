namespace MovieTicketBookingManagementWeb.Models
{
    public class Genre
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;

       public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
