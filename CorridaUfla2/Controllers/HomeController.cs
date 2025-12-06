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
            if (!string.IsNullOrEmpty(busca))
            {
                lista = lista.Where(c => c.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase));
            }
            return View(lista.ToList());
        }

        public IActionResult Detalhes(int id)
        {
            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == id);
            if (corrida == null) return NotFound();

            // ==============================================================================
            // ALTERAÇÃO AQUI:
            // Antes buscávamos só os kits marcados na corrida.
            // Agora buscamos TODOS os kits que pertencem ao Organizador dono dessa corrida.
            // ==============================================================================

            var kitsDoOrganizador = DadosApp.Kits
                .Where(k => k.OrganizadorId == corrida.OrganizadorId)
                .ToList();

            // CRIA O KIT GRATUITO/BÁSICO (Sempre deve existir)
            var kitBasico = new Kit
            {
                Id = -1, // ID negativo para identificar internamente
                Nome = "Inscrição Básica (Sem Kit)",
                Preco = 0,
                Descricao = "Apenas participação no evento (Número de peito e Hidratação).",
                ItensTexto = "Número de Peito, Chip, Água",
                TemCamisa = false
            };

            // Adiciona o gratuito no topo da lista
            kitsDoOrganizador.Insert(0, kitBasico);

            // Envia para a tela
            ViewBag.KitsDisponiveis = kitsDoOrganizador;

            // Busca dados do Organizador para mostrar o PIX e Nome
            var organizador = DadosApp.Organizadores.FirstOrDefault(o => o.Id == corrida.OrganizadorId);
            ViewBag.Pix = organizador != null ? organizador.ChavePix : "Chave não cadastrada";
            ViewBag.NomeOrganizador = organizador != null ? organizador.Nome : "Organização";

            return View(corrida);
        }

        [HttpPost]
        public IActionResult Inscrever(int corridaId, int kitId, TamanhoCamisa tamanho)
        {
            if (HttpContext.Session.GetString("Tipo") != "Corredor")
                return RedirectToAction("Login", "Conta");

            var corrida = DadosApp.Corridas.FirstOrDefault(c => c.Id == corridaId);
            var idCorredor = HttpContext.Session.GetInt32("Id");

            if (corrida == null || idCorredor == null) return RedirectToAction("Index");

            // Preparar variáveis da inscrição
            string nomeKitInscricao;
            decimal valorInscricao;

            // VERIFICAÇÃO: É o Kit Gratuito (ID -1) ou um Kit Pago?
            if (kitId == -1)
            {
                nomeKitInscricao = "Inscrição Básica (Sem Kit)";
                valorInscricao = 0;
                // Se é sem kit, forçamos o tamanho da camisa para NaoAplica
                tamanho = TamanhoCamisa.NaoAplica;
            }
            else
            {
                // É um kit pago, busca no banco
                var kitBanco = DadosApp.Kits.FirstOrDefault(k => k.Id == kitId);
                if (kitBanco == null) return RedirectToAction("Index"); // Segurança

                nomeKitInscricao = kitBanco.Nome;
                valorInscricao = kitBanco.Preco;
            }

            // Cria a inscrição
            var inscricao = new Inscricao
            {
                Id = DadosApp.NextIdInscricao(),
                CorredorId = idCorredor.Value,
                CorridaId = corridaId,
                NomeCorrida = corrida.Nome,
                KitId = kitId,
                NomeKit = nomeKitInscricao,
                ValorPago = valorInscricao,
                TamanhoCamisa = tamanho,
                // Se for grátis, já confirma. Se for pago, fica pendente.
                Status = valorInscricao == 0 ? "Confirmado (Gratuito)" : "Pendente (Aguardando PIX)"
            };

            DadosApp.Inscricoes.Add(inscricao);
            return RedirectToAction("MinhasInscricoes");
        }
        [HttpPost]
        public IActionResult CancelarInscricao(int id)
        {
            // 1. Segurança: Só corredor pode cancelar
            if (HttpContext.Session.GetString("Tipo") != "Corredor")
                return RedirectToAction("Login", "Conta");

            var idCorredor = HttpContext.Session.GetInt32("Id");

            // 2. Busca a inscrição: Tem que existir E pertencer ao usuário logado
            var inscricao = DadosApp.Inscricoes.FirstOrDefault(i => i.Id == id && i.CorredorId == idCorredor);

            if (inscricao != null)
            {
                DadosApp.Inscricoes.Remove(inscricao);
            }

            // 3. Volta para a lista
            return RedirectToAction("MinhasInscricoes");
        }

        public IActionResult MinhasInscricoes()
        {
            var idUser = HttpContext.Session.GetInt32("Id");
            var lista = DadosApp.Inscricoes.Where(i => i.CorredorId == idUser).ToList();
            return View(lista);
        }
    }
}