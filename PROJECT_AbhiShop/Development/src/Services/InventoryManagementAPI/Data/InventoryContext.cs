using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagementAPI.Data
{
    public class InventoryContext:DbContext
    {
        public InventoryContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryCategory>(ConfigureInventoryCategory);
            modelBuilder.Entity<InventoryItem>(ConfigureInventoryItem);
        }

        private void ConfigureInventoryItem(EntityTypeBuilder<InventoryItem> entityBuilder)
        {
            entityBuilder.ToTable("Product");
            entityBuilder.Property(c => c.id)                         /* FIX: 'id' naming a primary key in this way allows EF to recognize while migration  */
                .UseHiLo("product_hilo")
                .IsRequired(true);
            entityBuilder.Property(c => c.productName)
                .IsRequired(true)
                .HasMaxLength(50);
            entityBuilder.Property(c => c.price)
                .HasColumnType("decimal(18,2)")
                .IsRequired(true);
            entityBuilder.Property(c => c.imageURL)
                .IsRequired(false);
            entityBuilder.HasOne(c => c.InventoryCategory)
                .WithMany()
                .HasForeignKey(c => c.inventoryCategoryId);

        }

        private void ConfigureInventoryCategory(EntityTypeBuilder<InventoryCategory> entityBuilder)
        {
            entityBuilder.ToTable("InventoryCategory");
            entityBuilder.Property(c => c.Id)                                /* FIX: 'Id' naming a primary key in this way allows EF to recognize while migration  */
                .UseHiLo("category_hilo")
                .IsRequired(true);
            entityBuilder.Property(c => c.invCategory)
                .IsRequired()
                .HasMaxLength(100);
        }

        public DbSet<InventoryCategory> InventoryCategories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
    }
}
