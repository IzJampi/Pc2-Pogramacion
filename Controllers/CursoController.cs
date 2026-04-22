using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Pc2_Pogramacion.Data;
using Pc2_Pogramacion.Models;
using System.Text.Json;

namespace Pc2_Pogramacion.Controllers
{
    public class CursoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CursoController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // 🔹 CATÁLOGO + CACHE REDIS (60s)
        public async Task<IActionResult> Index(string nombre, int? minCreditos, int? maxCreditos)
{
    string cacheKey = "cursos_activos";
    List<Curso> cursos;

    string cacheData = null;

    try
    {
        cacheData = await _cache.GetStringAsync(cacheKey);
    }
    catch
    {
        // 🔥 Redis no disponible (local) → continuar normal
    }

    if (!string.IsNullOrEmpty(cacheData))
    {
        cursos = JsonSerializer.Deserialize<List<Curso>>(cacheData);
    }
    else
    {
        cursos = await _context.Cursos
            .Where(c => c.Activo)
            .ToListAsync();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        };

        try
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(cursos),
                options
            );
        }
        catch
        {
            // 🔥 ignora si Redis falla
        }
    }

    // 🔹 FILTROS
    if (!string.IsNullOrEmpty(nombre))
        cursos = cursos.Where(c => c.Nombre.Contains(nombre)).ToList();

    if (minCreditos.HasValue)
        cursos = cursos.Where(c => c.Creditos >= minCreditos).ToList();

    if (maxCreditos.HasValue)
        cursos = cursos.Where(c => c.Creditos <= maxCreditos).ToList();

    return View(cursos);
}

        // 🔹 DETALLE + SESIÓN
        public async Task<IActionResult> Details(int id)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null) return NotFound();

            // 🔥 GUARDAR EN SESIÓN
            HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);
            HttpContext.Session.SetInt32("UltimoCursoId", curso.Id);

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
            if (curso.HorarioFin <= curso.HorarioInicio)
            {
                ModelState.AddModelError("", "El horario es inválido");
            }

            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();

                // 🔥 INVALIDAR CACHE
                try
            {
                 await _cache.RemoveAsync("cursos_activos");
            }
            catch
            {
                // ignora si Redis no está disponible
            }

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

                    // 🔥 INVALIDAR CACHE
                   try
                    {
                         await _cache.RemoveAsync("cursos_activos");
                    }
                catch
                {
                   // ignora si Redis no está disponible
                }
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