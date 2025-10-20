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
using Microsoft.CodeAnalysis;
using DemonSlayer.ViewModels;

namespace DemonSlayer.Controllers
{
    public class MensajesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MensajesController(ApplicationDbContext context)
        {
            _context = context;
        }
        /*
        // GET: Mensajes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mensajes.ToListAsync());
        }
        */
        // MÉTODO SIMPLE para obtener el ID del usuario (igual que en Postulaciones)
        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return 0; // Si no encuentra, devuelve 0 (simple)
        }
        // En MensajesController - AGREGAR ESTE MÉTODO
        public async Task<IActionResult> PorProyecto(int proyectoId)
        {
            var userId = ObtenerUsuarioId();

            // Verificar que el usuario tiene acceso a este proyecto
            var tieneAcceso = await _context.Proyectos
                .AnyAsync(p => p.Id == proyectoId &&
                              (p.ClienteId == userId ||
                               _context.Postulaciones.Any(post => post.ProyectoId == proyectoId &&
                                                                post.ConsultorId == userId &&
                                                                post.Estado == "Aceptada")));

            if (!tieneAcceso)
            {
                return Forbid();
            }

            // Obtener los mensajes del proyecto
            var mensajes = await _context.Mensajes
                .Where(m => m.ProyectoId == proyectoId)
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            // Cargar información adicional
            var proyecto = await _context.Proyectos.FindAsync(proyectoId);
            var usuariosIds = mensajes.Select(m => m.RemitenteId).Distinct().ToList();
            var usuarios = await _context.Usuarios
                .Where(u => usuariosIds.Contains(u.IdUsuario))
                .ToDictionaryAsync(u => u.IdUsuario, u => u.NombreCompleto);

            ViewBag.Proyecto = proyecto;
            ViewBag.Usuarios = usuarios;
            ViewBag.UserId = userId;

            return View(mensajes);
        }
        // Chat entre dos usuarios específicos
        public async Task<IActionResult> Chat(int destinatarioId)
        {
            var userId = ObtenerUsuarioId();

            // Obtener mensajes entre estos dos usuarios
            var mensajes = await _context.Mensajes
                .Where(m => (m.RemitenteId == userId && m.DestinatarioId == destinatarioId) ||
                           (m.RemitenteId == destinatarioId && m.DestinatarioId == userId))
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            var destinatario = await _context.Usuarios.FindAsync(destinatarioId);

            ViewBag.Destinatario = destinatario;
            ViewBag.DestinatarioId = destinatarioId;
            ViewBag.UserId = userId;

            return View(mensajes);
        }

        // Lista de conversaciones (como WhatsApp)
        public async Task<IActionResult> Conversaciones()
        {
            var userId = ObtenerUsuarioId();

            // Obtener últimos mensajes de cada conversación
            var conversaciones = await _context.Mensajes
                .Where(m => m.RemitenteId == userId || m.DestinatarioId == userId)
                .GroupBy(m => m.RemitenteId == userId ? m.DestinatarioId : m.RemitenteId)
                .Select(g => new
                {
                    UsuarioId = g.Key,
                    UltimoMensaje = g.OrderByDescending(m => m.Fecha).FirstOrDefault(),
                    MensajesNoLeidos = g.Count(m => m.RemitenteId != userId && !m.Leido)
                })
                .ToListAsync();

            // Cargar información de usuarios
            var usuariosIds = conversaciones.Select(c => c.UsuarioId).ToList();
            var usuarios = await _context.Usuarios
                .Where(u => usuariosIds.Contains(u.IdUsuario))
                .ToDictionaryAsync(u => u.IdUsuario, u => u);

            ViewBag.Conversaciones = conversaciones;
            ViewBag.Usuarios = usuarios;

            return View();
        }

        // Enviar mensaje
        [HttpPost]
        public async Task<IActionResult> EnviarMensaje(int destinatarioId, string texto)
        {
            var mensaje = new Mensaje
            {
                RemitenteId = ObtenerUsuarioId(),
                DestinatarioId = destinatarioId,
                Texto = texto,
                Fecha = DateTime.Now
            };

            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", new { destinatarioId });
        }

        // GET: Mensajes - VERSIÓN SIMPLE Y FUNCIONAL
        // GET: Mensajes - VERSIÓN CON DEBUGGING

        public async Task<IActionResult> Index()
        {
            var userId = ObtenerUsuarioId();

            // DEBUG 1: Verificar que el usuario está logueado
            Console.WriteLine($"=== DEBUG MENSAJES ===");
            Console.WriteLine($"User ID obtenido: {userId}");
            Console.WriteLine($"Usuario autenticado: {User.Identity.IsAuthenticated}");
            Console.WriteLine($"Nombre usuario: {User.Identity.Name}");

            // DEBUG 2: Ver datos en la base de datos
            var todosMensajes = await _context.Mensajes.ToListAsync();
            Console.WriteLine($"Total mensajes en BD: {todosMensajes.Count}");

            foreach (var msg in todosMensajes)
            {
                Console.WriteLine($"Mensaje ID: {msg.Id}, RemitenteId: {msg.RemitenteId}, ProyectoId: {msg.ProyectoId}, Texto: {msg.Texto}");
            }

            // CONSULTA SIMPLE
            var mensajes = await _context.Mensajes
                .Where(m => m.RemitenteId == userId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            Console.WriteLine($"Mensajes encontrados para usuario {userId}: {mensajes.Count}");

            // DEBUG 3: Ver qué mensajes encontró
            foreach (var msg in mensajes)
            {
                Console.WriteLine($"ENCONTRADO - Mensaje ID: {msg.Id}, RemitenteId: {msg.RemitenteId}");
            }

            // Cargar información adicional
            if (mensajes.Any())
            {
                var proyectosIds = mensajes.Select(m => m.ProyectoId).Distinct().ToList();
                var usuariosIds = mensajes.Select(m => m.RemitenteId).Distinct().ToList();

                var proyectos = await _context.Proyectos
                    .Where(p => proyectosIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p.Titulo);

                var usuarios = await _context.Usuarios
                    .Where(u => usuariosIds.Contains(u.IdUsuario))
                    .ToDictionaryAsync(u => u.IdUsuario, u => u.NombreCompleto);

                ViewBag.Proyectos = proyectos;
                ViewBag.Usuarios = usuarios;

                Console.WriteLine($"Proyectos cargados: {proyectos.Count}");
                Console.WriteLine($"Usuarios cargados: {usuarios.Count}");
            }
            else
            {
                ViewBag.Proyectos = new Dictionary<int, string>();
                ViewBag.Usuarios = new Dictionary<int, string>();
                Console.WriteLine("No se cargaron proyectos/usuarios porque no hay mensajes");
            }

            ViewBag.UserId = userId;
            ViewBag.DebugInfo = $"Usuario: {userId}, Mensajes: {mensajes.Count}";

            return View(mensajes);
        }

        // GET: Mensajes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensaje = await _context.Mensajes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mensaje == null)
            {
                return NotFound();
            }

            return View(mensaje);
        }
        /*
        // GET: Mensajes/Create
        public IActionResult Create(int? proyectoId)
        {
            var mensaje = new Mensaje();

            if (proyectoId.HasValue)
            {
                mensaje.ProyectoId = proyectoId.Value;
            }

            return View(mensaje);
        }
        */
        // GET: Mensajes/Create
        public IActionResult Create(int? proyectoId)
        {
            // Si viene de un proyecto, pre-cargar el proyectoId
            if (proyectoId.HasValue)
            {
                ViewBag.ProyectoId = proyectoId.Value;
            }

            return View();
        }

        // POST: Mensajes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Texto,ProyectoId")] Mensaje mensaje)
        {
            if (ModelState.IsValid)
            {
                // IMPORTANTE: Asignar automáticamente el RemitenteId y Fecha
                mensaje.RemitenteId = ObtenerUsuarioId();
                mensaje.Fecha = DateTime.Now;

                _context.Add(mensaje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mensaje);
        }

        // GET: Mensajes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensaje = await _context.Mensajes.FindAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }
            return View(mensaje);
        }

        // POST: Mensajes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Texto,Fecha,ProyectoId,RemitenteId")] Mensaje mensaje)
        {
            if (id != mensaje.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mensaje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MensajeExists(mensaje.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mensaje);
        }

        // GET: Mensajes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensaje = await _context.Mensajes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mensaje == null)
            {
                return NotFound();
            }

            return View(mensaje);
        }

        // POST: Mensajes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mensaje = await _context.Mensajes.FindAsync(id);
            if (mensaje != null)
            {
                _context.Mensajes.Remove(mensaje);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MensajeExists(int id)
        {
            return _context.Mensajes.Any(e => e.Id == id);
        }
    }
}
