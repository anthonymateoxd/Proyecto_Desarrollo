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
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }
        /*
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        */
        
        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }
        

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,NombreCompleto,Correo,Clave,Rol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var userId = ObtenerUsuarioId();
            bool esAdmin = User.IsInRole("Admin");

            // Solo permitir edición si:
            // - Es Admin (puede editar cualquier usuario)
            // - O está editando su propio perfil
            if (!esAdmin && usuario.IdUsuario != userId)
            {
                return Forbid(); // O RedirectToAction("AccessDenied")
            }

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Solo Admin puede eliminar usuarios
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Solo Admin puede eliminar usuarios
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,NombreCompleto,Correo,Clave,Rol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            var userId = ObtenerUsuarioId();
            bool esAdmin = User.IsInRole("Admin");

            // Verificar permisos
            if (!esAdmin && usuario.IdUsuario != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // OBTENER el usuario existente de la base de datos
                    var usuarioExistente = await _context.Usuarios.FindAsync(id);
                    if (usuarioExistente == null)
                    {
                        return NotFound();
                    }

                    // Actualizar solo los campos permitidos
                    usuarioExistente.NombreCompleto = usuario.NombreCompleto;
                    usuarioExistente.Correo = usuario.Correo;

                    // Solo actualizar contraseña si se proporcionó una nueva
                    if (!string.IsNullOrEmpty(usuario.Clave))
                    {
                        usuarioExistente.Clave = usuario.Clave;
                    }

                    // Solo admin puede cambiar el rol
                    if (esAdmin)
                    {
                        usuarioExistente.Rol = usuario.Rol;
                    }

                    // Usar Update en la instancia existente, no en la nueva
                    _context.Update(usuarioExistente);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Perfil actualizado correctamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.IdUsuario))
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
            return View(usuario);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
