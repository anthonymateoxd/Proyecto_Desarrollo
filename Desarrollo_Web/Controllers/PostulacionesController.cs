using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemonSlayer.Data;
using DemonSlayer.Models;
using System.Security.Claims;

namespace DemonSlayer.Controllers
{
    public class PostulacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostulacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // MÉTODO SIMPLE para obtener el ID del usuario
        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return 0; // Si no encuentra, devuelve 0 (simple)
        }

        // GET: Postulaciones
        public async Task<IActionResult> Index()
        {
            /*
             return View(await _context.Postulaciones.ToListAsync());
            */
            // Obtener el ID del usuario logueado (como int)
            var userId = ObtenerUsuarioId();

            // Si es Consultor, solo ver SUS postulaciones
            if (User.IsInRole("Consultor"))
            {
                var postulacionesConsultor = await _context.Postulaciones
                    .Where(p => p.ConsultorId == userId)
                    .ToListAsync();
                return View(postulacionesConsultor);
            }

            // Si es Admin o Cliente, ver todas
            return View(await _context.Postulaciones.ToListAsync());
        }

        // GET: Postulaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postulacion = await _context.Postulaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postulacion == null)
            {
                return NotFound();
            }

            return View(postulacion);
        }
        /*
        // GET: Postulaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Postulaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MontoQ,Propuesta,Fecha,ProyectoId,ConsultorId,Aceptada")] Postulacion postulacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postulacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postulacion);
        }
        */
        // GET: Postulaciones/Create
        public IActionResult Create(int proyectoId)
        {
            // Crear una nueva postulación con el proyectoId
            var postulacion = new Postulacion
            {
                ProyectoId = proyectoId
            };

            return View(postulacion);
        }

        // POST: Postulaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Postulacion postulacion)
        {
            if (ModelState.IsValid)
            {
                // Asignar automáticamente los valores
                postulacion.ConsultorId = ObtenerUsuarioId(); // Se llena automáticamente
                postulacion.Fecha = DateTime.Now;
                postulacion.Aceptada = false; // Siempre false al crear

                _context.Add(postulacion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "¡Postulación enviada correctamente!";
                return RedirectToAction(nameof(Index));
            }

            return View(postulacion);
        }
        // GET: Postulaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null)
            {
                return NotFound();
            }

            // Verificar permisos - solo el creador o admin puede editar
            var userId = ObtenerUsuarioId();
            bool esAdmin = User.IsInRole("Admin");

            if (postulacion.ConsultorId != userId && !esAdmin)
            {
                return Forbid();
            }

            return View(postulacion);
        }

        // POST: Postulaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MontoQ,Propuesta,Fecha,ProyectoId,ConsultorId,Aceptada")] Postulacion postulacion)
        {
            if (id != postulacion.Id)
            {
                return NotFound();
            }

            // Verificar permisos primero
            var postulacionExistente = await _context.Postulaciones.FindAsync(id);
            if (postulacionExistente == null)
            {
                return NotFound();
            }

            var userId = ObtenerUsuarioId();
            bool esAdmin = User.IsInRole("Admin");

            if (postulacionExistente.ConsultorId != userId && !esAdmin)
            {
                return Forbid();
            }

            // Validaciones adicionales
            if (postulacion.MontoQ < 1 || postulacion.MontoQ > 9999999.99m)
            {
                ModelState.AddModelError("MontoQ", "El monto debe estar entre Q1.00 y Q9,999,999.99");
            }

            if (string.IsNullOrWhiteSpace(postulacion.Propuesta) || postulacion.Propuesta.Length < 50)
            {
                ModelState.AddModelError("Propuesta", "La propuesta debe tener al menos 50 caracteres");
            }

            if (postulacion.Propuesta?.Length > 2000)
            {
                ModelState.AddModelError("Propuesta", "La propuesta no puede tener más de 2000 caracteres");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar solo los campos permitidos en la instancia existente
                    postulacionExistente.MontoQ = postulacion.MontoQ;
                    postulacionExistente.Propuesta = postulacion.Propuesta;

                    // Solo admin puede cambiar el estado de aceptación
                    if (esAdmin)
                    {
                        postulacionExistente.Aceptada = postulacion.Aceptada;
                    }

                    // Mantener los campos originales que no deben cambiar
                    postulacionExistente.Fecha = postulacionExistente.Fecha; // Mantener fecha original
                    postulacionExistente.ProyectoId = postulacionExistente.ProyectoId; // Mantener proyecto original
                    postulacionExistente.ConsultorId = postulacionExistente.ConsultorId; // Mantener consultor original

                    _context.Update(postulacionExistente);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Postulación actualizada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostulacionExists(postulacion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(postulacion);
        }

        // GET: Postulaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postulacion = await _context.Postulaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postulacion == null)
            {
                return NotFound();
            }

            // Verificar permisos - solo el creador o admin puede eliminar
            var userId = ObtenerUsuarioId();
            bool esAdmin = User.IsInRole("Admin");

            if (postulacion.ConsultorId != userId && !esAdmin)
            {
                return Forbid();
            }

            return View(postulacion);
        }

        // POST: Postulaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null)
            {
                return NotFound();
            }

            // Verificar permisos - solo el creador o admin puede eliminar
            var userId = ObtenerUsuarioId();
            bool esAdmin = User.IsInRole("Admin");

            if (postulacion.ConsultorId != userId && !esAdmin)
            {
                return Forbid();
            }

            _context.Postulaciones.Remove(postulacion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Postulación eliminada correctamente";
            return RedirectToAction(nameof(Index));
        }

        // En PostulacionesController.cs
        public async Task<IActionResult> PropuestasPorProyecto(int proyectoId)
        {
            // Verificar que el usuario sea el dueño del proyecto o Admin
            var proyecto = await _context.Proyectos.FindAsync(proyectoId);

            if (proyecto == null)
            {
                return NotFound();
            }

            var userId = ObtenerUsuarioId();

            // Solo el cliente dueño o Admin puede ver las propuestas
            if (User.IsInRole("Cliente") && proyecto.ClienteId != userId)
            {
                return Forbid();
            }

            // Obtener todas las postulaciones del proyecto
            var postulaciones = await _context.Postulaciones
                .Where(p => p.ProyectoId == proyectoId)
                .ToListAsync();

            ViewData["ProyectoTitulo"] = proyecto.Titulo;
            ViewData["ProyectoId"] = proyectoId;

            return View(postulaciones);
        }
        // En PostulacionesController.cs
        [HttpPost]
        // POST: Postulaciones/AceptarPropuesta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceptarPropuesta(int id)
        {
            var postulacion = await _context.Postulaciones.FindAsync(id);
            if (postulacion == null)
            {
                return NotFound();
            }

            // Verificar que el usuario actual es el cliente dueño del proyecto
            var userId = ObtenerUsuarioId();
            var proyecto = await _context.Proyectos.FindAsync(postulacion.ProyectoId);

            if (proyecto?.ClienteId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Aceptar la propuesta
            postulacion.Aceptada = true;
            _context.Update(postulacion);
            await _context.SaveChangesAsync();

            // Opcional: Rechazar automáticamente otras postulaciones al mismo proyecto
            var otrasPostulaciones = await _context.Postulaciones
                .Where(p => p.ProyectoId == postulacion.ProyectoId && p.Id != id)
                .ToListAsync();

            foreach (var otra in otrasPostulaciones)
            {
                otra.Aceptada = false;
                _context.Update(otra);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Proyectos");
        }
        private bool PostulacionExists(int id)
        {
            return _context.Postulaciones.Any(e => e.Id == id);
        }
    }
}
