using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace voting_system.Models;

public partial class VotingSystemContext : DbContext
{
    public VotingSystemContext()
    {
    }

    public VotingSystemContext(DbContextOptions<VotingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Exhibtior> Exhibtiors { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    public virtual DbSet<VotingSetting> VotingSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=Voting_System;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin");

            entity.Property(e => e.PasswordHashed).HasMaxLength(60);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Exhibtior>(entity =>
        {
            entity.HasKey(e => e.ExhibitorsId);

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(50)
                .HasColumnName("PhotoURL");

            entity.HasMany(d => d.Categories).WithMany(p => p.Exhibitors)
                .UsingEntity<Dictionary<string, object>>(
                    "ExhibitorCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ExhibitorCategories_Categories"),
                    l => l.HasOne<Exhibtior>().WithMany()
                        .HasForeignKey("ExhibitorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ExhibitorCategories_Exhibtiors"),
                    j =>
                    {
                        j.HasKey("ExhibitorId", "CategoryId");
                        j.ToTable("ExhibitorCategories");
                    });
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(e => e.VisitorId).HasName("PK_Visitrs");

            entity.HasIndex(e => e.PhoneNumber, "IX_Visitors").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("PK_Votes_1");

            entity.HasIndex(e => new { e.VisitorId, e.CategoryId }, "IX_Votes").IsUnique();

            entity.HasOne(d => d.Category).WithMany(p => p.Votes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Votes_Categories");

            entity.HasOne(d => d.Exhibitor).WithMany(p => p.Votes)
                .HasForeignKey(d => d.ExhibitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Votes_Exhibtiors");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Votes)
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Votes_Visitors");
        });

        modelBuilder.Entity<VotingSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId);

            entity.ToTable("VotingSetting");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
