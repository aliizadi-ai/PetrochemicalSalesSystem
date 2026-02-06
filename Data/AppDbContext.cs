using PetrochemicalSalesSystem.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Runtime.Remoting.Contexts;

namespace PetrochemicalSalesSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=PetrochemicalSalesDB")
        {
            // غیرفعال کردن Lazy Loading برای عملکرد بهتر
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Accountant> Accountants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تنظیمات اضافی برای جدول
            modelBuilder.Entity<Accountant>()
                .Property(a => a.Username)
                .IsRequired()
                .HasMaxLength(100);

            // ایجاد ایندکس یکتا بر روی فیلد Username (روش EF6)
            modelBuilder.Entity<Accountant>()
                .Property(a => a.Username)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("IX_Accountants_Username")
                    {
                        IsUnique = true,
                        Order = 1
                    }));
        }
    }
}