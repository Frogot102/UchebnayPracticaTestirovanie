using Microsoft.EntityFrameworkCore;

namespace UchebnayPracticaTestirovanie.Data;

public class AppDbContext : DbContext
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Infotech;Username=postgres;Password=frogot1;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.HasIndex(e => e.Login).IsUnique();
            entity.HasOne(e => e.Role).WithMany(e => e.Users).HasForeignKey(e => e.RoleId);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("requests");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TechType).HasColumnName("tech_type");
            entity.Property(e => e.TechModel).HasColumnName("tech_model");
            entity.Property(e => e.Problem).HasColumnName("problem");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CompletionDate).HasColumnName("completion_date");
            entity.Property(e => e.RepairParts).HasColumnName("repair_parts");
            entity.Property(e => e.MasterId).HasColumnName("master_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.HasOne(e => e.Master).WithMany().HasForeignKey(e => e.MasterId);
            entity.HasOne(e => e.Client).WithMany().HasForeignKey(e => e.ClientId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.MasterId).HasColumnName("master_id");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.HasOne(e => e.Master).WithMany().HasForeignKey(e => e.MasterId);
            entity.HasOne(e => e.Request).WithMany(e => e.Comments).HasForeignKey(e => e.RequestId);
        });

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(type => type.GetForeignKeys()))
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
    }
}
