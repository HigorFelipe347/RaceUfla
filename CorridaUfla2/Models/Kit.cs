using System.ComponentModel.DataAnnotations;

namespace CorridaUfla2.Models
{
    public class Kit
    {
        public int Id { get; set; }

        [Required] public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required] public decimal Preco { get; set; }

        public string ItensTexto { get; set; }

        public bool TemCamisa { get; set; }

        public int OrganizadorId { get; set; }
    }
}
