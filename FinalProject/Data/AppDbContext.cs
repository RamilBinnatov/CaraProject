using FinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogImage> BlogImages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Social> Socials { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Poster> Posters { get; set; }
        public DbSet<Product_Size> Product_Size { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PageHeader> PageHeaders { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketProduct> BasketProducts { get; set; }
    }
}
