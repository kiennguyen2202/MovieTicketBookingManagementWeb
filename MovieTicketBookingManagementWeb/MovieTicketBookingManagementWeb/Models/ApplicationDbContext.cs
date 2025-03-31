using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MovieTicketBookingManagementWeb.Models;

namespace MovieTicketBookingManagementWeb.Models;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

   

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-N5O7TJD1\\SQLSERVER;Database=WEBBANVEXEMPHIM;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Cinemas__59C926260B1BBE46");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Movies__4BD2943AA05B8B91");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.Language)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PosterUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PosterURL");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Orders__3214EC279F18DDF8");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
           
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__OrderDet__3214EC277A369FC6");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.OrderID).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.TicketID).HasColumnName("TicketID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__1AD3FDA4");

            entity.HasOne(d => d.Ticket).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.TicketID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Ticke__1BC821DD");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Payments__9B556A588F0B63FC");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderID).HasColumnName("OrderID");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.PaymentTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            

        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Reviews__74BC79AEE3371759");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.MovieID).HasColumnName("MovieID");
            entity.Property(e => e.ReviewTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Rooms__3286391982D9C9F8");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.CinemaID).HasColumnName("CinemaID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Cinema).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.CinemaID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rooms__CinemaID__3F466844");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Seats__311713D3FAA4D100");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.RoomID).HasColumnName("RoomID");
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.SeatType).HasMaxLength(20);

            entity.HasOne(d => d.Room).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RoomID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__RoomID__45F365D3");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Showtime__32D31FC00CFB611F");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.MovieID).HasColumnName("MovieID");
            entity.Property(e => e.RoomID).HasColumnName("RoomID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtimes__Movie__4222D4EF");

            entity.HasOne(d => d.Room).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.RoomID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtimes__RoomI__4316F928");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Tickets__712CC627F99AA1C7");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.BookingTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DrinkPrice)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DrinkQuantity).HasDefaultValue(0);
            entity.Property(e => e.FinalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PopcornPrice)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PopcornQuantity).HasDefaultValue(0);
            entity.Property(e => e.SeatID).HasColumnName("SeatID");
            entity.Property(e => e.ShowtimeID).HasColumnName("ShowtimeID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TicketType).HasMaxLength(50);

            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__SeatID__52593CB8");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__Showtim__5165187F");
        });

        OnModelCreatingPartial(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
