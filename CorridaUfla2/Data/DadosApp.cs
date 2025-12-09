using CorridaUfla2.Models;

namespace CorridaUfla2.Data
{
    public static class DadosApp
    {
        public static List<Corredor> Corredores { get; set; } = new();
        public static List<Organizador> Organizadores { get; set; } = new();
        public static List<Kit> Kits { get; set; } = new();
        public static List<Corrida> Corridas { get; set; } = new();
        public static List<Inscricao> Inscricoes { get; set; } = new();

        public static int NextIdUsuario() =>
            Corredores.Count + Organizadores.Count + 1;

        public static int NextIdKit() =>
            Kits.Count + 1;

        public static int NextIdCorrida() =>
            Corridas.Count + 1;

        public static int NextIdInscricao() =>
            Inscricoes.Count + 1;
    }
}
