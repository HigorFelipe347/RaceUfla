using System.ComponentModel.DataAnnotations;

namespace CorridaUfla2.Models
{
    public class Organizador : Usuario
    {
        [Required] public string CpfCnpj { get; set; }
        [Required] public string ChavePix { get; set; }
    }
}