using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pc2_Pogramacion.Data;
using Pc2_Pogramacion.Models;

namespace Pc2_Pogramacion.Controllers
{
    [Authorize]
    public class MatriculaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 🔥 LISTAR MIS MATRÍCULAS
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var matriculas = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == user.Id)
                .ToListAsync();

            return View(matriculas);
        }

        // 🔥 INSCRIBIRSE
        [HttpPost]
        public async Task<IActionResult> Inscribirse(int cursoId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Mensaje"] = "Debes iniciar sesión";
                return RedirectToAction("Details", "Curso", new { id = cursoId });
            }

            var curso = await _context.Cursos
                .Include(c => c.Matriculas)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null)
            {
                TempData["Mensaje"] = "Curso no encontrado";
                return RedirectToAction("Index", "Curso");
            }

            // 🔥 VALIDACIÓN 1: duplicado
            bool yaExiste = await _context.Matriculas
                .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == user.Id);

            if (yaExiste)
            {
                TempData["Mensaje"] = "Ya estás inscrito en este curso";
                return RedirectToAction("Details", "Curso", new { id = cursoId });
            }

            // 🔥 VALIDACIÓN 2: cupo máximo
            int inscritos = await _context.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);

            if (inscritos >= curso.CupoMaximo)
            {
                TempData["Mensaje"] = "El curso ya alcanzó su cupo máximo";
                return RedirectToAction("Details", "Curso", new { id = cursoId });
            }

            // 🔥 VALIDACIÓN 3: cruce de horarios
            var misCursos = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada)
                .ToListAsync();

            bool hayCruce = misCursos.Any(m =>
                curso.HorarioInicio < m.Curso.HorarioFin &&
                curso.HorarioFin > m.Curso.HorarioInicio
            );

            if (hayCruce)
            {
                TempData["Mensaje"] = "Tienes conflicto de horario con otro curso";
                return RedirectToAction("Details", "Curso", new { id = cursoId });
            }

            // 🔥 CREAR MATRÍCULA
            var matricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = user.Id,
                Estado = EstadoMatricula.Pendiente,
                FechaRegistro = DateTime.Now
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Inscripción realizada correctamente";

            return RedirectToAction("Details", "Curso", new { id = cursoId });
        }
    }
}