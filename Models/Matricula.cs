using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pc2_Pogramacion.Models
{
    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Matricula
    {
        public int Id { get; set; }

        public int CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public EstadoMatricula Estado { get; set; }
    }
}