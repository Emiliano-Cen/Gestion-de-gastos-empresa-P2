using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Dominio;
using System;
using System.Collections.Generic;

namespace WebAppObl2Cendali.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Perfil()
        {
            // logeuado
            string email = HttpContext.Session.GetString("EmailLogueado");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login");
            }

            Sistema sistema = Sistema.GetInstancia();
            Usuario usuario = sistema.BuscarUsuarioPorEmail(email);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // gastado en mes
            DateTime hoy = DateTime.Today;
            int anio = hoy.Year;
            int mes = hoy.Month;

            double totalMes = sistema.TotalGastadoUsuarioEnMes(usuario, anio, mes);

            ViewBag.TotalMes = totalMes;

            // si es gerente busca equipo
            List<Usuario> miembros = null;
            bool esGerente = false;

            if (usuario.Rol == RolUsuario.Gerente && usuario.Equipo != null)
            {
                esGerente = true;
                miembros = sistema.MiembrosDeEquipoOrdenadosPorEmail(usuario.Equipo);
            }

            ViewBag.EsGerente = esGerente;
            ViewBag.Miembros = miembros;

            return View(usuario);
        }
    }
}