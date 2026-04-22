using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pc2_Pogramacion.Models;

namespace Pc2_Pogramacion.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 🔹 Código único
        builder.Entity<Curso>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        // 🔹 Horario válido
        builder.Entity<Curso>()
            .HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");

        // 🔹 Un usuario no puede matricularse dos veces en el mismo curso
        builder.Entity<Matricula>()
            .HasIndex(m => new { m.CursoId, m.UsuarioId })
            .IsUnique();

        // 🔹 Seed de cursos
        builder.Entity<Curso>().HasData(
            new Curso
            {
                Id = 1,
                Codigo = "CS101",
                Nombre = "Programación",
                Creditos = 4,
                CupoMaximo = 30,
                HorarioInicio = new DateTime(2025, 1, 1, 8, 0, 0),
                HorarioFin = new DateTime(2025, 1, 1, 10, 0, 0),
                Activo = true
            },
            new Curso
            {
                Id = 2,
                Codigo = "CS102",
                Nombre = "Base de Datos",
                Creditos = 3,
                CupoMaximo = 25,
                HorarioInicio = new DateTime(2025, 1, 1, 10, 0, 0),
                HorarioFin = new DateTime(2025, 1, 1, 12, 0, 0),
                Activo = true
            },
            new Curso
            {
                Id = 3,
                Codigo = "CS103",
                Nombre = "Redes",
                Creditos = 3,
                CupoMaximo = 20,
                HorarioInicio = new DateTime(2025, 1, 1, 12, 0, 0),
                HorarioFin = new DateTime(2025, 1, 1, 14, 0, 0),
                Activo = true
            }
        );
    }
}