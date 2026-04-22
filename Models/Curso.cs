using System.ComponentModel.DataAnnotations;

namespace Pc2_Pogramacion.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required]
        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores a 0")]
        public int Creditos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser mayor a 0")]
        public int CupoMaximo { get; set; }

        [Required]
        public DateTime HorarioInicio { get; set; }

        [Required]
        public DateTime HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        // Relación
        public ICollection<Matricula> Matriculas { get; set; }
    }
}