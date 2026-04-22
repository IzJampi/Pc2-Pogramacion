using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pc2_Pogramacion.Data;
using Pc2_Pogramacion.Models;

namespace Pc2_Pogramacion.Controllers
{
    public class CursoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 CATÁLOGO + FILTROS
        public async Task<IActionResult> Index(string nombre, int? minCreditos, int? maxCreditos)
        {
            var cursos = _context.Cursos.Where(c => c.Activo);

            if (!string.IsNullOrEmpty(nombre))
                cursos = cursos.Where(c => c.Nombre.Contains(nombre));

            if (minCreditos.HasValue)
                cursos = cursos.Where(c => c.Creditos >= minCreditos);

            if (maxCreditos.HasValue)
                cursos = cursos.Where(c => c.Creditos <= maxCreditos);

            return View(await cursos.ToListAsync());
        }

        // 🔹 DETALLE
        public async Task<IActionResult> Details(int id)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            return View(curso);
        }

        // 🔹 CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 🔹 CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            // 🔥 VALIDACIÓN SERVER-SIDE
            if (curso.HorarioFin <= curso.HorarioInicio)
            {
                ModelState.AddModelError("", "El horario es inválido");
            }

            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(curso);
        }

        // 🔹 EDIT (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            return View(curso);
        }

        // 🔹 EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.Id) return NotFound();

            // 🔥 VALIDACIÓN SERVER-SIDE
            if (curso.HorarioFin <= curso.HorarioInicio)
            {
                ModelState.AddModelError("", "El horario es inválido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cursos.Any(e => e.Id == curso.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(curso);
        }
    }
}