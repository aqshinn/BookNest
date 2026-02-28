using BookNest.Core.Entities;
using BookNest.Core.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Data.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<ReadingList> ReadingLists { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bg => bg.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(bg => bg.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReadingList>()
                .HasOne(rl => rl.Book)
                .WithMany()
                .HasForeignKey(rl => rl.BookId)
                .OnDelete(DeleteBehavior.Cascade); // If the book is deleted, it should also be removed from the shelf.

            modelBuilder.Entity<ReadingList>()
                .HasOne(rl => rl.AppUser)
                .WithMany()
                .HasForeignKey(rl => rl.AppUserId)
                .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, their shelf will also be deleted.

            
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews) // We link to the Reviews collection inside the book
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Review>()
                .HasOne(r => r.AppUser)
                .WithMany()
                .HasForeignKey(r => r.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.AppUserId, r.BookId })
                .IsUnique(); // Ensure a user can only review a book once
        }

    }
}
