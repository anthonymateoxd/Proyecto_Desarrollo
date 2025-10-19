using DemonSlayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemonSlayer.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Postulacion> Postulaciones { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .IsRequired();
        });

        // Proyecto
        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Titulo)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(1000);

            entity.Property(e => e.PresupuestoQ)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Activo");
        });

        // Postulacion
        modelBuilder.Entity<Postulacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MontoQ)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Propuesta)
                .HasMaxLength(1000);

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("GETDATE()");
        });

        // Mensaje
        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Texto)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("GETDATE()");
        });
    }

}
