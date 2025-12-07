using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorridaUfla2.Models
{
    public class Corrida
    {
        public int Id { get; set; }

        [Required] public string Nome { get; set; }

        public string Descricao { get; set; }

        // Endereço
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public UnidadeFederativa UF { get; set; }

        public double DistanciaKm { get; set; }
        public int MaximoParticipantes { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan HorarioLargada { get; set; }

        public string CaminhoImagemMapa { get; set; }

        // Upload sem mapear no BD
        [NotMapped]
        public IFormFile ArquivoImagem { get; set; }

        public int OrganizadorId { get; set; }

        // Kits escolhidos
        public List<int> KitsSelecionados { get; set; } = new();
    }
}
