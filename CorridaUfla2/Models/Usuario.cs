using System.ComponentModel.DataAnnotations;

namespace CorridaUfla2.Models
{
    public abstract class Usuario
    {
        public int Id { get; set; }

        [Required] public string Nome { get; set; }

        [Required, EmailAddress] public string Email { get; set; }

        [Required, DataType(DataType.Password)] public string Senha { get; set; }

        public string Telefone { get; set; }

        // Endereço
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CEP { get; set; }
        public UnidadeFederativa UF { get; set; }
    }
}
