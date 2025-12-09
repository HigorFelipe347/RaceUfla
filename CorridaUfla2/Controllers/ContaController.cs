using Microsoft.AspNetCore.Mvc;
using CorridaUfla2.Data;
using CorridaUfla2.Models;

namespace CorridaUfla2.Controllers
{
    public class ContaController : Controller
    {
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            var corredor = DadosApp.Corredores
                .FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (corredor != null)
            {
                SetSession(corredor.Id, corredor.Nome, TipoUsuario.Corredor);
                return RedirectToAction("Index", "Home");
            }

            var org = DadosApp.Organizadores
                .FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (org != null)
            {
                SetSession(org.Id, org.Nome, TipoUsuario.Organizador);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Email ou senha incorretos";
            return View();
        }

        public IActionResult CadastroCorredor() => View();

        [HttpPost]
        public IActionResult CadastroCorredor(Corredor model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Id = DadosApp.NextIdUsuario();
            DadosApp.Corredores.Add(model);

            return RedirectToAction("Login");
        }

        public IActionResult CadastroOrganizador() => View();

        [HttpPost]
        public IActionResult CadastroOrganizador(Organizador model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Id = DadosApp.NextIdUsuario();
            DadosApp.Organizadores.Add(model);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private void SetSession(int id, string nome, TipoUsuario tipo)
        {
            HttpContext.Session.SetInt32("Id", id);
            HttpContext.Session.SetString("Nome", nome);
            HttpContext.Session.SetString("Tipo", tipo.ToString());
        }
    }
}
