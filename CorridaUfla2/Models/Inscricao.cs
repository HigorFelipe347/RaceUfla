namespace CorridaUfla2.Models
{
    public class Inscricao
    {
        public int Id { get; set; }
        public int CorredorId { get; set; }
        public int CorridaId { get; set; }

        public string NomeCorrida { get; set; }
        public int KitId { get; set; }
        public string NomeKit { get; set; }

        public TamanhoCamisa TamanhoCamisa { get; set; }

        public decimal ValorPago { get; set; }

        public string Status { get; set; } = "Pendente";
    }
}
