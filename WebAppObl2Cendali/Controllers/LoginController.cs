using Dominio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppObl2Cendali.Models; 

namespace WebAppObl2Cendali.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index()
        {
            return View(new LoginViewModel());

        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {


            if (model == null)
            {

                ViewBag.Error = "Datos inválidos";
                return View(new LoginViewModel());
            }

            Sistema sistema = Sistema.GetInstancia();

            Usuario? usuario = sistema.ValidarLogin(model.Email, model.Password);


            if (usuario == null)
            {
                ViewBag.Error = "Credenciales incorrectas";
                return View(model);

            }

            // info básica
            HttpContext.Session.SetString("EmailLogueado", usuario.Email);
            HttpContext.Session.SetString("RolLogueado", usuario.Rol.ToString());


            return RedirectToAction("Index", "Home");

        }

        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
