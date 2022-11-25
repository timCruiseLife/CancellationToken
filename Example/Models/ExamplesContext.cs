using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Example.Models
{
    public partial class ExamplesContext : DbContext
    {
        public ExamplesContext()
        {
        }

        public ExamplesContext(DbContextOptions<ExamplesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MessageBoard> MessageBoards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;database=example;user=root;password=User_123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.11-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<MessageBoard>(entity =>
            {
                entity.ToTable("message_board");

                entity.HasIndex(e => new { e.CreateTime, e.Name }, "create_time");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasMaxLength(200)
                    .HasColumnName("content");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("create_time");

                entity.Property(e => e.Name)
                    .HasMaxLength(40)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("update_time");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
