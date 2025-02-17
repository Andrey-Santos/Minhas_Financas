using MinhasFinancas.Models;

namespace MinhasFinancas.ViewModels
{
    public class HomeViewModel
    {
        public decimal SaldoInicial { get; set; }
        public decimal SaldoAtual { get; set; }
        public decimal SaldoPrevisto { get; set; }
        public string? MesAtual { get; set; }
        public int MesSelecionado { get; set; }
        public int MesAnterior { get; set; }
        public int MesProximo { get; set; }
        public List<TransacaoModel>? UltimasTransacoes { get; set; }
    }
}
