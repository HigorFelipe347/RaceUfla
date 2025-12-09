using Microsoft.AspNetCore.Mvc;
using CorridaUfla2.Data;
using CorridaUfla2.Models;

namespace CorridaUfla2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string busca)
        {
            var lista = DadosApp.Corridas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
                lista = lista.Where(c => c.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase));

            return View(lista.ToList());
        }

        public IActionResult Detalhes(int id)
        {
            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == id);
            if (corrida == null) return NotFound();

            var kits = DadosApp.Kits
                .Where(k => k.OrganizadorId == corrida.OrganizadorId)
                .ToList();

            kits.Insert(0, new Kit
            {
                Id = -1,
                Nome = "Inscrição Básica (Sem Kit)",
                Preco = 0,
                Descricao = "Apenas participação no evento.",
                ItensTexto = "Number, Chip, Água",
                TemCamisa = false
            });

            ViewBag.KitsDisponiveis = kits;

            var org = DadosApp.Organizadores.FirstOrDefault(o => o.Id == corrida.OrganizadorId);
            ViewBag.Pix = org?.ChavePix ?? "Chave não cadastrada";
            ViewBag.NomeOrganizador = org?.Nome ?? "Organizador";

            return View(corrida);
        }

        [HttpPost]
        public IActionResult Inscrever(int corridaId, int kitId, TamanhoCamisa tamanho)
        {
            if (HttpContext.Session.GetString("Tipo") != "Corredor")
                return RedirectToAction("Login", "Conta");

            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == corridaId);
            var idCorredor = HttpContext.Session.GetInt32("Id");

            if (corrida == null || idCorredor == null)
                return RedirectToAction("Index");

            string nomeKit;
            decimal preco;

            if (kitId == -1)
            {
                nomeKit = "Inscrição Básica (Sem Kit)";
                preco = 0;
                tamanho = TamanhoCamisa.NaoAplica;
            }
            else
            {
                var kit = DadosApp.Kits.FirstOrDefault(k => k.Id == kitId);
                if (kit == null) return RedirectToAction("Index");

                nomeKit = kit.Nome;
                preco = kit.Preco;
            }

            DadosApp.Inscricoes.Add(new Inscricao
            {
                Id = DadosApp.NextIdInscricao(),
                CorredorId = idCorredor.Value,
                CorridaId = corridaId,
                NomeCorrida = corrida.Nome,
                KitId = kitId,
                NomeKit = nomeKit,
                ValorPago = preco,
                TamanhoCamisa = tamanho,
                Status = preco == 0 ? "Confirmado (Gratuito)" : "Pendente (Aguardando PIX)"
            });

            return RedirectToAction("MinhasInscricoes");
        }

        [HttpPost]
        public IActionResult CancelarInscricao(int id)
        {
            if (HttpContext.Session.GetString("Tipo") != "Corredor")
                return RedirectToAction("Login", "Conta");

            var idUser = HttpContext.Session.GetInt32("Id");
            var insc = DadosApp.Inscricoes.FirstOrDefault(i => i.Id == id && i.CorredorId == idUser);

            if (insc != null)
                DadosApp.Inscricoes.Remove(insc);

            return RedirectToAction("MinhasInscricoes");
        }

        public IActionResult MinhasInscricoes()
        {
            var id = HttpContext.Session.GetInt32("Id");
            var lista = DadosApp.Inscricoes.Where(i => i.CorredorId == id).ToList();
            return View(lista);
        }
    }
}
