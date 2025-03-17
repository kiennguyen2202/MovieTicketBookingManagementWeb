using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MovieTicketBookingManagementWeb.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-N5O7TJD1\\SQLSERVER;Database=WEBBANVEXEMPHIM;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cinemas__59C926260B1BBE46");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Movies__4BD2943AA05B8B91");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.Language)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__9B556A588F0B63FC");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.PaymentTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__UserID__5812160E");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__74BC79AEE3371759");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.ReviewTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Movie).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__MovieID__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserID__5CD6CB2B");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3286391982D9C9F8");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CinemaId).HasColumnName("CinemaID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Cinema).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.CinemaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rooms__CinemaID__3F466844");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seats__311713D3FAA4D100");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.SeatType).HasMaxLength(20);

            entity.HasOne(d => d.Room).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__RoomID__45F365D3");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Showtime__32D31FC00CFB611F");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtimes__Movie__4222D4EF");

            entity.HasOne(d => d.Room).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtimes__RoomI__4316F928");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tickets__712CC627F99AA1C7");

            entity.Property(e => e.Id).HasColumnName("ID");
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
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.PopcornPrice)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PopcornQuantity).HasDefaultValue(0);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SeatId).HasColumnName("SeatID");
            entity.Property(e => e.ShowtimeId).HasColumnName("ShowtimeID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TicketType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Payment).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Tickets_Payments");

            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__SeatID__52593CB8");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__Showtim__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__UserID__5070F446");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__1788CCACDA95FF49");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105349C68A34A").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
