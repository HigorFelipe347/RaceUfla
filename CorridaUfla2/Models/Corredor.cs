using System.ComponentModel.DataAnnotations;

namespace CorridaUfla2.Models
{
    public class Corredor : Usuario
    {
        [Required] public string CPF { get; set; }
        public string RG { get; set; }
        public string Sexo { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }
    }
}
