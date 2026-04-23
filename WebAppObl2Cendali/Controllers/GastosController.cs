using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Dominio;
using System;
using System.Collections.Generic;

namespace WebAppObl2Cendali.Controllers
{
    public class GastosController : Controller
    {
        // login y gerente
        private bool EsGerente()
        {
            string email = HttpContext.Session.GetString("EmailLogueado");
            string rol = HttpContext.Session.GetString("RolLogueado");

            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            if (rol != "Gerente")
            {
                return false;
            }

            return true;
        }

        //list
        public IActionResult Index()
        {
            if (!EsGerente())
            {
                return RedirectToAction("Index", "Login");
            }

            Sistema sistema = Sistema.GetInstancia();
            List<TipoGasto> tipos = sistema.GetTiposGasto();

            return View(tipos);
        }

        // crear
        public IActionResult Crear()
        {
            if (!EsGerente())
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Crear(string nombre, string descripcion)
        {
            if (!EsGerente())
            {
                return RedirectToAction("Index", "Login");
            }

            Sistema sistema = Sistema.GetInstancia();

            try
            {
                if (nombre == null) nombre = "";
                if (descripcion == null) descripcion = "";

                TipoGasto tg = new TipoGasto(nombre, descripcion);
                sistema.AltaTipoGasto(tg);

                ViewBag.Msg = "Tipo de gasto creado correctamente.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // elim
        public IActionResult Eliminar(int id)
        {
            if (!EsGerente())
            {
                return RedirectToAction("Index", "Login");
            }

            Sistema sistema = Sistema.GetInstancia();
            List<TipoGasto> tipos = sistema.GetTiposGasto();

            TipoGasto encontrado = null;
            foreach (TipoGasto t in tipos)
            {
                if (t.Id == id)
                {
                    encontrado = t;
                    break;
                }
            }

            if (encontrado == null)
            {
                // si no existe vuelvo
                return RedirectToAction("Index");
            }

            return View(encontrado);
        }

        [HttpPost]
        public IActionResult Eliminar(int id, string IsChecked)
        {
            if (!EsGerente())
            {
                return RedirectToAction("Index", "Login");
            }

            Sistema sistema = Sistema.GetInstancia();
            List<TipoGasto> tipos = sistema.GetTiposGasto();

            TipoGasto encontrado = null;
            foreach (TipoGasto t in tipos)
            {
                if (t.Id == id)
                {
                    encontrado = t;
                    break;
                }
            }

            if (encontrado == null)
            {
                return RedirectToAction("Index");
            }

            // verifico check marcado
            if (string.IsNullOrEmpty(IsChecked))
            {
                ViewBag.Error = "Debe marcar la casilla de confirmación para eliminar.";
                return View(encontrado);
            }

            try
            {
                sistema.EliminarTipoGasto(encontrado);

                //actualiza el listado y muestra
                List<TipoGasto> lista = sistema.GetTiposGasto();
                ViewBag.Msg = "Tipo de gasto eliminado correctamente.";
                return View("Index", lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(encontrado);
            }
        }
    }
}

