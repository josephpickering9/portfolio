using Microsoft.EntityFrameworkCore;
using Work_Experience_Search.Models;
using Work_Experience_Search.Types;
using Work_Experience_Search.Extensions;

namespace Work_Experience_Search.Services;

public sealed class Database(DbContextOptions<Database> options) : DbContext(options)
{
    public DbSet<Project> Project { get; set; }
    public DbSet<ProjectImage> ProjectImage { get; set; }
    public DbSet<ProjectRepository> ProjectRepository { get; set; }
    public DbSet<ProjectStat> ProjectStat { get; set; }
    public DbSet<Tag> Tag { get; set; }
    public DbSet<Company> Company { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Tag>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToDescriptionString(),
                v => EnumExtensions.FromDescriptionString<TagType>(v));

        modelBuilder.Entity<Project>()
            .Property(e => e.Id)
            .HasGuidIdConversion()
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Project>()
            .Property(e => e.CompanyId)
            .HasNullableGuidIdConversion();

        modelBuilder.Entity<ProjectImage>()
            .Property(e => e.Id)
            .HasGuidIdConversion()
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ProjectImage>()
            .Property(e => e.ProjectId)
            .HasGuidIdConversion();

        modelBuilder.Entity<ProjectRepository>()
            .Property(e => e.Id)
            .HasGuidIdConversion()
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ProjectRepository>()
            .Property(e => e.ProjectId)
            .HasGuidIdConversion();

        modelBuilder.Entity<ProjectStat>()
            .Property(e => e.Id)
            .HasGuidIdConversion()
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ProjectStat>()
            .Property(e => e.ProjectId)
            .HasGuidIdConversion();

        modelBuilder.Entity<Tag>()
            .Property(e => e.Id)
            .HasGuidIdConversion()
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Company>()
            .Property(e => e.Id)
            .HasGuidIdConversion()
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<ProjectImage>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToDescriptionString(),
                v => EnumExtensions.FromDescriptionString<ImageType>(v));

        modelBuilder.Entity<Project>()
            .HasIndex(b => b.Slug)
            .IsUnique();

        modelBuilder.Entity<Tag>()
            .HasIndex(b => b.Slug)
            .IsUnique();

        modelBuilder.Entity<Company>()
            .HasIndex(b => b.Slug)
            .IsUnique();
    }
}
