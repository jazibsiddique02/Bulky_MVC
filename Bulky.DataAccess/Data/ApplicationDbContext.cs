using Bulky.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<ShoppingCart> ShoppingCart {  get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IdentityUserLogin<string> requires a primary key to be defined error handling
            base.OnModelCreating(modelBuilder);




            // Seed Data For Category Table
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Action",
                    DisplayOrder = 1
                },
                new Category
                {
                    Id = 2,
                    Name="Science Fiction",
                    DisplayOrder = 2
                },
                new Category
                {
                    Id = 3,
                    Name = "History",
                    DisplayOrder = 3
                }
                );

            //// Seed Data For Product Table
            modelBuilder.Entity<Product>().HasData(

                new Product
                {
                    Id = 1,
                    Title = "Dune",
                    Description = "Science Fiction Novel by Frank Herbert",
                    ISBN = "978-0441013593",
                    Author = "Frank Herbert",
                    ListPrice = 25.99,
                    Price = 20.99,
                    Price50 = 19.99,
                    Price100 = 18.99,
                    CategoryId = 2,
                    ImageUrl=""
                },
                new Product
                {
                    Id = 2,
                    Title = "Sapiens: A Brief History of Humankind",
                    Description = "A book by Yuval Noah Harari that explores the history of humankind.",
                    ISBN = "978-0062316097",
                    Author = "Yuval Noah Harari",
                    ListPrice = 22.99,
                    Price = 18.99,
                    Price50 = 17.99,
                    Price100 = 16.99,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "1984",
                    Description = "A dystopian novel by George Orwell.",
                    ISBN = "978-0451524935",
                    Author = "George Orwell",
                    ListPrice = 15.99,
                    Price = 12.99,
                    Price50 = 11.99,
                    Price100 = 10.99,
                    CategoryId = 3,
                    ImageUrl = ""
                }

                );


            // Seed Data for Companies

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id= 1,
                    Name = "David",
                    StreetAddress = "123 Street,Dallas, Texas",
                    City = "Dallas",
                    State = "Texas",
                    PostalCode="2000",
                    PhoneNumber="111-222-333"

                },

                new Company
                {
                    Id= 2,
                    Name = "Peter",
                    StreetAddress = "123 Street,Dallas, Texas",
                    City = "Dallas",
                    State = "Texas",
                    PostalCode = "2000",
                    PhoneNumber = "111-222-333"

                }

                );


        }

    }
}
