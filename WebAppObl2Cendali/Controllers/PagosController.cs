using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Dominio;
using System;
using System.Collections.Generic;

namespace WebAppObl2Cendali.Controllers
{
    public class PagosController : Controller
    {

        public IActionResult MisPagos()
        {
            // logueado
            string email = HttpContext.Session.GetString("EmailLogueado");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login");
            }

            Sistema sis = Sistema.GetInstancia();
            Usuario usuario = sis.BuscarUsuarioPorEmail(email);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            DateTime hoy = DateTime.Today;
            int anio = hoy.Year;
            int mes = hoy.Month;

            List<Pago> lista = sis.PagosUsuarioEnMes(usuario, anio, mes);

            ViewBag.Anio = anio;
            ViewBag.Mes = mes;

            return View(lista); 
        }

        //crea pago
        public IActionResult Crear()
        {
            string email = HttpContext.Session.GetString("EmailLogueado");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index", "Login");

            Sistema sis = Sistema.GetInstancia();
            ViewBag.Tipos = sis.GetTiposGasto();

            return View();
        }

        [HttpPost]
        public IActionResult Crear(
            string descripcion,
            double monto,
            MetodoPago metodo,
            int tipoGastoId,
            string tipoPago,
            DateTime fechaPago,
            DateTime fechaInicio,
            DateTime? fechaFin)
        {
            string email = HttpContext.Session.GetString("EmailLogueado");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index", "Login");

            Sistema sis = Sistema.GetInstancia();
            Usuario user = sis.BuscarUsuarioPorEmail(email);

            // buscar tipo gasto
            TipoGasto tipo = null;
            List<TipoGasto> listaTipos = sis.GetTiposGasto();
            foreach (TipoGasto tg in listaTipos)
            {
                if (tg.Id == tipoGastoId)
                {
                    tipo = tg;
                    break;
                }
            }

            if (tipo == null)
            {
                ViewBag.Error = "Tipo de gasto inválido.";
                ViewBag.Tipos = sis.GetTiposGasto();
                return View();
            }

            try
            {
                Pago nuevo = null;

                if (tipoPago == "Unico")
                {
                    string nroRecibo = "REC" + Guid.NewGuid().ToString("N").Substring(0, 5);
                    nuevo = new PagoUnico(metodo, tipo, user, descripcion, monto, fechaPago, nroRecibo);
                }
                else if (tipoPago == "Recurrente")
                {
                    nuevo = new PagoRecurrente(metodo, tipo, user, descripcion, monto, fechaInicio, fechaFin);
                }

                sis.AltaPago(nuevo);
                ViewBag.Msg = "Pago creado correctamente.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Tipos = sis.GetTiposGasto();
            return View();
        }
    }
}
