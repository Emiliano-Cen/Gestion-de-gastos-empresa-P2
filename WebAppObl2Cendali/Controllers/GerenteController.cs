using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Dominio;
using System;
using System.Collections.Generic;

namespace WebAppObl2Cendali.Controllers
{
    public class GerenteController : Controller
    {
        public IActionResult PagosEquipo(int? anio, int? mes)
        {
            string email = HttpContext.Session.GetString("EmailLogueado");
            string rol = HttpContext.Session.GetString("RolLogueado");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login");
            }

            // es gerente
            if (rol != "Gerente")
            {
                return RedirectToAction("Index", "Home");
            }

            Sistema sistema = Sistema.GetInstancia();
            Usuario gerente = sistema.BuscarUsuarioPorEmail(email);

            // mes/año
            DateTime hoy = DateTime.Today;
            int anioBuscar = anio.HasValue ? anio.Value : hoy.Year;
            int mesBuscar = mes.HasValue ? mes.Value : hoy.Month;

            // pagos del equipo mes
            List<Pago> pagos = sistema.PagosEquipoEnMes(gerente.Equipo, anioBuscar, mesBuscar);

            ViewBag.Anio = anioBuscar;
            ViewBag.Mes = mesBuscar;
            ViewBag.NombreEquipo = gerente.Equipo.Nombre;

            return View(pagos);
        }
    }
}

