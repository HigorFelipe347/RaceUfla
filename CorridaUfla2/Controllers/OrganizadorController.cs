using Microsoft.AspNetCore.Mvc;
using CorridaUfla2.Data;
using CorridaUfla2.Models;

namespace CorridaUfla2.Controllers
{
    public class OrganizadorController : Controller
    {
        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================
        private bool IsOrganizador()
        {
            return HttpContext.Session.GetString("Tipo") == "Organizador";
        }

        private int GetId()
        {
            return HttpContext.Session.GetInt32("Id") ?? 0;
        }

        // ==========================================
        // GESTÃO DE KITS
        // ==========================================

        public IActionResult MeusKits()
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");
            var kits = DadosApp.Kits.Where(k => k.OrganizadorId == GetId()).ToList();
            return View(kits);
        }

        public IActionResult CriarKit()
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");
            return View();
        }

        [HttpPost]
        public IActionResult CriarKit(Kit model, List<string> itensSelecionados, string itemOutros)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            ModelState.Remove("Id");

            // Processa Itens
            if (!string.IsNullOrWhiteSpace(itemOutros))
            {
                var outrosArray = itemOutros.Split(',');
                foreach (var item in outrosArray)
                {
                    if (!string.IsNullOrWhiteSpace(item)) itensSelecionados.Add(item.Trim());
                }
            }
            model.ItensTexto = string.Join(", ", itensSelecionados);
            model.TemCamisa = itensSelecionados.Any(i => i.Contains("Camisa", StringComparison.OrdinalIgnoreCase));

            model.Id = DadosApp.NextIdKit();
            model.OrganizadorId = GetId();

            DadosApp.Kits.Add(model);

            return RedirectToAction("MeusKits");
        }

        public IActionResult EditarKit(int id)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");
            var kit = DadosApp.Kits.FirstOrDefault(k => k.Id == id);

            if (kit == null || kit.OrganizadorId != GetId()) return NotFound();

            return View(kit);
        }

        [HttpPost]
        public IActionResult EditarKit(Kit model)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            var kitOriginal = DadosApp.Kits.FirstOrDefault(k => k.Id == model.Id);
            if (kitOriginal != null && kitOriginal.OrganizadorId == GetId())
            {
                kitOriginal.Nome = model.Nome;
                kitOriginal.Preco = model.Preco;
                kitOriginal.Descricao = model.Descricao;
                kitOriginal.ItensTexto = model.ItensTexto;
                kitOriginal.TemCamisa = model.TemCamisa;
            }
            return RedirectToAction("MeusKits");
        }

        public IActionResult ExcluirKit(int id)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");
            var kit = DadosApp.Kits.FirstOrDefault(k => k.Id == id);

            if (kit != null && kit.OrganizadorId == GetId())
            {
                DadosApp.Kits.Remove(kit);
            }
            return RedirectToAction("MeusKits");
        }

        // ==========================================
        // GESTÃO DE CORRIDAS
        // ==========================================

        public IActionResult MinhasCorridas()
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");
            var corridas = DadosApp.Corridas.Where(c => c.OrganizadorId == GetId()).ToList();
            return View(corridas);
        }

        public IActionResult CriarCorrida()
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");
            ViewBag.MeusKits = DadosApp.Kits.Where(k => k.OrganizadorId == GetId()).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CriarCorrida(Corrida model)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            if (model.ArquivoImagem != null)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string nomeArquivo = Guid.NewGuid().ToString() + "_" + model.ArquivoImagem.FileName;
                using (var stream = new FileStream(Path.Combine(folder, nomeArquivo), FileMode.Create))
                {
                    model.ArquivoImagem.CopyTo(stream);
                }
                model.CaminhoImagemMapa = nomeArquivo;
            }

            if (model.KitsSelecionados == null) model.KitsSelecionados = new();

            model.Id = DadosApp.NextIdCorrida();
            model.OrganizadorId = GetId();
            DadosApp.Corridas.Add(model);

            return RedirectToAction("MinhasCorridas");
        }

        // --- MÉTODOS DE EDIÇÃO (ONDE ESTAVA O ERRO) ---

        // GET: Exibe o formulário (Recebe ID int)
        public IActionResult EditarCorrida(int id)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == id);

            if (corrida == null || corrida.OrganizadorId != GetId()) return NotFound();

            ViewBag.MeusKits = DadosApp.Kits.Where(k => k.OrganizadorId == GetId()).ToList();

            return View(corrida);
        }

        // POST: Salva os dados (Recebe Modelo Corrida)
        [HttpPost]
        public IActionResult EditarCorrida(Corrida model)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            var original = DadosApp.Corridas.FirstOrDefault(c => c.Id == model.Id);

            if (original != null && original.OrganizadorId == GetId())
            {
                original.Nome = model.Nome;
                original.DataInicio = model.DataInicio;
                original.HorarioLargada = model.HorarioLargada;
                original.Descricao = model.Descricao;
                original.Cidade = model.Cidade;
                original.Bairro = model.Bairro;
                original.Logradouro = model.Logradouro;
                original.Numero = model.Numero;
                original.DistanciaKm = model.DistanciaKm;
                original.MaximoParticipantes = model.MaximoParticipantes;

                if (model.KitsSelecionados == null) model.KitsSelecionados = new();
                original.KitsSelecionados = model.KitsSelecionados;

                if (model.ArquivoImagem != null)
                {
                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens");
                    string nomeArquivo = Guid.NewGuid().ToString() + "_" + model.ArquivoImagem.FileName;
                    using (var stream = new FileStream(Path.Combine(folder, nomeArquivo), FileMode.Create))
                    {
                        model.ArquivoImagem.CopyTo(stream);
                    }
                    original.CaminhoImagemMapa = nomeArquivo;
                }
            }
            return RedirectToAction("MinhasCorridas");
        }

        public IActionResult ExcluirCorrida(int id)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");
            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == id);

            if (corrida != null && corrida.OrganizadorId == GetId())
            {
                DadosApp.Corridas.Remove(corrida);
            }
            return RedirectToAction("MinhasCorridas");
        }

        // ==========================================
        // GESTÃO DE PARTICIPANTES / PAGAMENTOS
        // ==========================================

        public IActionResult InformacoesCorrida(int id)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == id);
            if (corrida == null || corrida.OrganizadorId != GetId()) return NotFound();

            ViewBag.Inscricoes = DadosApp.Inscricoes.Where(i => i.CorridaId == id).ToList();
            ViewBag.TodosCorredores = DadosApp.Corredores;

            return View(corrida);
        }

        [HttpPost]
        public IActionResult ConfirmarPagamento(int idInscricao)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            var inscricao = DadosApp.Inscricoes.FirstOrDefault(i => i.Id == idInscricao);
            if (inscricao == null) return NotFound();

            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == inscricao.CorridaId);
            if (corrida == null || corrida.OrganizadorId != GetId()) return RedirectToAction("MinhasCorridas");

            inscricao.Status = "Confirmado";

            return RedirectToAction("InformacoesCorrida", new { id = inscricao.CorridaId });
        }

        [HttpPost]
        public IActionResult NegarInscricao(int idInscricao)
        {
            if (!IsOrganizador()) return RedirectToAction("Login", "Conta");

            var inscricao = DadosApp.Inscricoes.FirstOrDefault(i => i.Id == idInscricao);
            if (inscricao == null) return NotFound();

            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == inscricao.CorridaId);
            if (corrida == null || corrida.OrganizadorId != GetId()) return RedirectToAction("MinhasCorridas");

            DadosApp.Inscricoes.Remove(inscricao);

            return RedirectToAction("InformacoesCorrida", new { id = inscricao.CorridaId });
        }
    }
}