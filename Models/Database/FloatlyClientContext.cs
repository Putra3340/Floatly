using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Floatly.Models.Database;

public partial class FloatlyClientContext : DbContext
{
    public FloatlyClientContext()
    {
    }

    public FloatlyClientContext(DbContextOptions<FloatlyClientContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DownloadedSong> DownloadedSong { get; set; }

    public virtual DbSet<Queue> Queue { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    #if DEBUG
            optionsBuilder.UseSqlServer(
                "Data Source=DESKTOP-86R216N;Initial Catalog=FloatlyClient;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
    #else
            optionsBuilder.UseSqlite("Data Source=database.db");
    #endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DownloadedSong>(entity =>
        {
            entity.Property(e => e.ArtistBio).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.SongLength)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Queue>(entity =>
        {
            entity.Property(e => e.ArtistBio).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.SongLength)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
