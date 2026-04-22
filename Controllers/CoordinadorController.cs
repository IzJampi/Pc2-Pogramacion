using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pc2_Pogramacion.Data;
using Pc2_Pogramacion.Models;

namespace Pc2_Pogramacion.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 PANEL PRINCIPAL
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos.ToListAsync();
            return View(cursos);
        }

        // 🔹 LISTA MATRÍCULAS POR CURSO
        public async Task<IActionResult> Matriculas(int cursoId)
        {
            var matriculas = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.CursoId == cursoId)
                .ToListAsync();

            ViewBag.CursoId = cursoId;

            return View(matriculas);
        }

        // 🔹 CONFIRMAR
        public async Task<IActionResult> Confirmar(int id)
        {
            var m = await _context.Matriculas.FindAsync(id);
            if (m == null) return NotFound();

            m.Estado = EstadoMatricula.Confirmada;
            await _context.SaveChangesAsync();

            return RedirectToAction("Matriculas", new { cursoId = m.CursoId });
        }

        // 🔹 CANCELAR
        public async Task<IActionResult> Cancelar(int id)
        {
            var m = await _context.Matriculas.FindAsync(id);
            if (m == null) return NotFound();

            m.Estado = EstadoMatricula.Cancelada;
            await _context.SaveChangesAsync();

            return RedirectToAction("Matriculas", new { cursoId = m.CursoId });
        }
    }
}