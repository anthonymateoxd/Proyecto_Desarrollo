using Microsoft.AspNetCore.Mvc;

using DemonSlayer.Data;
using DemonSlayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using DemonSlayer.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace DemonSlayer.Controllers
{
    public class AccessoController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public AccessoController(ApplicationDbContext applicationDbContext) 
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden XD";
                return View();
            }

            // Validar que el rol sea solo Cliente o Consultor
            if (modelo.Rol != "Cliente" && modelo.Rol != "Consultor")
            {
                ModelState.AddModelError("Rol", "Tipo de usuario no válido");
                return View(modelo);
            }

            Usuario usuario = new Usuario()
            {
                NombreCompleto = modelo.NombreCompleto,
                Correo = modelo.Correo,
                Clave = modelo.Clave,
                Rol = modelo.Rol
            };

            await _applicationDbContext.Usuarios.AddAsync(usuario);
            await _applicationDbContext.SaveChangesAsync();

            if (usuario.IdUsuario != 0) return RedirectToAction("Login", "Accesso");

            ViewData["Mensaje"] = "No se pudo crear el usuario, error Fatal";

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            Usuario? usuario_encontrado = await _applicationDbContext.Usuarios
                .Where(u =>
                    u.Correo == modelo.Correo &&
                    u.Clave == modelo.Clave
                ).FirstOrDefaultAsync();

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias, XD";
                return View();
            }

            List<Claim> claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Name, usuario_encontrado.NombreCompleto),
        new Claim(ClaimTypes.Role, usuario_encontrado.Rol),
        new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()) // ← AGREGAR ESTA LÍNEA
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }

    }
}
